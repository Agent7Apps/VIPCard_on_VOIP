using System;

using MRoundedButton;


using ObjCRuntime;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;



namespace privatephone
{
	public partial class ViewController : UIViewController
    {
        PushKit.PKPushRegistry pushRegistry = null;
        TVOCallInvite twilioCallInvite = null;
        TVOCall twilioCall = null;

        public string deviceTokenString;

        CallKit.CXProvider callKitProvider = null;
        CallKit.CXCallController callController = null;

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		nfloat buttonSize = 80F;
		nfloat backgroundViewHeight;
		nfloat backgroundViewWidth;

		List<RoundedButton> ButtonList = new List<RoundedButton>();

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

           // TwilioVoice.SharedInstance.LogLevel = TVOLogLevel.Verbose; //todo: make configurable
 
			UIImageView imageView = new UIImageView(View.Bounds);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.Image = UIImage.FromBundle("pic");
			View.AddSubview(imageView);

			backgroundViewHeight = (float)Math.Ceiling(UIScreen.MainScreen.Bounds.Height);
			backgroundViewWidth = UIScreen.MainScreen.Bounds.Width;

			int i = 0;
			CGRect backgroundRect = new CGRect(
				0,
				backgroundViewHeight * i,
				backgroundViewWidth,
				backgroundViewHeight);
			HollowBackgroundView backgroundView = new HollowBackgroundView(backgroundRect);
			backgroundView.ForegroundColor = UIColor.White;
			View.AddSubview(backgroundView);


			RoundedButton but1 = SetButtonProperties2(4, 3, 1, "");
			backgroundView.AddSubview(but1);
			ButtonList.Add(but1);

			RoundedButton but2 = SetButtonProperties2(4, 2, 2, "A B C");
			backgroundView.AddSubview(but2);
			ButtonList.Add(but2);

			RoundedButton but3 = SetButtonProperties2(4, 1, 3, "D E F");
			backgroundView.AddSubview(but3);
			ButtonList.Add(but3);

			RoundedButton but4 = SetButtonProperties2(3, 3, 4, "G H I");
			backgroundView.AddSubview(but4);
			ButtonList.Add(but4);

			RoundedButton but5 = SetButtonProperties2(3, 2, 5, "J K L");
			backgroundView.AddSubview(but5);
			ButtonList.Add(but5);

			RoundedButton but6 = SetButtonProperties2(3, 1, 6, "M N O");
			backgroundView.AddSubview(but6);
			ButtonList.Add(but6);

			RoundedButton but7 = SetButtonProperties2(2, 3, 7, "P Q R S");
			backgroundView.AddSubview(but7);
			ButtonList.Add(but7);

			RoundedButton but8 = SetButtonProperties2(2, 2, 8, "T U V");
			backgroundView.AddSubview(but8);
			ButtonList.Add(but8);

			RoundedButton but9 = SetButtonProperties2(2, 1, 9, "W X Y Z");
			backgroundView.AddSubview(but9);
			ButtonList.Add(but9);

			RoundedButton but10 = SetButtonProperties2(1, 3, 0, "*");
			backgroundView.AddSubview(but10);
			ButtonList.Add(but10);

			RoundedButton but11 = SetButtonProperties2(1, 2, 0, "+");
			backgroundView.AddSubview(but11);
			ButtonList.Add(but11);

			RoundedButton but12 = SetButtonProperties2(1, 1, 0, "#");
			backgroundView.AddSubview(but12);
			ButtonList.Add(but12);
		}

        //void configureCallKit()
        //{
        //    CallKit.CXProviderConfiguration configuration = new CallKit.CXProviderConfiguration("Callkit Quickstart");
        //    configuration.MaximumCallGroups = 1;
        //    configuration.MaximumCallsPerCallGroup = 1;

        //    UIImage callKitIcon = new UIImage(filename: "iconMask80");
        //    configuration.IconTemplateImageData = callKitIcon;

        //    callKitProvider = new CallKit.CXProvider(configuration);
        //    callKitProvider.SetDelegate(this, null);

        //    callController = new CallKit.CXCallController();
        //}

        // ACTION 
        //     void PlaceCall()
        //     {
        //         if (this.twilioCall != null && this.twilioCall.State == TVOCallState.Connected)
        //         {
        //             this.twilioCall.Disconnect();
        //             ToggleUIState(false);
        //}
        //    else
        //    {
        //        Guid callID = Guid.NewGuid();
        //        //this.
        //    }

        //    //NSString* handle = @"Voice Bot";

        //    //[self performStartCallActionWithUUID:uuid handle:handle];
        //    //  }
        //}

        //void ToggleUIState (bool isEnabled)
        //{
        //    // set the "placecall" to enabled / disabled. 
        //}

        //void PerformStartCallAction(NSUuid UUID, string userAgent)
        //{
        //    if ( UUID == null || string.IsNullOrEmpty(userAgent) )
        //        return;

        //    CallKit.CXHandle handle = new CallKit.CXHandle(CallKit.CXHandleType.Generic, userAgent);
        //    CallKit.CXStartCallAction startAction = new CallKit.CXStartCallAction(UUID, handle);
        //    CallKit.CXTransaction transaction = new CallKit.CXTransaction(startAction);

        //    //todo: make async
        //    callController.RequestTransaction(transaction, (NSError obj) =>
        //     {
        //         if (obj != null)
        //         {
        //             Console.WriteLine("an error occurred");
        //         }
        //         else
        //         {
        //             CallKit.CXCallUpdate callupdate = new CallKit.CXCallUpdate();
        //             callupdate.RemoteHandle = callhan;
        //             callupdate.SupportsDtmf = true;
        //             callupdate.SupportsHolding = false;
        //             callupdate.SupportsGrouping = false;
        //             callupdate.SupportsUngrouping = false;
        //             callupdate.HasVideo = false;

        //             callKitProvider.ReportCall(UUID, callupdate);
        //         }
        //    });
        //}

        //     void ReportIncomingCallFrom(string fromID, NSUuid uuid)
        //     {
        //         CallKit.CXHandle callHandle = new CallKit.CXHandle(CallKit.CXHandleType.Generic, value: fromID);

        //CallKit.CXCallUpdate callupdate = new CallKit.CXCallUpdate();
        //callupdate.RemoteHandle = callhan;
        //callupdate.SupportsDtmf = true;
        //callupdate.SupportsHolding = false;
        //callupdate.SupportsGrouping = false;
        //callupdate.SupportsUngrouping = false;
        //callupdate.HasVideo = false;

        //        callKitProvider.ReportNewIncomingCall(uuid, callupdate,(NSError obj) =>
        //        {
        //            if (obj == null)
        //            {
        //	// incoming call successfully reported 

        //	// RCP: Workaround per https://forums.developer.apple.com/message/169511
        //	TwilioVoice.SharedInstance.ConfigureAudioSession(); 
        //}
        //            else
        //            {
        //	//NSLog(@"Failed to report incoming call successfully: %@.", [error localizedDescription]);
        //}
        //    });
        //}

        void PerformEndCallActionWithUUID(NSUuid uuid)
        {
            if (uuid == null)
            {
                return;
            }

            CallKit.CXEndCallAction endCallAction = new CallKit.CXEndCallAction(uuid);
            CallKit.CXTransaction transaction = new CallKit.CXTransaction(endCallAction);

            callController.RequestTransaction(transaction, (NSError obj) =>
             {
                 if (obj != null)
                 {
                    //             NSLog(@"EndCallAction transaction request failed: %@", [error localizedDescription]);
                }
                 else
                 {
                    //NSLog(@"EndCallAction transaction request successful");
                }
             });
        }


        //#region PKPushRegistryDelegate
        //void PushRegistry(PushKit.PKPushRegistry registry, PushKit.PKPushCredentials credentials)
        //{
        //    if (true)
        //    {
        //        this.deviceTokenString = credentials.Token.Description;
        //        NSString accessToken = this.FetchAccessToken();

        //        TwilioVoice.SharedInstance.RegisterWithAccessToken(accessToken, deviceTokenString, (NSError obj) =>
        //          {
        //              if (obj != null)
        //              {
        //                //NSLog(@"An error occurred while registering: %@", [error localizedDescription]);
        //            }
        //              else
        //              {
        //                //NSLog(@"Successfully registered for VoIP push notifications.");
        //            }
        //          });
        //    }
        //}


		//// didInvalidatePushTokenForType
		//void PushRegistry(PushKit.PKPushRegistry registry, PushKit.PKPushType pushType)
     //   {
     //       //     NSLog(@"pushRegistry:didInvalidatePushTokenForType:");

     //       if (pushType == "PKPushTypeVoIP")
     //       {
     //           string accessToken = this.FetchAccessToken();

     //           TwilioVoice.SharedInstance.UnregisterWithAccessToken(accessToken, deviceTokenString,(NSError obj) => 
     //           {
					//if (obj != null)
					//{
					//	//                 NSLog(@"An error occurred while unregistering: %@", [error localizedDescription]);

					//}
					//else
					//{
					//	//NSLog(@"Successfully unregistered for VoIP push notifications.");
					//}
        //        });
        //    }
        //}

   //     void pushRegistry(PushKit.PKPushRegistry registry, PushKit.PKPushType pushType)
   //     {
			////NSLog(@"pushRegistry:didReceiveIncomingPushWithPayload:forType:");
   //         if (true ) // PKPushTypeVoIP
			//{
        //        TwilioVoice.SharedInstance.HandleNotification(payload: payload.DictionaryPayload, TVONotificationDelegate: this);
        //    }
        //}
        //#endregion
      

        RoundedButton SetButtonProperties2(int row, int column, int topText, string bottomText)
		{
			var minButtonAdjacentHorizontalSpace = buttonSize * 3;
			var freeSpaceWidth = backgroundViewWidth - minButtonAdjacentHorizontalSpace;
			if (freeSpaceWidth > 0)
			{
			}
			else
			{
				// ERROR
			}

			var minButtonAdjacentVerticalSpace = buttonSize * 4;
			var freeSpaceHeight = backgroundViewHeight - minButtonAdjacentVerticalSpace;
			if (freeSpaceHeight > 0)
			{
			}
			else
			{
				// ERROR
			}

			// Arrange into neat columns and rows. (rows are closer visually than columns.  See iPhone app) 
			var hBuffer = freeSpaceWidth / 4;
			if (hBuffer > 1)
			{
				hBuffer = buttonSize / 4;
			}

			var vBuffer = freeSpaceHeight / 6;
			if (vBuffer > 1)
			{
				vBuffer = buttonSize / 6;
			}

			// Calculate what's needed to center the group of buttons above. 
			var totalUsedHeightSpace = (vBuffer * 6) + (buttonSize * 4);
			var moveButtonGroupUp = (backgroundViewHeight - totalUsedHeightSpace) / 2;

			var totalUsedWidthSpace = (hBuffer * 4) + (buttonSize * 4);

			var moveButtonGroupLeft = totalUsedWidthSpace - backgroundViewWidth; // center it

			CGRect buttonRect2 = new CGRect(
				(backgroundViewWidth - moveButtonGroupLeft - (hBuffer * column) - (buttonSize * column)),
				(backgroundViewHeight - moveButtonGroupUp - (vBuffer * row) - (buttonSize * row)),
				buttonSize,
				buttonSize
			);
			RoundedButton button2 = new RoundedButton(
						   buttonRect2,
						   RoundedButtonStyle.Subtitle,
						   "1" // identifier defined in App Delegate as "appearance proxy."
						);

			switch (bottomText)
			{
				case "*":
					button2.TextLabel.Text = "*";
					button2.DetailTextLabel.Text = "";
					break;
				case "#":
					button2.TextLabel.Text = "#";
					button2.DetailTextLabel.Text = "";
					break;
				default:
					button2.TextLabel.Text = topText.ToString();
					button2.DetailTextLabel.Text = bottomText;
					button2.TouchUpInside += (sender, ea) =>
					{
						SendToCallKit(topText);
					};
					break;
			}

			button2.BackgroundColor = UIColor.Clear;
			button2.TextLabel.Font = UIFont.SystemFontOfSize(40);
			button2.DetailTextLabel.Font = UIFont.SystemFontOfSize(10);

			// Remember to dispose this!
			return button2;
		}


		//public ActiveCallManager CallManager { get; set; }
		//public ProviderDelegate CallProviderDelegate { get; set; }
		//public string TestSessionToken = @"";
		//public string twilioAccountSID = "ACe2d47a68500e9193089cfdc331dfacd8";
		//public string asdf = "1-925-420-9111";
		//public string VOIPServerURL = "https://privatevoice.azurewebsites.net";
		//public string VOIPServerURLEndpoint = "BurnerPhone/Token";

		public void SendToCallKit(int thingToSend)
		{
            AppDelegate.SharedAppInstance.CallManager.StartCall("+16468970907");

            //JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            //var messageList = JsonConvert.DeserializeObject<List<Message>>(jsonResponse["messages"].ToString());
            //foreach (var message in messageList)
            //{
            //	Console.WriteLine("To: {0}", message.To);
            //	Console.WriteLine("From: {0}", message.From);
            //	Console.WriteLine("Body: {0}", message.Body);
            //	Console.WriteLine("Status: {0}", message.Status);
            //}
        }
	 

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		protected override void Dispose(bool disposing)
		{
			foreach (var button in ButtonList)
			{
				//button.TouchUpInside -= HandleTouchUpInside;
				button.Dispose();
			}
			//base.Dispose(disposing
		}
	}
}