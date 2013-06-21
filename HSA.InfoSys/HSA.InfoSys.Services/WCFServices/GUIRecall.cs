// ------------------------------------------------------------------------
// <copyright file="SearchRecall.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using HSA.InfoSys.Common.Services.LocalServices;
    using System.Net.Mail;

    /// <summary>
    /// This class implements the method for
    /// indicating that a search request for 
    /// an org unit is finished.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GUIRecall : Service, IGUIRecall
    {
        /// <summary>
        /// The logger of SearchRecall
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SearchRecall");

        /// <summary>
        /// The search recall
        /// </summary>
        private static GUIRecall guiRecall;

        /// <summary>
        /// The amount of running searches.
        /// </summary>
        private int runningSearches = 0;

        /// <summary>
        /// The timeout by default 10 minutes.
        /// </summary>
        private int timeout = 600000;

        /// <summary>
        /// Prevents a default instance of the <see cref="GUIRecall"/> class from being created.
        /// </summary>
        private GUIRecall()
        {
        }

        /// <summary>
        /// Gets the search recall.
        /// </summary>
        /// <value>
        /// The search recall.
        /// </value>
        public static GUIRecall GUIRecallFactory
        {
            get
            {
                if (guiRecall == null)
                {
                    guiRecall = new GUIRecall();
                }

                return guiRecall;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                if (!this.Running && value > 0 && this.timeout != value)
                {
                    this.timeout = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return this.Running;
            }
        }

        /// <summary>
        /// Gets the amount of running searches.
        /// </summary>
        /// <value>
        /// The amount of running searches.
        /// </value>
        public int RunningSearches
        {
            get
            {
                return this.runningSearches;
            }
        }

        /// <summary>
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="results">The results.</param>
        public void SearchRecall(Guid orgUnitGUID, Result[] results)
        {
            Log.DebugFormat(Properties.Resources.SEARCH_RECALL, orgUnitGUID);

            this.ServiceMutex.WaitOne();
            this.runningSearches--;
            this.ServiceMutex.ReleaseMutex();

            if (this.Running)
            {
                try
                {
                    var proxy = WCFControllerClient<IDBManager>.ClientProxy;
                    var orgUnit = proxy.GetEntity(orgUnitGUID, proxy.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
                    var mailBody = string.Empty;
                    var oldResultGUID = Guid.Empty;

#warning Emails können null sein? Die OrgUnitConfig wird ja von einem Benutzer angelegt, dessen Email Adresse im System hinterlegt ist...
                    if (orgUnit.OrgUnitConfig.Emails != null)
                    {
                        var addresses = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails);
                        var mail = this.BuildMail("Michael", string.Format("New issues for System {0} found.", orgUnit.Name));

                        foreach (var address in addresses)
                        {
                            Log.DebugFormat("Send mail to {0} for OrgUnit {1}.", address, orgUnitGUID);
                            this.AddMailReceipients(mail, address);
                        }

                        foreach (var result in results.ToList<Result>())
                        {
                            if (oldResultGUID.Equals(Guid.Empty) || oldResultGUID.Equals(result.EntityId) == false)
                            {
                                var component = proxy.GetEntity(result.ComponentGUID) as Component;
                                mailBody += string.Format("{0}:\n\n", component.Name);
                            }

                            oldResultGUID = result.EntityId;
                            mailBody += string.Format("{0} - {1}\n", result.Title, result.URL);
                        }

                        this.AddMailBody(mail, mailBody);
                        this.SendMail(mail);
                    }
                }
                catch (CommunicationException ce)
                {
                    Log.ErrorFormat("Communication error: {0}", ce);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Common error: {0}", e);
                }
            }
        }

        /// <summary>
        /// Called if the crawl failed.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void CrawlFailedRecall(Guid orgUnitGUID)
        {
            try
            {
                var proxy = WCFControllerClient<IDBManager>.ClientProxy;
                var orgUnit = proxy.GetEntity(orgUnitGUID, proxy.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
                var mailBody = string.Format("The crawl for {0} failed.", orgUnit.Name);

#warning Emails können null sein? Die OrgUnitConfig wird ja von einem Benutzer angelegt, dessen Email Adresse im System hinterlegt ist...
                if (orgUnit.OrgUnitConfig.Emails != null)
                {
                    var addresses = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails);
                    var mail = this.BuildMail("Michael", string.Format("Crawl failed.", orgUnit.Name));

                    foreach (var address in addresses)
                    {
                        Log.DebugFormat("Send mail to {0} for OrgUnit {1}.", address, orgUnitGUID);
                        this.AddMailReceipients(mail, address);
                    }

                    this.AddMailBody(mail, mailBody);
                    this.SendMail(mail);
                }
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat("Communication error: {0}", ce);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Common error: {0}", e);
            }
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            this.ServiceMutex.WaitOne();
            this.runningSearches++;
            this.ServiceMutex.ReleaseMutex();

            Log.DebugFormat(Properties.Resources.SEARCH_RECALL_START_SERVICE, this.RunningSearches);

            if (!this.Running)
            {
                Log.Info(Properties.Resources.LOG_START_SERVICE);

                this.Timeout = 600000;
                base.StartService();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            Log.Info(Properties.Resources.LOG_STOP_SERVICE);

            this.timeout = 0;
            this.runningSearches = 0;
            this.Cancel = cancel;

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            this.ServiceMutex.WaitOne();
            var runningSearches = this.RunningSearches;
            this.ServiceMutex.ReleaseMutex();

            while (this.Running && runningSearches > 0 && this.Timeout > 0)
            {
                Log.DebugFormat(
                    Properties.Resources.SEARCH_RECALL_THREAD_STATE,
                    this.Running,
                    this.runningSearches,
                    this.Timeout);

                this.timeout -= 1000;

                this.ServiceMutex.WaitOne();
                runningSearches = this.RunningSearches;
                this.ServiceMutex.ReleaseMutex();

                Thread.Sleep(1000);
            }

            Log.InfoFormat(
                    Properties.Resources.SEARCH_RECALL_THREAD_STATE_END,
                    this.Running,
                    this.runningSearches,
                    this.Timeout);
        }

        /// <summary>
        /// Builds the mail.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        private MailMessage BuildMail(string from, string subject)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(from);
            mail.Subject = subject;

            return mail;
        }

        /// <summary>
        /// Adds the mail receipients.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="address">The address.</param>
        private void AddMailReceipients(MailMessage mail, string address)
        {
            mail.To.Add(address);
        }

        /// <summary>
        /// Adds the mail body.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="body">The body.</param>
        private void AddMailBody(MailMessage mail, string body)
        {
            mail.Body = body;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mail">The mail.</param>
        private void SendMail(MailMessage mail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.hs-augsburg.de");
            SmtpServer.Send(mail);
        }
    }
}
