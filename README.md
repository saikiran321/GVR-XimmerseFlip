# Gvr-XimmerseFlip

**Modified gvr-unity-sdk** with Ximmerse-Flip SDK and Provider modification to bind Ximmerse Flip on Google VR Services for Unity3d Cardboard Build.

## Prerequesite : 
> Google VR Services  [Download](https://play.google.com/store/apps/details?id=com.google.vr.vrcore)

> Google VR SDK Unity  [Download](https://github.com/googlevr/gvr-unity-sdk/releases)

> Flip BTConfig [Download](https://github.com/Ximmerse/SDK_Flip/raw/master/Tools/BTConfig%5B1.0.0-Flip%5D.apk)

> Unity 2017.x.x
## Installation :
#### Android
> Bind your Flip Controller to **Bluetooth** and **BT-Config**.

#### Unity
> Import Google VR SDK package and push in your Assets project folder the content that corresponds to your version of Google VR SDK.  Set your PlayerSettings Virtual-Reality to " **Cardboard** " ( switch to Daydream ignore modification and use the native provider) .

> **Build your own project and enjoy it !**

## Tips :
> If you want to use the GvrRecenterOnlyController edit "GoogleVR/Scripts/Controller/GvrRecenterOnlyController.cs" and remove :
```
#if UNITY_ANDROID && !UNITY_EDITOR
        if (VRSettings.loadedDeviceName != "daydream")
        {
            return;
        }
#endif
```

