using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.Utilities.Wcf.Client;
using System.ServiceModel;

namespace MHM.WinFlexOne.Services.Integration.Factories
{
    public class ImporterProxyFactory : IProxyFactory
    {
        private readonly object _Lock = new object();
        private Dictionary<string, object> m_ProxyInstanceCache = new Dictionary<string, object>();

        public TServiceInterface Build<TServiceInterface>(BindingEnum binding) where TServiceInterface : class
        {
            string _CacheKey = typeof(TServiceInterface).Name + binding.ToString();
            lock (_Lock)
            {
                if (m_ProxyInstanceCache.ContainsKey(_CacheKey) == false)
                {
                    var _Proxy = GetProxy<TServiceInterface>(binding);
                    ((ICommunicationObject)_Proxy).Faulted += new EventHandler(Proxy_Faulted);
                    m_ProxyInstanceCache.Add(_CacheKey, _Proxy);
                }

                return (TServiceInterface)m_ProxyInstanceCache[_CacheKey];
            }
        }

        private TServiceInterface GetProxy<TServiceInterface>(BindingEnum binding) where TServiceInterface : class
        {
            switch (binding)
            {
                case BindingEnum.NetTcpStreaming:
                    NetTcpStreamingServiceClient<TServiceInterface> _StreamingTcpClient = new NetTcpStreamingServiceClient<TServiceInterface>(
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.UrlRoot,
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.UrlPath,
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.TcpPort);

                    _StreamingTcpClient.SendTimeout = 60 * 30;	//30 minutes, if a report takes that long the client is probably going to be upset anyhow
                    _StreamingTcpClient.Open();
                    return (TServiceInterface)_StreamingTcpClient.Channel;

                case BindingEnum.NetTcp:
                    NetTcpServiceClient<TServiceInterface> _NetTcpClient = new NetTcpServiceClient<TServiceInterface>(
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.UrlRoot,
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.UrlPath,
                        MHM.Utilities.Configuration.Utilities.Wcf.ImporterService.TcpPort);

                    _NetTcpClient.SendTimeout = 60 * 30;	//30 minutes, if a report takes that long the client is probably going to be upset anyhow
                    _NetTcpClient.Open();
                    return (TServiceInterface)_NetTcpClient.Channel;

                default:
                    throw new NotSupportedException(string.Format("Binding '{0}' is not supported.", binding));
            }
        }

        private void Proxy_Faulted(object sender, EventArgs e)
        {
            //the channel has faulted, so clear the cache - we dont want any clients trying to use a faulted channel
            //any new calls to get a proxy will have the create a new proxy instance.
            lock (_Lock)
            {
                m_ProxyInstanceCache.Clear();
            }
        }
    }
}
