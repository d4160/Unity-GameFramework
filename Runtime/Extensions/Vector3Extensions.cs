using d4160.Collections;

namespace d4160.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>
    /// A collection of extension methods (or one method) for Vector3s.
    /// This is just here to show how C# Extension Methods work. They're cool!
    /// </para><para>
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
    /// </para>
    /// </summary>
    public static class Vector3Extensions
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
        public static Vector3 ComponentDivide(this Vector3 v0, Vector3 v1)
        {
            Vector3 vRes = v0;

            if (v1.x != 0)
            {
                vRes.x = v0.x / v1.x;
            }
            if (v1.y != 0)
            {
                vRes.y = v0.y / v1.y;
            }
            if (v1.z != 0)
            {
                vRes.z = v0.z / v1.z;
            }

            return vRes;
        }

        public static Vector3 ComponentMultiply(this Vector3 v0, Vector3 v1)
        {
            Vector3 vRes = v0;

            vRes.x = v0.x * v1.x;
            vRes.y = v0.y * v1.y;
            vRes.z = v0.z * v1.z;

            return vRes;
        }

        public static int Scalar(this Vector3 v)
        {
            return (int)(v.x * v.y * v.z);
        }

        public static int Scalar(this Vector3Int v)
        {
            return v.x * v.y * v.z;
        }

        // Equals: Fill1 and Fill2 are identical in size and values
        public static bool EqualsVectorArray(this Vector3[] v1, Vector3[] v2)
        {
            if (v1.Length != v2.Length)
                return false;
            bool equals = true;

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                        break;

                    if (j == v2.Length - 1)
                        equals = false;
                }

                if (equals == false)
                    break;
            }

            return equals;
        }

        public static bool EqualsVectorArray(this Vector3Int[] v1, Vector3Int[] v2)
        {
            if (v1 == null || v2 == null)
                return false;

            if (v1.Length != v2.Length)
                return false;

            bool equals = true;

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                        break;

                    if (j == v2.Length - 1)
                        equals = false;
                }

                if (equals == false)
                    break;
            }

            return equals;
        }

        public static Vector3[] Union(this Vector3[] v1, Vector3[] v2)
        {
            List<Vector3> tmp = new List<Vector3>(v2);

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                        break;

                    if (j == v2.Length - 1)
                        tmp.Add(v1[i]);
                }
            }

            return tmp.ToArray();
        }

        public static Vector3Int[] Union(this Vector3Int[] v1, Vector3Int[] v2)
        {
            List<Vector3Int> tmp = new List<Vector3Int>(v2);

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                        break;

                    if (j == v2.Length - 1)
                        tmp.Add(v1[i]);
                }
            }

            return tmp.ToArray();
        }

        public static Vector3[] Difference(this Vector3[] v1, Vector3[] v2)
        {
            List<Vector3> tmp = new List<Vector3>(v1);

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                    {
                        tmp.Remove(v2[j]);
                        break;
                    }
                }
            }

            return tmp.ToArray();
        }

        public static Vector3Int[] Difference(this Vector3Int[] v1, Vector3Int[] v2)
        {
            List<Vector3Int> tmp = new List<Vector3Int>(v1);

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                    {
                        tmp.Remove(v2[j]);
                        break;
                    }
                }
            }

            return tmp.ToArray();
        }

        public static Vector3[] Intersection(this Vector3[] v1, Vector3[] v2)
        {
            List<Vector3> tmp = new List<Vector3>();

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                    {
                        tmp.Add(v2[j]);
                        break;
                    }
                }
            }

            return tmp.ToArray();
        }

        public static Vector3Int[] Intersection(this Vector3Int[] v1, Vector3Int[] v2)
        {
            List<Vector3Int> tmp = new List<Vector3Int>();

            for (int i = 0; i < v1.Length; i++)
            {
                for (int j = 0; j < v2.Length; j++)
                {
                    if (v1[i] == v2[j])
                    {
                        tmp.Add(v2[j]);
                        break;
                    }
                }
            }

            return tmp.ToArray();
        }

        public static Vector3[] Complement(this Vector3[] v1, Vector3 capacity)
        {
            List<Vector3> complement = new List<Vector3>();

            for (int z = 0; z < capacity.z; z++)
            {
                for (int y = 0; y < capacity.y; y++)
                {
                    for (int x = 0; x < capacity.x; x++)
                    {
                        bool isComplement = true;
                        Vector3 v = new Vector3(x, y, z);

                        for (int i = 0; i < v1.Length; i++)
                        {
                            if (v1[i] == v)
                            {
                                isComplement = false;
                                break;
                            }
                        }

                        if (isComplement)
                            complement.Add(v);
                    }
                }
            }
            return complement.ToArray();
        }

        public static Vector3Int[] Complement(this Vector3Int[] v1, Vector3Int capacity)
        {
            List<Vector3Int> complement = new List<Vector3Int>();

            for (int z = 0; z < capacity.z; z++)
            {
                for (int y = 0; y < capacity.y; y++)
                {
                    for (int x = 0; x < capacity.x; x++)
                    {
                        bool isComplement = true;
                        Vector3Int v = new Vector3Int(x, y, z);

                        for (int i = 0; i < v1.Length; i++)
                        {
                            if (v1[i] == v)
                            {
                                isComplement = false;
                                break;
                            }
                        }

                        if (isComplement)
                            complement.Add(v);
                    }
                }
            }
            return complement.ToArray();
        }

        // // Try to transofmr from -> to, checking Positive or Negative behaviour of the result
        // Op: Operator, Add or Substract
        public static Vector3[] To(this Vector3[] from, Vector3[] to, out int op)
        {
            Vector3[] diff = to.Difference(from);
            op = +1;
            if (diff.Length == 0)
            {
                diff = from.Difference(to);
                op = -1;

                if (diff.Length == 0)
                    op = 0;
            }

            return diff;
        }

        public static Vector3Int[] To(this Vector3Int[] from, Vector3Int[] to, out int op)
        {
            Vector3Int[] diff = to.Difference(from);
            op = +1;
            if (diff.Length == 0)
            {
                diff = from.Difference(to);
                op = -1;

                if (diff.Length == 0)
                    op = 0;
            }

            return diff;
        }

        /// <sumary>
        /// Get a new array of 'number' values from this vector array
        /// </sumary>
        public static Vector3[] Random(this Vector3[] v1, int number)
        {
            if (v1.Length == number)
                return (Vector3[])v1.Clone(); //(Vector3[])v1.Clone() since passing arrays using the same Object pointer into methods but with new this link is broken

            if (v1.IsValidIndex(number - 1))
            {
                Vector3[] randomArray = new Vector3[number];
                for (int i = 0; i < number; i++)
                {
                    if (i == 0)
                        randomArray[i] = v1[v1.RandomIndex()];
                    //else
                    //    randomArray[i] = VectorUtilities.GetDifferent(v1, randomArray, i);
                }

                return randomArray;
            }

            return null;
        }

        public static Vector3Int[] Random(this Vector3Int[] v1, int number)
        {
            if (v1.Length == number)
                return (Vector3Int[])v1.Clone(); //(Vector3Int[])v1.Clone() since passing arrays using the same Object pointer into methods but with new this link is broken

            if (v1.IsValidIndex(number - 1))
            {
                Vector3Int[] randomArray = new Vector3Int[number];
                for (int i = 0; i < number; i++)
                {
                    if (i == 0)
                        randomArray[i] = v1[v1.RandomIndex()];
                    //else
                    //    randomArray[i] = VectorUtilities.GetDifferent(v1, randomArray, i);
                }

                return randomArray;
            }

            return null;
        }

        public static Vector3 DirectionTo(this Vector3 v, Vector3 to)
        {
            return (to - v).normalized;
        }

        public static Vector3 DirectionFrom(this Vector3 v, Vector3 from)
        {
            return (v - from).normalized;
        }

        /// <summary>
        /// Gets the magnitude on an axis given a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="axis">The axis on which to calculate the magnitude.</param>
        /// <returns>The magnitude.</returns>
        public static float GetMagnitudeOnAxis(this Vector3 vector, Vector3 axis)
        {
            var vectorMagnitude = vector.magnitude;
            if (vectorMagnitude <= 0)
            {
                return 0;
            }
            var dot = Vector3.Dot(axis, vector / vectorMagnitude);
            var val = dot * vectorMagnitude;
            return val;
        }

        /// <summary>
        /// Get the square magnitude from vectorA to vectorB.
        /// </summary>
        /// <returns>The sqr magnitude.</returns>
        /// <param name="vectorA">First vector.</param>
        /// <param name="vectorB">Second vector.</param>
        public static float SqrMagnitudeFrom(this Vector3 vectorA, Vector3 vectorB)
        {
            var diff = vectorA - vectorB;
            return diff.sqrMagnitude;
        }
    }
}