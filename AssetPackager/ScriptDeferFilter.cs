using System;
using System.Web;
using System.IO;
using System.Text;
using AssetPackager;

namespace AssetPackager
{
	/// <summary>
	/// Combines all script blocks and moves them to the bottom of the page. Combines multiple
	/// external JavaScripts into single one (see <see cref="CombineScripts" />).
	/// </summary>
	public class ScriptDeferFilter : Stream
	{
		/// <summary>
		/// Name of the hidden field with loaded scripts list.
		/// </summary>
		public const string SCRIPT_DEFER_HIDDEN_FIELD = "__SCRIPTDEFER";

		#region Private fields

		private readonly Stream _responseStream;
		private long _position;

		/// <summary>
		/// When this is true, script blocks are suppressed and captured for 
		/// later rendering
		/// </summary>
		private bool _captureScripts;

		/// <summary>
		/// Holds all script blocks that are injected by the controls
		/// The script blocks will be moved after the form tag renders
		/// </summary>
		private readonly StringBuilder _scriptBlocks;

		private readonly Encoding _encoding;

		/// <summary>
		/// Holds characters from last Write(...) call where the start tag did not
		/// end and thus the remaining characters need to be preserved in a buffer so 
		/// that a complete tag can be parsed
		/// </summary>
		private char[] _pendingBuffer;

		/// <summary>
		/// Initializes a new instance of <see cref="ScriptDeferFilter" /> class.
		/// </summary>
		/// <param name="response">Response stream.</param>
		public ScriptDeferFilter(HttpResponse response)
		{
			_encoding = response.Output.Encoding;
			_responseStream = response.Filter;

			_scriptBlocks = new StringBuilder(5000);
			// When this is on, script blocks are captured and not written to output
			_captureScripts = true;
		}

		#endregion

		#region Filter overrides

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <value>Always <c>false</c>.</value>
		public override bool CanRead
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <value>Always <c>false</c>.</value>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <value>Always <c>true</c>.</value>
		public override bool CanWrite
		{
			get { return true; }
		}

		/// <summary>
		/// Closes the current stream and releases any resources (such as sockets and file 
		/// handles) associated with the current stream.
		/// </summary>
		public override void Close()
		{
			FlushPendingBuffer();
			_responseStream.Close();
		}

		private void FlushPendingBuffer()
		{
			if (null == _pendingBuffer) return;
			// Some characters were left in the buffer 
			WriteOutput(_pendingBuffer, 0, _pendingBuffer.Length);
			_pendingBuffer = null;
		}

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered 
		/// data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			FlushPendingBuffer();
			_responseStream.Flush();
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		/// <value>Always <c>0</c>.</value>
		public override long Length
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get { return _position; }
			set { _position = value; }
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
		/// <param name="origin">A value of type <see cref="SeekOrigin" /> indicating the reference point 
		/// used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _responseStream.Seek(offset, origin);
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="length">The desired length of the current stream in bytes.</param>
		public override void SetLength(long length)
		{
			_responseStream.SetLength(length);
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position 
		/// within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer 
		/// contains the specified byte array with the values between <paramref name="offset" /> and 
		/// (<paramref name="offset" /> + <paramref name="count" /> - <c>1</c>) replaced by the bytes 
		/// read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which 
		/// to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the 
		/// number of bytes requested if that many bytes are not currently available, or 
		/// zero (<c>0</c>) if the end of the stream has been reached.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return _responseStream.Read(buffer, offset, count);
		}

		#endregion

		public override void Write(byte[] buffer, int offset, int count)
		{
			// If we are not capturing script blocks anymore, just redirect to response stream
			if (!_captureScripts)
			{
				_responseStream.Write(buffer, offset, count);
				return;
			}

			// Script and HTML can be in one of the following combinations in the specified buffer:          
			// .....<script ....>.....</script>.....
			// <script ....>.....</script>.....
			// <script ....>.....</script>
			// <script ....>.....</script> .....
			// ....<script ....>..... 
			// <script ....>..... 
			// .....</script>.....
			// .....</script>
			// <script>.....
			// .... </script>
			// ......
			// Here, "...." means html content between and outside script tags

			char[] content;
			char[] charBuffer = _encoding.GetChars(buffer, offset, count);

			// If some bytes were left for processing during last Write call
			// then consider those into the current buffer
			if (null != _pendingBuffer)
			{
				content = new char[charBuffer.Length + _pendingBuffer.Length];
				Array.Copy(_pendingBuffer, 0, content, 0, _pendingBuffer.Length);
				Array.Copy(charBuffer, 0, content, _pendingBuffer.Length, charBuffer.Length);
				_pendingBuffer = null;
			}
			else
			{
				content = charBuffer;
			}

			int scriptTagStart = 0;
			int lastScriptTagEnd = 0;
			bool scriptTagStarted = false;

			int pos;
			for (pos = 0; pos < content.Length; pos++)
			{
				// See if tag start
				char c = content[pos];
				if (c != '<') continue;

				// Make sure there are enough characters available in the buffer to finish 
				// tag start. This will happen when a tag partially starts but does not end
				// For example, a partial script tag
				// <script
				// Or it's the ending html tag or some tag closing that ends the whole response
				// </html>
				if (pos + "script".Length >= content.Length)
				{
					// a tag started but there are less than 10 characters available. So, let's
					// store the remaining content in a buffer and wait for another Write(...) or
					// flush call.
					_pendingBuffer = new char[content.Length - pos];
					Array.Copy(content, pos, _pendingBuffer, 0, content.Length - pos);
					break;
				}

				int tagStart = pos;
				// Check if it's a tag ending
				if (content[pos + 1] == '/')
				{
					pos += 2; // go past the </ 

					// See if script tag is ending
					if (IsScriptTag(content, pos))
					{
						// Script tag just ended. Get the whole script
						// and store in buffer
						pos = pos + "script>".Length;
						_scriptBlocks.Append(content, scriptTagStart, pos - scriptTagStart);
						_scriptBlocks.Append(Environment.NewLine);
						lastScriptTagEnd = pos;

						scriptTagStarted = false;

						pos--; // continue will increase pos by one again
						continue;
					}
					if (IsBodyTag(content, pos))
					{
						// body tag has just end. Time for rendering all the script
						// blocks we have suppressed so far and stop capturing script blocks

						if (_scriptBlocks.Length > 0)
						{
							// Render all pending html output till now
							WriteOutput(content, lastScriptTagEnd, tagStart - lastScriptTagEnd);

							// Render the script blocks
							RenderAllScriptBlocks();

							// Stop capturing for script blocks
							_captureScripts = false;

							// Write from the body tag start to the end of the inut buffer and return
							// from the function. We are done.
							WriteOutput(content, tagStart, content.Length - tagStart);
							return;
						}
					}
					else
					{
						// some other tag's closing. safely skip one character as smallest
						// html tag is one character e.g. <b>. just an optimization to save one loop
						pos++;
					}
				}
				else
				{
					if (IsScriptTag(content, pos + 1))
					{
						// Script tag started. Record the position as we will 
						// capture the whole script tag including its content
						// and store in an internal buffer.
						scriptTagStart = pos;

						// Write html content since last script tag closing upto this script tag 
						WriteOutput(content, lastScriptTagEnd, scriptTagStart - lastScriptTagEnd);

						// Skip the tag start to save some loops
						pos += "<script".Length;

						scriptTagStarted = true;
					}
					else
					{
						// some other tag started
						// safely skip 2 character because the smallest tag is one character e.g. <b>
						// just an optimization to eliminate one loop 
						pos++;
					}
				}
			}

			// If a script tag is partially sent to buffer, then the remaining content
			// is part of the last script block
			if (scriptTagStarted)
			{
				_scriptBlocks.Append(content, scriptTagStart, pos - scriptTagStart);
			}
			else
			{
				/// Render the characters since the last script tag ending
				WriteOutput(content, lastScriptTagEnd, pos - lastScriptTagEnd);
			}
		}

		/// <summary>
		/// Render collected scripts blocks all together
		/// </summary>
		private void RenderAllScriptBlocks()
		{
			string output = _scriptBlocks.ToString();
			output = CombineScripts.CombineScriptBlocks(output);
			byte[] scriptBytes = _encoding.GetBytes(output);
			_responseStream.Write(scriptBytes, 0, scriptBytes.Length);
		}

		private void WriteOutput(char[] content, int pos, int length)
		{
			if (length == 0) return;

			byte[] buffer = _encoding.GetBytes(content, pos, length);
			_responseStream.Write(buffer, 0, buffer.Length);
		}

		private static bool IsScriptTag(char[] content, int pos)
		{
			if (pos + 5 < content.Length)
				return ((content[pos] == 's' || content[pos] == 'S')
				        && (content[pos + 1] == 'c' || content[pos + 1] == 'C')
				        && (content[pos + 2] == 'r' || content[pos + 2] == 'R')
				        && (content[pos + 3] == 'i' || content[pos + 3] == 'I')
				        && (content[pos + 4] == 'p' || content[pos + 4] == 'P')
				        && (content[pos + 5] == 't' || content[pos + 5] == 'T'));
			return false;
		}

		private static bool IsBodyTag(char[] content, int pos)
		{
			if (pos + 3 < content.Length)
				return ((content[pos] == 'f' || content[pos] == 'F')
				        && (content[pos + 1] == 'o' || content[pos + 1] == 'O')
				        && (content[pos + 2] == 'r' || content[pos + 2] == 'R')
				        && (content[pos + 3] == 'm' || content[pos + 3] == 'M'));
			return false;
		}
	}
}