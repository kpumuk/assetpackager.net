<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contact.ascx.cs" Inherits="DemoProject.Controls.Contact" %>

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
