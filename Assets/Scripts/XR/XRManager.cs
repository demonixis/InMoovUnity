#define OCULUS_DISABLED
#define PICO_LEGACYXR_SUPPORT_
#define WAVEVR_MOBILE_SUPPORT_
#if (UNITY_STANDALONE_WIN || UNITY_ANDROID) && !OCULUS_DISABLED
#define OCULUS_SUPPORTED
#endif
#if (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && !OPENXR_DISABLED
#define OPENXR_SUPPORTED
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Demonixis.ToolboxV2.XR
{
    #region Enumerations

    public enum XRVendor
    {
        None = 0,
        Oculus,
        OpenXR,
        WindowsMR,
        OpenVR,
        Sony,
        MockHMD,
        Pico,
        WaveVR,
        Unknown
    }

    public enum XRControllerType
    {
        None = 0,
        OculusTouch,
        OculusTouchQuestS,
        OculusQuest2,
        OculusGo,
        Vive,
        ValveIndex,
        WindowsMR,
        PSVR,
        PicoG2,
        PicoNeo,
        PicoNeo2,
        PicoNeo3,
        PicoNeo4,
        ViveFocus3,
        Unknown
    }

    public enum XRHeadset
    {
        None = 0,
        OculusGo,
        OculusQuest,
        OculusQuest2,
        OculusRiftS,
        OculusRiftCV1,
        WindowsMR,
        HTCVive,
        PSVR,
        ValveIndex,
        HTCViveFocus3,
        PicoG2,
        PicoNeo,
        PicoNeo2,
        PicoNeo3,
        PicoNeo4,
        Unknown
    }

    #endregion

    public static class XRManager
    {
        private static bool? _XREnabled;
        private static readonly List<InputDevice> _inputDevices = new List<InputDevice>();
        public static bool Enabled => IsXREnabled(false);
        public static XRVendor Vendor { get; private set; }
        public static XRHeadset Headset { get; private set; }
        public static XRControllerType Controller { get; private set; }

        public static bool IsOpenXREnabled()
        {
            var loader = GetXRLoader();
            if (loader == null) return false;
            var name = loader.name.ToLower();
            return name.Contains("open") && name.Contains("xr");
        }

        public static XRLoader GetXRLoader()
        {
            return XRGeneralSettings.Instance?.Manager?.activeLoader;
        }

        public static XRInputSubsystem GetXRInput()
        {
            var xrLoader = GetXRLoader();
            return xrLoader?.GetLoadedSubsystem<XRInputSubsystem>();
        }

        public static XRDisplaySubsystem GetDisplay()
        {
            var xrLoader = GetXRLoader();
            return xrLoader?.GetLoadedSubsystem<XRDisplaySubsystem>();
        }

        public static XRVendor GetXRVendor(string name)
        {
            var names = Enum.GetNames(typeof(XRVendor));

            for (var i = 0; i < names.Length; i++)
            {
                if (names[i].ToLower() == name.ToLower())
                {
                    return (XRVendor) i;
                }
            }

            return XRVendor.None;
        }

        public static bool IsControllerConnected(bool left)
        {
            var input = GetXRInput();
            input.TryGetInputDevices(_inputDevices);

            foreach (var device in _inputDevices)
            {
                if (!device.isValid) continue;
                if (device.characteristics == InputDeviceCharacteristics.Left && left) return true;
                if (device.characteristics == InputDeviceCharacteristics.Right && !left) return true;
            }

            return false;
        }

        public static bool IsXREnabled(bool force = false)
        {
            if (_XREnabled.HasValue && !force)
            {
                return _XREnabled.Value;
            }

            var loader = GetXRLoader();

            _XREnabled = loader != null;

#if OPENXR_SUPPORTED
            if (loader is UnityEngine.XR.OpenXR.OpenXRLoader)
            {
                Vendor = ParseVendor(UnityEngine.XR.OpenXR.OpenXRRuntime.name);
                Headset = ParseHeadset(UnityEngine.XR.OpenXR.OpenXRRuntime.name);
                Controller = ParseController(UnityEngine.XR.OpenXR.OpenXRRuntime.name);
            }
#endif

#if OCULUS_SUPPORTED
            if (loader is Unity.XR.Oculus.OculusLoader)
            {
                Vendor = XRVendor.Oculus;

                switch (Unity.XR.Oculus.Utils.GetSystemHeadsetType())
                {
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Quest:
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Link_Quest:
                        Headset = XRHeadset.OculusQuest;
                        Controller = XRControllerType.OculusTouchQuestS;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Link_Quest_2:
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Quest_2:
                        Headset = XRHeadset.OculusQuest2;
                        Controller = XRControllerType.OculusQuest2;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Rift_CV1:
                        Headset = XRHeadset.OculusRiftCV1;
                        Controller = XRControllerType.OculusTouch;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Rift_S:
                        Headset = XRHeadset.OculusRiftS;
                        Controller = XRControllerType.OculusTouchQuestS;
                        break;
                }
            }
#endif

#if PICO_LEGACYXR_SUPPORT
            if (loader is Unity.XR.PXR.PXR_Loader)
            {
                Vendor = XRVendor.Pico;

                var controller = Unity.XR.PXR.PXR_Input.GetActiveController();

                switch (controller)
                {
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.G2:
                        Headset = XRHeadset.PicoG2;
                        Controller = XRControllerType.PicoG2;
                        break;
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.Neo2:
                        Headset = XRHeadset.PicoNeo2;
                        Controller = XRControllerType.PicoNeo2;
                        break;
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.Neo3:
                        Headset = XRHeadset.PicoNeo3;
                        Controller = XRControllerType.PicoNeo3;
                        break;
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.NewController:
                        Headset = XRHeadset.PicoNeo4;
                        Controller = XRControllerType.PicoNeo4;
                        break;
                    default:
                        Headset = XRHeadset.PicoNeo;
                        Controller = XRControllerType.PicoNeo;
                        break;
                }
            }
#endif

#if WAVEVR_MOBILE_SUPPORT
            if (loader is Wave.XR.Loader.WaveXRLoader)
            {
                Vendor = XRVendor.WaveVR;
                Headset = XRHeadset.HTCViveFocus3;
                Controller = XRControllerType.ViveFocus3;
            }
#endif

            return _XREnabled.Value;
        }

        public static void TryInitialize()
        {
            var manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null)
            {
                return;
            }

            manager.InitializeLoaderSync();
            manager.StartSubsystems();
        }

        public static void TryShutdown()
        {
            var manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader == null)
            {
                return;
            }

            manager.DeinitializeLoader();
            manager.StopSubsystems();
        }

        private static bool InArray(string word, params string[] terms)
        {
            foreach (var term in terms)
            {
                if (word.Contains(term))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool InArrayAnd(string word, params string[] terms)
        {
            var counter = 0;
            foreach (var term in terms)
            {
                if (word.Contains(term))
                {
                    counter++;
                }
            }

            return counter == terms.Length;
        }

        public static XRVendor ParseVendor(string name)
        {
            name = name.ToLower();

            if (name.Contains("oculus"))
            {
                return XRVendor.Oculus;
            }
            else if (InArray(name, "windows", "holographic"))
            {
                return XRVendor.WindowsMR;
            }
            else if (InArrayAnd(name, "focus", "htc", "vive"))
            {
                return XRVendor.WaveVR;
            }
            else if (InArray(name, "valve", "vive", "steam", "openvr"))
            {
                return XRVendor.OpenVR;
            }
            else if (InArray(name, "sony", "psvr"))
            {
                return XRVendor.Sony;
            }

            return XRVendor.Unknown;
        }

        public static XRHeadset ParseHeadset(string name)
        {
            name = name.ToLower();

            if (name.Contains("oculus"))
            {
                if (InArray(name, "rift", "cv1"))
                {
                    return XRHeadset.OculusRiftCV1;
                }
                else if (InArray(name, "rift", "rift s", "rift-s"))
                {
                    return XRHeadset.OculusRiftS;
                }
                else if (InArray(name, "go"))
                {
                    return XRHeadset.OculusGo;
                }
                else if (InArray(name, "quest", "2"))
                {
                    return XRHeadset.OculusQuest2;
                }
                else if (InArray(name, "oculus", "quest"))
                {
                    return XRHeadset.OculusQuest;
                }

                return XRHeadset.OculusQuest;
            }
            else if (name.Contains("windows"))
            {
                return XRHeadset.WindowsMR;
            }
            else if (name.Contains("vive"))
            {
                if (name.Contains("focus"))
                    return XRHeadset.HTCViveFocus3;

                return XRHeadset.HTCVive;
            }
            else if (InArray("valve", "index"))
            {
                return XRHeadset.ValveIndex;
            }
            else if (InArray("sony", "psvr"))
            {
                return XRHeadset.PSVR;
            }

            return XRHeadset.Unknown;
        }

        public static XRControllerType ParseController(string name)
        {
            if (!Enabled)
            {
                return XRControllerType.None;
            }

            if (name.Contains("oculus"))
            {
                if (InArray(name, "rift", "cv1"))
                {
                    return XRControllerType.OculusTouch;
                }
                else if (InArray(name, "rift", "s"))
                {
                    return XRControllerType.OculusTouchQuestS;
                }
                else if (InArray(name, "go"))
                {
                    return XRControllerType.OculusGo;
                }
                else if (InArray(name, "quest", "2"))
                {
                    return XRControllerType.OculusQuest2;
                }
                else if (InArray(name, "oculus", "quest"))
                {
                    return XRControllerType.OculusTouchQuestS;
                }

                return XRControllerType.OculusTouchQuestS;
            }
            else if (name.Contains("windows"))
            {
                return XRControllerType.WindowsMR;
            }
            else if (name.Contains("Vive"))
            {
                if (name.Contains("focus"))
                    return XRControllerType.ViveFocus3;

                return XRControllerType.Vive;
            }
            else if (InArray("valve", "index"))
            {
                return XRControllerType.ValveIndex;
            }
            else if (InArray("sony", "psvr"))
            {
                return XRControllerType.PSVR;
            }

            return XRControllerType.Unknown;
        }

        public static IEnumerator GetXRInfos(Action<XRVendor, XRHeadset, XRControllerType> callback)
        {
            var vendor = XRVendor.None;
            var headset = XRHeadset.None;
            var controller = XRControllerType.None;

            var head = new List<InputDevice>();
            var left = new List<InputDevice>();
            var right = new List<InputDevice>();

            do
            {
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice, head);
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left |
                    InputDeviceCharacteristics.TrackedDevice, left);
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right |
                    InputDeviceCharacteristics.TrackedDevice, right);
                yield return null;
            } while (head.Count + left.Count + right.Count < 2);

            foreach (var h in head)
            {
                vendor = ParseVendor(h.name);
                headset = ParseHeadset(h.name);
            }

            left.AddRange(right);
            foreach (var hand in left)
            {
                if (headset == XRHeadset.None || headset == XRHeadset.Unknown)
                {
                    vendor = ParseVendor(hand.name);
                    headset = ParseHeadset(hand.name);
                }

                if (controller == XRControllerType.None || controller == XRControllerType.Unknown)
                {
                    controller = ParseController(hand.name);
                }
            }

            Vendor = Vendor;
            Headset = headset;
            Controller = controller;

            callback(Vendor, Headset, Controller);
        }

        public static void SetTrackingOriginMode(TrackingOriginModeFlags origin, bool recenter)
        {
            var xrInput = GetXRInput();
            xrInput?.TrySetTrackingOriginMode(origin);

            if (recenter)
            {
                Recenter();
            }
        }

        public static void Recenter()
        {
            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);

            foreach (var subsystem in subsystems)
            {
                subsystem.TryRecenter();
            }
        }

        public static void Vibrate(MonoBehaviour target, XRNode node, float amplitude = 0.5f, float seconds = 0.25f)
        {
            if (!target.gameObject.activeSelf || !target.gameObject.activeInHierarchy)
            {
                return;
            }

            var device = InputDevices.GetDeviceAtXRNode(node);

            if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
            {
                if (capabilities.supportsBuffer)
                {
                    byte[] buffer = { };

                    if (GenerateBuzzClip(seconds, node, ref buffer))
                    {
                        device.SendHapticBuffer(0, buffer);
                    }
                }
                else if (capabilities.supportsImpulse)
                {
                    device.SendHapticImpulse(0, amplitude, seconds);
                }

                target.StartCoroutine(StopVibrationCoroutine(device, seconds));
            }
        }

        private static IEnumerator StopVibrationCoroutine(InputDevice device, float delay)
        {
            yield return new WaitForSeconds(delay);
            device.StopHaptics();
        }

        public static bool GenerateBuzzClip(float seconds, XRNode node, ref byte[] clip)
        {
            var device = InputDevices.GetDeviceAtXRNode(node);
            var result = device.TryGetHapticCapabilities(out HapticCapabilities caps);

            if (result)
            {
                var clipCount = (int) (caps.bufferFrequencyHz * seconds);
                clip = new byte[clipCount];

                for (int i = 0; i < clipCount; i++)
                {
                    clip[i] = byte.MaxValue;
                }
            }

            return result;
        }
    }
}