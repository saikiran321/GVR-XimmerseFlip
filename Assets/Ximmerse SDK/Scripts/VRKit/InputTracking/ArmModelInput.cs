//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public enum ArmModelNode {
		Shoulder,
		Elbow,
		Wrist,
		Pointer,
	}

	[System.Serializable]
	public partial class ArmModel:IInputTracking{

		public string name;
		public TrackedControllerInput Controller;
		public ArmModelNode defaultNode=ArmModelNode.Pointer;
		public ControllerType handedness;

		public ArmModel(string name,ControllerType handedness) {
			this.name=name;
			this.handedness=handedness;
		}

		public virtual Vector3 GetHeadOrientation() {
			return VRContext.currentDevice.GetRotation()*Vector3.forward;
		}

		public virtual bool IsControllerRecentered() {
			// TODO : 
			return false;
		}

		public virtual bool Exists(int node) {
			return true;
		}

		public virtual Vector3 GetLocalPosition(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			switch((ArmModelNode)node) {
				case ArmModelNode.Pointer:return pointerPosition;
				case ArmModelNode.Wrist:return wristPosition;
				case ArmModelNode.Elbow:return elbowPosition;
				case ArmModelNode.Shoulder:return shoulderPosition;
			}
			//
			return Vector3.zero;
		}

		public virtual Quaternion GetLocalRotation(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			switch((ArmModelNode)node) {
				case ArmModelNode.Pointer:return pointerRotation;
				case ArmModelNode.Wrist:return wristRotation;
				case ArmModelNode.Elbow:return elbowRotation;
				case ArmModelNode.Shoulder:return shoulderRotation;
			}
			//
			return Quaternion.identity;
		}
	}

	public class ArmModelInput:MonoBehaviour,IInputModule{

		#region Fields

		[SerializeField]protected ArmModel[] m_Controllers=new ArmModel[2]{
			new ArmModel("LeftController",ControllerType.LeftController),
			new ArmModel("RightController",ControllerType.RightController),
		};

		#endregion Fields

		#region Messages

		public virtual int InitInput() {
			//if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0)&0xF000)!=0x1000) {
			//	return -1;
			//}
			// Override ControllerInput in ControllerInputManager.
			if(true){
				//
				ControllerInputManager mgr=ControllerInputManager.instance;
				ControllerInput ci;
				if(mgr!=null) {
					for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
						if(m_Controllers[i].handedness!=ControllerType.None) {
							ci=mgr.GetControllerInput(m_Controllers[i].name);
							if(ci is TrackedControllerInput) {
								m_Controllers[i].Controller=ci as TrackedControllerInput;
								m_Controllers[i].Controller.inputTracking=m_Controllers[i];
								m_Controllers[i].Controller.node=-1;
							}else {
								m_Controllers[i].Controller=new TrackedControllerInput(m_Controllers[i].name,m_Controllers[i],-1);
								mgr.AddControllerInput(m_Controllers[i].name,m_Controllers[i].Controller);
							}
							//
							m_Controllers[i].Start();
						}
					}
				}else {
					return -1;
				}
			}
			//
			return 0;
		}

		public virtual int UpdateInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnControllerUpdate();
				}
			}
			return 0;
		}

		public virtual int ExitInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnDestroy();
				}
			}
			return 0;
		}

		#endregion Messages

	}
}