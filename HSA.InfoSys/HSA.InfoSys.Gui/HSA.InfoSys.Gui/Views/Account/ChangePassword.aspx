<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HSA.InfoSys.Gui.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Kennwort ändern
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container">

        <% if (ViewData["success"] != null) { %>
            <div class="alert alert-success">
                <button type="button" class="close" data-dismiss="alert">
                    &times;</button>
                <h4>Success</h4>
                <p><%= ViewData["success"] %></p>
            </div>
        <% } %>

        <% if (!Html.ViewData.ModelState.IsValid) { %>
            <div class="alert alert-error">
                <button type="button" class="close" data-dismiss="alert">
                    &times;</button>
                <h4>Warning!</h4>
                <%= Html.ValidationSummary(true, ""+ViewData["invalidMsg"])%>
                <%= Html.ValidationMessageFor(m => m.OldPassword) %>
                <%= Html.ValidationMessageFor(m => m.NewPassword) %>
                <%= Html.ValidationMessageFor(m => m.ConfirmPassword)%>
            </div>
        <% } %>

        <div class="contentbox cb-large left-nomargin">
            <div class="contentbox-header">
                <i class="icon-wrench"></i>
                <b>Change Password <%= ViewData["success"] %></b>
            </div>
            <div class="contentbox-content cutline-bot">
                <span>Neue Kennwörter müssen eine Länge von mindestens <%= Html.Encode(ViewData["PasswordLength"]) %> Zeichen aufweisen.</span>
            </div>
            <div class="contentbox-content">
                <form class="form-horizontal" method="post" action="/Account/ChangePassword">
                    <div id="OldPasswordGroup" class="control-group">
                        <label class="control-label" for="inputPassword">
                            <%= Html.LabelFor(m => m.OldPassword)%>
                        </label>
                        <div class="controls">
                            <%= Html.PasswordFor(m => m.OldPassword) %>
                        </div>
                    </div>
                    <div id="NewPasswordGroup" class="control-group">
                        <label class="control-label" for="inputPassword">
                            <%= Html.LabelFor(m => m.NewPassword)%>
                        </label>
                        <div class="controls">
                            <%= Html.PasswordFor(m => m.NewPassword) %>
                        </div>
                    </div>
                    <div id="ConfirmPasswordGroup" class="control-group">
                        <label class="control-label" for="inputPassword">
                            <%= Html.LabelFor(m => m.ConfirmPassword)%>
                        </label>
                        <div class="controls">
                            <%= Html.PasswordFor(m => m.ConfirmPassword) %>
                        </div>
                    </div>
                    <div class="control-group">
                        <div class="controls">
                            <button type="submit" class="btn"><i class="icon-play"></i>
                            &nbsp;&nbsp;Change Password</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {

            // Add Placeholders
            $("#OldPassword").attr("placeholder", "Old Password");
            $("#NewPassword").attr("placeholder", "New Password");
            $("#ConfirmPassword").attr("placeholder", "Confirm New Password");


            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("OldPassword") %>' === 'False') {
                $("#OldPasswordGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("NewPassword") %>' === 'False') {
                $("#NewPasswordGroup").addClass("control-group error");
            }

            // Error Input
            if ('<%= Html.ViewData.ModelState.IsValidField("ConfirmPassword") %>' === 'False') {
                $("#ConfirmPasswordGroup").addClass("control-group error");
            }

        });
    </script>

</asp:Content>
