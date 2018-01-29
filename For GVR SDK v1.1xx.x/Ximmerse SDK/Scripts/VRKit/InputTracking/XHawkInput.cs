//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkInput:TrackerInput {

		#region Nested Types

		[System.Serializable]public class StringIntPair:UKeyValuePair<string,int>{}
#if UNITY_EDITOR
		[UnityEditor.CustomPropertyDrawer(typeof(StringIntPair))]public class StringIntPairDrawer:UKeyValuePairDrawer<string,int>{}
#endif

		#endregion Nested Types

		#region Fields

		[SerializeField]protected StringIntPair[] m_Controllers=new StringIntPair[3]{
			new StringIntPair{key="XCobra-0",value=0},
			new StringIntPair{key="XCobra-1",value=1},
			new StringIntPair{key="VRDevice",value=2}
		};

		[System.NonSerialized]public TrackedControllerInput[] controllers;
		[System.NonSerialized]protected bool m_IsInited,m_IsRequestVR;
		[System.NonSerialized]protected ControllerInput m_HmdInput;

		#endregion Fields

		#region Messages

		public override int InitInput() {
			int ret=base.InitInput();
			if(ret==0) {
				return InitInternal()?ret:-1;
			}
			return ret;
		}

		#endregion Messages
		
		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual bool InitInternal() {
			if(m_IsInited) {
				return true;
			}
			m_IsInited=true;
			// Plugin context.
			deviceName="XHawk-0";
			if(m_Handle==-1) {
				m_Handle=XDevicePlugin.GetInputDeviceHandle(deviceName);
			}
			XDevicePlugin.SetInt(m_Handle,XDevicePlugin.kField_TrackingOriginInt,(int)VRContext.trackingOrigin);
			XDevicePlugin.SendMessage(m_Handle,XDevicePlugin.kMessage_RecenterSensor,0,0);
			//
			if(true){
				int i=0,imax=m_Controllers.Length;
				controllers=new TrackedControllerInput[imax];
				//
				ControllerInputManager mgr=ControllerInputManager.instance;
				ControllerInput ci;
				if(mgr!=null) {
					for(;i<imax;++i) {
						ci=mgr.GetControllerInput(m_Controllers[i].key);
						if(ci is TrackedControllerInput) {
							controllers[i]=ci as TrackedControllerInput;
							controllers[i].inputTracking=this;
							controllers[i].node=m_Controllers[i].value;
						}else {
							controllers[i]=CreateControllerInput(m_Controllers[i].key,this,m_Controllers[i].value);
							mgr.AddControllerInput(controllers[i].name,controllers[i]);
						}
					}
				}
			}
			// VR Context
			m_HmdInput=ControllerInputManager.GetInput(ControllerType.Hmd);
			  // VRContext must have a CenterEyeAnchor at least.
			m_IsRequestVR=(VRContext.GetAnchor(VRNode.CenterEye)!=null);
			EnsureAnchor();
			//
			Log.i("XHawkInput","Initialize successfully.");
			//
			return true;
		}

		public override void UpdateState() {
			if(Time.frameCount!=m_PrevFrameCount){
			if(EnsureAnchor()) {
				UpdateAnchor();
				base.UpdateState();
			}}
		}

		public virtual TrackedControllerInput CreateControllerInput(string name,TrackerInput trackerInput,int defaultNode) {
			return new TrackedControllerInput(name,trackerInput,defaultNode);
		}

		/// <summary>
		/// We will lose the VR context,when reloading level.
		/// Calling this function per frame can ensure that the anchor is alive.
		/// </summary>
		public virtual bool EnsureAnchor() {
			// <!-- TODO: VR Legacy Mode. -->
			// If the X-Hawk isn't connected,the game will run as legacy VR mode(Gets input events with GearVR touchpad).
			if(XDevicePlugin.GetInt(m_Handle,XDevicePlugin.kField_ConnectionStateInt,0)!=(int)DeviceConnectionState.Connected) {
				XDevicePlugin.SetInt(m_HmdInput.handle,XDevicePlugin.kField_ConnectionStateInt,(int)DeviceConnectionState.Disconnected);
				return false;
			}
			//
			if(trackingSpace==null) {
				trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
			}
			//
			if(anchor==null) {
				Transform centerEyeAnchor=VRContext.GetAnchor(VRNode.CenterEye);
				if(m_IsRequestVR&&centerEyeAnchor==null) {
					return false;
				}else {
					CreateAnchor();
					//
					if(m_IsRequestVR) {
						VRContext.main.onRecenter-=Recenter;
						VRContext.main.onRecenter+=Recenter;
					}
				}
			}
			return true;
		}

		public virtual bool CreateAnchor() {
			throw new System.NotImplementedException();
		}

		public virtual void UpdateAnchor() {
			throw new System.NotImplementedException();
		}

		public override void Recenter() {
			throw new System.NotImplementedException();
		}

		public override Quaternion GetLocalRotation(int node) {
			UpdateState();
			//
			if(controllers==null) {
				return Quaternion.identity;
			}
			//
			ControllerInput input=null;
			for(int i=0,imax=controllers.Length;i<imax;++i) {
				if(controllers[i]!=null){if(controllers[i].node==node) {
					input=controllers[i];
					break;
				}}
			}
			//
			if(input==null) {
				return Quaternion.identity;
			}else {
				return input.GetRotation();
			}
		}

		#endregion Methods

	}
}
