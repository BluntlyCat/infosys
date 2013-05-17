svcutil http://localhost:8085/GetMetaInformation /out:Proxy.cs
copy Proxy.cs HSA.InfoSys.GUI\WCFProxy.cs
copy Proxy.cs HSA.InfoSys.WCFTesting\WCFProxy.cs
del Proxy.cs