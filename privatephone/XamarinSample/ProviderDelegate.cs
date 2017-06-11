using System;
using Foundation;
using CallKit;
using UIKit;

namespace privatephone
{

    /// <summary>
    /// External events, like incoming calls, call ended, call connected. 
    /// </summary>
	public class ProviderDelegate : CXProviderDelegate
	{
		#region Computed Properties
		public ActiveCallManager CallManager { get; set; }
		public CXProviderConfiguration Configuration { get; set; }
		public CXProvider Provider { get; set; }
		#endregion
 
		#region Constructors
        /// <summary>
        /// Allow the app to access the list of calls,  by UUID
        /// </summary>
        /// <param name="callManager"></param>
		public ProviderDelegate(ActiveCallManager callManager)
		{
			// Save connection to call manager
			CallManager = callManager;

			// Define handle types
			var handleTypes = new[] { (NSNumber)(int)CXHandleType.PhoneNumber };

			// Get Image Mask
			var maskImage = UIImage.FromFile("telephone_receiver.png");

            // Setup the initial configurations
            Configuration = new CXProviderConfiguration(localizedName: "MonkeyCall")
            {
                MaximumCallsPerCallGroup = 1,
                SupportedHandleTypes = new NSSet<NSNumber>(handleTypes),
                //IconMaskImageData = maskImage.AsPNG(),
                RingtoneSound = "musicloop01.wav",

                //MaximumCallGroups = 3,
                //SupportsVideo = false
			};


            // TODO: WWDC 2016 11:12
            //if (CXProvider.authorization = Notddetermiened )
                  // provider.requestAuthorization

            // Create a new provider
            Provider = new CXProvider(Configuration);

			// Attach this delegate
			Provider.SetDelegate(this, null);


		}
		#endregion

		#region Override Methods
		public override void DidReset(CXProvider provider)
		{
            Console.WriteLine("CXProviderDelegate: DidReset");
            // Remove all calls
            CallManager.Calls.Clear();
		}

		public override void PerformStartCallAction(CXProvider provider, CXStartCallAction action)
		{
			Console.WriteLine("CXProviderDelegate: PerformStartCallAction");

			// Create new call record
			var activeCall = new ActiveCall(action.CallUuid, action.CallHandle.Value, true);

			// Monitor state changes
			activeCall.StartingConnectionChanged += (call) =>
			{
				if (call.isConnecting)
				{
						// Inform system that the call is starting
						Provider.ReportConnectingOutgoingCall(call.UUID, call.StartedConnectingOn.ToNSDate());
				}
			};

			activeCall.ConnectedChanged += (call) =>
			{
				if (call.isConnected)
				{
						// Inform system that the call has connected
						provider.ReportConnectedOutgoingCall(call.UUID, call.ConnectedOn.ToNSDate());
				}
			};

			// Start call
			activeCall.StartCall((successful) =>
			{
                // Was the call able to be started?
                if (successful)
				{
						// Yes, inform the system
						action.Fulfill();

						// Add call to manager
						CallManager.Calls.Add(activeCall);
				}
				else
				{
						// No, inform system
						action.Fail();
				}
			});
		}

        public override void PerformAnswerCallAction(CXProvider provider, CXAnswerCallAction action)
        {
			Console.WriteLine("CXProviderDelegate: PerformAnswerCallAction " + action.CallUuid );

			// Find requested call
			var call = CallManager.FindCall(action.CallUuid);

            // Found?
            if (call == null)
            {
                // No, inform system and exit
                action.Fail();
                return;
            }
             
            // Attempt to answer call
            call.AnswerCall((successful) =>
            {
                // Was the call successfully answered?
                if (successful)
                {
                    // Yes, inform system
                    action.Fulfill();
                }
                else
                {
                    // No, inform system
                    action.Fail();
                }
            });
        }
 
        public override void PerformEndCallAction(CXProvider provider, CXEndCallAction action)
        {
			Console.WriteLine("CXProviderDelegate: PerformEndCallAction " + action.CallUuid);

			// Find requested call
			var call = CallManager.FindCall(action.CallUuid);

            // Found?
            if (call == null)
            {
                // No, inform system and exit
                action.Fail();
                return;
            }

            // Attempt to answer call
            call.EndCall((successful) =>
            {
                // Was the call successfully answered?
                if (successful)
                {
                    // Remove call from manager's queue
                    CallManager.Calls.Remove(call);

                    // Yes, inform system
                    action.Fulfill();
                }
                else
                {
                    // No, inform system
                    action.Fail();
                }
            });
        }

		public override void PerformSetHeldCallAction(CXProvider provider, CXSetHeldCallAction action)
		{
			Console.WriteLine("CXProviderDelegate: PerformSetHeldCallAction " + action.CallUuid);

			// Find requested call
			var call = CallManager.FindCall(action.CallUuid);
             
			// Found?
			if (call == null)
			{
				// No, inform system and exit
				action.Fail();
				return;
			}

			// Update hold status
			call.isOnHold = action.OnHold;

			// Inform system of success
			action.Fulfill();
		}

		public override void TimedOutPerformingAction(CXProvider provider, CXAction action)
		{
			Console.WriteLine("CXProviderDelegate: TimedOutPerformingAction " );

			// Inform user that the action has timed out
		}


        /// <summary>
        /// Causes the VOIP app to be handled at a higher priority
        ///  WWDC 2016 15:54
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="audioSession"></param>
        public override void DidActivateAudioSession(CXProvider provider, AVFoundation.AVAudioSession audioSession)
		{
			Console.WriteLine("CXProviderDelegate: DidActivateAudioSession ");

			// The application needs to use this method to signal the SDK to start audio I/O 
			// units when receiving the audio activation callback of CXProviderDelegate.
			TwilioVoice.SharedInstance.StartAudioDevice();
		}

        /// <summary>
        /// Causes the VOIP app to be handled at a higher priority
        ///  WWDC 2016 15:54
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="audioSession"></param>
        public override void DidDeactivateAudioSession(CXProvider provider, AVFoundation.AVAudioSession audioSession)
		{
			Console.WriteLine("CXProviderDelegate: DidDeactivateAudioSession ");

			// End the calls audio session and restart any non-call
			// related audio
		}

		//public override void DidBegin(CXProvider provider)
		//{
		//    //base.DidBegin(provider);
		//}
		//public override bool ExecuteTransaction(CXProvider provider, CXTransaction transaction)
		//{
		//    //return base.ExecuteTransaction(provider, transaction);
		//}



		#endregion

		#region Public Methods
		public void ReportIncomingCall(NSUuid uuid, string handle)
		{
			Console.WriteLine("CXProviderDelegate: ReportIncomingCall ");

			// Create update to describe the incoming call and caller
			var update = new CXCallUpdate();
			update.RemoteHandle = new CXHandle(CXHandleType.Generic, handle);
            //update.RemoteHandle = new CXHandle(CXHandleType.PhoneNumber, handle);
            //update.RemoteHandle = new CXHandle(CXHandleType.EmailAddress, handle);

			// Report incoming call to system
			Provider.ReportNewIncomingCall(uuid, update, (error) =>
			{
                // Was the call accepted
                if (error == null)
				{
						// Yes, report to call manager
						CallManager.Calls.Add(new ActiveCall(uuid, handle, false));
				}
				else
				{
						// Report error to user here
						Console.WriteLine("Error: {0}", error);
				}
			});
		}
		#endregion

	}

}