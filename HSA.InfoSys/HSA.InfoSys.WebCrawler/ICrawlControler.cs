namespace HSA.InfoSys.WebCrawler
{
    using System.ServiceModel;

    /// <summary>
    /// This is the interface for communication between the GUI and the Webcrawler
    /// If we change something in this interface we have to create a new Proxy.cs
    /// class by typing the following command:
    /// svcutil http://localhost:8085/GetMetaInformation /out:Proxy.cs
    /// But there is also a script called updateProxy in the main project folder for doing this.
    /// </summary>
    [ServiceContract]
    public interface ICrawlControler
    {
        [OperationContract]
        string StartSearch();

        [OperationContract]
        bool ShutdownServices();
    }
}
