using UnityEngine;
using d4160.Collections;
using Agora.Rtc;
using System;
using System.Collections.Generic;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Collections/ScreenCaptureSourceInfo")]
    public class ScreenCaptureSourceInfoRuntimeSetSO : RuntimeSetSOBase<ScreenCaptureSourceInfo>
    {
        public TextureFormat imageFormat =
#if UNITY_STANDALONE_OSX
            TextureFormat.RGBA32;
#elif UNITY_STANDALONE_WIN
            TextureFormat.BGRA32;
#else
            TextureFormat.RGBA32;
#endif

        public ScreenCaptureSourceInfo SelectedScreenCaptureSource { get; private set; }
        public List<Texture2D> ScreenCaptureSourceThumbs { get; private set; }

        internal bool IsValidIndex(int index) => Items.IsValidIndex(index);

        public void SetScreenCaptureSource(int index)
        {
            if (Items.IsValidIndex(index))
            {
                SelectedScreenCaptureSource = Items[index];
            }
        }

        internal void SetScreenCaptureSources(ScreenCaptureSourceInfo[] sources, params string[] ignores)
        {
            Clear();

            if (ignores.Length > 0)
            {
                //Debug.Log($"Ignoring: {ignores.Length}");
                List<ScreenCaptureSourceInfo> sourceList = new();
                for (int i = 0; i < sources.Length; i++)
                {
                    bool ignore = false;
                    for (int j = 0; j < ignores.Length; j++)
                    {
                        if (sources[i].sourceName.Contains(ignores[j]))
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (!ignore)
                    {
                        sourceList.Add(sources[i]);
                    }
                }

                AddRange(sourceList);
            }
            else
            {
                AddRange(sources);
            }

            SetScreenCaptureSource(0);
        }

        internal List<Texture2D> GetScreenCaptureSourceThumbs()
        {
            ScreenCaptureSourceThumbs = new List<Texture2D>(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                var tex = GetTexture2D(Items[i].thumbImage);
                ScreenCaptureSourceThumbs.Add(tex);
            }

            return ScreenCaptureSourceThumbs;
        }

        internal List<Texture2D> GetScreenCaptureSourceIcons()
        {
            ScreenCaptureSourceThumbs = new List<Texture2D>(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                var tex = GetTexture2D(Items[i].iconImage);
                ScreenCaptureSourceThumbs.Add(tex);
            }

            return ScreenCaptureSourceThumbs;
        }

        private Texture2D GetTexture2D(ThumbImageBuffer imageBuffer)
        {
            Texture2D texture = null;

            if (imageBuffer.buffer.Length == 0) return texture;

#if UNITY_STANDALONE_OSX
            texture = new Texture2D((int)imageBuffer.width, (int)imageBuffer.height, imageFormat, false);
#elif UNITY_STANDALONE_WIN
            texture = new Texture2D((int)imageBuffer.width, (int)imageBuffer.height, imageFormat, false);
#endif
            texture.LoadRawTextureData(imageBuffer.buffer);
            texture.Apply();
            
            return texture;
        }
    }
}