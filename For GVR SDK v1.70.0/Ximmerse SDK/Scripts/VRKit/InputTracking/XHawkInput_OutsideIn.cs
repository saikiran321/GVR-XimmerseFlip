//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public class XHawkInput_OutsideIn:XHawkInput {

		#region Fields

		[Header("Outside-In")]
		public GameObject anchorPrefab;
		public bool makeTrackerForward=false;
		
		[System.NonSerialized]protected Transform m_CenterEye;
		[System.NonSerialized]protected VRDevice m_VRDevice;
		[System.NonSerialized]protected PlayAreaHelper m_PlayAreaHelper;

		#endregion Fields

		#region Methods

		public override bool InitInternal(){
			if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0x4000)&0xF000)!=0x4000) {
				return false;
			}
			//
			return base.InitInternal();
		}

		public override bool CreateAnchor() {
			if(anchor!=null) {
				return false;
			}
			//
			anchor=(anchorPrefab==null||!m_IsRequestVR?// in 2D mode.
				new GameObject():
				Object.Instantiate(anchorPrefab)
			).transform;
			anchor.name="TrackerAnchor(Outside-In)";
			anchor.SetParent(trackingSpace);
			//
			UpdateAnchorFromPlugin();
			m_PlayAreaHelper=anchor.GetComponentInChildren<PlayAreaHelper>();
			//
			m_UseAnchorProjection=true;
			VRContext.SetAnchor(VRNode.TrackerDefault,anchor);
			//
			//
			return true;
		}

		public override void UpdateAnchor() {
		}

		public override void Recenter() {
			//
			for(int i=0,imax=controllers.Length;i<imax;++i) {
				controllers[i].OnVRContextRecenter();
			}
			//
			if((ControllerInputManager.GetButtonDown(ControllerType.LeftController,(uint)XimmerseButton.Home)&&ControllerInputManager.GetButton(ControllerType.RightController,(uint)XimmerseButton.Home))||
				(ControllerInputManager.GetButton(ControllerType.LeftController,(uint)XimmerseButton.Home)&&ControllerInputManager.GetButtonDown(ControllerType.RightController,(uint)XimmerseButton.Home))
			){
				XDevicePlugin.SendMessage(m_Handle,XDevicePlugin.kMessage_RecenterSensor,1,0);
				UpdateAnchorFromPlugin();
				//
				if(m_PlayAreaHelper!=null) {
					m_PlayAreaHelper.OnTrackerRecenter();
				}
			}
		}

		public override Vector3 GetLocalPosition(int node) {
			Vector3 position=base.GetLocalPosition(node);
			if(node==m_Controllers[2].value) {
				if(m_CenterEye==null) {
					m_CenterEye=VRContext.GetAnchor(VRNode.CenterEye);
					m_VRDevice=VRContext.currentDevice;
				}
				if(m_CenterEye!=null&&m_VRDevice!=null) {
					position-=m_CenterEye.localRotation*(m_VRDevice.neckToEye+m_VRDevice.outsideInMarkPose.position);
				}
			}
			return position;
		}

		[System.NonSerialized]protected float[] m_UpdateAnchorFromPluginTRS=new float[3+4+3];
		public virtual void UpdateAnchorFromPlugin(){
			XDevicePlugin.GetObject(m_Handle,XDevicePlugin.kField_TRSObject,m_UpdateAnchorFromPluginTRS,0);
			if(true) {
				anchor.localPosition=new Vector3(
					 m_UpdateAnchorFromPluginTRS[0],
					 m_UpdateAnchorFromPluginTRS[1],
					-m_UpdateAnchorFromPluginTRS[2]
				);
				anchor.localRotation=new Quaternion(
					-m_UpdateAnchorFromPluginTRS[3],
					-m_UpdateAnchorFromPluginTRS[4],
					 m_UpdateAnchorFromPluginTRS[5],
					 m_UpdateAnchorFromPluginTRS[6]
				);
				//
				if(makeTrackerForward) {
				if(trackingSpace!=null) {
					trackingSpace.localRotation=Quaternion.Euler(0.0f,-(anchor.localRotation.eulerAngles.y+180.0f),0.0f);
				}}
			}
		}

		#endregion Methods

	}
}
