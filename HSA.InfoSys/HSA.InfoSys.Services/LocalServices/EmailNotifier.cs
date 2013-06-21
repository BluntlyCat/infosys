namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    public class EmailNotifier
    {
        /// <summary>
        /// The logger of EmailNotifier
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("EmailNotifier");

        /// <summary>
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="results">The results.</param>
        public void SearchFinished(Guid orgUnitGUID, Result[] results)
        {
            Log.DebugFormat(Properties.Resources.SEARCH_RECALL, orgUnitGUID);

            try
            {
                var dbManager = DBManager.ManagerFactory;
                var orgUnit = dbManager.GetEntity(orgUnitGUID, dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
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
                            var component = dbManager.GetEntity(result.ComponentGUID) as Component;
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

        /// <summary>
        /// Called if the crawl failed.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void CrawlFailed(Guid orgUnitGUID)
        {
            try
            {
                var dbManager = DBManager.ManagerFactory;
                var orgUnit = dbManager.GetEntity(orgUnitGUID, dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
                var mailBody = string.Format("The crawl for {0} failed.", orgUnit.Name);

#warning Emails können null sein? Die OrgUnitConfig wird ja von einem Benutzer angelegt, dessen Email Adresse im System hinterlegt ist...
                if (orgUnit.OrgUnitConfig.Emails != null)
                {
                    var addresses = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails);
                    var mail = this.BuildMail("michael.juenger1@hs-augsburg.de", string.Format("Crawl failed.", orgUnit.Name));

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
