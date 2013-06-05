// ------------------------------------------------------------------------
// <copyright file="NutchManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------

namespace HSA.InfoSys.Nutch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// mang
    /// </summary>
    public class NutchManager
    {
       private string PrefixPath;
       private string URLPath;


       public NutchManager()
       {
           this.URLPath = "C:/Users/Sanim/Documents/apache-nutch-1.6/bin/urls";
           this.PrefixPath = "C:/Users/Sanim/Documents/apache-nutch-1.6/conf/regex-urlfilter.txt";
       }

       public void MkUserDir(string User)
       {
           string NewDirectory = string.Format("{0}/{1}", this.URLPath, User);
           Directory.CreateDirectory(NewDirectory);
       }
       private void AddURL(bool Prefix, string[] URLs, string User)
       {

       }
       
    }
}
