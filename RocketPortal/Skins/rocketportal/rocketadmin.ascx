<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="META" Src="~/Admin/Skins/Meta.ascx" %>

<dnn:META ID="META1" runat="server" Name="viewport" Content="width=device-width,initial-scale=1" />

    <script src="/DesktopModules/DNNrocket/js/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="/DesktopModules/DNNrocket/Simplisity/js/simplisity.js?v=1"></script>

    <link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">

<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/jquery.validate.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.0/dist/additional-methods.min.js"></script>

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
				<div class="w3-light-grey">
				  <div id="myBar" class="w3-container w3-grey" style="height:24px;width:1%"></div>
				</div>  
			  
                <script>
                    $(document).ready(function () {
                        move();
                    });

                    function move() {
                        var elem = document.getElementById("myBar");
                        var width = 1;
                        var id = setInterval(frame, 200);
                        function frame() {
                            if (width >= 100) {
                                clearInterval(id);
                            } else {
                                width++;
                                elem.style.width = width + '%';
                            }
                        }
                    }
                </script>
        </div>
    </div>

	
    <script>

        $(document).ready(function () {
            $('#adminpanel').getSimplisity('/Desktopmodules/dnnrocket/api/rocket/action', 'rocketsystem_adminpanel', '{"systemkey":"<%= Server.HtmlEncode(PortalSettings.ActiveTab.TabName) %>"}', '')
            $('#adminpanel').show();
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






