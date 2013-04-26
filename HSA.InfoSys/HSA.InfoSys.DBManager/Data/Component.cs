namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Logging;
    using log4net;


    public class Component
    {
        private static readonly ILog log = Logging.GetLogger("Component");

        protected Component()
        {
            log.Error("NHIBERNATE calls this...");
        }

        public virtual Guid userGUID  { get; set; }

        public virtual string name { get; set; }

        public virtual string category { get; set; }

        public virtual int componentId { get; set; }

       
    }
}
