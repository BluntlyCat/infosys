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
        private IDBManager dbManager = DBManager.ManagerFactory;

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
                var orgUnit = this.dbManager.GetEntity(
                    orgUnitGUID,
                    this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                var mailBody = string.Empty;
                var oldResultGUID = Guid.Empty;

                var addresses = DeserializeAddresses(orgUnit);
                var subject = string.Format(
                    "New issues for System {0} found.",
                    orgUnit.Name);

                var mail = this.BuildMail(
                    "michael.juenger1@hs-augsburg.de",
                    subject);

                this.AddMailRecipient(mail, orgUnitGUID, addresses);

                foreach (var result in results.ToList<Result>())
                {
                    if (oldResultGUID.Equals(Guid.Empty) || oldResultGUID.Equals(result.EntityId) == false)
                    {
                        var component = this.dbManager.GetEntity(result.ComponentGUID) as Component;
                        mailBody += string.Format("{0}:\n\n", component.Name);
                    }

                    oldResultGUID = result.EntityId;
                    mailBody += string.Format("{0} - {1}\n", result.Title, result.URL);
                }

                this.AddMailBody(mail, mailBody);
                this.SendMail(mail);
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
                var orgUnit = this.dbManager.GetEntity(
                    orgUnitGUID,
                    this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                var mailBody = string.Format("The crawl for {0} failed.", orgUnit.Name);

                var addresses = DeserializeAddresses(orgUnit);

                var mail = this.BuildMail(
                    "michael.juenger1@hs-augsburg.de",
                    "Crawl failed.");

                this.AddMailRecipient(mail, orgUnitGUID, addresses);

                this.AddMailBody(mail, mailBody);
                this.SendMail(mail);
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
        /// Sends the mail to entity owner.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public void SendMailToEntityOwner(Entity entity, string subject, string body)
        {
            var orgUnit = GetOrgUnit(entity);

            var mailBody = body;

            var addresses = DeserializeAddresses(orgUnit);

            var mail = this.BuildMail(
                "michael.juenger1@hs-augsburg.de",
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
        private static OrgUnit GetOrgUnit(Entity entity)
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

            return orgUnit;
        }

        /// <summary>
        /// Deserializes the addresses.
        /// </summary>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>A string array containing the mail addresses.</returns>
        private static IList<string> DeserializeAddresses(OrgUnit orgUnit)
        {
#warning Emails können null sein? Die OrgUnitConfig wird ja von einem Benutzer angelegt, dessen Email Adresse im System hinterlegt ist...
            if (orgUnit.OrgUnitConfig.Emails != null)
            {
                var addresses = JsonConvert.DeserializeObject<string[]>(orgUnit.OrgUnitConfig.Emails);
                return addresses.ToList<string>();
            }
            else
            {
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
                Log.DebugFormat("Send mail to {0} for OrgUnit {1}.", address, orgUnitGUID);
            }
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
            SmtpClient smtpServer = new SmtpClient("smtp.hs-augsburg.de");
            smtpServer.Send(mail);
        }
    }
}
