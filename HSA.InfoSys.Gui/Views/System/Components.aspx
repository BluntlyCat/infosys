<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    My Systems
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">

        <div class="contentbox" style="width:100%;">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>&nbsp;<b><%= ViewData["orgUnitName"] %></b>
            </div>
            <div class="contentbox-content cutline-bot">
                <a class="btn btn-primary" style="margin-right: 6px;" href="/System/Components?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Components</b></a>
                <a class="btn" style="margin-right: 6px;" href="/System/SearchConfig?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Search Configuration</b></a>
            </div>

            <form action="/System/SubmitComponent?sysguid=<%= ViewData["systemguid"] %>" method="post">

                <div class="contentbox-content">

                    <!-- InputBox -->
                    <input id="mainInputBox" class="input-components" type="text" placeholder="new component" name="components" />
                    <!-- save button -->
                    <button type="submit" class="btn btn-success" style="margin-bottom: 10px; margin-left: 5px;">
                        <i class="icon-check icon-white"></i>&nbsp;&nbsp;<b>Save Component</b>
                    </button>

                    <!-- Components Table -->
                    <% if (this.ViewData.ContainsKey("components")) { %>
                        <table class="table table-hover table-bordered" style="width: 910px;">
                            <thead>
                                <tr>
                                    <th style="width: 800px;">
                                        Component
                                    </th>
                                    <th>
                                        Results
                                    </th>
                                    <th>
                                        Delete
                                    </th>
                                </tr>
                            </thead>
                            <tbody>

                                <% foreach (var item in this.ViewData["components"] as List<Component>)
                                    { %>

                                    <tr>
                                        <td>
                                            <%= item.Name %>
                                        </td>
                                        <td>
                                            <a class="btn btn-small" href="/System/ShowResults/?sysguid=<%= ViewData["systemguid"] %>&compguid=<%= item.EntityId %>"><i class="icon-list"></i></a>
                                        </td>
                                        <td>
                                            <a class="btn btn-small" href="/System/DeleteComponent?sysguid=<%= ViewData["systemguid"] %>&compid=<%= item.EntityId %>"><i class="icon-minus"></i></a> 
                                        </td>
                                    </tr>

                                <% } %>
                            </tbody>
                        </table>
                    <% } %>

                </div>

            </form>

        </div>

    </div>

    <script>
        $(document).ready(function () {

        });
    </script>

</asp:Content>
