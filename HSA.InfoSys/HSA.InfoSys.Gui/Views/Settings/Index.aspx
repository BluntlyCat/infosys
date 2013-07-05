<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HSA.InfoSys.Common.Entities"%>
<%@ Import Namespace="HSA.InfoSys.Common.Extensions"%>
<%@ Import Namespace="Newtonsoft.Json"%>

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

                        var defaultURLs = JsonConvert.DeserializeObject<string[]>(nutchSettings.DefaultURLs).ElementsToString();
                        var nutchClients = JsonConvert.DeserializeObject<string[]>(nutchSettings.NutchClients).ElementsToString();
                    %>
                    
                        <div class="control-group">
                            <label class="control-label" for="homepath" style="text-align:left">
                                Home path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="homepath" placeholder="" value="<%= nutchSettings.HomePath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchpath" style="text-align:left">
                                Nutch home:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchpath" placeholder="" value="<%= nutchSettings.NutchPath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchcommand" style="text-align:left">
                                Nutch command:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchcommand" placeholder="" value="<%= nutchSettings.NutchCommand %>" />
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
                            <label class="control-label" for="defaulturls" style="text-align:left">
                                Default URLs:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="defaulturls" placeholder="" value="<%= defaultURLs %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawldepth" style="text-align:left">
                                Crawl depth:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawldepth" placeholder="" value="<%= nutchSettings.CrawlDepth %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawltopn" style="text-align:left">
                                Crawl TopN:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawltopn" placeholder="" value="<%= nutchSettings.CrawlTopN %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="solrserver" style="text-align:left">
                                Solr server:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="solrserver" placeholder="" value="<%= nutchSettings.SolrServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="javahome" style="text-align:left">
                                Java home:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="javahome" placeholder="" value="<%= nutchSettings.JavaHome %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepath" style="text-align:left">
                                SSH Certificate:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="<%= nutchSettings.CertificatePath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefix" style="text-align:left">
                                Prefix:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefix" placeholder="" value="<%= nutchSettings.Prefix %>">
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
                    %>

                        <div class="control-group">
                            <label class="control-label" for="host" style="text-align:left">
                                Host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="host" placeholder="" value="<%= solrClientSettings.Host %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="port" style="text-align:left">
                                Port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="port" placeholder="" value="<%= solrClientSettings.Port %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="collection" style="text-align:left">
                                Collection:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="collection" placeholder="" value="<%= solrClientSettings.Collection %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filterquery" style="text-align:left">
                                Filter query:</label>
                            <div class="controls">
                                <textarea class="input-xxlarge" rows="5" name="filterquery" placeholder=""><%= solrClientSettings.FilterQuery %></textarea>
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
                    %>

                        <div class="control-group">
                            <label class="control-label" for="smtpserver" style="text-align:left">
                                SMTP server:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpserver" placeholder="" value="<%= mailSettings.SmtpServer %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="smtpport" style="text-align:left">
                                SMTP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpport" placeholder="" value="<%= mailSettings.SmtpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="mailfrom" style="text-align:left">
                                Mail account:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="mailfrom" placeholder="" value="<%= mailSettings.MailFrom %>">
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
                    %>

                        <div class="control-group">
                            <label class="control-label" for="httphost" style="text-align:left">
                                HTTP host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httphost" placeholder="" value="<%= wcfSettings.HttpHost %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httpport" style="text-align:left">
                                HTTP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpport" placeholder="" value="<%= wcfSettings.HttpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httppath" style="text-align:left">
                                HTTP path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httppath" placeholder="" value="<%= wcfSettings.HttpPath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcphost" style="text-align:left">
                                NET TCP host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcphost" placeholder="" value="<%= wcfSettings.NetTcpHost %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpport" style="text-align:left">
                                NET TCP port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpport" placeholder="" value="<%= wcfSettings.NetTcpPort %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcppath" style="text-align:left">
                                NET TCP path:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcppath" placeholder="" value="<%= wcfSettings.NetTcpPath %>">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepath" style="text-align:left">
                                CertificatePath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="<%= wcfSettings.CertificatePath %>" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepassword" style="text-align:left">
                                CertificatePassword:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepassword" placeholder="" value="<%= Encryption.Decrypt(wcfSettings.CertificatePassword) %>">
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