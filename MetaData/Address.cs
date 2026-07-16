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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.MetaData;

/// <summary>Represents the textual parts of a geographic address.</summary>
/// <param name="_country">The country name.</param>
/// <param name="_province">The province or state name.</param>
/// <param name="_city">The city name.</param>
/// <param name="_street">The street address.</param>
public sealed class Address(string _country = "", string _province = "", string _city = "", string _street = "") : IEquatable<Address>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Address, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(Address other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Country != null ? Country.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Province != null ? Province.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
            return hashCode;
        }
    }

    /// <summary>Serializes the address to compact JSON.</summary>
    /// <returns>The serialized address.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the address to indented JSON.</summary>
    /// <returns>The formatted address JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes an address from JSON.</summary>
    /// <param name="jsonString">The JSON to deserialize.</param>
    /// <returns>The address, or <see langword="null" /> when the input is null or empty.</returns>
    public static Address FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the country name.</summary>
    public string Country => _country;

    /// <summary>Gets the province or state name.</summary>
    public string Province => _province;

    /// <summary>Gets the city name.</summary>
    public string City => _city;

    /// <summary>Gets the street address.</summary>
    public string Street => _street;

    /// <summary>Gets the non-empty address parts joined from street to country.</summary>
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

    /// <summary>Gets whether at least one address part is set.</summary>
    public bool IsSet => Country.Length != 0 ||
                         Province.Length != 0 ||
                         City.Length != 0 ||
                         Street.Length != 0;

    /// <summary>Gets whether every address part is set.</summary>
    public bool IsAllSet => Country.Length != 0 &&
                            Province.Length != 0 &&
                            City.Length != 0 &&
                            Street.Length != 0;

    /// <summary>Gets an address with no defined parts.</summary>
    public static Address Undefined { get; } = new();

    /// <summary>Gets the sentinel address used when an address lookup found no result.</summary>
    public static Address NotFound { get; } = new("", "", "", "NotFound");

#endregion

#region Internal

    internal static Address FromJson(JObject jsonObject)
    {
        return new(
            (string)jsonObject["country"],
            (string)jsonObject["province"],
            (string)jsonObject["city"],
            (string)jsonObject["street"]
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

#region Private

    private bool EqualsImp(Address other)
    {
        return string.Equals(Country, other.Country, StringComparison.InvariantCulture) &&
               string.Equals(Province, other.Province, StringComparison.InvariantCulture) &&
               string.Equals(City, other.City, StringComparison.InvariantCulture) &&
               string.Equals(Street, other.Street, StringComparison.InvariantCulture);
    }

#endregion
}