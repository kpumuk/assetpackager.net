<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DemoProject._Default" %>
<%@ Register Assembly="AssetPackager" Namespace="AssetPackager.WebControls" TagPrefix="apn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <apn:AssetPackagerScriptManager ID="smCurrent" runat="server">
			<Scripts>
				<asp:ScriptReference Path="~/javascripts/swfobject.js" />
			</Scripts>
        </apn:AssetPackagerScriptManager>
        <ajaxToolkit:TabContainer runat="server" CssClass="linkedin linkedin-blue">
			<ajaxToolkit:TabPanel ID="tabForm" runat="server" HeaderText="Contact Form">
				<ContentTemplate>
					<asp:UpdatePanel runat="server" ID="upForm" UpdateMode="Conditional">
						<ContentTemplate>
							<asp:PlaceHolder runat="server" ID="phContent">
								<div class="row">
									<asp:Label runat="server" ID="lblFirstName" AssociatedControlID="txtFirstName" AccessKey="f" Text="First name: " />
									<div class="field">
										<asp:TextBox runat="server" ID="txtFirstName" TabIndex="1" />
									</div>
								</div>
				
								<div class="row">
									<asp:Label runat="server" ID="lblLastName" AssociatedControlID="txtLastName" AccessKey="l" Text="Last name: " />
									<div class="field">
										<asp:TextBox runat="server" ID="txtLastName" TabIndex="2" />
									</div>
								</div>

								<div class="row">
									<asp:Label runat="server" ID="lblEmail" AssociatedControlID="txtEmail" AccessKey="e" Text="Email: " CssClass="required" />
									<div class="field">
										<asp:RequiredFieldValidator runat="server" ID="rfvEmail" ControlToValidate="txtEmail" CssClass="error" ForeColor="" Display="Dynamic">
											Please enter your email address.<br />
										</asp:RequiredFieldValidator>
										<asp:RegularExpressionValidator runat="server" ID="revEmail" 
											ControlToValidate="txtEmail" CssClass="error" ForeColor="" Display="Dynamic" 
											ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
											Please enter a valid email address.<br />
										</asp:RegularExpressionValidator>
										<asp:TextBox runat="server" ID="txtEmail" TabIndex="3" /><br />
										<small>Your email address is safe with us, until we're acquired.</small>
									</div>
								</div>
								
								<div class="row">
									<div class="field">
										<asp:Button runat="server" ID="btnSubmit" AccessKey="s" TabIndex="4" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn-submit" />
									</div>
								</div>
							</asp:PlaceHolder>
						</ContentTemplate>
					</asp:UpdatePanel>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel ID="tabMovie" runat="server" HeaderText="Youtube Video">
				<ContentTemplate>
					<div class="youtube">
						<div id="youtube">
							<p>Alternative content</p>
						</div>
					</div>
					<script type="text/javascript">
						swfobject.embedSWF("http://www.youtube.com/v/Oe3FG4EOgyU&hl=en", "youtube", "425", "344", "9.0.0");
					</script>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </form>
</body>
</html>
