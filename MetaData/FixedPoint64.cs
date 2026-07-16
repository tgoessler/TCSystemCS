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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData;

/// <summary>Represents a signed 32.32 fixed-point number.</summary>
/// <param name="_rawValue">The raw fixed-point representation.</param>
public readonly struct FixedPoint64(long _rawValue) : IEquatable<FixedPoint64>
{
#region Public

    /// <summary>Converts a double-precision value to 32.32 fixed point.</summary>
    /// <param name="val">The value to convert.</param>
    public FixedPoint64(double val) : this((long)(val * (1L << 32))) { }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is FixedPoint64 fixedPoint && Equals(fixedPoint);
    }

    /// <inheritdoc />
    public bool Equals(FixedPoint64 other)
    {
        return RawValue == other.RawValue;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return RawValue.GetHashCode();
    }


    /// <summary>Serializes the raw fixed-point value to JSON.</summary>
    /// <returns>The serialized raw value.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Formats the represented value using the invariant culture.</summary>
    /// <returns>The decimal representation.</returns>
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Deserializes a raw 32.32 fixed-point value from JSON.</summary>
    /// <param name="jsonString">The JSON number to deserialize.</param>
    /// <returns>The deserialized value.</returns>
    public static FixedPoint64 FromJsonString(string jsonString)
    {
        return FromJson(JToken.Parse(jsonString));
    }

    /// <summary>Determines whether two fixed-point values are equal.</summary>
    public static bool operator ==(FixedPoint64 lhs, FixedPoint64 rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two fixed-point values are not equal.</summary>
    public static bool operator !=(FixedPoint64 lhs, FixedPoint64 rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>Gets the represented double-precision value.</summary>
    public double Value => _rawValue / (double)(1L << 32);

    /// <summary>Gets the raw 32.32 representation.</summary>
    public long RawValue => _rawValue;

#endregion

#region Internal

    internal static FixedPoint64 FromJson(JToken jsonToken)
    {
        return new((long)jsonToken);
    }

    internal JToken ToJson()
    {
        return RawValue;
    }

#endregion
}