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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
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
    public sealed class Address : IEquatable<Address>
    {
#region Public

        public Address(string country = "", string province = "", string city = "",
                       string street = "")
        {
            Country = country;
            Province = province;
            City = city;
            Street = street;
        }

        public override bool Equals(object obj)
        {
            return EqualsUtil.Equals(this, obj as Address, EqualsImp);
        }

        public bool Equals(Address other)
        {
            return EqualsUtil.Equals(this, other, EqualsImp);
        }

        private bool EqualsImp(Address other)
        {
            return string.Equals(Country, other.Country, StringComparison.InvariantCulture) &&
                   string.Equals(Province, other.Province, StringComparison.InvariantCulture) &&
                   string.Equals(City, other.City, StringComparison.InvariantCulture) &&
                   string.Equals(Street, other.Street, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Country != null ? Country.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Province != null ? Province.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
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

        public static Address FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public string Country { get; }
        public string Province { get; }
        public string City { get; }
        public string Street { get; }

        public string FormattedAddress
        {
            get
            {
                var val = "";
                if (Street.Length != 0)
                {
                    val = Street;
                }

                if (City.Length != 0)
                {
                    val += val.Length > 0 ? ", " : "";
                    val += City;
                }

                if (Province.Length != 0)
                {
                    val += val.Length > 0 ? ", " : "";
                    val += Province;
                }

                if (Country.Length != 0)
                {
                    val += val.Length > 0 ? ", " : "";
                    val += Country;
                }

                return val;
            }
        }

        public bool IsSet => Country.Length != 0 ||
                             Province.Length != 0 ||
                             City.Length != 0 ||
                             Street.Length != 0;

        public bool IsAllSet => Country.Length != 0 &&
                                Province.Length != 0 &&
                                City.Length != 0 &&
                                Street.Length != 0;

        public static Address Undefined { get; } = new();
        public static Address NotFound { get; } = new("", "", "", "NotFound");

#endregion

#region Internal

        internal static Address FromJson(JObject jsonObject)
        {
            return new(
                (string) jsonObject["country"],
                (string) jsonObject["province"],
                (string) jsonObject["city"],
                (string) jsonObject["street"]
            );
        }

        internal JObject ToJson()
        {
            var obj = new JObject
            {
                ["country"] = Country,
                ["province"] = Province,
                ["city"] = City,
                ["street"] = Street
            };

            return obj;
        }

#endregion
    }
}