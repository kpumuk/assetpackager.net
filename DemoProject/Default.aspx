<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DemoProject._Default" %>
<%@ Register Assembly="AssetPackager" Namespace="AssetPackager.WebControls" TagPrefix="apn" %>
<%@ Register Src="~/Controls/Contact.ascx" TagName="Contact" TagPrefix="apn" %>
<%@ Register Src="~/Controls/Settings.ascx" TagName="Settings" TagPrefix="apn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AssetPackager.Net Demo Site</title>
</head>
<body>
    <form id="form1" runat="server">
        <apn:AssetPackagerScriptManager ID="smCurrent" runat="server">
			<Scripts>
				<asp:ScriptReference Path="~/javascripts/swfobject.js" />
			</Scripts>
        </apn:AssetPackagerScriptManager>
        <ajaxToolkit:TabContainer ID="tbcPages" runat="server" CssClass="linkedin linkedin-blue">
			<ajaxToolkit:TabPanel ID="tabContactAjax" runat="server" HeaderText="Contact Form (Ajax)">
				<ContentTemplate>
					<asp:UpdatePanel runat="server" ID="upForm" UpdateMode="Conditional">
						<ContentTemplate>
							<apn:Contact runat="server" ID="ucContactAjax" />
						</ContentTemplate>
					</asp:UpdatePanel>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel ID="tabContactPostback" runat="server" HeaderText="Contact Form (Postback)">
				<ContentTemplate>
					<apn:Contact runat="server" ID="ucContactPostback" />
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
			<ajaxToolkit:TabPanel ID="tabSettings" runat="server" HeaderText="Settings">
				<ContentTemplate>
					<apn:Settings runat="server" ID="ucSettings" />
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </form>
</body>
</html>
