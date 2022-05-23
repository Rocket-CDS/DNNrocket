<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>

<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USER" Src="~/Admin/Skins/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGIN" Src="~/Admin/Skins/Login.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>

<dnn:JQUERY ID="jquertydnninject" runat="server" />

<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

    <!-- UserControlPanel  -->
	<div class="w3-row w3-padding">
		<div class="w3-left w3-padding">
			<dnn:LANGUAGE runat="server" id="LANGUAGE1"  showMenu="False" showLinks="True" />
		</div>
		<div id="login" class="w3-right w3-padding">
			<dnn:LOGIN ID="dnnLogin" CssClass="w3-button" runat="server" LegacyMode="false" />
		</div>
	</div>

		<div id="ContentPane" runat="server" valign="top" class="w3-row w3-center">

				<a class="w3-bar-item w3-button w3-green w3-margin w3-xlarge" href="/" >Admin</a>

		</div>


    <script>
        $(document).ready(function () {

        });
    </script>


    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>

    <link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">





