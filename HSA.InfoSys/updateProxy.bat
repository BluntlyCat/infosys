svcutil http://localhost:8085/GetMetaInformation /out:Proxy.cs
copy Proxy.cs HSA.InfoSys.GUI\Proxy.cs
copy Proxy.cs HSA.InfoSys.WCFTesting\Proxy.cs
del Proxy.cs