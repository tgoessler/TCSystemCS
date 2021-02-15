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

#endregion

#region Usings

using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public sealed class GpsPoint
    {
#region Public

        public GpsPoint(GpsPosition latitude, GpsPosition longitude, FixedPoint32? altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is GpsPoint point && Equals(point);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                hashCode = (hashCode * 397) ^ (Altitude != null ? Altitude.GetHashCode() : 0);
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

#endregion

#region Private

        private bool Equals(GpsPoint other)
        {
            return Equals(Latitude, other.Latitude) &&
                   Equals(Longitude, other.Longitude) &&
                   Equals(Altitude, other.Altitude);
        }

#endregion
    }
}