//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

namespace Ximmerse.InputSystem {

	public partial class XDevicePlugin {

		public const int
			ID_CONTEXT=0xFF
		;

		public const int
			// Bool
			kField_CtxCanProcessInputEventBool = 0,
			kField_IsAbsRotationBool           =10,
            kField_IsDeviceSleepBool           =11,
            // Int
            kField_CtxSdkVersionInt            = 0,
			kField_CtxDeviceVersionInt         = 1,
			kField_CtxPlatformVersionInt       = 2,
            kField_CtxLogLevelInt              = 3,
			kField_ErrorCodeInt                = 2,
			kField_ConnectionStateInt          = 3,
			kField_BatteryLevelInt             = 4,
			kField_TrackingResultInt           = 5,
			kField_TrackingOriginInt           = 6,
            // Float
			kField_PositionScaleFloat          = 0,
			kField_TrackerHeightFloat          = 1,
			kField_TrackerDepthFloat           = 2,
			kField_TrackerPitchFloat           = 3,
			// Object
			kField_TRSObject                   = 0,
			// Message
			kMessage_TriggerVibration          = 1,
			kMessage_RecenterSensor            = 2,
            kMessage_SleepMode                 = 3,
            //
			OK                                 = 0
		;
	}

    enum LOGLevel
    {
        LOG_VERBOSE = 0,
        LOG_DEBUG = 1,
        LOG_INFO = 2,
        LOG_WARN = 3,
        LOG_ERROR = 4,
        LOG_OFF = 5,
    };

    // Reference from GoogleVR.

    /// <summary>
    /// Represents the device's current connection state.
    /// </summary>
    public enum DeviceConnectionState {
		/// <summary>
		/// Indicates that the device is disconnected.
		/// </summary>
		Disconnected,
		/// <summary>
		/// Indicates that the host is scanning for devices.
		/// </summary>
		Scanning,
		/// <summary>
		/// Indicates that the device is connecting.
		/// </summary>
		Connecting,
		/// <summary>
		/// Indicates that the device is connected.
		/// </summary>
		Connected,
		/// <summary>
		/// Indicates that an error has occurred.
		/// </summary>
		Error,
	};

	[System.Flags]
	public enum TrackingResult{
		NotTracked      =    0,
		RotationTracked = 1<<0,
		PositionTracked = 1<<1,
		PoseTracked     = (RotationTracked|PositionTracked),
	};

	public enum XimmerseButton {
		// Daydream Standard
		Touch   = ControllerRawButton.LeftThumbMove,
		Click   = ControllerRawButton.LeftThumb,
		App     = ControllerRawButton.Back,
		Home    = ControllerRawButton.Start,
		// Touchpad To Dpad
		DpadUp    = ControllerRawButton.DpadUp,
		DpadDown  = ControllerRawButton.DpadDown,
		DpadLeft  = ControllerRawButton.DpadLeft,
		DpadRight = ControllerRawButton.DpadRight,
		// Ximmerse Ex
		Trigger = ControllerRawButton.LeftTrigger,
		GripL   = ControllerRawButton.LeftShoulder,
		GripR   = ControllerRawButton.RightShoulder,
		Grip    = GripL|GripR,
		TouchpadClick   = Click|DpadUp|DpadDown|DpadLeft|DpadRight,
	}

	public static class XimmerseExtension {

		public static bool GetButton(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButton((uint)buttonMask);
		}

		public static bool GetButtonDown(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonDown((uint)buttonMask);
		}

		public static bool GetButtonUp(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonUp((uint)buttonMask);
		}
	}
}
