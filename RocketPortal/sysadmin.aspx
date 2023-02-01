<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RocketCDS SysAdmin</title>

    <script type="text/javascript" src="/Resources/Libraries/jQuery/03_05_01/jquery.js"></script>

    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js?v=1"></script>
    <link rel="stylesheet" href="/DesktopModules/DNNrocket/Simplisity/css/simplisity.css">

    <link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/jquery.validate.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/additional-methods.min.js"></script>

<link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/css/rocketcds-theme.css">

<style>
    #editBarContainer { display: none !important }
    .personalBarContainer { display: none !important }
    #Body { margin-left: 0px !important }
    .material-icons { vertical-align: middle; }
    iframe.editBar-iframe{ display: none !important; }
</style>

</head>
<body>

        <div id="adminpanel">
            <div class="simplisity_loader">
                <span class=" simplisity_loader_inner">
                </span>
            </div>
        </div>    
        
        <script type="text/javascript">
    
            $(document).ready(function () {
                $('#adminpanel').getSimplisity('/Desktopmodules/dnnrocket/api/rocket/action', 'dashboard_adminpanel', '{"systemkey":"rocketportal"}', '')
            });
    
        </script>
    
    <div class="w3-container w3-center w3-padding-64">
    
    
        <div class="w3-third">
        &nbsp;
        </div>
        
        <div id="ContentPane" class="w3-third w3-center contentPane" runat="server"></div>
        
        <div class="w3-third">
        &nbsp;
        </div>
    
    </div>

</body>
</html>

