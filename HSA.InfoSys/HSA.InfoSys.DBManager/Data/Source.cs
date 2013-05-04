

namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Source
    {
        public virtual Guid sourceGUID { get; set; }
        //public virtual int sourceId { get; set; }
        public virtual string URL { get; set; }
    }
}
