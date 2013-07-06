using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.Interfaces.Public.ClaimsService;
using MHM.Utilities.Wcf.Client;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using MHM.WinFlexOne.Interfaces.Public.CardRequestBatch;

namespace MHM.WinFlexOne.Services.Integration.Factories
{
	public class CardRequestBatchProxyFactory : IProxyFactory
	{
		private readonly object _Lock = new object();
		private Dictionary<string, object> m_ProxyInstanceCache = new Dictionary<string, object>();

		public TServiceInterface Build<TServiceInterface>( BindingEnum binding ) where TServiceInterface : class
		{
			string _CacheKey = typeof( TServiceInterface ).Name + binding.ToString();
			lock ( _Lock )
			{
				if ( m_ProxyInstanceCache.ContainsKey( _CacheKey ) == false )
				{
					var _Proxy = GetProxy( binding );
					m_ProxyInstanceCache.Add( _CacheKey, _Proxy );
				}

				return (TServiceInterface)m_ProxyInstanceCache[_CacheKey];
			}
		}

		private ICardRequestBatchService GetProxy( BindingEnum binding )
		{
			switch ( binding )
			{
				case BindingEnum.InProcess:
					return new CardRequestBatchService.CardRequestBatchService();

				default:
					throw new NotSupportedException( string.Format( "Binding '{0}' is not supported.", binding ) );
			}
		}

		private void Proxy_Faulted( object sender, EventArgs e )
		{
			//the channel has faulted, so clear the cache - we dont want any clients trying to use a faulted channel
			//any new calls to get a proxy will have the create a new proxy instance.
			lock ( _Lock )
			{
				m_ProxyInstanceCache.Clear();
			}
		}
	}
}
