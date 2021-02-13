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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public sealed class Location
    {
#region Public

        public Location(Address address, GpsPoint point)
        {
            Address = address;
            Point = point;
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

            return obj is Location location && Equals(location);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Address.GetHashCode();
                hashCode = (hashCode * 397) ^ Point.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ToJson().ToString(Formatting.Indented);
        }

        public static Location FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public Address Address { get; }
        public GpsPoint Point { get; }


        public bool IsAllSet => Point.IsSet && Address.IsSet;
        public bool IsSet => Point.IsSet || Address.IsSet;

#endregion

#region Internal

        internal static Location FromJson(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                return new Location(
                    Address.FromJson(jsonObject),
                    GpsPoint.FromJson(jsonObject)
                );
            }

            return null;
        }

        internal JObject ToJson()
        {
            var obj = Address.ToJson();
            var pos = Point.ToJson();
            obj.Merge(pos);

            return obj;
        }

#endregion

#region Private

        private bool Equals(Location other)
        {
            return Equals(Point, other.Point) &&
                   Equals(Address, other.Address);
        }

#endregion
    }
}