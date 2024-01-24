﻿// *******************************************************************************
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
    public sealed class GpsPoint : IEquatable<GpsPoint>
    {
#region Public

        public GpsPoint(GpsPosition latitude, GpsPosition longitude, float altitude)
        :this(latitude, longitude, new FixedPoint32(altitude))
        {
        }

        public GpsPoint(GpsPosition latitude=null, GpsPosition longitude=null, FixedPoint32? altitude=null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public override bool Equals(object obj)
        {
            return EqualsUtil.Equals(this, obj as GpsPoint, EqualsImp);
        }

        public bool Equals(GpsPoint other)
        {
            return EqualsUtil.Equals(this, other, EqualsImp);
        }

        private bool EqualsImp(GpsPoint other)
        {
            return Equals(Latitude, other.Latitude) &&
                   Equals(Longitude, other.Longitude) &&
                   Equals(Altitude, other.Altitude);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Latitude?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Longitude?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Altitude?.GetHashCode() ?? 0;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Latitude}:{Longitude}:{Altitude}";
        }

        public static GpsPoint FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public GpsPosition Latitude { get; }
        public GpsPosition Longitude { get; }
        public FixedPoint32? Altitude { get; }
        public bool IsSet => Longitude != null && Latitude != null;

        public static GpsPoint Undefined { get; } = new ();
#endregion

#region Internal

        internal static GpsPoint FromJson(JObject jsonObject)
        {
            var altitudeJson = jsonObject["altitude"];
            FixedPoint32? altitude = null;
            if (altitudeJson != null)
            {
                altitude = FixedPoint32.FromJson(altitudeJson);
            }

            return new GpsPoint(
                GpsPosition.FromJson((JObject) jsonObject["latitude"]),
                GpsPosition.FromJson((JObject) jsonObject["longitude"]),
                altitude);
        }

        internal JObject ToJson()
        {
            var obj = new JObject();
            if (IsSet)
            {
                obj["latitude"] = Latitude.ToJson();
                obj["longitude"] = Longitude.ToJson();
            }

            if (Altitude != null)
            {
                obj["altitude"] = Altitude.Value.ToJson();
            }

            return obj;
        }

        internal string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

#endregion

#region Private

#endregion
    }
}