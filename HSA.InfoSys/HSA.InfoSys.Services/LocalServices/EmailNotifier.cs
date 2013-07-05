// ------------------------------------------------------------------------
// <copyright file="EmailNotifier.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using Newtonsoft.Json;
    using WCFServices;

    /// <summary>
    /// This class sends emails to notify the owner
    /// of something what happed during crawling.
    /// </summary>
    public class EmailNotifier
    {
        /// <summary>
        /// The logger of email notifier
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("EmailNotifier");

        /// <summary>
        /// The db manager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly EmailNotifierSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotifier" /> class.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        public EmailNotifier(IDbManager dbManager)
        {
            this.dbManager = dbManager;
            this.settings = this.dbManager.GetMailSettings();
        }

        /// <summary>
        /// Called when the search for a component
        /// finished and there are new results found.
        /// Then this method sends this results to the
        /// subscribed email addresses.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="results">The results.</param>
        public void SearchFinished(Guid orgUnitGUID, IEnumerable<Result> results)
        {
            Log.DebugFormat(Properties.Resources.SEARCH_RECALL, orgUnitGUID);

            if (this.settings.IsDefault() == false)
            {
                try
                {
                    Log.InfoFormat(Properties.Resources.EMAIL_NOTIFIER_SEARCH_FINISHED, orgUnitGUID);

                    var orgUnit = this.dbManager.GetEntity(
                        orgUnitGUID,
                        this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                    var mailBody = string.Empty;
                    var oldComponentGUID = Guid.Empty;

                    var addresses = this.DeserializeAddresses(orgUnit);

                    if (orgUnit != null)
                    {
                        var subject = string.Format(
                            Properties.Resources.EMAIL_NOTIFIER_ISSUE_FOUND_SUBJECT,
                            orgUnit.Name);

                        var mail = this.BuildMail(
                            this.settings.MailFrom,
                            subject);

                        this.AddMailRecipient(mail, addresses);

                        foreach (var result in results)
                        {
                            if (oldComponentGUID.Equals(Guid.Empty) ||
                                oldComponentGUID.Equals(result.ComponentGUID) == false)
                            {
                                var component = this.dbManager.GetEntity(result.ComponentGUID) as Component;

                                if (component != null)
                                {
                                    mailBody += string.Format("{0}:\n", component.Name);
                                }
                            }

                            oldComponentGUID = result.ComponentGUID;
                            mailBody += string.Format("\t{0} - {1}\n\n", result.Title, result.URL);
                        }

                        this.AddMailBody(mail, mailBody);
                        this.SendMail(mail);
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
            }
            else
            {
                Log.WarnFormat(Properties.Resources.EMAIL_NOTIFIER_NO_SETTINGS);
            }
        }

        /// <summary>
        /// Called if the crawl failed.
        /// This method sends a message to all
        /// subscribed mail addresses who wants
        /// to crawl on this webpage if the crawl
        /// failed.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="urls">The urls.</param>
        public void CrawlFailed(string log, IEnumerable<string> urls)
        {
            try
            {
                Log.Warn(Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED);

                if (string.IsNullOrEmpty(log) == false)
                {
                    var mailBody = string.Format(Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED_BODY, log);

                    var mail = this.BuildMail(
                        this.settings.MailFrom,
                        Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED_SUBJECT);

                    var addresses = dbManager.GetEmailsByUrls(urls.ToArray());

                    this.AddMailRecipient(mail, addresses);

                    this.AddMailBody(mail, mailBody);
                    this.SendMail(mail);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }

        /// <summary>
        /// Deserializes the addresses.
        /// </summary>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>A string array containing the mail addresses.</returns>
        private IEnumerable<string> DeserializeAddresses(OrgUnit orgUnit)
        {
            var addresses = JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails).ToList<string>();

            Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_DESERIALIZE_ADDESSSES, addresses);

            return addresses;
        }

        /// <summary>
        /// Builds the mail.
        /// </summary>
        /// <param name="from">Email sender.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// A new mail message.
        /// </returns>
        private MailMessage BuildMail(string from, string subject)
        {
            Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_BUID_MAIL, from, subject);

            var mail = new MailMessage
                {
                    From = new MailAddress(@from),
                    Subject = subject
                };

            return mail;
        }

        /// <summary>
        /// Adds the mail recipients.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="addresses">The addresses.</param>
        private void AddMailRecipient(MailMessage mail, IEnumerable<string> addresses)
        {
            foreach (var address in addresses)
            {
                mail.To.Add(address);
                Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_ADD_RECIPIENT, address);
            }
        }

        /// <summary>
        /// Adds the mail body.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="body">The body.</param>
        private void AddMailBody(MailMessage mail, string body)
        {
            Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_ADD_MAILBODY, body);
            mail.Body = body;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mail">The mail.</param>
        private void SendMail(MailMessage mail)
        {
            Log.InfoFormat(Properties.Resources.EMAIL_NOTIFIER_SEND_MAIL, mail.From, mail.Subject, mail.To);
            var smtpServer = new SmtpClient(this.settings.SmtpServer);

            smtpServer.Send(mail);
        }
    }
}
