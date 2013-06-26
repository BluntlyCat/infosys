<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Server Settings
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Server Settings</h2>

    <div class="accordion" id="accordion2">

        <!-- Mail Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseOne">Email Notifier Settings</a>
            </div>
            <div id="collapseOne" class="accordion-body collapse">
                <div class="accordion-inner">

                    <form class="form-horizontal" action="/Settings/Email" method="post">

                    <% var mailSettings = this.ViewData["mailSettings"] as EmailNotifierSettings; %>

                        <div class="control-group">
                            <label class="control-label" for="smtpserver">
                                SmtpServer:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpserver" placeholder="" value="<%= mailSettings.SmtpServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="mailfrom">
                                MailFrom:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="mailfrom" placeholder="" value="<%= mailSettings.MailFrom %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="smtpport">
                                SmtpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpport" placeholder="" value="<%= mailSettings.SmtpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <div class="controls">
                                <button type="submit" class="btn">Save</button>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>

        <!-- Nutch Controller Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseTwo">Nutch Controller Client Settings</a>
            </div>
            <div id="collapseTwo" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Nutch" method="post">

                    <% var nutchSettings = this.ViewData["NutchClientSettings"] as NutchControllerClientSettings; %>
                    
                        <div class="control-group">
                            <label class="control-label" for="solrserver">
                                SolrServer:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="solrserver" placeholder="" value="<%= nutchSettings.SolrServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="seedfilename">
                                SeedFileName:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="seedfilename" placeholder="" value="<%= nutchSettings.SeedFileName %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="baseurlpath">
                                BaseUrlPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="baseurlpath" placeholder="" value="<%= nutchSettings.BaseUrlPath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchcommand">
                                NutchCommand:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchcommand" placeholder="" value="<%= nutchSettings.NutchCommand %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawlrequest">
                                CrawlRequest:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawlrequest" placeholder="" value="<%= nutchSettings.CrawlRequest %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="basecrawlpath">
                                BaseCrawlPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="basecrawlpath" placeholder="" value="<%= nutchSettings.BaseCrawlPath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawldepth">
                                CrawlDepth:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawldepth" placeholder="" value="<%= nutchSettings.CrawlDepth %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawltopn">
                                CrawlTopN:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawltopn" placeholder="" value="<%= nutchSettings.CrawlTopN %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefixpath">
                                PrefixPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefixpath" placeholder="" value="<%= nutchSettings.PrefixPath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefixfilename">
                                PrefixFileName:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefixfilename" placeholder="" value="<%= nutchSettings.PrefixFileName %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefix">
                                Prefix:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefix" placeholder="" value="<%= nutchSettings.Prefix %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchclients">
                                NutchClients:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchclients" placeholder="" value="<%= nutchSettings.NutchClients %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <div class="controls">
                                <button type="submit" class="btn">Save</button>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>

        <!-- Solr Search Client Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseThree">Solr Search Client Settings</a>
            </div>
            <div id="collapseThree" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Solr" method="post">

                    <% var solrClientSettings = this.ViewData["SolrClientSettings"] as SolrSearchClientSettings; %>

                        <div class="control-group">
                            <label class="control-label" for="host">
                                Host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="host" placeholder="" value="<%= solrClientSettings.Host %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="port">
                                Port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="port" placeholder="" value="<%= solrClientSettings.Port %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="collection">
                                Collection:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="collection" placeholder="" value="<%= solrClientSettings.Collection %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="queryformat">
                                QueryFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="queryformat" placeholder="" value="<%= solrClientSettings.QueryFormat %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="requestformat">
                                RequestFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="requestformat" placeholder="" value="<%= solrClientSettings.RequestFormat %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filterqueryformat">
                                FilterQueryFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="filterqueryformat" placeholder="" value="<%= solrClientSettings.FilterQueryFormat %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filter">
                                Filter:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="filter" placeholder="" value="<%= solrClientSettings.Filter %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <div class="controls">
                                <button type="submit" class="btn">Save</button>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>
        
        <!-- WCF Controller Addresses Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseFour">WCF Controller Addresses Settings</a>
            </div>
            <div id="collapseFour" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/WcfAddresses" method="post">

                    <% var wcfAddressesSettings = this.ViewData["WCFAddressesSettings"] as WCFControllerAddressesSettings; %>

                        <div class="control-group">
                            <label class="control-label" for="httpaddress">
                                HttpAddress:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpaddress" placeholder="" value="<%= wcfAddressesSettings.HttpAddress %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpaddress">
                                NetTcpAddress:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpaddress" placeholder="" value="<%= wcfAddressesSettings.NetTcpAddress %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httpport">
                                HttpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpport" placeholder="" value="<%= wcfAddressesSettings.HttpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpport">
                                NetTcpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpport" placeholder="" value="<%= wcfAddressesSettings.NetTcpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <div class="controls">
                                <button type="submit" class="btn">Save</button>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>

        <!-- WCF Controller Host Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseFive">WCF Controller Host Settings</a>
            </div>
            <div id="collapseFive" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/WcfHost" method="post">

                    <% var wcfControllerSettings = this.ViewData["WCFControllerSettings"] as WCFControllerHostSettings; %>

                        <div class="control-group">
                            <label class="control-label" for="certificatepath">
                                CertificatePath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="<%= wcfControllerSettings.CertificatePath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepassword">
                                CertificatePassword:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepassword" placeholder="" value="<%= wcfControllerSettings.CertificatePassword %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <div class="controls">
                                <button type="submit" class="btn">Save</button>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>

    </div>

</asp:Content>