namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Issue
    {

        public virtual Guid issueGUID { get; set; }

        public virtual string text { get; set; }

        public virtual string titel { get; set; }

        public virtual int threatLevel { get; set; }

        //public virtual int issueId { get; set; }

        public virtual DateTime date { get; set; }

        public virtual Component component { get; set; }

        public virtual Source source { get; set; }
    }
}
