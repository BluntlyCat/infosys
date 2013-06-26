<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Server Settings
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Server Settings</h2>

    <div class="accordion" id="accordion2">

        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseOne">Email Notifier Settings</a>
            </div>
            <div id="collapseOne" class="accordion-body collapse">
                <div class="accordion-inner">

                    <form class="form-horizontal" action="/Settings/Email" method="post">

                        <div class="control-group">
                            <label class="control-label" for="smtpserver">
                                SmtpServer:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpserver" placeholder="" value="smtp.hs-augsburg.de" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="mailfrom">
                                MailFrom:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="mailfrom" placeholder="" value="michael.juenger1@hs-augsburg.de">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="smtpport">
                                SmtpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="smtpport" placeholder="" value="25">
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

        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseTwo">Nutch Controller Client Settings</a>
            </div>
            <div id="collapseTwo" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Nutch" method="post">

                        <div class="control-group">
                            <label class="control-label" for="solrserver">
                                SolrServer:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="solrserver" placeholder="" value="http://infosys.informatik.hs-augsburg.de" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="seedfilename">
                                SeedFileName:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="seedfilename" placeholder="" value="seed.txt">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="baseurlpath">
                                BaseUrlPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="baseurlpath" placeholder="" value=".nutch/urls">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nutchcommand">
                                NutchCommand:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nutchcommand" placeholder="" value="nutch">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawlrequest">
                                CrawlRequest:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawlrequest" placeholder="" value="crawl {0} -solr {1} -depth {2} -topN {3}">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="basecrawlpath">
                                BaseCrawlPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="basecrawlpath" placeholder="" value="crawler">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawldepth">
                                CrawlDepth:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawldepth" placeholder="" value="100">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="crawltopn">
                                CrawlTopN:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="crawltopn" placeholder="" value="1000">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefixpath">
                                PrefixPath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefixpath" placeholder="" value="/usr/local/apache-nutch-1.6/conf">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefixfilename">
                                PrefixFileName:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefixfilename" placeholder="" value="regex-urlfilter.txt">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="prefix">
                                Prefix:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="prefix" placeholder="" value="+^http://([a-z0-9]*\\.)*">
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

        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseThree">Solr Search Client Settings</a>
            </div>
            <div id="collapseThree" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/Solr" method="post">

                        <div class="control-group">
                            <label class="control-label" for="host">
                                Host:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="host" placeholder="" value="141.82.59.139" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="port">
                                Port:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="port" placeholder="" value="8983">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="collection">
                                Collection:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="collection" placeholder="" value="InfoSys">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="queryformat">
                                QueryFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="queryformat" placeholder="" value="solr/{0}/select?q={1}&wt={2}&indent=true">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="requestformat">
                                RequestFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="requestformat" placeholder="" value="GET {0} HTTP/1.1{1}Host: {2}{3}Content-Length: 0{4}">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filterqueryformat">
                                FilterQueryFormat:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="filterqueryformat" placeholder="" value="(title:({0} AND {1}) OR title:({0} AND {2}) OR title:({0} AND {3}))">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="filter">
                                Filter:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="filter" placeholder="" value="Bedrohung,Threat,Sicherheit">
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

        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseFour">WCF Controller Addresses Settings</a>
            </div>
            <div id="collapseFour" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/WcfAddresses" method="post">

                        <div class="control-group">
                            <label class="control-label" for="httpaddress">
                                HttpAddress:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpaddress" placeholder="" value="http://localhost:{0}/CrawlerProxy/" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpaddress">
                                NetTcpAddress:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpaddress" placeholder="" value="net.tcp://localhost:{0}/CrawlerProxy/">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="httpport">
                                HttpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="httpport" placeholder="" value="8085">
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="nettcpport">
                                NetTcpPort:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="nettcpport" placeholder="" value="8086">
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

        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#collapseFive">WCF Controller Host Settings</a>
            </div>
            <div id="collapseFive" class="accordion-body collapse">
                <div class="accordion-inner">
                    
                    <form class="form-horizontal" action="/Settings/WcfHost" method="post">

                        <div class="control-group">
                            <label class="control-label" for="certificatepath">
                                CertificatePath:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepath" placeholder="" value="Certificates/InfoSys.pfx" />
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label" for="certificatepassword">
                                CertificatePassword:</label>
                            <div class="controls">
                                <input class="input-xxlarge" type="text" name="certificatepassword" placeholder="" value="Aes2xe1baetei8Y">
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