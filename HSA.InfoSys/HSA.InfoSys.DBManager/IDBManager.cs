using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using HSA.InfoSys.DBManager.Data;

namespace HSA.InfoSys.DBManager
{
    public interface IDBManager
    {
        void addNewObject(Object obj);
        void updateObject(Object obj);
        
    }
}
