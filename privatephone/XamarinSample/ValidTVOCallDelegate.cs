using System;
using Foundation;
using CoreFoundation;

namespace privatephone
{
    public class ValidTVONotificationDelegate : TVONotificationDelegate
    {
        public override void CallInviteCancelled(TVOCallInvite callInvite)
        {
            //		NSLog(@"callInviteCancelled:");


            //[self performEndCallActionWithUUID:callInvite.uuid];

            //self.callInvite = nil;
        }

        public override void CallInviteReceived(TVOCallInvite callInvite)
        {
            //		if (self.callInvite && self.callInvite == TVOCallInviteStatePending)
            //		{
            //			NSLog(@"Already a pending incoming call invite.");
            //			NSLog(@"  >> Ignoring call from %@", callInvite.from);
            //			return;
            //		}
            //		else if (self.call)
            //		{
            //			NSLog(@"Already an active call.");
            //			NSLog(@"  >> Ignoring call from %@", callInvite.from);
            //			return;
            //		}

            //		self.callInvite = callInvite;

            //[self reportIncomingCallFrom:@"Voice Bot" withUUID:callInvite.uuid];

        }

        public override void NotificationError(NSError error)
        {
            //     NSLog(@"notificationError: %@", [error localizedDescription]);
        }
    }

    public class ValidTVOCallDelegate : TVOCallDelegate
    {
        ActiveCall _currentCall = null;
        ActiveCall.ActiveCallbackDelegate _completionHandler = null;
                  
        public ValidTVOCallDelegate(ActiveCall.ActiveCallbackDelegate completionHandler
                                    ,ActiveCall currentCall)
        {
            _currentCall = currentCall;
           _completionHandler = completionHandler;
  		}
 

        public override void Call(TVOCall call, NSError error)
        {
			//		NSLog(@"call:didFailWithError: %@", [error localizedDescription]);

			//      DispatchQueue.MainQueue.DispatchAsync(() =>
			//{
			//    // Note that the call is starting
			//    isConnecting = true;
			//});
			//[self performEndCallActionWithUUID:call.uuid];

			//self.call = nil;
			//[self toggleUIState:YES];
			//[self stopSpin];

            _completionHandler(false);
		}

        public override void CallDidConnect(TVOCall call)
        {
            _currentCall.isConnected = true;
			//_currentCall.ConnectedChanged();

			//      self.call = call;

			//[self.placeCallButton setTitle:@"Hang Up" forState:UIControlStateNormal];

			//[self toggleUIState:YES];
			//[self stopSpin]; 
            RouteAudioToSpeaker();


			//DispatchQueue.MainQueue.DispatchAfter(() =>
			//      {
			//          // Note that the call has connected
			//          isConnecting = false;
			//          isConnected = true;
			//      });
			_completionHandler(true);
		}

        public override void CallDidDisconnect(TVOCall call)
        {
            //		NSLog(@"callDidDisconnect:");

            _currentCall.isConnected = false;
            //_currentCall.ConnectedChanged();
            //[self performEndCallActionWithUUID:call.uuid];

            //self.call = nil;

            //[self.placeCallButton setTitle:@"Place Outgoing Call" forState:UIControlStateNormal];

            //[self toggleUIState:YES];
        }

        public   void RouteAudioToSpeaker()
        {
            NSError error = null;

            AVFoundation.AVAudioSession.SharedInstance()
                        .SetCategory("AVAudioSessionCategoryPlayAndRecord",
                                     options: AVFoundation.AVAudioSessionCategoryOptions.DefaultToSpeaker, outError: out error);

            if (error != null)
            {
                //NSLog(@"Unable to reroute audio: %@", [error localizedDescription]);
            }
        }
    }
}
