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
                <a class="btn" style="margin-right: 6px;" href="/System/Components">
                    <b>Components</b></a>
                <a class="btn btn-primary" style="margin-right: 6px;" href="/System/SearchConfig">
                    <b>Search Configuration</b></a>
            </div>
            <div class="contentbox-content cutline-bot">
                <p><b>Saved Configs</b></p>
                <span>Du kannst bereits gespeicherte Konfigurationen deiner anderen Systeme für dieses System verwenden.<br />
                Wähle einfach die entsprechende Konfiguration, die geladen werden soll:</span>
                <div id="loadconfig" style="width: 500px; margin-left: 100px; margin-top: 20px;">
                    <select>
                        <option></option>
                        <option>Workstation1</option>
                        <option>Backup-Server 07</option>
                    </select>
                    <button type="submit" class="btn" style="margin-bottom: 10px; margin-left: 5px;"><b>Load</b></button>
                </div>
            </div>
            <div class="contentbox-content cutline-bot">
                <p><b>E-Mail</b></p>
                <span>Lass Dir Deine Suchergebnisse per E-Mail schicken. Hier kannst Du die E-Mail Benachrichtigung aktivieren.<br />
                Du kannst mehrere E-Mail Adressen angeben.</span>
                <div id="emailEnabled" style="width: 500px; margin-left: 100px; margin-bottom: 10px; margin-top: 20px;">
                    <label class="checkbox inline">
                        <input type="checkbox" id="inlineCheckbox1" value="option1"> send email with results to:
                    </label>
                </div>
                <div id="emails" style="margin-left: 100px; width: 500px;">
                    <div class="input-prepend input-append">
                        <span class="add-on"><i class="icon-envelope"></i></span>
                        <input class="input-xlarge" id="prependedInput" type="text" placeholder="email address" value="rafael.schmitt@hs-augsburg.de">
                        <button type="button" class="btn"><i class="icon-minus"></i></button>
                    </div>
                    <div class="input-prepend input-append">
                        <span class="add-on"><i class="icon-envelope"></i></span>
                        <input class="input-xlarge" id="Text1" type="text" placeholder="email address" value="harmdest@gmx.de">
                        <button type="button" class="btn"><i class="icon-minus"></i></button>
                    </div>
                    <div class="input-prepend input-append">
                        <span class="add-on"><i class="icon-envelope"></i></span>
                        <input class="input-xlarge" id="Text2" type="text" placeholder="email address">
                        <button type="button" class="btn"><i class="icon-minus"></i></button>
                    </div>
                    <a class="btn" style="margin-bottom: 10px; margin-right: 10px;" href="#">
                        <i class="icon-plus"></i>&nbsp;&nbsp;<b>Add E-Mail address</b></a>
                </div>
            </div>
            <div class="contentbox-content cutline-bot">
                <p><b>Websites</b></p>
                <span>Hier kannst Du bestimmen, welche Websiten durchsucht werden sollen, und kannst zusätzliche Websites angeben.</span>
                <div id="defaultWebsites" style="width: 500px; margin-left: 100px; margin-bottom: 10px; margin-top: 20px;">
                    <label class="checkbox inline">
                        <input type="checkbox" id="Checkbox1" value="checked"> heise.de
                    </label><br />
                    <label class="checkbox inline">
                        <input type="checkbox" id="Checkbox2" value="checked"> irgendeineseite.de
                    </label>
                </div>
                <div id="websites" style="margin-left: 100px; width: 600px; margin-top: 20px;">
                    <div class="input-prepend input-append">
                        <span class="add-on"><i class="icon-globe"></i></span>
                        <input class="input-xxlarge" id="Text3" type="text" placeholder="website-url" value="http://irgendeinforum.com">
                        <button type="button" class="btn"><i class="icon-minus"></i></button>
                    </div>
                    <div class="input-prepend input-append">
                        <span class="add-on"><i class="icon-globe"></i></span>
                        <input class="input-xxlarge" id="Text4" type="text" placeholder="website-url">
                        <button type="button" class="btn"><i class="icon-minus"></i></button>
                    </div>
                    <a class="btn" style="margin-bottom: 10px; margin-right: 10px;" href="#">
                        <i class="icon-plus"></i>&nbsp;&nbsp;<b>Add URL</b></a>
                </div>
            </div>
            <div class="contentbox-content">
                    <button type="submit" class="btn btn-success" style="float:right; margin-bottom: 10px; margin-right: 10px;">
                        <i class="icon-check icon-white"></i>&nbsp;&nbsp;<b>Save Changes</b></button>
            </div>
        </div>

    </div>

    <script>
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
