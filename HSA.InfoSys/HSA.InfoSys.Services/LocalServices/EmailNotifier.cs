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
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using Newtonsoft.Json;
    using NHibernate;

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
        private IDBManager dbManager = DBManager.ManagerFactory(Guid.NewGuid());

        /// <summary>
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="results">The results.</param>
        public void SearchFinished(Guid orgUnitGUID, List<Result> results)
        {
            Log.DebugFormat(Properties.Resources.SEARCH_RECALL, orgUnitGUID);

            try
            {
                Log.InfoFormat(Properties.Resources.EMAIL_NOTIFIER_SEARCH_FINISHED, orgUnitGUID);

                var orgUnit = this.dbManager.GetEntity(
                    orgUnitGUID,
                    this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                var mailBody = string.Empty;
                var oldComponentGUID = Guid.Empty;

                var addresses = this.DeserializeAddresses(orgUnit);
                var subject = string.Format(
                    Properties.Resources.EMAIL_NOTIFIER_ISSUE_FOUND_SUBJECT,
                    orgUnit.Name);

                var mail = this.BuildMail(
                    Properties.Settings.Default.EMAIL_NOTIFIER_FROM,
                    subject);

                this.AddMailRecipient(mail, orgUnitGUID, addresses);

                foreach (var result in results)
                {
                    if (oldComponentGUID.Equals(Guid.Empty) || oldComponentGUID.Equals(result.ComponentGUID) == false)
                    {
                        var component = this.dbManager.GetEntity(result.ComponentGUID) as Component;
                        mailBody += string.Format("{0}:\n", component.Name);
                    }

                    oldComponentGUID = result.ComponentGUID;
                    mailBody += string.Format("\t{0} - {1}\n\n", result.Title, result.URL);
                }

                this.AddMailBody(mail, mailBody);
                this.SendMail(mail);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
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
                Log.WarnFormat(Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED, orgUnitGUID);

                var orgUnit = this.dbManager.GetEntity(
                    orgUnitGUID,
                    this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                var mailBody = string.Format(Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED_BODY, orgUnit.Name);

                var addresses = this.DeserializeAddresses(orgUnit);

                var mail = this.BuildMail(
                    Properties.Settings.Default.EMAIL_NOTIFIER_FROM,
                    Properties.Resources.EMAIL_NOTIFIER_CRAWL_FAILED_SUBJECT);

                this.AddMailRecipient(mail, orgUnitGUID, addresses);

                this.AddMailBody(mail, mailBody);
                this.SendMail(mail);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }

        /// <summary>
        /// Sends the mail to entity owner.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public void SendMailToEntityOwner(Entity entity, string subject, string body)
        {
            Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_SEND_MAIL_TO_ENTITY_OWNER, entity, subject, body);

            var orgUnit = this.GetOrgUnit(entity);

            var mailBody = body;

            var addresses = this.DeserializeAddresses(orgUnit);

            var mail = this.BuildMail(
                Properties.Settings.Default.EMAIL_NOTIFIER_FROM,
                subject);

            this.AddMailRecipient(mail, orgUnit.EntityId, addresses);

            this.AddMailBody(mail, mailBody);
            this.SendMail(mail);
        }

        /// <summary>
        /// Gets the org unit config.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The OrgUnitConfig.</returns>
        private OrgUnit GetOrgUnit(Entity entity)
        {
            OrgUnit orgUnit = null;

            if (entity.GetType() == typeof(OrgUnit))
            {
                return entity as OrgUnit;
            }
            else if (entity.GetType() == typeof(Component))
            {
                var component = entity as Component;

                using (ISession session = DBManager.Session)
                {
                    orgUnit = session.QueryOver<OrgUnit>()
                        .Where(x => x.EntityId == component.OrgUnitGUID)
                        .SingleOrDefault();

                    orgUnit.OrgUnitConfig.Unproxy();
                }
            }
            else if (entity.GetType() == typeof(Result))
            {
                using (ISession session = DBManager.Session)
                {
                    var result = entity as Result;
                    var component = DBManager.Session.QueryOver<Component>()
                        .Where(x => x.EntityId == result.ComponentGUID)
                        .SingleOrDefault();

                    orgUnit = DBManager.Session.QueryOver<OrgUnit>()
                        .Where(x => x.EntityId == component.OrgUnitGUID)
                        .SingleOrDefault();

                    orgUnit.OrgUnitConfig.Unproxy();
                }
            }

            Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_GET_ORG_UNIT, orgUnit, entity);

            return orgUnit;
        }

        /// <summary>
        /// Deserializes the addresses.
        /// </summary>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>A string array containing the mail addresses.</returns>
        private IList<string> DeserializeAddresses(OrgUnit orgUnit)
        {
#warning Emails können null sein? Die OrgUnitConfig wird ja von einem Benutzer angelegt, dessen Email Adresse im System hinterlegt ist...
            if (orgUnit.OrgUnitConfig.Emails != null)
            {
                var addresses = JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails).ToList<string>();

                Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_DESERIALIZE_ADDESSSES, addresses);

                return addresses;
            }
            else
            {
                Log.Warn(Properties.Resources.EMAIL_NOTIFIER_NO_ADDRESSES);
                return new List<string>();
            }
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

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(from);
            mail.Subject = subject;

            return mail;
        }

        /// <summary>
        /// Adds the mail recipients.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="addresses">The addresses.</param>
        private void AddMailRecipient(MailMessage mail, Guid orgUnitGUID, IList<string> addresses)
        {
            foreach (var address in addresses)
            {
                mail.To.Add(address);
                Log.DebugFormat(Properties.Resources.EMAIL_NOTIFIER_ADD_RECIPIENT, address, orgUnitGUID);
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
            SmtpClient smtpServer = new SmtpClient(Properties.Settings.Default.SMTP_SERVER);
            smtpServer.Send(mail);
        }
    }
}
