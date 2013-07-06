namespace MHM.WinFlexOne.Services.Integration
{
    public interface IProxyFactory
    {
        TServiceInterface Build<TServiceInterface>(BindingEnum binding) where TServiceInterface : class;
    }

    public enum BindingEnum
    { 
        InProcess,
        BasicHttp,
        WsHttp,
        NetTcp,
        NetTcpStreaming,
    }
}
