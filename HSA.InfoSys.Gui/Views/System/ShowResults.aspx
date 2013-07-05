<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Results
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container" >
        <div class="contentbox" style="width:100%;">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>&nbsp;<b>Results</b>
            </div>
            <div class="btn-group accordion-group" style="margin:15px;">
                <a class="btn dropdown-toggle" data-toggle="dropdown" href="#"><%= this.ViewData["selectedComp"] %> <span class="caret">
                </span></a>
                <ul class="dropdown-menu">
                    <% foreach (var item in this.ViewData["components"] as List<Component>){ %>
                         <li><a href="/System/ShowResults/?sysguid=<%= this.ViewData["systemguid"] %>&compguid=<%= item.EntityId %>""><%= item.Name %></a></li>
                      <% } %>
                </ul>
            </div>
            <hr />
        
            <% foreach (var item in this.ViewData["results"] as List<Result>) { %>
            <table class="table table-bordered">
                <tr style="background-color: #F5F5F5">
                    <td>
                        <i class="icon-exclamation-sign"></i>&nbsp;<b><%= item.Title %></b>
                    </td>
                    <td width="100px"><i class="icon-calendar"></i>&nbsp;<%= item.Time %></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <a href="<%= item.URL %>"><%= item.URL %></a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"><%= item.Content %></td>
                </tr>
            </table>
            <% } %>
        </div>
    </div>

    <script>
        $(document).ready(function () {

        });
    </script>

</asp:Content>
