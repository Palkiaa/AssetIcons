using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AssetIconAttribute : Attribute
    {
        public AssetIconAttribute(string unityIcon)
        {
            UnityIcon = unityIcon;
        }

        public string UnityIcon { get; set; }
    }
}