using UnityEngine;
using UnityEngine.VR;
using Ximmerse.InputSystem;
using Gvr;


namespace Gvr.Internal {
	public class XimmerseControllerProvider : IControllerProvider
{
  private ControllerState state = new ControllerState();
		protected ControllerInput ctrl;
		public bool SupportsBatteryStatus {
			get { return true; }
		}
  #region IControllerProvider implementation
  void IControllerProvider.OnPause ()
  {
  }
  void IControllerProvider.OnResume ()
  {
  }
			 
  // this is called every frame
  void IControllerProvider.ReadState (ControllerState outState)
  {

			if (ctrl == null) {
					XDevicePlugin.Init ();
					int handle = XDevicePlugin.GetInputDeviceHandle ("XCobra-0");
					ctrl = new ControllerInput (handle);
			}

			lock (state) {

				if (ctrl != null) {
					ctrl.UpdateState ();
					// GVR Hack Detection Controller
					if (ctrl.connectionState == DeviceConnectionState.Connected) {
						state.connectionState = GvrConnectionState.Connected;
					} else if (ctrl.connectionState == DeviceConnectionState.Connecting) {
						state.connectionState = GvrConnectionState.Connecting;
					} else {
						state.connectionState = GvrConnectionState.Disconnected;
					}
					
					// GVR Input Mapping
					state.apiStatus = GvrControllerApiStatus.Ok;
					state.appButtonState = ctrl.GetButton (XimmerseButton.App);
					state.appButtonDown = ctrl.GetButtonDown (XimmerseButton.App);
					state.appButtonUp = ctrl.GetButtonUp (XimmerseButton.App);
					state.homeButtonDown = ctrl.GetButtonDown (XimmerseButton.Home);
					state.homeButtonState = ctrl.GetButton (XimmerseButton.Home);
					state.clickButtonDown = ctrl.GetButtonDown (XimmerseButton.Click) || ctrl.GetButtonDown (XimmerseButton.Trigger);
					state.clickButtonState = ctrl.GetButton (XimmerseButton.Click) || ctrl.GetButton (XimmerseButton.Trigger);
					state.clickButtonUp = ctrl.GetButtonUp (XimmerseButton.Click) || ctrl.GetButtonUp (XimmerseButton.Trigger);
					state.orientation = ctrl.GetRotation ();
					state.gyro = -ctrl.GetGyroscope ();
					state.accel = ctrl.GetAccelerometer ();
					state.touchPos = ctrl.touchPos;

					// GVR Battery Indicator
					if (ctrl.batteryLevel > 80) {
						state.batteryLevel = GvrControllerBatteryLevel.Full;
					}
					if (ctrl.batteryLevel > 60 && ctrl.batteryLevel <= 80) {
						state.batteryLevel = GvrControllerBatteryLevel.AlmostFull;
					}
					if (ctrl.batteryLevel > 40 && ctrl.batteryLevel <= 60) {
						state.batteryLevel = GvrControllerBatteryLevel.Medium;
					}
					if (ctrl.batteryLevel > 20 && ctrl.batteryLevel <= 40) {
						state.batteryLevel = GvrControllerBatteryLevel.Low;
					}
					if (ctrl.batteryLevel >= 0 && ctrl.batteryLevel <= 20) {
						state.batteryLevel = GvrControllerBatteryLevel.CriticalLow;
					}

					// GVR Recenter Touchpad Detection
					if (ctrl.GetButtonDown (ControllerButton.PrimaryThumbMove) || ctrl.GetButtonDown (XimmerseButton.Click)) {
						state.touchDown = true;
						state.isTouching = true;
					}
					if (ctrl.GetButton (ControllerButton.PrimaryThumbMove) || ctrl.GetButton (XimmerseButton.Click)) {
						state.isTouching = true;
					}
					if (ctrl.GetButtonUp (ControllerButton.PrimaryThumbMove) || ctrl.GetButtonUp (XimmerseButton.Click)) {
						state.touchUp = true;
						state.isTouching = false;
					}


					// GVR Recenter Interactions
					if (ctrl.GetButtonDown (XimmerseButton.Home)) {
						state.recentering = true;
					}
					if (ctrl.GetButtonUp (XimmerseButton.Home)) {
						GvrCardboardHelpers.Recenter ();
						ctrl.Recenter ();
						state.recentering = false;
						state.recentered = true;
					}

				} 

				else {
					state.connectionState = GvrConnectionState.Disconnected;
					state.clickButtonState = false;
					state.clickButtonDown = false;
					state.clickButtonUp = false;
					state.appButtonState = false;
					state.appButtonDown = false;
					state.appButtonUp = false;
				}


				outState.CopyFrom (state);
			}
    			state.ClearTransientState();
  }
			
  #endregion

}

}
