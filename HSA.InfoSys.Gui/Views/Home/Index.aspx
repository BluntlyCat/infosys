<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Informationssystem für sicherheitskritische Komponenten
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <p>Willkommen auf dem Informationssystem für sicherheitskritische Komponenten!</p>

    <script type="text/javascript">
        $(document).ready(function () {

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
