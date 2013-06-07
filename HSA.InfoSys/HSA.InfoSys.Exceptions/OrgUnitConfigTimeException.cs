namespace HSA.InfoSys.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class OrgUnitConfigTimeException : Exception
    {
        public OrgUnitConfigTimeException(string name) : base(Properties.Resources.ORGUNIT_CONFIG_TIME_ZERO_EXCEPTION)
        {
            this.Source = name;
        }
    }
}
