<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About</h2>
    <p>
        <%= ViewData["message"] %>
    </p>

    <h4>Done!</h4>
    <ul>
        <li>reCaptcha eingebaut</li>
        <li>cookiesEnabled check eingebaut</li>
        <li>Register Funktion eingebaut</li>
        <li>User Einstellungen eingebaut</li>
        <li>Password ändern Option eingebaut</li>
        <li>navbar rechts -> optisch geändert (Login-/Register-Button u. Logoutbutton)</li>
    </ul>
</asp:Content>