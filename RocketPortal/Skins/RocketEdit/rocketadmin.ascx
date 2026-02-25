<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>
<dnn:JQUERY ID="jquertydnninject" runat="server" />
<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

<script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>
<link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
<link rel="stylesheet" href="/DesktopModules/DNNrocket/Simplisity/css/simplisity.css">
<link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/css/dnn-theme.css">
<link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
<link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

<style>
    /* Hide and remove width from PersonaBar */
    .personaBarContainer { 
        display: none !important; 
        width: 0 !important;
    }
    
    /* Also hide the iframe inside */
    #personaBar-iframe { 
        width: 0 !important;
        display: none !important;
    }
    
    /* Remove body margin added for PersonaBar */
    body { 
        margin-left: 0px !important; 
    }
    
    /* Additional overrides */
    #editBarContainer { display: none !important; }
    #Body { margin-left: 0px !important; }
    iframe.editBar-iframe { display: none !important; }
    .material-icons { vertical-align: middle; }
</style>
  
<div id="ContentPane" class="contentPane" runat="server"></div>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        var loader = document.querySelector(".simplisity_loader");      
        if (loader) {
            loader.style.display = "none";
        }
    });
</script>
