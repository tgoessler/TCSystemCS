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

/// <summary>Combines a textual address with a GPS point.</summary>
/// <param name="_address">The textual address.</param>
/// <param name="_point">The GPS point.</param>
public sealed class Location(Address _address, GpsPoint _point) : IEquatable<Location>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Location, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(Location other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Address.GetHashCode();
            hashCode = (hashCode * 397) ^ Point.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the location to compact JSON.</summary>
    /// <returns>The serialized location.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the location to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes a location from JSON.</summary>
    /// <returns>The location, or <see langword="null" /> for null or empty input.</returns>
    public static Location FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the address, or <see cref="Address.Undefined" />.</summary>
    public Address Address => _address ?? Address.Undefined;

    /// <summary>Gets the GPS point, or <see cref="GpsPoint.Undefined" />.</summary>
    public GpsPoint Point => _point ?? GpsPoint.Undefined;

    /// <summary>Gets whether both the GPS point and at least one address part are set.</summary>
    public bool IsAllSet => Point.IsSet && Address.IsSet;

    /// <summary>Gets whether either the GPS point or at least one address part is set.</summary>
    public bool IsSet => Point.IsSet || Address.IsSet;

    /// <summary>Gets the sentinel location with no address or GPS point.</summary>
    public static Location NoLocation { get; } = new(Address.Undefined, GpsPoint.Undefined);

#endregion

#region Internal

    internal static Location FromJson(JObject jsonObject)
    {
        if (jsonObject != null)
        {
            return new(
                Address.FromJson(jsonObject),
                GpsPoint.FromJson(jsonObject)
            );
        }

        return null;
    }

    internal JObject ToJson()
    {
        JObject obj = Address.ToJson();
        JObject pos = Point.ToJson();
        obj.Merge(pos);

        return obj;
    }

#endregion

#region Private

    private bool EqualsImp(Location other)
    {
        return Equals(Point, other.Point) &&
               Equals(Address, other.Address);
    }

#endregion
}