namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;

    /// <summary>
    /// This is the interface for communication between the GUI and the Webcrawler
    /// If we change something in this interface we have to create a new Proxy.cs
    /// class by typing the following command:
    /// svcutil http://localhost:8085/GetMetaInformation /out:Proxy.cs
    /// </summary>
    [ServiceContract]
    public interface ICrawlControler
    {
        [OperationContract]
        string StartSearch();
    }
}
