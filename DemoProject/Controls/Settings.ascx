<%@ Import Namespace="System.Globalization"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DemoProject.Controls.Settings" %>

<div class="row">
	<label>Enabled:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.Enabled.ToString(CultureInfo.InvariantCulture) %>
	</div>
</div>

<div class="row">
	<label>CacheKey:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.CacheKey %>
	</div>
</div>

<div class="row">
	<label>ScriptsCacheKeyFormat:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.ScriptsCacheKeyFormat %>
	</div>
</div>

<div class="row">
	<label>CacheDuration:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.CacheDuration.ToString(CultureInfo.InvariantCulture) %>
	</div>
</div>

<div class="row">
	<label>HiddenFieldName:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.HiddenFieldName %>
	</div>
</div>

<div class="row">
	<label>AppVersion:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.AppVersion %>
	</div>
</div>

<div class="row">
	<label>EncryptQueryString:</label>
	<div class="field single">
		<%= AssetPackager.Configuration.Settings.EncryptQueryString.ToString(CultureInfo.InvariantCulture)%>
	</div>
</div>
