<%@ Control Language="C#" CodeBehind="~/DesktopModules/Skins/skin.cs" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>


<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USER" Src="~/Admin/Skins/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGIN" Src="~/Admin/Skins/Login.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>

<div id="w3-container">
    <!-- UserControlPanel  -->
	<div class="w3-row w3-padding">
		<div class="w3-left w3-padding">
			<dnn:LANGUAGE runat="server" id="LANGUAGE1"  showMenu="False" showLinks="True" />
		</div>
		<div id="login" class="w3-right w3-padding">
			<dnn:LOGIN ID="dnnLogin" CssClass="w3-button" runat="server" LegacyMode="false" />
		</div>
	</div>


	<%
  var isLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated;
  if (isLoggedIn)
  {
%>

    <div class="w3-row w3-center">
		<div class="w3-row ">
			<div id="ContentPane" class="contentPane" runat="server"></div>
		</div>
    </div>


<% } %>
<% else %>
<% { %>

    <div class="w3-row w3-center">
		<div class='w3-jumbo'>Toasted</div>
    </div>

<% } %>



    
</div>

    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>

    <link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">




