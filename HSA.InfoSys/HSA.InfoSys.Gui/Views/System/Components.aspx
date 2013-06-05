<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    My Systems
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">

        <div class="contentbox" style="width:100%;">
            <div class="contentbox-header">
                <i class="icon-align-justify"></i>&nbsp;<b>Webserver lnx07</b>
            </div>
            <div class="contentbox-content cutline-bot">
                <a class="btn btn-primary" style="margin-right: 6px;" href="/System/Components?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Components</b></a>
                <a class="btn" style="margin-right: 6px;" href="/System/SearchConfig?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Search Configuration</b></a>
            </div>
            <div class="contentbox-content cutline-bot">
                <span>Add all your components here</span>
            </div>

            <form action="/System/ComponentsSubmit?sysguid=<%= ViewData["systemguid"] %>" method="post">

                <div class="contentbox-content cutline-bot">
                    <div id="allBoxes" style="margin-left: 100px;">
                        <!-- addedBoxes -->
                        <div id="addedBoxes">
                        </div>
                        <!-- mainInputBox -->
                        <div class="input-append">
                            <input id="mainInputBox" class="input-components" type="text" placeholder="component"
                                name="components[]" />
                            <button id="addButton" type="button" class="btn">
                                <i class="icon-plus"></i>
                            </button>
                        </div>
                    </div>
                </div>
                <div class="contentbox-content">
                        <button type="submit" class="btn btn-success" style="float:right; margin-bottom: 10px; margin-right: 10px;">
                            <i class="icon-check icon-white"></i>&nbsp;&nbsp;<b>Save Changes</b></button>
                </div>
            </form>
        </div>

    </div>

    <script>
        $(document).ready(function () {

            // addButton click event
            $('#addButton').click(function () {
                addInputBox();
            });
            
            /*
            // clearAllButton click event
            $('#clearAllButton').click(function () {
                // delete all added InputBoxes
                $('#addedBoxes').empty();
                // set value of mainInputBox to empty
                $('#mainInputBox').val('');
            });
            */

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
            var newButton = '<button type="button" class="btn" onclick="removeInputBox($(this))"><i class="icon-minus"></i></button>';
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
