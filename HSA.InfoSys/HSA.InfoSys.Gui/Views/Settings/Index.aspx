<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Server Settings
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Server Settings</h2>

    <div class="accordion" id="accordion2">

        <!-- Nutch Controller Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseTwo">Nutch Settings</a>
            </div>
            <div id="collapseTwo" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Nutch" method="post">

                    <% 
                        var nutchSettings = this.ViewData["NutchClientSettings"] as NutchControllerClientSettings;

                        var homePath = string.Empty;
                        var nutchPath = string.Empty;
                        var nutchCommand = string.Empty;
                        var nutchClients = string.Empty;
                        var crawlDepth = 0;
                        var crawlTopN = 0;
                        var solrServer = string.Empty;
                        var javaHome = string.Empty;
                        var certificatePath = string.Empty;
                        var prefix = string.Empty;

                        if (nutchSettings != null)
                        {
                            homePath = nutchSettings.HomePath;
                            nutchPath = nutchSettings.NutchPath;
                            nutchCommand = nutchSettings.NutchCommand;
                            nutchClients = nutchSettings.NutchClients;
                            crawlDepth = nutchSettings.CrawlDepth;
                            crawlTopN = nutchSettings.CrawlTopN;
                            solrServer = nutchSettings.SolrServer;
                            javaHome = nutchSettings.JavaHome;
                            certificatePath = nutchSettings.CertificatePath;
                            prefix = nutchSettings.Prefix;
                        }
                    %>
                    
                        <div class="control-group">
                            <label class="control-label" for="homepath" style="text-align:left">
                                Home path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="homepath" placeholder="" value="<%= homePath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchpath" style="text-align:left">
                                Nutch home:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchpath" placeholder="" value="<%= nutchPath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchcommand" style="text-align:left">
                                Nutch command:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchcommand" placeholder="" value="<%= nutchCommand %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchclients" style="text-align:left">
                                Nutch clients:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchclients" placeholder="" value="<%= nutchClients %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawldepth" style="text-align:left">
                                Crawl depth:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawldepth" placeholder="" value="<%= crawlDepth %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawltopn" style="text-align:left">
                                Crawl TopN:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawltopn" placeholder="" value="<%= crawlTopN %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="solrserver" style="text-align:left">
                                Solr server:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="solrserver" placeholder="" value="<%= solrServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="javahome" style="text-align:left">
                                Java home:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="javahome" placeholder="" value="<%= javaHome %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepath" style="text-align:left">
                                SSH Certificate:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="<%= certificatePath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefix" style="text-align:left">
                                Prefix:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefix" placeholder="" value="<%= prefix %>">
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
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseThree">Solr Settings</a>
            </div>
            <div id="collapseThree" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Solr" method="post">

                    <%
                        var solrClientSettings = this.ViewData["SolrClientSettings"] as SolrSearchClientSettings;

                        var host = string.Empty;
                        var port = 0;
                        var collection = string.Empty;
                        var filterQuery = string.Empty;

                        if (solrClientSettings != null)
                        {
                            host = solrClientSettings.Host;
                            port = solrClientSettings.Port;
                            collection = solrClientSettings.Collection;
                            filterQuery = solrClientSettings.FilterQuery;
                        }
                    %>

                        <div class="control-group">
                            <label class="control-label" for="host" style="text-align:left">
                                Host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="host" placeholder="" value="<%= host %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="port" style="text-align:left">
                                Port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="port" placeholder="" value="<%= port %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="collection" style="text-align:left">
                                Collection:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="collection" placeholder="" value="<%= collection %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filterquery" style="text-align:left">
                                Filter query:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="filterquery" placeholder="" value="<%= filterQuery %>">
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

        <!-- Mail Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseOne">Email Settings</a>
            </div>
            <div id="collapseOne" class="accordion-body collapse">
                <div class="accordion-inner">

                    <form class="form-horizontal" action="/Settings/Email" method="post">

                    <% 
                        var mailSettings = this.ViewData["mailSettings"] as EmailNotifierSettings;

                        var smtpServer = string.Empty;
                        var smtpPort = 0;
                        var mailFrom = string.Empty;

                        if (mailSettings != null)
                        {
                            smtpServer = mailSettings.SmtpServer;
                            smtpPort = mailSettings.SmtpPort;
                            mailFrom = mailSettings.MailFrom;
                        }
                    %>

                        <div class="control-group">
                            <label class="control-label" for="smtpserver" style="text-align:left">
                                SMTP server:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpserver" placeholder="" value="<%= smtpServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="smtpport" style="text-align:left">
                                SMTP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpport" placeholder="" value="<%= smtpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="mailfrom" style="text-align:left">
                                Mail account:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="mailfrom" placeholder="" value="<%= mailFrom %>">
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

        <!-- WCF Settings -->
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseFive">WCF Settings</a>
            </div>
            <div id="collapseFive" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/WcfSettings" method="post">

                    <%
                        var wcfSettings = this.ViewData["WCFSettings"] as WCFSettings;

                        var httpHost = string.Empty;
                        var httpPort = 0;
                        var httpPath = string.Empty;

                        var netTcpHost = string.Empty;
                        var netTcpPort = 0;
                        var netTcpPath = string.Empty;

                        var wcfCertificatePath = string.Empty;
                        var certificatePassword = string.Empty;
                        
                        if (wcfSettings != null)
                        {
                            httpHost = wcfSettings.HttpHost;
                            httpPort = wcfSettings.HttpPort;
                            httpPath = wcfSettings.HttpPath;

                            netTcpHost = wcfSettings.NetTcpHost;
                            netTcpPort = wcfSettings.NetTcpPort;
                            netTcpPath = wcfSettings.NetTcpPath;

                            wcfCertificatePath = wcfSettings.CertificatePath;
                            certificatePassword = Encryption.Decrypt(wcfSettings.CertificatePassword);
                        }
                    %>

                        <div class="control-group">
                            <label class="control-label" for="httphost" style="text-align:left">
                                HTTP host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httphost" placeholder="" value="<%= httpHost %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httpport" style="text-align:left">
                                HTTP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpport" placeholder="" value="<%= httpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httppath" style="text-align:left">
                                HTTP path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httppath" placeholder="" value="<%= httpPath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcphost" style="text-align:left">
                                NET TCP host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcphost" placeholder="" value="<%= netTcpHost %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpport" style="text-align:left">
                                NET TCP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpport" placeholder="" value="<%= netTcpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcppath" style="text-align:left">
                                NET TCP path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcppath" placeholder="" value="<%= netTcpPath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepath" style="text-align:left">
                                CertificatePath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="<%= wcfCertificatePath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepassword" style="text-align:left">
                                CertificatePassword:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepassword" placeholder="" value="<%= certificatePassword %>">
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