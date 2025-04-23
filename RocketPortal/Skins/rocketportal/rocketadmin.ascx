<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>
<dnn:JQUERY ID="jquertydnninject" runat="server" />
<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

<style>
    #editBarContainer { display: none !important }
    .personalBarContainer { display: none !important }
    #Body { margin-left: 0px !important }
    .material-icons { vertical-align: middle; }
    iframe.editBar-iframe{ display: none !important; }
</style>
  
<div id="ContentPane" class="contentPane" runat="server"></div>