<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>

<dnn:JQUERY ID="jquertydnninject" runat="server" />


<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

    <link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">

    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js?v=1"></script>
    <link rel="stylesheet" href="/DesktopModules/DNNrocket/Simplisity/css/simplisity.css">

    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/jquery.validate.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/additional-methods.min.js"></script>

<link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/css/rocketcds-theme.css">

<style>
    body,h1,h2,h3,h4,h5,h6 {font-family: Roboto, sans-serif;}
    #editBarContainer { display: none !important }
    .personalBarContainer { display: none !important }
    #Body { margin-left: 0px !important }
    .material-icons { vertical-align: middle; }
    iframe.editBar-iframe{ display: none !important; }
</style>

<script>
    $(document).ready(function () {
    });
</script>

<div class="w3-container w3-margin-top ">


    <div class="w3-col m1">
    &nbsp;
    </div>
    
    <div id="ContentPane" class="w3-col m10 contentPane" runat="server">
        <div class="simplisity_loader">
            <span class=" simplisity_loader_inner">
            </span>
        </div>
    </div>
    
    <div class="w3-col m1">
    &nbsp;
    </div>

</div>






