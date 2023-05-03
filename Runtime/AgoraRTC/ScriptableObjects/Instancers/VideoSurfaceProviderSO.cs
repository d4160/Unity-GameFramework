using Agora.Rtc;
using d4160.Collections;
using d4160.Coroutines;
using d4160.Instancers;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
using System.Collections;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Instancers/VideoSurface Provider")]
    public class VideoSurfaceProviderSO : ScriptableObject
    {
        public Vector3 fixRotationRawImage = new (0f, 0f, 180.0f);
        public Vector3 fixRotationRenderer = new (-90f, 0f, 0f);
        public Vector3 fixLocalScale = new (-1f, 1f, 1f);

        [Header("Variables")]
        [SerializeField] private BoolVariableSO _desactiveVSurfaceOnDisableVar;

        [Header("Data")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ComponentFactorySO _videoSurfaceFactory;
        [SerializeField] private VideoSurfaceDicRuntimeSetSO _runtimeDictionary;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private VideoEncoderConfigurationSO _videoEncoderConfig;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private AgoraRtcChannelSO _channel;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private UsersRuntimeSetSO _usersRuntimeSet;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ScreenCaptureSO _screenCapture;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ChannelMediaOptionsSO _mediaOptionsScreenTrack;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ChannelMediaOptionsSO _mediaOptionsCameraTrack;

        public VideoSurfaceDicRuntimeSetSO RuntimeDictionary => _runtimeDictionary;

        public List<VideoSurface> StaticVideoSurfaces { get; set; }
        public VideoSurface LocalVideoSurface { get; set; }

        public float ScaleFactor => _videoEncoderConfig.DimensionsHeight / (float) _videoEncoderConfig.DimensionsWidth;

        private WaitForEndOfFrame _waitForEndOfFrame;

        public void Setup()
        {
            _runtimeDictionary.Clear();

            if (LocalVideoSurface)
                LocalVideoSurface.SetEnable(false);

            for (int i = 0; i < StaticVideoSurfaces.Count; i++)
            {
                if (StaticVideoSurfaces[i])
                    StaticVideoSurfaces[i].SetEnable(false);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableLocalVideoSurface()
        {
            if (LocalVideoSurface)
            {
                LocalVideoSurface.SetForUser();
                LocalVideoSurface.SetEnable(true);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisableLocalVideoSurface()
        {
            if (LocalVideoSurface)
            {
                LocalVideoSurface.SetEnable(false);
            }
        }

        public void EnableStaticVideoSurfaceByIndex(int index, uint userId, bool isScreenCapture = false)
        {
            EnableStaticVideoSurface(_usersRuntimeSet.LocalUid == userId ? 0 : userId, false, index, isScreenCapture);
        }

        public void EnableStaticVideoSurface(uint userId = 0, bool removeFromList = true, int index = 0, bool isScreenCapture = false)
        {
            //Debug.Log($"[EnableStaticVideoSurface]");
            if (StaticVideoSurfaces != null && StaticVideoSurfaces.IsValidIndex(index) && (!removeFromList || !_runtimeDictionary.ContainsKey(userId)))
            {
                bool stoppedLocalCameraOrScreen = false;
                VideoSurface vSurface = StaticVideoSurfaces[index];

                if (userId == 0)
                {
                    if (!isScreenCapture)
                    {
                        _channel.EnableLocalVideo();
                        _mediaOptionsCameraTrack.UpdateChannelMediaOptions();
                    }
                    else
                    {
                        _screenCapture.StartScreenCapture();
                        _mediaOptionsScreenTrack.UpdateChannelMediaOptions();
                    }

                    // Sync correctly for local. Agora API 4.1 doesn't allow share camera and screen at the same moment with one uid
                    int count = 0;
                    for (int i = 0; i < StaticVideoSurfaces.Count; i++)
                    {
                        if (i != index)
                        {
                            if (StaticVideoSurfaces[i].UID == 0 && StaticVideoSurfaces[i].ENABLE)
                            {
                                if (count == 0)
                                {

                                    if (isScreenCapture && StaticVideoSurfaces[i].SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
                                    {
                                        _channel.DisableLocalVideo();
                                    }
                                    else if (!isScreenCapture && StaticVideoSurfaces[i].SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN)
                                    {
                                        _screenCapture.StopScreenCapture();
                                    }

                                    stoppedLocalCameraOrScreen = true;
                                    count++;
                                }


                                StaticVideoSurfaces[i].SetEnable(false);
                                StaticVideoSurfaces[i].SetForUser(0, "", isScreenCapture ? VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN : VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
                                int _i = i;
                                CoroutineStarter.Instance.WaitForEndOfFrameAndExecute(() => {
                                    StaticVideoSurfaces[_i].SetEnable(true);
                                });

                                // TODO: Update locally the new state, so need to override the logic for local, not NetworkList
                            }
                        }
                    }
                }
                else
                {
                    if (vSurface.UID == 0 && vSurface.ENABLE)
                    {
                        bool moreWithLocalId = false;
                        for (int i = 0; i < StaticVideoSurfaces.Count; i++)
                        {
                            if (i != index)
                            {
                                if (StaticVideoSurfaces[i].UID == 0 && StaticVideoSurfaces[i].ENABLE)
                                {
                                    moreWithLocalId = true;
                                    break;
                                }
                            }
                        }

                        if (!moreWithLocalId)
                        {
                            if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
                                _channel.DisableLocalVideo();
                            else if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN)
                                _screenCapture.StopScreenCapture();
                        }
                    }
                }

                FixVideoSurface(vSurface);
                vSurface.SetForUser(userId, userId == 0 ? "" : _channel.ChannelName, userId == 0 ? (isScreenCapture ? VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN : VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA) : VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);

                if (removeFromList)
                {
                    _runtimeDictionary.Add(userId, vSurface);
                    StaticVideoSurfaces.RemoveAt(0);
                }

                if (vSurface.ENABLE)
                {
                    if (userId == 0 && !stoppedLocalCameraOrScreen)
                    {
                        if (!isScreenCapture)
                        {
                            _screenCapture.StopScreenCapture();
                        }
                        else
                        {
                            _channel.DisableLocalVideo();
                        }
                    }

                    vSurface.SetEnable(false);
                    CoroutineStarter.Instance.WaitForEndOfFrameAndExecute(() => {
                        vSurface.SetEnable(true);
                    });
                }
                else
                {
                    vSurface.SetEnable(true);
                }
            }
        }

        public void FixVideoSurface(VideoSurface vSurface)
        {
            if (!vSurface.gameObject.activeSelf) vSurface.gameObject.SetActive(true);
            float size = Mathf.Abs(vSurface.transform.localScale.x);
            bool isRenderer = vSurface.GetComponent<Renderer>();
            Vector3 scale = GetFixedScale(new Vector3(size, isRenderer ? 1f : size * ScaleFactor, isRenderer ? size * ScaleFactor : 1));
            vSurface.transform.localScale = scale;
            vSurface.transform.localEulerAngles = isRenderer ? fixRotationRenderer : fixRotationRawImage;
        }

        private Vector3 GetFixedScale(Vector3 v)
        {
            Vector3 fixedScale = fixLocalScale;
            return new Vector3(v.x * fixedScale.x, v.y * fixedScale.y, v.z * fixedScale.z);
        }

        public void DisableStaticVideoSurface(uint userId = 0, bool addToList = true)
        {
            DisableStaticVideoSurface(userId, addToList, _desactiveVSurfaceOnDisableVar);
        }

        public void DisableStaticVideoSurface(uint userId, bool addToList, bool desactiveVSurfaceOnDisable)
        {
            if (_runtimeDictionary.ContainsKey(userId) && StaticVideoSurfaces != null)
            {
                VideoSurface vSurface = _runtimeDictionary[userId];
                vSurface.SetEnable(false);
                _runtimeDictionary.RemoveWithoutDestroy(userId);

                if (addToList)
                {
                    StaticVideoSurfaces.Add(vSurface);
                }

                if (userId == 0)
                {
                    if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
                        _channel.DisableLocalVideo();
                    else if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN)
                        _screenCapture.StopScreenCapture();
                }

                if (desactiveVSurfaceOnDisable) {
                    CoroutineStarter.Instance.WaitForEndOfFrameAndExecute(() => {
                        vSurface.gameObject.SetActive(false);
                    });
                }
            }
        }

        public void DisableStaticVideoSurfaceByIndex(int index)
        {
            DisableStaticVideoSurfaceByIndex(index, _desactiveVSurfaceOnDisableVar);
        }

        public void DisableStaticVideoSurfaceByIndex(int index, bool desactiveVSurfaceOnDisable)
        {
            //Debug.Log($"DisableByIndex: {index}; Count: {StaticVideoSurfaces.Count}");
            if (StaticVideoSurfaces != null && StaticVideoSurfaces.IsValidIndex(index))
            {
                VideoSurface vSurface = StaticVideoSurfaces[index];
                vSurface.SetEnable(false);

                if (vSurface.UID == 0)
                {
                    bool moreWithLocalId = false;
                    for (int i = 0; i < StaticVideoSurfaces.Count; i++)
                    {
                        if (i != index && StaticVideoSurfaces[i].ENABLE && StaticVideoSurfaces[i].UID == 0)
                        {
                            moreWithLocalId = true;
                            break;
                        }
                    }

                    if (!moreWithLocalId)
                    {
                        if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
                            _channel.DisableLocalVideo();
                        else if (vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN)
                            _screenCapture.StopScreenCapture();
                    }
                }

                if (desactiveVSurfaceOnDisable)
                {
                    CoroutineStarter.Instance.WaitForEndOfFrameAndExecute(() => {
                        vSurface.gameObject.SetActive(false);
                    });
                }
            }
        }

        public void DisableAllStaticVideoSurfaces(uint uid)
        {
            DisableAllStaticVideoSurfaces(uid, _desactiveVSurfaceOnDisableVar);
        }

        public void DisableAllStaticVideoSurfaces(uint uid, bool desactiveVSurfaceOnDisable)
        {
            if (StaticVideoSurfaces != null)
            {
                bool isScreenCapture = false;
                for (int i = 0; i < StaticVideoSurfaces.Count; i++)
                {
                    VideoSurface vSurface = StaticVideoSurfaces[i];
                    if (vSurface.UID == uid)
                    {
                        vSurface.SetEnable(false);

                        if (uid == 0)
                            isScreenCapture = vSurface.SOURCE_TYPE == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN;

                        if (desactiveVSurfaceOnDisable)
                        {
                            int _i = i;
                            CoroutineStarter.Instance.WaitForEndOfFrameAndExecute(() => {
                                StaticVideoSurfaces[_i].gameObject.SetActive(false);
                            });
                        }
                    }
                }

                if (uid == 0)
                {
                    if (!isScreenCapture)
                        _channel.DisableLocalVideo();
                    else
                        _screenCapture.StopScreenCapture();
                }
            }
        }
    }
}