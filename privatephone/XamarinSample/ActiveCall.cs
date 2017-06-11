using System;
using CoreFoundation;
using Foundation;

using RestSharp;

using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace privatephone
{

	public class ActiveCall
	{
		#region Private Variables
		private bool _isConnecting;
		private bool _isConnected;
		private bool _isOnhold;
		#endregion

		#region Computed Properties
		public NSUuid UUID { get; set; }
		public bool isOutgoing { get; set; }
		public string Handle { get; set; }
		public DateTime StartedConnectingOn { get; set; }
		public DateTime ConnectedOn { get; set; }
		public DateTime EndedOn { get; set; }

		public bool isConnecting
		{
			get { return _isConnecting; }
			set
			{
				_isConnecting = value;
				if (_isConnecting) StartedConnectingOn = DateTime.Now;
				RaiseStartingConnectionChanged();
			}
		}

		public bool isConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				if (_isConnected)
				{
					ConnectedOn = DateTime.Now;
				}
				else
				{
					EndedOn = DateTime.Now;
				}
				RaiseConnectedChanged();
			}
		}

		public bool isOnHold
		{
			get { return _isOnhold; }
			set
			{
				_isOnhold = value;
			}
		}
		#endregion

		#region Constructors
		public ActiveCall()
		{
		}

		public ActiveCall(NSUuid uuid, string handle, bool outgoing)
		{
			// Initialize
			this.UUID = uuid;
			this.Handle = handle;
			this.isOutgoing = outgoing;
		}
        #endregion

        ValidTVOCallDelegate callDelegate;
        TVOCall callInstance;

		public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
		{
			var tcs = new TaskCompletionSource<IRestResponse>();
			theClient.ExecuteAsync(theRequest, response =>
			{
				tcs.SetResult(response);
			});
			return tcs.Task;
		}


		#region Public Methods
		public void StartCall(ActiveCallbackDelegate completionHandler)
		{
            // todo: make sure the start call and call status cahnged events are handled correctly. 

             callDelegate = new ValidTVOCallDelegate(completionHandler, this);
            // todo: make the delegate methods call the relevant ACTION delegates (connect/hangup)

            NSDictionary<NSString, NSString> twimlParam = new NSDictionary<NSString, NSString>();

			// The application needs to use this method to set up the AVAudioSession 
            // with desired configuration before letting the CallKit framework activate the audio session.
			TwilioVoice.SharedInstance.ConfigureAudioSession();

			var client = new RestClient("http://privatevoice.azurewebsites.net/");

			var request = new RestRequest("BurnerPhone/Token?clientName=test", Method.GET);
			//request.AddParameter("To", "+17143913337");
			//request.AddParameter("From", "+15416677319");
			//request.AddParameter("Body", "Yo yo yo remove the json!");      

			client.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator("ACda7fae80dbacc70808a453daf19951db", "c50527004eebf9ff58cb3749f51d540f");

			var response = new RestResponse();

			Task.Run(async () =>
				{
					response = await GetResponseContentAsync(client, request) as RestResponse;
				}).Wait();


			var output = response.Content;
			Console.WriteLine("JWT= " +output);
 
            callInstance  = TwilioVoice.SharedInstance.Call(
                output,
                twimlParam,
                callDelegate
            );
     
            // Simulate the call starting successfully
            completionHandler(true);

            //// Simulate making a starting and completing a connection
            //DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 3000), () =>
            //{
            //	// Note that the call is starting
            //	isConnecting = true;

            //	// Simulate pause before connecting
            //	DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 1500), () =>
            //		{
            //			// Note that the call has connected
            //			isConnecting = false;
            //			isConnected = true;
            //		});
            //});
        }

		public void AnswerCall(ActiveCallbackDelegate completionHandler)
		{
            ValidTVONotificationDelegate incomingDelegate = new ValidTVONotificationDelegate();

			// RCP: Workaround from https://forums.developer.apple.com/message/169511 suggests configuring audio in the
			//      completion block of the `reportNewIncomingCallWithUUID:update:completion:` method instead of in
			//      `provider:performAnswerCallAction:` per the WWDC examples.
			// [[TwilioVoice sharedInstance] configureAudioSession];

			NSDictionary<NSString, NSString> twimlParam = new NSDictionary<NSString, NSString>();

           // incomingDelegate

			// Simulate the call being answered
			isConnected = true;
			completionHandler(true);
		}

		public void EndCall(ActiveCallbackDelegate completionHandler)
		{
            TwilioVoice.SharedInstance.StopAudioDevice();

            if (callInstance != null)
            {
				isConnected = false;
				// Calling this method on a TVOCall that does not have the state of TVOCallStateConnected will have no effect.
				callInstance.Disconnect();
				completionHandler(true);
			}
            else
            {
                completionHandler(false);
			}
        }
		#endregion

		#region Events
		public delegate void ActiveCallbackDelegate(bool successful);
		public delegate void ActiveCallStateChangedDelegate(ActiveCall call);

		public event ActiveCallStateChangedDelegate StartingConnectionChanged;
		internal void RaiseStartingConnectionChanged()
		{
			if (this.StartingConnectionChanged != null) this.StartingConnectionChanged(this);
		}

		public event ActiveCallStateChangedDelegate ConnectedChanged;
		internal void RaiseConnectedChanged()
		{
			if (this.ConnectedChanged != null) this.ConnectedChanged(this);
		}
		#endregion
	}

}