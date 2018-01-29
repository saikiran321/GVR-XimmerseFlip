using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.VR;
using Ximmerse.InputSystem;


namespace Gvr.Internal {
	public class XimmerseControllerProvider : IControllerProvider
	{
		private ControllerState state = new ControllerState();
		private XDevicePlugin.ControllerState m_leftControllerState;
		private int handle;
		private bool EnableXdevice = false;
		protected ControllerInput ctrl;
		private bool FlipAppInstalled() {
			#if UNITY_ANDROID
			AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject launchIntent = null;
			try{
				launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage","com.ximmerse.xtools.bt_bind" );
			}catch(Exception ex){}
			if(launchIntent == null) {
				return false;
			} else {
			return true;
			}
			#else
			return false;
			#endif
		}


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
			lock (state) {

				if (ctrl != null) {

					XDevicePlugin.UpdateInputState(handle);
					XDevicePlugin.GetInputState(handle, ref m_leftControllerState);
					state.orientation = new Quaternion(
						-m_leftControllerState.rotation[0],
						-m_leftControllerState.rotation[1],
						m_leftControllerState.rotation[2],
						m_leftControllerState.rotation[3]
						);
					state.gyro = new Vector3(
						-m_leftControllerState.gyroscope[0],
						-m_leftControllerState.gyroscope[1],
						m_leftControllerState.gyroscope[2]
						);
					state.accel = new Vector3(
						m_leftControllerState.accelerometer[0],
						m_leftControllerState.accelerometer[1],
						-m_leftControllerState.accelerometer[2]
						);
					state.touchPos = ctrl.touchPos;
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
					state.gvrPtr = IntPtr.Zero;

					if (ctrl.GetButtonUp (XimmerseButton.Home)) {
						GvrCardboardHelpers.Recenter ();
						ctrl.Recenter ();
						state.recentered = true;
					}

				} 

				else {
					if (EnableXdevice == false && FlipAppInstalled() == true) {
					EnableXdevice = true;
					XDevicePlugin.Init ();
					handle = XDevicePlugin.GetInputDeviceHandle ("XCobra-0");
					ctrl = new ControllerInput (handle);
		          		}
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
