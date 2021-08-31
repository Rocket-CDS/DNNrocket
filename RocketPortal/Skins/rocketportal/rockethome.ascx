<%@ Control Language="C#" CodeBehind="~/DesktopModules/Skins/skin.cs" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>


<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USER" Src="~/Admin/Skins/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGIN" Src="~/Admin/Skins/Login.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>


<div id="siteWrapper">

    <!-- UserControlPanel  -->
    <div id="topHeader">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="language">
                        <dnn:LANGUAGE runat="server" id="LANGUAGE1"  showMenu="False" showLinks="True" />
                    </div>
                    <div id="login" class="pull-right">
                        <dnn:LOGIN ID="dnnLogin" CssClass="LoginLink" runat="server" LegacyMode="false" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Page Content -->
    <div class="container">
        <main class="no-bg" role="main">
            <div id="mainContent-inner">                     
                <div class="row dnnpane">
                    <div id="ContentPane" class="col-md-12 contentPane" runat="server"></div>
                </div>
            </div><!-- /.mainContent-inner -->
        </main><!-- /.mainContent -->
    </div><!-- /.container -->

    
</div><!-- /.SiteWrapper -->


    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>

    <link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">



