﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Results
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
    <div class="contentbox accordion-inner">
        <div class="btn-group accordion-group">
            <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">choose Component<span class="caret">
            </span></a>
            <ul class="dropdown-menu">
                <% foreach (var item in this.ViewData["components"] as List<Component>){ %>
                     <li><a href="#"><%= item.Name %></a></li>
                  <% } %>
            </ul>
        </div>
        <hr />
        
            <!--dummy -->
            <table class="table table-bordered" >
                <tr style="background-color:#F5F5F5">
                    <td><i class="icon-exclamation-sign"></i>&nbsp;<b>Title des Resultsafsdfdsfsdfdsfsdfsdfsdfsdfsfsjfhsfionhsöfhjösiföosifhöiosföoidsfösidföosjsefd</b></td>
                    <td width="100px"><i class="icon-calendar"></i>&nbsp;10.01.2099</td>
                </tr>
                <tr>
                    <td colspan="2"><a href="http://www.heise.de">http://www.heise.de</a></td>
                </tr>
                <tr>
                    <td colspan="2">Jährlich fallen mehr als 20 Millionen Tonnen Bananenschalen an – mit einem großen Potenzial für die Reinigung von Abwässern und der Heilung von Arteriosklerose...</td>
                </tr>
            </table>

            <table class="table table-bordered" >
                <tr style="background-color:#F5F5F5">
                    <td><i class="icon-exclamation-sign"></i>&nbsp;<b>Title des Results</b></td>
                    <td width="100px"><i class="icon-calendar"></i>&nbsp;10.01.2099</td>
                </tr>
                <tr>
                    <td colspan="2"><a href="http://www.heise.de">http://www.heise.de</a></td>
                </tr>
                <tr>
                    <td colspan="2">Jährlich fallen mehr als 20 Millionen Tonnen Bananenschalen an – mit einem großen Potenzial für die Reinigung von Abwässern und der Heilung von Arteriosklerose...</td>
                </tr>
            </table>

            <table class="table table-bordered" >
                <tr style="background-color:#F5F5F5">
                    <td><i class="icon-exclamation-sign"></i>&nbsp;<b>Title des Results</b></td>
                    <td width="100px"><i class="icon-calendar"></i>&nbsp;10.01.2099</td>
                </tr>
                <tr>
                    <td colspan="2"><a href="http://www.heise.de">http://www.heise.de</a></td>
                </tr>
                <tr>
                    <td colspan="2">Jährlich fallen mehr als 20 Millionen Tonnen Bananenschalen an – mit einem großen Potenzial für die Reinigung von Abwässern und der Heilung von Arteriosklerose...</td>
                </tr>
            </table>
        </div>
    </div>

    <script>
        $(document).ready(function () {

        });
    </script>

</asp:Content>
