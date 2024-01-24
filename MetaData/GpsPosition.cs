// *******************************************************************************
// 
//  *******   ***   ***               *
//     *     *     *                  *
//     *    *      *                *****
//     *    *       ***  *   *   **   *    **    ***
//     *    *          *  * *   *     *   ****  * * *
//     *     *         *   *      *   * * *     * * *
//     *      ***   ***    *     **   **   **   *   *
//                         *
// *******************************************************************************
//  see https://github.com/ThE-TiGeR/TCSystemCS for details.
//  Copyright (C) 2003 - 2024 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData
{
    public sealed class GpsPosition : IEquatable<GpsPosition>
    {
#region Public

        public GpsPosition(int deg, int min, int sec, int subSec, bool neg)
        {
            Degrees = deg;
            Minutes = min;
            Seconds = sec;
            SubSeconds = subSec;
            Negative = neg;
        }

        public double ToDouble()
        {
            return (Degrees +
                    Minutes / 60d +
                    Seconds / 3600d +
                    SubSeconds / (3600d * SubSecondsUnit)) * (Negative ? -1 : 1);
        }

        public override bool Equals(object obj)
        {
            return EqualsUtil.Equals(this, obj as GpsPosition, EqualsImp);
        }

        public bool Equals(GpsPosition other)
        {
            return EqualsUtil.Equals(this, other, EqualsImp);
        }

        private bool EqualsImp(GpsPosition other)
        {
            return Degrees == other.Degrees &&
                   Minutes == other.Minutes &&
                   Seconds == other.Seconds &&
                   SubSeconds == other.SubSeconds &&
                   Negative == other.Negative;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Degrees.GetHashCode();
                hashCode = (hashCode * 397) ^ Minutes.GetHashCode();
                hashCode = (hashCode * 397) ^ Seconds.GetHashCode();
                hashCode = (hashCode * 397) ^ SubSeconds.GetHashCode();
                hashCode = (hashCode * 397) ^ Negative.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var sign = Negative ? "-" : "";
            return $"{sign}{Degrees}.{Minutes}.{Seconds}.{SubSeconds}";
        }

        public static GpsPosition FromString(string val)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                var list = val.Split('.');
                if (list.Length == 4)
                {
                    var deg = int.Parse(list[0]);
                    var min = int.Parse(list[1]);
                    var sec = int.Parse(list[2]);
                    var subSec = int.Parse(list[3]);
                    var neg = deg < 0;
                    return new GpsPosition(Math.Abs(deg), min, sec, subSec, neg);
                }
            }

            return null;
        }

        public static GpsPosition FromDoublePosition(double coordinate)
        {
            var sec = Math.Abs(coordinate) * 3600d;
            var deg = (int) (sec / 3600d);
            sec = sec - deg * 3600d;
            var min = (int) (sec / 60d);
            sec = sec - min * 60d;

            return new GpsPosition(
                deg,
                min,
                (int) sec,
                (int) ((sec - (int) sec) * SubSecondsUnit),
                coordinate < 0);
        }

        public static GpsPosition FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public int Degrees { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int SubSeconds { get; }
        public bool Negative { get; }

#endregion

#region Internal

        internal static GpsPosition FromJson(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                return new GpsPosition(
                    (int) jsonObject["degrees"],
                    (int) jsonObject["minutes"],
                    (int) jsonObject["seconds"],
                    (int) jsonObject["sub_seconds"],
                    (bool) jsonObject["negative"]
                );
            }

            return null;
        }

        internal JObject ToJson()
        {
            var obj = new JObject
            {
                ["degrees"] = Degrees,
                ["minutes"] = Minutes,
                ["seconds"] = Seconds,
                ["sub_seconds"] = SubSeconds,
                ["negative"] = Negative
            };

            return obj;
        }

        internal string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

        #endregion

        #region Private

        private const double SubSecondsUnit = 10000;

#endregion
    }
}