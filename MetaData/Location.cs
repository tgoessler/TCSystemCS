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

namespace TCSystem.MetaData;

public sealed class Location : IEquatable<Location>
{
#region Public

    public Location(Address address, GpsPoint point)
    {
        Address = address ?? Address.Undefined;
        Point = point ?? GpsPoint.Undefined;
    }

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Location, EqualsImp);
    }

    public bool Equals(Location other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Address.GetHashCode();
            hashCode = (hashCode * 397) ^ Point.GetHashCode();
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

    public static Location FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public Address Address { get; }
    public GpsPoint Point { get; }


    public bool IsAllSet => Point.IsSet && Address.IsSet;
    public bool IsSet => Point.IsSet || Address.IsSet;

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