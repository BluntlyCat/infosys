<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<script runat="server">
    private CrawlControllerClient client = new CrawlControllerClient();
    
    void Page_Load(object sender, EventArgs e)
    {

    }

    void StartSearchButton_Click(object sender, EventArgs e)
    {
        client.StartSearch();
    }   
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Informationssystem für sicherheitskritische Komponenten
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form action="/Home/SearchResult" method="post">
        <div class="container">
            <div class="contentbox cb-large left-nomargin">
                <div class="contentbox-header">
                    <i class="icon-align-justify"></i>
                    <b><%= ViewData["label1"] %></b>
                </div>
                <div class="contentbox-content cutline-bot">
                    <span>Add all your components here</span>
                </div>
                <div class="contentbox-content">
                    <div id="allBoxes">
                        <!-- addedBoxes -->
                        <div id="addedBoxes"></div>
                        <!-- mainInputBox -->
                        <div class="input-append">
                            <input id="mainInputBox" class="input-components" type="text" placeholder="component" name="components[]" />
                            <button id="addButton" type="button" class="btn btn-success"><i class="icon-plus-sign"></i></button>
                        </div>
                        <!-- clearAllButton -->
                        <button id="clearAllButton" type="button" class="btn btn-danger marginBot-10"><i class="icon-trash"></i></button>
                    </div>
                </div>
            </div>

            <div class="contentbox cb-medium right-nomargin">
                <div class="contentbox-header">
                    <i class="icon-align-justify"></i>
                    <b>Search Configuration</b>
                </div>
                <div class="contentbox-content">
                    <button type="submit" class="btn btn-primary"><i class="icon-search icon-white"></i>&nbsp;&nbsp;<b>Search</b></button>
                </div>
            </div>
        </div> 
    </form>

    <form runat="server" action="/">
        <div>
            <asp:Button id="StartSearchButton" 
                Text="Start search"
                OnClick="StartSearchButton_Click"
                runat="server" />
        </div>
    </form>

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
