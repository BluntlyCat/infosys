<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    My Systems
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div class="contentbox" style="width: 100%;">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>&nbsp;<b>My Systems</b>
            </div>
            <div class="contentbox-content">
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th style="width: 35px;">
                                <i class="icon-search"></i>
                            </th>
                            <th style="width: 600px;">
                                System
                            </th>
                            <th>
                                Edit
                            </th>
                            <th>
                                Delete
                            </th>
                            <th>
                                Search
                            </th>
                            <th>
                                Results
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        
                        <% 
                            if (this.ViewData["orgUnits"] as List<OrgUnit> != null)
                            {
                                var items = this.ViewData["orgUnits"] as List<OrgUnit>;
                                HSA.InfoSys.Common.Logging.Logger<string>.GetLogger("SystemIndex").DebugFormat("Items: {0}", items);
                                foreach (var item in items)
                                { %>

                                <tr>
                                    <td>
                                        <input type="checkbox" value="1" />
                                    </td>
                                    <td>
                                        <%= item.Name%>
                                    </td>
                                    <td>
                                        <a class="btn btn-small" href="/System/Components/?sysguid=<%= item.EntityId %>"><i class="icon-cog"></i></a> 
                                    </td>
                                    <td>
                                        <a class="btn btn-small" href="#deleteModal<%= item.EntityId %>" data-toggle="modal"><i class="icon-trash"></i></a>
                                    </td>
                                    <td>
                                        <a class="btn btn-small" href="/System/RealTimeSearch/?sysguid=<%= item.EntityId %>"><i class="icon-search"></i></a>
                                    </td>
                                    <td>
                                        <a class="btn btn-small" href="/System/ShowResults/?sysguid=<%= item.EntityId %>"><i class="icon-list"></i></a>
                                    </td>
                                </tr>

                                <!-- deleteModal -->
                                <div id="deleteModal<%= item.EntityId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                                    aria-hidden="true">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                            ×</button>
                                        <h3 id="H1">
                                            Delete System</h3>
                                    </div>
                                    <div class="modal-body">
                                        <p>Are you sure you want to delete the following system?</p>
                                        <span class="label"><%= item.Name%></span>
                                    </div>
                                    <div class="modal-footer">
                                        <button class="btn" data-dismiss="modal" aria-hidden="true">
                                            Close</button>
                                        <a class="btn btn-danger" href="/System/DeleteOrgUnit?sysguid=<%= item.EntityId %>"">
                                            <i class="icon-trash icon-white"></i>&nbsp;&nbsp;<b>Delete</b></a>
                                    </div>
                                </div>
                                <% } %>
                          <% } %>
                    </tbody>
                </table>
            </div>
            <div id="buttons" style="float: right;">
                <a class="btn btn-success" style="margin-bottom: 10px; margin-right: 10px;" href="#createModal" data-toggle="modal">
                    <i class="icon-plus icon-white"></i>&nbsp;&nbsp;<b>Create new System</b></a>
            </div>
        </div>
    </div>

    <!-- createModal -->
    <form id="Form1" action="/System/IndexSubmit" method="post">
    <div id="createModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                ×</button>
            <h3 id="myModalLabel">
                Create new system</h3>
        </div>
        <div class="modal-body">
            <p>Please enter a name for your new system:</p>
            <input type="text" name="newsystem" style="width: 440px;" placeholder="i.e. Webserver, Working-Station, Backup-Server">
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal" aria-hidden="true">
                Close</button>
            <button id="createSystem" class="btn btn-success"><i class="icon-plus icon-white"></i>&nbsp;&nbsp;<b>Create</b></button>
        </div>
    </div>
    </form>

    


    <script>
        $(document).ready(function () {

            $('#createSystem').click(function () {
                $('#createSystemForm').submit();
            });

            // addButton click event
            $('#addButton').click(function () {
                addInputBox();
            });

            // clearAllButton click event
            $('#clearAllButton').click(function () {
                // delete all added InputBoxes
                $('#addedBoxes').empty();
                // set value of mainInputBox to empty
                $('#mainInputBox').val('');
            });

        });

        /**
        * adds a new component-InputBox
        */
        function addInputBox() {

            // get Value of mainInputBox
            var value = $('#mainInputBox').val();

            // clear Value of mainInputBox
            $('#mainInputBox').val('');

            // append the new InputBox
            var newInputBox = '<input class="input-components" type="text" placeholder="component" value="' + value + '" name="components[]">';
            var newButton = '<button type="button" class="btn btn-danger" onclick="removeInputBox($(this))"><i class="icon-minus-sign"></i></button>';
            $('#addedBoxes').append(
                    '<div class="input-append">'
                    + newInputBox
                    + newButton
                    + '</div>'
                );
        }

        /**
        * deletes a component-InputBox
        */
        function removeInputBox(button) {

            // remove the parent element of the button (in this case the div)
            button.parent().remove();

            //$('#addedBoxes').find(':last-child').not(':only-child').remove();
        }

    </script>

</asp:Content>
