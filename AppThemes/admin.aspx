<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="DNNrocketAPI" %>

<script runat="server">

    public string ModuleId { get; set; }
    public string TabId { get; set; }
    public string IframeEdit { get; set; }
    public string displaynone { get; set; }
    public string editlang { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        // We are using an aspx page querystring param so we can pass the moduleid from DNN.  The API requires this.
        ModuleId = HttpContext.Current.Request.QueryString["moduleid"];
        TabId = HttpContext.Current.Request.QueryString["tabid"];
        editlang = HttpContext.Current.Request.QueryString["editlang"];
    }

</script>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Panel</title>


    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/css/coconutmilk-theme.css">

    <!-- CDN -->
    <!-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    <link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <script src="https://code.jquery.com/jquery-3.3.1.min.js" integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js" integrity="sha256-VazP97ZCwtekAsvgPBSUwPFKdrwD3unUfSGVYrahUqU=" crossorigin="anonymous"></script>
    -->
    <!-- LOCAL -->
    <!-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ -->
    <link rel="stylesheet" href="/DesktopModules/DNNrocket/css/w3.css">
    <link rel="stylesheet" href="/DesktopModules/DNNrocket/fa/css/font-awesome.min.css">
    <script src="/DesktopModules/DNNrocket/js/jquery-3.3.1.min.js"></script>
    <script src="/DesktopModules/DNNrocket/js/jquery-ui.min.js"></script>
    <!-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ -->



    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js"></script>
    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/jquery.fileupload.js"></script>

    <script src="https://cdn.ckeditor.com/4.6.2/standard/ckeditor.js"></script>

</head>
<body class="w3-light-grey">


    <!-- Top container -->
    <div class="w3-bar w3-top w3-theme-d3 w3-large w3-padding-small " id="menubar" style="z-index:2;height:55px">
        <div class="w3-container">
            <div class="w3-threequarter">
                <button class="w3-bar-item w3-button w3-hide-large w3-hover-none w3-hover-text-light-grey" onclick="w3_open();"><i class="fa fa-bars"></i></button>
                <span id="menubuttons" class="w3-margin-left simplisity_buttonpanel"></span>
            </div>
            <div class="w3-quarter w3-right-align">
                <span id="menulangflags" class="w3-right-align"></span>
            </div>
        </div>
    </div>


    <!-- Side Navigation (use mask div if in iframe editmode)-->
    <nav class="w3-sidebar w3-bar-block w3-collapse w3-theme-d5 w3-animate-left w3-card" style="z-index:3;width:260px;top: 55px;" id="mySidebar">
        <img src="/DesktopModules/DNNrocket/API/Themes/config-w3/img/img_avatar4.png" alt="Avatar" style="width:20%" class="w3-circle w3-margin">
        <div id="sidebarplaceholder" class="simplisity_panel" s-cmd="getsidemenu" s-fields="tabid:<%= String.IsNullOrEmpty(TabId) ? "" : TabId %>,moduleid:<%= String.IsNullOrEmpty(ModuleId) ? "" : ModuleId %>,theme:config-w3,template:SideMenu.cshtml,interfacekey:rocketapptheme,relpath:/DesktopModules/DNNrocket/AppThemes/"></div>
    </nav>

        <!-- !PAGE CONTENT! -->
    <div class="w3-main" style="margin-left:300px;margin-top:60px; " id="base-panel">

        <div id="simplisity_startpanel" class="simplisity_panel" s-cmd="rocketapptheme_edit" s-track="true" s-fields="tabid:<%= String.IsNullOrEmpty(TabId) ? "" : TabId %>,moduleid:<%= String.IsNullOrEmpty(ModuleId) ? "" : ModuleId %>,theme:config-w3,template:edit.cshtml,interfacekey:rocketapptheme"></div>

        <!-- End page content -->
    </div>

        <script>
            $(document).ready(function () {
                $(document).simplisityStartUp('/DesktopModules/DNNrocket/api/api.ashx', { systemprovider: 'dnnrocketapptheme', usehistory: false });
            });

            // Get the Sidebar
            var mySidebar = document.getElementById("mySidebar");

            // Get the DIV with overlay effect
            var overlayBg = document.getElementById("myOverlay");

            // Toggle between showing and hiding the sidebar, and add overlay effect
            function w3_open() {
                if (mySidebar.style.display === 'block') {
                    mySidebar.style.display = 'none';
                    overlayBg.style.display = "none";
                } else {
                    mySidebar.style.display = 'block';
                    overlayBg.style.display = "block";
                }
            }

            // Close the sidebar with the close button
            function w3_close() {
                mySidebar.style.display = "none";
                overlayBg.style.display = "none";
            }


        </script>


</body>
</html>

