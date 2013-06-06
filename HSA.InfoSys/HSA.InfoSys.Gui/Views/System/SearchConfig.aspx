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
                <a class="btn" style="margin-right: 6px;" href="/System/Components?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Components</b></a>
                <a class="btn btn-primary" style="margin-right: 6px;" href="/System/SearchConfig?sysguid=<%= ViewData["systemguid"] %>">
                    <b>Search Configuration</b></a>
            </div>

            <!-- saved Configs -->
            <div id="savedconfigsblock" class="contentbox-content cutline-bot">
                <p><b>Saved Configs</b></p>
                <span>Du kannst bereits gespeicherte Konfigurationen deiner anderen Systeme für dieses System verwenden.<br />
                Wähle einfach die entsprechende Konfiguration, die geladen werden soll:</span>
                <form action="/System/SearchConfig" method="post">
                    <div id="loadconfig" style="width: 500px; margin-left: 100px; margin-top: 20px;">
                        <select>
                            <option></option>
                            <option>Workstation1</option>
                            <option>Backup-Server 07</option>
                        </select>
                        <button type="submit" class="btn" style="margin-bottom: 10px; margin-left: 5px;"><b>Load & Save</b></button>
                    </div>
                </form>
            </div>

            <form action="/System/SearchConfigSubmit?sysguid=<%= ViewData["systemguid"] %>" method="post">

                <!-- Scheduler -->
                <div id="schedulerblock" class="contentbox-content cutline-bot">
                    <p><b>Zeitplan</b></p>
                    <span>Lege einen Zeitplan für die Suche fest. Hier kannst Du angeben, wann immer ein neuer Suchlauf für dieses
                    System durchgeführt werden soll.</span>
                    <div id="schedulerEnabled" style="width: 500px; margin-left: 100px; margin-bottom: 10px; margin-top: 20px;">
                        <label class="checkbox inline">
                            <% if ((bool)ViewData["schedulerActive"] == true)
                               { %>
                                <input id="schedulerOn" type="checkbox" name="schedulerOn" checked="checked" /> Zeitplan verwenden
                            <% }
                               else
                               { %>
                                <input id="schedulerOn" type="checkbox" name="schedulerOn" /> Zeitplan verwenden
                            <% } %>
                        </label>
                    </div>
                    <div id="schedulerconfig" class="form-horizontal" style="margin-left: 100px; width: 700px;">
                       Suche alle 
                       <input style="width: 30px;" id="sc_days" type="text" name="sc_days" placeholder="z.B. 3" value="<%= ViewData["sc_days"] %>">
                       Tage um 
                       <select id="sc_time" style="width: 75px;" name="sc_time">
                            <% for (int i = 0; i < 24; i++) { %>
                                <% 
                                   string time = i + ":00";
                                   if (i < 10) {
                                       time = "0" + i + ":00";
                                   }
                                %>
                                <% if ((int)ViewData["sc_hours"] == i) { %>
                                    <option value="<%= i %>" selected><%= time %></option>
                                <% } else { %>
                                    <option value="<%= i %>"><%= time %></option>
                                <% } %>
                            <% } %>
                        </select> 
                        Uhr. 
<%--                        Starte erste Suche am
                        <div class="input-append date">
                            <input id="sc_date" data-date="12-02-2012" data-date-format="dd-mm-yyyy" class="span2" size="16" type="text" name="sc_date" value="12-02-2012">
                            <span class="add-on"><i class="icon-th"></i></span>
                        </div>--%>
                    </div>
                </div>

                <!-- Email -->
                <div id="emailblock" class="contentbox-content cutline-bot">
                    <p><b>E-Mail</b></p>
                    <span>Lass Dir Deine Suchergebnisse per E-Mail schicken. Hier kannst Du die E-Mail Benachrichtigung aktivieren.<br />
                    Du kannst mehrere E-Mail Adressen angeben.</span>
                    <div id="emailEnabled" style="width: 500px; margin-left: 100px; margin-bottom: 10px; margin-top: 20px;">
                        <label class="checkbox inline">
                            <% if ((bool)ViewData["emailActive"] == true)
                               { %>
                                <input id="emailsOn" type="checkbox" name="emailsOn" checked="checked" /> Sende E-Mail mit Suchergebnissen an:
                            <% }
                               else
                               { %>
                                <input id="emailsOn" type="checkbox" name="emailsOn" /> Sende E-Mail mit Suchergebnissen an:
                            <% } %>
                        </label>
                    </div>
                    <div id="emails" style="margin-left: 100px; width: 500px;">
                        
                        <% foreach (var email in this.ViewData["emails"] as string[])
                           { %>
                            <div class="input-prepend input-append">
                                <span class="add-on"><i class="icon-envelope"></i></span>
                                <input class="input-xlarge" type="text" placeholder="email address" value="<%= email %>" name="emails[]" />
                                <button type="button" class="btn" onclick="removeInputBox($(this))"><i class="icon-minus"></i></button>
                            </div>
                          <% } %>

                        <div id="addedEmails">
                        </div>
                        <div>
                            <button id="addEmail" class="btn" type="button" style="margin-bottom: 10px;">
                                <i class="icon-plus"></i>&nbsp;&nbsp;<b>Add E-Mail address</b></button>
                        </div>
                        
                    </div>
                </div>

                <!-- websites -->
                <div id="websitesblock" class="contentbox-content cutline-bot">
                    <p><b>Websites</b></p>
                    <span>Hier kannst Du bestimmen, welche Websiten durchsucht werden sollen, und kannst zusätzliche Websites angeben.</span>
                    <div id="defaultWebsites" style="width: 500px; margin-left: 100px; margin-bottom: 10px; margin-top: 20px;">
                        <label class="checkbox inline">
                            <input type="checkbox" id="Checkbox1" name="heise" checked="checked" /> heise.de
                        </label><br />
                        <label class="checkbox inline">
                            <input type="checkbox" id="Checkbox2" name="irgendeineseite" checked="checked" /> irgendeineseite.de
                        </label><br />
                        <label class="checkbox inline">
                            <% if ((bool)ViewData["urlActive"] == true)
                               { %>
                                <input id="websitesOn" type="checkbox" name="websitesOn" checked="checked" /> weitere...
                            <% }
                               else
                               { %>
                                <input id="websitesOn" type="checkbox" name="websitesOn"/> weitere...
                            <% } %>
                        </label>
                    </div>

                    <div id="websites" style="margin-left: 100px; width: 600px; margin-top: 20px;">
                        
                        <% foreach (var website in this.ViewData["urls"] as string[])
                           { %>
                            <div class="input-prepend input-append">
                                <span class="add-on"><i class="icon-globe"></i></span>
                                <input class="input-xxlarge" type="text" placeholder="website-url" value="<%= website %>" name="websites[]" />
                                <button type="button" class="btn" onclick="removeInputBox($(this))"><i class="icon-minus"></i></button>
                            </div>
                          <% } %>

                        <div id="addedWebsites">
                        </div>
                        <div>
                            <button id="addWebsite" class="btn" type="button" style="margin-bottom: 10px; margin-right: 10px;">
                                <i class="icon-plus"></i>&nbsp;&nbsp;<b>Add E-Mail address</b></button>
                        </div>
                    </div>

                </div>

                <!-- submit -->
                <div class="contentbox-content">
                    <button type="submit" class="btn btn-success" style="float:right; margin-bottom: 10px; margin-right: 10px;">
                    <i class="icon-check icon-white"></i>&nbsp;&nbsp;<b>Save Changes</b></button>
                </div>

            </form>

        </div>

    </div>

    <script>
        $(document).ready(function () {

            // initialize datepicker
            initDatepicker();

            // show / hide scheduler
            if ($('#schedulerOn').is(':checked')) {
                $('#schedulerconfig').show();
            } else {
                $('#schedulerconfig').hide();
            }

            $('#schedulerOn').click(function () {
                if ($(this).is(':checked')) {
                    $('#schedulerconfig').show();
                } else {
                    $('#schedulerconfig').hide();
                }
            });

            // show / hide email
            if ($('#emailsOn').is(':checked')) {
                $('#emails').show();
            } else {
                $('#emails').hide();
            }

            $('#emailsOn').click(function () {
                if ($(this).is(':checked')) {
                    $('#emails').show();
                } else {
                    $('#emails').hide();
                }
            });

            // show / hide email
            if ($('#websitesOn').is(':checked')) {
                $('#websites').show();
            } else {
                $('#websites').hide();
            }

            $('#websitesOn').click(function () {
                if ($(this).is(':checked')) {
                    $('#websites').show();
                } else {
                    $('#websites').hide();
                }
            });

            // addButton click event
            $('#addEmail').click(function () {
                addEmailBox();
            });

            // addButton click event
            $('#addWebsite').click(function () {
                addWebsiteBox();
            });

        });

        /**
        * initialize datepicker
        */
        function initDatepicker() {

            var nowTemp = new Date();
            var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

            // bind datepicker on element
            $('#sc_date').datepicker('setValue', now);

        }

        /**
        * adds a new email-InputBox
        */
        function addEmailBox() {

            // get Value of mainBox
            //var value = $('#mainEmailBox').val();

            // clear Value of mainBox
            //$('#mainEmailBox').val('');

            var newEmailBox = 
                '<div class="input-prepend input-append">'
                + '<span class="add-on"><i class="icon-envelope"></i></span>'
                + '<input class="input-xlarge" type="text" placeholder="email address" value="" name="emails[]" />'
                + '<button type="button" class="btn" onclick="removeInputBox($(this))"><i class="icon-minus"></i></button>'
                + '</div>';

            $('#addedEmails').append(newEmailBox);
        }

        /**
        * adds a new website-InputBox
        */
        function addWebsiteBox() {

            // get Value of mainBox
            //var value = $('#mainWebsiteBox').val();

            // clear Value of mainBox
            //$('#mainWebsiteBox').val('');

            var newEmailBox = 
                '<div class="input-prepend input-append">'
                + '<span class="add-on"><i class="icon-globe"></i></span>'
                + '<input class="input-xxlarge" type="text" placeholder="website-url" value="" name="websites[]" />'
                + '<button type="button" class="btn" onclick="removeInputBox($(this))"><i class="icon-minus"></i></button>'
                + '</div>';

            $('#addedWebsites').append(newEmailBox);
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
