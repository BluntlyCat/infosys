<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HSA.InfoSys.Gui.Models.RegisterModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">


    <div class="container">
        <% if (!Html.ViewData.ModelState.IsValid) { %>
            <div class="alert alert-error">
                <button type="button" class="close" data-dismiss="alert">
                    &times;</button>
                <h4>Warning!</h4>
                <%= Html.ValidationSummary(true, ""+ViewData["invalidRegisterMsg"])%>
                <%= Html.ValidationMessageFor(m => m.UserName) %>
                <%= Html.ValidationMessageFor(m => m.Email) %>
                <%= Html.ValidationMessageFor(m => m.Password) %>
                <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %>
            </div>
        <% } %>
        
        <div class="contentbox cb-large left-nomargin">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>
                <b>Register</b>
            </div>
            <div class="contentbox-content cutline-bot">
                <span>Verwenden Sie zum Erstellen eines neuen Kontos das Formular weiter unten.<br /> 
            Kennwörter müssen eine Länge von mindestens <%= Html.Encode(ViewData["PasswordLength"]) %> Zeichen aufweisen.</span>
            </div>
            <div class="contentbox-content">
                <form class="form-horizontal" method="post" action="/Account/Register">
                    <div id="UserNameGroup" class="control-group">
                        <label class="control-label" for="inputEmail">
                            <%= Html.LabelFor(m => m.UserName) %>
                        </label>
                        <div class="controls">
                            <%= Html.TextBoxFor(m => m.UserName) %>
                        </div>
                    </div>
                    <div id="EmailGroup" class="control-group">
                        <label class="control-label" for="inputEmail">
                            <%= Html.LabelFor(m => m.Email) %>
                        </label>
                        <div class="controls">
                            <%= Html.TextBoxFor(m => m.Email) %>
                        </div>
                    </div>
                    <div id="PasswordGroup" class="control-group">
                        <label class="control-label" for="inputPassword">
                            <%= Html.LabelFor(m => m.Password) %>
                        </label>
                        <div class="controls">
                            <%= Html.PasswordFor(m => m.Password) %>
                        </div>
                    </div>
                    <div id="ConfirmPasswordGroup" class="control-group">
                        <label class="control-label" for="inputPassword">
                            <%= Html.LabelFor(m => m.ConfirmPassword)%>
                        </label>
                        <div class="controls">
                            <%= Html.PasswordFor(m => m.ConfirmPassword)%>
                        </div>
                    </div>
                    <div id="ReCaptchaGroup" style="margin-left: 15px; margin-bottom: 15px;">
                        <%= Html.GenerateCaptcha() %>
                    </div>
                    <div class="control-group">
                        <div class="controls">
                            <button type="submit" class="btn"><i class="icon-play"></i>
                            &nbsp;&nbsp;Register</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        

    </div>

    <script>
        $(document).ready(function () {

            // Add Placeholders
            $("#UserName").attr("placeholder", "Username");
            $("#Email").attr("placeholder", "Email");
            $("#Password").attr("placeholder", "Password");
            $("#ConfirmPassword").attr("placeholder", "Confirm Password");


            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("UserName") %>' === 'False') {
                $("#UserNameGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("Email") %>' === 'False') {
                $("#EmailGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("Password") %>' === 'False') {
                $("#PasswordGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("ConfirmPassword") %>' === 'False') {
                $("#ConfirmPasswordGroup").addClass("control-group error");
            }

        });
    </script>

</asp:Content>
