<%@ Control Language="C#" CodeBehind="~/DesktopModules/Skins/skin.cs" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>

    <script src="/DesktopModules/DNNrocket/js/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>

    <link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

<style>
#editBarContainer {
    display: none !important
}

.personalBarContainer {
    display: none !important
}
#Body {
    margin-left: 0px !important
}


</style>


    <div id="adminpanel" style="display:none;">
        <div class="w3-container w3-center w3-padding-64">
            <span class="material-icons w3-jumbo w3-spin ">
                motion_photos_on
            </span>
        </div>

    </div>

    <script>

        $(document).ready(function () {
            $('#adminpanel').getSimplisity('/Desktopmodules/dnnrocket/api/rocket/action', 'rocketportal_activesystems', '{"systemkey":"rocketportal"}', '')
            $('#adminpanel').show();
        });

    </script>




<div id="ContentPane" runat="server" valign="top"></div>





