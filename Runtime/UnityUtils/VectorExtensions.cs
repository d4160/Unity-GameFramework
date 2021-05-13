using UnityEngine;

namespace d4160.UnityUtils {
    public static class VectorExtensions {

        public static Vector3 SumVector2 (this Vector3 v1, Vector2 v2) {
            return new Vector3 (v1.x + v2.x, v1.y + v2.y, v1.z);
        }

        public static Vector2 SumVector3 (this Vector2 v1, Vector3 v2) {
            return new Vector2 (v1.x + v2.x, v1.y + v2.y);
        }
    }
}