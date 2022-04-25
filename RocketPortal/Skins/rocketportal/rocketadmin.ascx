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


    <div id="adminpanel">
		<div class="w3-overlay  w3-white loader" style='display:block;z-index:999;'>
			<span class="w3-display-middle">
				<img class="w3-spin" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEgAAABICAQAAAD/5HvMAAACmklEQVRo3u3YrU9CURgG8EcHm5sfRebURACLSSsGRtBioN2G06CFIIVKISPFYOUPUItzFpNuWpTOGMHNoBM3UGe4cky691yvcM8H555N3xvd+/DzcO857wX4rz9bJdtAzDYSs43EbCMx20jMNhKzjcRsI7HwSTNwUMUJGrj/wTFMmkYeV76IEEhzqOA1AMYIKYoiuoExQwclcSuEGTIni45NnB24Ph/ZRg2bSCOBCbOcbR/METKIhLMxZn+szgVS4R0dSc+946IQ5uEaxQ3HecZquONH0cNZVBzQ4nBUOPPcNuhiTXGEjaOFB0zJgyrc+uwqDvlxtMDAUJY/Ql8I51LxNeiLw9DBuBwoz61PSulejKBJsnJyIdfcNqhaxyTtXG786pGIjDIoTdI+EBMPcEjAE3dIyNUoN1euiwfskfaalj3tgCRKPGmn6jehp7ZI4pl4e4O0p7WAlkliXbz9kbQvaAHNksQ78fZ30j6pBTRGEt/E299I+5gW0CRJfBdvvyPts1pACyTxUby9TtqXtIDo1tgQbz8j7RtaQDmSeCreXibt+1pANZK4J96+zj2kI8qcCJ5IosTcGMMHCVhRBmVIWg8zMhHnJOJQGXRE0q7Vb8Km4nmf4oa9vFzI+PcbWQtxxQH2knBeMC37f5UDc1jfv+9y61ORX+gpPATk9AOtca/iXcyrfPdOQM7voEU8c+tTHO6bben7g/xr1cO5QdQMxx9U8Pxu0kHSFIf5POgXnh9xXGTNcRh3SGS4bfDr2jbJYZhAAmlsooa2D8bFjllO/6tj9ssadN2avJUHXV0Uh/ugi4BeUcEcDFQQ0hXy8keoLtI9GjhBFY7c+KWbFHqVbAMNOjpCJ8E2EmwjwTYSbCPBNtJ//dX6BHY6L4BgfX6bAAAAAElFTkSuQmCC" />
			</span>
		</div>
    </div>

	
    <script>

        $(document).ready(function () {
            $('#adminpanel').getSimplisity('/Desktopmodules/dnnrocket/api/rocket/action', '<%= Server.HtmlEncode(PortalSettings.ActiveTab.TabName.ToLower()) %>_adminpanel', '{"systemkey":"<%= Server.HtmlEncode(PortalSettings.ActiveTab.TabName.ToLower()) %>"}', '')
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






