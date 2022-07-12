using System;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WebGl
{
	public class WebGLConnect : MonoBehaviour
	{		
		[SerializeField] private WebGL.SupportedWallets _defaultWallet = WebGL.SupportedWallets.None;
		[SerializeField] private NetworkName _defaultNetwork = NetworkName.Rinkeby;
		[SerializeField] private bool _connectOnAwake;
		[SerializeField] private bool _connectOnStart = true;
		private UniTaskCompletionSource<WebGL.SupportedWallets> _walletCompletionSource;
		
		public WebGL.WebGLWrapper Session { get; private set; }
		public EventHandler OnNeedPanel;
		public EventHandler<WebGL.WebGLWrapper> OnConnect;
		

		private async void Awake()
		{
#if UNITY_WEBGL
			if (_connectOnAwake)
			{
				await Connect(); 
			}
#endif
		}

		private async void Start()
		{
#if UNITY_WEBGL
			if (_connectOnStart)
			{
				await Connect(); 
			}
#endif
		}

		private async UniTask Connect()
		{
			Session = new WebGL.WebGLWrapper();
			var wallet = _defaultWallet;
			if (wallet == WebGL.SupportedWallets.None || wallet == null)
			{
				OnNeedPanel?.Invoke(this, EventArgs.Empty);
				_walletCompletionSource = new UniTaskCompletionSource<WebGL.SupportedWallets>();
				wallet = await _walletCompletionSource.Task;
			}
			await Session.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
			OnConnect?.Invoke(this, Session);
		}

		public void SetWallet(WebGL.SupportedWallets wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}
		
		public void SetNetwork(NetworkName network)
		{
			_defaultNetwork = network;
		}
	}
}