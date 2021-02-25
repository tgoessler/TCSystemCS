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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public readonly struct FaceDistanceInfo : IEquatable<FaceDistanceInfo>
    {
#region Public
        public FaceDistanceInfo(long faceId1, FixedPoint32 distance, long faceId2)
        {
            FaceId1 = faceId1;
            Distance = distance;
            FaceId2 = faceId2;
        }

        public override bool Equals(object obj)
        {
            return obj is FaceDistanceInfo other && Equals(other);
        }

        public bool Equals(FaceDistanceInfo other)
        {
            return FaceId1.Equals(other.FaceId1) &&
                   Distance.Equals(other.Distance) &&
                   FaceId2.Equals(other.FaceId2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FaceId1.GetHashCode();
                hashCode = (hashCode * 397) ^ Distance.GetHashCode();
                hashCode = (hashCode * 397) ^ FaceId2.GetHashCode();
                return hashCode;
            }
        }

        public string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

        public override string ToString()
        {
            return ToJson().ToString(Formatting.Indented);
        }

        public static FaceDistanceInfo FromJsonString(string jsonString)
        {
            return FromJson(JObject.Parse(jsonString));
        }

        public static IEnumerable<FaceDistanceInfo> FromJsonStringArray(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return Array.Empty<FaceDistanceInfo>();
            }

            var array = JArray.Parse(jsonString);
            return array.Select(v => FromJson((JObject) v));
        }

        public static bool operator ==(FaceDistanceInfo lhs, FaceDistanceInfo rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FaceDistanceInfo lhs, FaceDistanceInfo rhs)
        {
            return !lhs.Equals(rhs);
        }

        public long FaceId1 { get; }
        public FixedPoint32 Distance { get; }
        public long FaceId2 { get; }

#endregion

#region Private

        private static FaceDistanceInfo FromJson(JObject jsonObject)
        {
            return new(
                (long) jsonObject["face_id_1"],
                FixedPoint32.FromJson(jsonObject["distance"]),
                (long) jsonObject["face_id_2"]
            );
        }

        private JObject ToJson()
        {
            var obj = new JObject
            {
                ["face_id_1"] = FaceId1,
                ["distance"] = Distance.ToJson(),
                ["face_id_2"] = FaceId2
            };

            return obj;
        }

#endregion
    }
}