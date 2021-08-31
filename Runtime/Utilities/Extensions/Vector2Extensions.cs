namespace d4160.Core
{
    using UnityEngine;

    /// <summary>
    /// <para>
    /// A collection of extension methods (or one method) for Vector2s.
    /// This is just here to show how C# Extension Methods work. They're cool!
    /// </para><para>
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
    /// </para>
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// <para>
        /// Divides the individual components of v0 by those of v1.
        /// </para><para>
        /// e.g. v0.ComponentDivide(v1) returns [ v0.x/v1.x, v0.y/v1.y, v0.z/v1.z ]
        /// </para><para>
        /// If any of the components of v1 are 0, then that component of v0 will
        /// remain unchanged to avoid divide by zero errors.
        /// </para>
        /// </summary>
        /// <returns>The Vector3 result of the ComponentDivide.</returns>
        /// <param name="v0">The numerator Vector3</param>
        /// <param name="v1">The denominator Vector3</param>
        static public Vector2 ComponentDivide(this Vector2 v0, Vector2 v1)
        {
            Vector2 vRes = v0;

            // Avoid divide by zero errors
            if (v1.x != 0)
            {
                vRes.x = v0.x / v1.x;
            }
            if (v1.y != 0)
            {
                vRes.y = v0.y / v1.y;
            }

            return vRes;
        }

        static public Vector2 ComponentMultiply(this Vector2 v0, Vector2 v1)
        {
            Vector2 vRes = v0;

            vRes.x = v0.x * v1.x;
            vRes.y = v0.y * v1.y;

            return vRes;
        }

        public static float Random(this Vector2 v0)
        {
            if (v0.x > v0.y)
                return UnityEngine.Random.Range(v0.y, v0.x);
            if (v0.x < v0.y)
                return UnityEngine.Random.Range(v0.x, v0.y);

            return v0.x;
        }

        public static int Random(this Vector2Int v0)
        {
            if (v0.x == v0.y) return v0.x;

            if (v0.x > v0.y)
                return UnityEngine.Random.Range(v0.y, v0.x + 1);

            if (v0.x < v0.y)
                return UnityEngine.Random.Range(v0.x, v0.y + 1);

            return v0.x;
        }

        /// <summary>
        /// Gets an interpolation time using <see cref="m_MinValue"/> and <see cref="m_MaxValue"/>.
        /// </summary>
        /// <param name="oldValue">The current value.</param>
        /// <param name="newValue">The new value to approach.</param>
        /// <returns>The interpolated value.</returns>
        public static float GetInterpolationTime(this Vector2 source, float oldValue, float newValue)
        {
            var valueDifference = Mathf.Clamp(Mathf.Abs(oldValue - newValue), 0.0f, 1.0f);
            var interpolationDifference = source.y - source.x;
            return source.y - (valueDifference * interpolationDifference);
        }
    }
}