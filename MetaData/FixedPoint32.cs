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

/// <summary>Represents a signed 16.16 fixed-point number.</summary>
/// <param name="_rawValue">The raw fixed-point representation.</param>
public readonly struct FixedPoint32(int _rawValue) : IEquatable<FixedPoint32>
{
#region Public

    /// <summary>Converts a single-precision value to 16.16 fixed point.</summary>
    /// <param name="val">The value to convert.</param>
    public FixedPoint32(float val) : this((int)(val * (1 << 16))) { }

    /// <summary>Converts a double-precision value to 16.16 fixed point.</summary>
    /// <param name="val">The value to convert.</param>
    public FixedPoint32(double val) : this((int)(val * (1 << 16))) { }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is FixedPoint32 fixedPoint && Equals(fixedPoint);
    }

    /// <inheritdoc />
    public bool Equals(FixedPoint32 other)
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

    /// <summary>Deserializes a raw 16.16 fixed-point value from JSON.</summary>
    /// <param name="jsonString">The JSON number to deserialize.</param>
    /// <returns>The deserialized value.</returns>
    public static FixedPoint32 FromJsonString(string jsonString)
    {
        return FromJson(JToken.Parse(jsonString));
    }

    /// <summary>Determines whether two fixed-point values are equal.</summary>
    public static bool operator ==(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two fixed-point values are not equal.</summary>
    public static bool operator !=(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>Determines whether the left value is greater than the right value.</summary>
    public static bool operator >(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return lhs.RawValue > rhs.RawValue;
    }

    /// <summary>Determines whether the left value is less than or equal to the right value.</summary>
    public static bool operator <=(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return lhs.RawValue <= rhs.RawValue;
    }

    /// <summary>Determines whether the left value is greater than or equal to the right value.</summary>
    public static bool operator >=(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return lhs.RawValue >= rhs.RawValue;
    }

    /// <summary>Determines whether the left value is less than the right value.</summary>
    public static bool operator <(FixedPoint32 lhs, FixedPoint32 rhs)
    {
        return lhs.RawValue < rhs.RawValue;
    }

    /// <summary>Gets the represented single-precision value.</summary>
    public float Value => _rawValue / (float)(1 << 16);

    /// <summary>Gets the raw 16.16 representation.</summary>
    public int RawValue => _rawValue;

#endregion

#region Internal

    internal static FixedPoint32 FromJson(JToken jsonToken)
    {
        return new((int)jsonToken);
    }

    internal JToken ToJson()
    {
        return RawValue;
    }

#endregion
}