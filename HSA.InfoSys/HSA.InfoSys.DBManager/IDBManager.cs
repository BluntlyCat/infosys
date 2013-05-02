using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace HSA.InfoSys.DBManager
{
    public interface IDBManager
    {

        void addNewIssue(Guid issueGUID, int issueId, string text, string titel, int threatLevel, DateTime date);
        void addNewComponent(Guid componentGUID, int componentId, string category, string name);
        void addNewSource(Guid sourceGUID, int sourceId, string URL);

    }
}
