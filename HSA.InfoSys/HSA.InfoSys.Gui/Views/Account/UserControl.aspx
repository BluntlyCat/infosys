<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="userControlTitle" ContentPlaceHolderID="TitleContent" runat="server">
    User Control
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div>
    <table class="table table-striped">
    <thead>
        <tr>
            <th>Username</th>
            <th>E-Mail</th>
            <th>Activated</th>
            <th>De/Activate</th>
            <th>Delete</th>
        </tr>
    </thead>
    <tbody>

        <% foreach (var item in Membership.GetAllUsers())
           {
               MembershipUser user = (MembershipUser)item;

               if (user.UserName != this.User.Identity.Name)
               { 
               %>
                <tr>
                    <td><%= user.UserName %></td>
                    <td><%= user.Email %></td>
                    <td><%= user.IsApproved %></td>

                    <% if (user.IsApproved){ %>
                        <td><a href="/Account/ActivateUser?username=<%= user.UserName %>"><i class="icon-remove"></i></a></td>
                    <%} else { %>
                        <td><a href="/Account/ActivateUser?username=<%= user.UserName %>"><i class="icon-ok"></i></a></td>
                    <% } %>            
                    <td><a href="/Account/DeleteUser?username=<%= user.UserName %>"><i class="icon-trash"></i></a></td>
                </tr>
            <% } %>
        <% } %>
    </tbody>

    </table>
</div>
</asp:Content>
