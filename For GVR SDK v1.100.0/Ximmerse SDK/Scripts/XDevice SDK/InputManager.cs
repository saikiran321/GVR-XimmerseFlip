//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.InputSystem {
	
	/// <summary>
	/// 
	/// </summary>
	public interface IInputModule{
		int InitInput();
		int UpdateInput();
		int ExitInput();
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class InputManager:MonoBehaviour,IInputModule {

		#region Static

		protected static InputManager s_Instance;
		protected static bool s_InstanceCached=false;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <returns> InputManager Singleton</returns>
        public static InputManager instance {
			get {
				if(!s_InstanceCached) {//
					s_InstanceCached=true;
					if(s_Instance==null) {
						//
						InputManager newInstance=FindObjectOfType<InputManager>();
						//
						if(newInstance==null) {
							string prefabName="Default Input Manager";
							newInstance=(Instantiate(Resources.Load("Input/"+prefabName))
								as GameObject).GetComponent<InputManager>();
							newInstance.name=prefabName;
						}
						if(newInstance!=null) {
							newInstance.Awake();
							s_Instance=newInstance;
						}
					}
				}
				return s_Instance;
			}
		}

		#endregion Static

		#region Fields

		public MonoBehaviour[] monoInputModules=new MonoBehaviour[0];
		public IInputModule[] inputModules;

		#endregion Fields

		#region Unity Messages

		protected virtual void Awake() {
			InitInput();
		}

		protected virtual void Update() {
			UpdateInput();
		}

		protected virtual void OnDestroy() {
			ExitInput();
		}

		#endregion Unity Messages

		#region Methods
		
		public virtual int InitInput() {
			XDevicePlugin.Init();
			//
			List<IInputModule> list=new List<IInputModule>();
			int ret;
			//
			for(int i=0,imax=monoInputModules.Length;i<imax;++i) {
				//
				if(monoInputModules[i]!=null) {
					if(monoInputModules[i].gameObject.activeSelf&&monoInputModules[i].enabled) {
						ret=(monoInputModules[i] as IInputModule).InitInput();
					}else {
						ret=-1;
					}
					//
					monoInputModules[i].enabled=false;
				}else {
					ret=-1;
				}
				//
				if(ret==0) {
					list.Add(monoInputModules[i] as IInputModule);
				}else {
					monoInputModules[i]=null;
				}
			}
			inputModules=list.ToArray();
			//
			return 0;
		}

		public virtual int UpdateInput() {
			int ret;
			//
			for(int i=0,imax=inputModules.Length;i<imax;++i) {
				if(inputModules[i]!=null) {
					ret=inputModules[i].UpdateInput();
				}
			}
			//
			return 0;
		}

		public virtual int ExitInput() {
			int ret;
			//
			for(int i=0,imax=inputModules.Length;i<imax;++i) {
				if(inputModules[i]!=null) {
					ret=inputModules[i].ExitInput();
				}
			}
			//
			XDevicePlugin.Exit();
			//
			return 0;
		}

		#endregion Methods

	}

}