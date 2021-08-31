using System;
using System.Text;
using UnityEngine;

namespace d4160.Texts
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return a '00...' formatted string
        /// </summary>
        /// <param name="current"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToZerosFormattedString(this int current, int length)
        {
            int digits = Mathf.FloorToInt(Mathf.Log10(current) + 1);

            if (digits >= length) return current.ToString();

            StringBuilder sb = new StringBuilder(length);

            for (int i = digits; i < length; i++)
            {
                sb.Append('0');
            }

            sb.Append(current);
            return sb.ToString();
        }

        /// <summary>
        /// Returns a new string with 'length' filling with 'char'
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Put(this string s, int length, char c)
        {
            if (s.Length >= length) return s;

            var left = length - s.Length;

            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < left; i++)
            {
                sb.Append(c);
            }

            sb.Append(s);

            return sb.ToString();
        }

        public static string Bold(this string s)
        {
            return $"<b>{s}</b>";
        }

        public static string Italic(this string s)
        {
            return $"<i>{s}</i>";
        }

        public static string Underline(this string s)
        {
            return $"<u>{s}</u>";
        }

        public static string StrikeThrough(this string s)
        {
            return $"<s>{s}</s>";
        }

        /// <summary>
        /// #RRGGBB format (00 to ff) (don't need '#')
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string Color(this string s, string c)
        {
            return $"<color=#{c}>{s}</color>";
        }

        /// <summary>
        /// #RRGGBB format (00 to ff) (don't need '#')
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string Color(this string s, Color c)
        {
            string cFormat = $"{Convert.ToString((int)(c.r * 255), 16).Put(2, '0')}{Convert.ToString((int)(c.g * 255), 16).Put(2, '0')}{Convert.ToString((int)(c.b * 255), 16).Put(2, '0')}";

            return $"<color=#{cFormat}>{s}</color>";
        }

        /// <summary>
        /// #AA format (00 to ff) (don't need '#')
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string Alpha(this string s, string a)
        {
            return $"<alpha=#{a}>{s}</alpha>";
        }
    }
}