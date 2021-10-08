<%@ Control Language="C#" CodeBehind="~/DesktopModules/Skins/skin.cs" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Skin" %>

<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USER" Src="~/Admin/Skins/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGIN" Src="~/Admin/Skins/Login.ascx" %>
<%@ Register TagPrefix="dnn" TagName="jQuery" src="~/Admin/Skins/jQuery.ascx" %>

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

<%
  var isLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated;
  if (isLoggedIn)
  {
%>


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

            // clear sessionparam, incase they are invalid after an error.
            simplisity_sessionremove();

            $('#adminpanel').getSimplisity('/Desktopmodules/dnnrocket/api/rocket/action', 'dashboard_adminpanel', '{"systemkey":"rocketportal"}', '')
            $('#adminpanel').show();

        });

    </script>

<%
}
else
  {
%>

<div class="w3-row w3-orange w3-padding">
&nbsp;
</div>

<div class="w3-container w3-center w3-padding-64">


    <div class="w3-third">
    &nbsp;
    </div>
    
    <div id="ContentPane" class="w3-third w3-center contentPane" runat="server"></div>
    
    <div class="w3-third">
    &nbsp;
    </div>

</div>


<% } %>



