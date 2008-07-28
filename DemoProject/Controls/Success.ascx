<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Success.ascx.cs" Inherits="DemoProject.Controls.Success" %>

<div class="row">
	<asp:Label runat="server" ID="lblRating" Text="Rating: " AssociatedControlID="atrRating" />
	<div class="field">
		<ajaxToolkit:Rating ID="atrRating" runat="server" CurrentRating="2"
			MaxRating="5" ReadOnly="false" style="float: left;"
			StarCssClass="ratingStar"
			WaitingStarCssClass="savedRatingStar"
			FilledStarCssClass="filledRatingStar"
			EmptyStarCssClass="emptyRatingStar" />
		<div style="clear:both"><!-- --></div>
	</div>
</div>
