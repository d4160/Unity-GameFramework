using System;
using UnityEngine;

namespace BetterDefines.Editor.Entity
{
    public class BuildPlatform
    {
        public bool Enabled;
        /// <summary>
        /// Used to load default platform icon
        /// </summary>
        public string Id;
        public string Name;

        [NonSerialized]
        public Texture2D Icon;

        public BuildPlatform(string name, string id, bool enabled, Texture2D icon)
        {
            Name = name;
            Id = id;
            Enabled = enabled;
            Icon = icon;
        }

        public GUIContent ToGUIContent()
        {
            return new GUIContent(Icon, Name);
        }
    }
}