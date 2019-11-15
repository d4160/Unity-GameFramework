namespace d4160.Utilities
{
    using UnityEngine;

    public static class MathUtilities
    {
        /// <summary>
        /// Get the percentage of 'value' between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns>-1 if there are invalid params </returns>
        public static float GetRate(Vector2 minMax, float value)
        {
            return GetRate(minMax.x, minMax.y, value);
        }

        /// <summary>
        /// Get the percentage of 'value' between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns>-1 if there are invalid params </returns>
        public static float GetRate(float min, float max, float value)
        {
            if (min < max && (value >= min && value <= max))
            {
                var x1 = max - min;
                var x0 = value - min;
                return x0 / x1;
            }

            return -1;
        }

        /// <summary>
        /// Get the value that represents 'percentage' between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="percentage"></param>
        /// <returns>-1 when send invalid values</returns>
        public static float GetValue(Vector2 minMax, float percentage)
        {
            return GetValue(minMax.x, minMax.y, percentage);
        }
        /// <summary>
        /// Get the value that represents 'percentage' between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="percentage"></param>
        /// <returns>-1 when send invalid values</returns>
        public static float GetValue(float min, float max, float percentage)
        {
            if (max > min && percentage >= 0 && percentage <= 1)
            {
                var x1 = max - min;

                return percentage * x1 + min;
            }

            return -1;
        }

        /// <summary>
        /// Returns a needed YVelocity to reach distance 's'
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector2 GetInitYVelocity2D(float s)
        {
#if UNITY_PHYSICS2D
            // from v^2 = u^2 + 2 * a * s
            var yInitVelocity = Mathf.Sqrt(-2 * (-Physics2D.gravity.magnitude) * s);

            //Debug.Log(yInitVelocity);

            return Vector2.up * yInitVelocity;
#else
            return Vector2.one;
#endif
        }

        /// <summary>
        /// Calculate a MouseWorldPoint using a camera and related target as reference for Z deep
        /// Used the Dot product of the Camera forward and the Dir to the RelTarget to calculate the z deep
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="relatedTargetT"></param>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPoint(Camera camera, Transform relatedTargetT)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = Vector3.Dot(camera.transform.forward, relatedTargetT.position - camera.transform.position);
            return camera.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// Calculate a MouseWorldPoint using a camera and related target as reference for Z deep
        /// Used the Dot product of the Camera forward and the Dir to the RelTarget to calculate the z deep
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="relatedTarget"></param>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPoint(Camera camera, Vector3 relatedTarget)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = Vector3.Dot(camera.transform.forward, relatedTarget - camera.transform.position);
            return camera.ScreenToWorldPoint(mousePos);
        }

        public static Quaternion ClampRotationAroundXAxis(Quaternion q, float MinimumX, float MaximumX)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public static Quaternion ClampRotationAroundXAxis(Vector3 euler, float MinimumX, float MaximumX)
        {
            var q = Quaternion.Euler(euler);
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        /// <summary>
        /// Calculates the power necessary to get the value from two raised to
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetInverseTwoRaisedPower(int value)
        {
            var pow = (int)Mathf.Log(value, 2);

            return pow;
        }

        /// <summary>
        /// Calculates two raised to the power of
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static int GetTwoRaisedPower(int power)
        {
            var pot = (int)Mathf.Pow(2, power);

            return pot;
        }
    }
}