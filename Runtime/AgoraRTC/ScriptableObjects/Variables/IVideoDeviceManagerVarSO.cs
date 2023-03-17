using UnityEngine;
using d4160.Variables;
using Agora.Rtc;
using d4160.Collections;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Variables/IVideoDeviceManager")]
    public class IVideoDeviceManagerVarSO : VariableSOBase<VideoDeviceManagerRef>
    {

    }

    public class VideoDeviceManagerRef
    {
        public IVideoDeviceManager VideoDeviceManager { get; private set; }
        public DeviceInfo[] Devices { get; private set; }

        public VideoDeviceManagerRef(IVideoDeviceManager videoDeviceManager)
        {
            VideoDeviceManager = videoDeviceManager;
        }

        public DeviceInfo[] EnumerateVideoDevices()
        {
            if (VideoDeviceManager != null)
            {
                Devices = VideoDeviceManager.EnumerateVideoDevices();
            }

            return Devices;
        }

        public int SetDevice(int index)
        {
            if (VideoDeviceManager != null && Devices != null && Devices.IsValidIndex(index))
            {
                return VideoDeviceManager.SetDevice(Devices[index].deviceId);
            }

            return -1;
        }
    }
}