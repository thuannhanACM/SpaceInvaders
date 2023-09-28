using UnityEngine;

namespace Core.Framework
{
    public static class GameObjectExtension
    {
        public static T SetActive<T>(this T comp, bool active) where T : Component
        {
            comp.gameObject.SetActive(active);
            return comp;
        }

        public static bool ActiveSelf<T>(this T comp) where T : Component
        {
            return comp.gameObject.activeSelf;
        }

        public static Transform ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
            return transform;
        }

        public static GameObject ClearChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
            return gameObject;
        }
    }
}