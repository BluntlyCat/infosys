<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HSA.InfoSys.Gui.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Login
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div id="messageBoxes">
            <% if (!Html.ViewData.ModelState.IsValid) { %>
                <div class="alert alert-error">
                    <button type="button" class="close" data-dismiss="alert">
                        &times;</button>
                    <h4>Warning!</h4>
                    <%= Html.ValidationSummary(true, ""+ViewData["invalidLoginMsg"])%>
                    <%= Html.ValidationMessageFor(m => m.UserName) %>
                    <%= Html.ValidationMessageFor(m => m.Password) %>
                    <%= Html.ValidationMessageFor(m => m.RememberMe)%>
                </div>
            <% } %>
        </div>

        <div class="contentbox cb-large left-nomargin">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>
                <b>Login</b>
            </div>
            <div class="contentbox-content cutline-bot">
                <span>Please enter your <b>username</b> and your <b>password</b>.</span>
            </div>
            <div class="contentbox-content">
                <form class="form-horizontal" method="post" action="/Account/LogOn">
   
                    <div id="UserNameGroup" class="control-group">
                        <label class="control-label" for="inputEmail">
                            <%= Html.LabelFor(m => m.UserName) %>
                        </label>
                        <div class="controls">
                            <%= Html.TextBoxFor(m => m.UserName) %>
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
                    <div class="control-group">
                        <div class="controls">
                            <label class="checkbox">
                                <%= Html.CheckBoxFor(m => m.RememberMe) %>
                                <%= Html.LabelFor(m => m.RememberMe) %>
                            </label>
                            <button type="submit" class="btn"><i class="icon-play"></i>
                            &nbsp;&nbsp;Sign in</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <div class="contentbox cb-medium right-nomargin">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>
                <b>Register</b>
            </div>
            <div class="contentbox-content cutline-bot">
                <span>To open your free account please register now</span>
            </div>
            <div class="contentbox-content">
                <a class="btn btn-primary" href="/Account/Register"><i class="icon-list-alt icon-white"></i>
                &nbsp;&nbsp;<b>Register</b></a>
            </div>
        </div>

    </div>

    <script>
        $(document).ready(function () {

            // Add Placeholders
            $("#UserName").attr("placeholder", "Username");
            $("#Password").attr("placeholder", "Password");

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("UserName") %>' === 'False') {
                $("#UserNameGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("Password") %>' === 'False') {
                $("#PasswordGroup").addClass("control-group error");
            }

            // check if cookies enabled
            if (navigator.cookieEnabled === false) {
                // create error box
                 $('#messageBoxes').append(
                    '<div class="alert alert-error">'
                    + '<button type="button" class="close" data-dismiss="alert">&times;</button><h4>Warning!</h4>'
                    + 'Die Cookies sind in Ihrem Browser nicht aktiviert. Für den Login werden Cookies benötigt.'
                    + '</div>'
                );
            }

        });
    </script>

</asp:Content>


