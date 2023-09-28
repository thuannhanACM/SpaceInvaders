
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Core.FrameworkHelper
{
    public static class DUltil
    {
        private static Dictionary<float, WaitForSeconds> WaitForDic = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (!WaitForDic.ContainsKey(seconds))
            {
                WaitForDic.Add(seconds, new WaitForSeconds(seconds));
            }
            return WaitForDic[seconds];
        }
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
        public static bool IsDoubleClick(float _lastTime)
        {
            return _lastTime + 0.75f > Time.time;
        }

        public static float FloatRoundToDecimal(float value)
        {
            return Mathf.Round(value * 10.0f) * 0.1f;
        }
    }
}
