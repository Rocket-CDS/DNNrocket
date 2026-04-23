<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>
<dnn:JQUERY ID="jquertydnninject" runat="server" />
<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />


<style>
    #editBarContainer { display: none !important; }
    .personaBarContainer { display: none !important; }
    .personalBarContainer { display: none !important; } /* typo-safe variant */
    #personaBar-iframe, iframe.editBar-iframe { display: none !important; }
    body.personabar-visible, #Body { margin-left: 0 !important; }
	
	.w3-container.default.clearfix {
		margin-top: 0 !important;
		padding: 0 !important;
	}
	#Body {
		margin-top: 0 !important;
		padding-top: 0 !important;
	}
</style>

<div id="ContentPane" class="contentPane" runat="server">
    <div class="simplisity_loader">
        <span class=" simplisity_loader_inner">
        </span>
    </div>
</div>
