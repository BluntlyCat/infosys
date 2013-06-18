<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Informationssystem für sicherheitskritische Komponenten
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% foreach (var comp in (ViewData["components"] as Array)) { %>

        <p><%= comp %></p>

    <% } %>
   

</asp:Content>
