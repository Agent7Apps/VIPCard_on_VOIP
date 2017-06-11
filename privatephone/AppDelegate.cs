using Foundation;
using UIKit;
using Intents;
using System;
using CallKit;
using MRoundedButton;
using System.Collections.Generic;
using System.Linq;

// https://developer.apple.com/library/content/documentation/iPhone/Conceptual/iPhoneOSProgrammingGuide/StrategiesforImplementingYourApp/StrategiesforImplementingYourApp.html
//https://developer.xamarin.com/guides/ios/platform_features/introduction-to-ios10/security-privacy-enhancements/

namespace privatephone
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		#region Constructors
		public override UIWindow Window { get; set; }
		public ActiveCallManager CallManager { get; set; }
		public ProviderDelegate CallProviderDelegate { get; set; }

        public static  AppDelegate SharedAppInstance = null;
 
        // APP Side
		public string TestSessionToken = @" .   . ";
		//test sid AC01351d2df40bfcd9ccf167356f407a07
		public string twilioAccountSID = "ACe2d47a68500e9193089cfdc331dfacd8";
        public string testNumber = "1-925-420-9111";
		//public string VOIPServerURL = "https://voip.getvalid.com";
		public string VOIPServerURL = "https://privatevoice.azurewebsites.net";
		public string VOIPServerURLEndpoint = "BurnerPhone/Token";
		#endregion


		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
		//	Xamarin.Calabash.Start();
#endif
 

            Console.WriteLine("Twilio Voice Version: " +  TwilioVoice.SharedInstance.Version);

            NSDictionary appearanceProxy1 = GetNSDictionary(new Dictionary<string, object> {
				{ RoundedButtonAppearanceKeys.CornerRadius , 40 },
				{ RoundedButtonAppearanceKeys.BorderWidth  , 2 },
				{ RoundedButtonAppearanceKeys.BorderColor  , UIColor.Clear },
				{ RoundedButtonAppearanceKeys.ContentColor , UIColor.Black },
				{ RoundedButtonAppearanceKeys.ContentAnimateToColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ForegroundColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ForegroundAnimateToColor , UIColor.Clear }
			});
			RoundedButtonAppearanceManager.RegisterAppearanceProxy (appearanceProxy1, "1");
			

			// Initialize the call handlers
			CallManager = new ActiveCallManager();
			CallProviderDelegate = new ProviderDelegate(CallManager);

            SharedAppInstance = this;

			return true;
		}

		static NSDictionary GetNSDictionary(Dictionary<string, object> source)
		{
			return NSDictionary.FromObjectsAndKeys(
				source.Values.ToArray(),
				source.Keys.ToArray());
		}
		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background execution this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transition from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.

		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			// Get handle from url
			var handle = StartCallRequest.CallHandleFromURL(url);

			// Found?
			if (handle == null)
			{
				// No, report to system
				Console.WriteLine("Unable to get call handle from URL: {0}", url);
				return false;
			}
			else
			{
				// Yes, start call and inform system
				CallManager.StartCall(handle);
				return true;
			}
		}


		public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			var handle = StartCallRequest.CallHandleFromActivity(userActivity);

			// Found?
			if (handle == null)
			{
				// No, report to system
				Console.WriteLine("Unable to get call handle from User Activity: {0}", userActivity);
				return false;
			}
			else
			{
				// Yes, start call and inform system
				CallManager.StartCall(handle);
				return true;
			}
		}

        void DisplayIncomingCall(NSUuid uuid, string handle)
        {
            if (CallProviderDelegate != null)
            {
                CallProviderDelegate.ReportIncomingCall(uuid, handle);
            }
            else
            {
                // Report error, object not instalized.
            }
        }
         
	}
}
