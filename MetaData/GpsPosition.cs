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

#endregion

namespace TCSystem.MetaData;

/// <summary>Represents a signed geographic coordinate in degrees, minutes, seconds, and ten-thousandths of a second.</summary>
/// <param name="_degrees">The absolute whole degrees.</param>
/// <param name="_minutes">The whole minutes.</param>
/// <param name="_seconds">The whole seconds.</param>
/// <param name="_subSeconds">The ten-thousandths of a second.</param>
/// <param name="_negative">Whether the coordinate is negative.</param>
public readonly struct GpsPosition(int _degrees, int _minutes, int _seconds, int _subSeconds,
                                   bool _negative) : IEquatable<GpsPosition>
{
#region Public

    /// <summary>Converts the coordinate to signed decimal degrees.</summary>
    /// <returns>The coordinate in decimal degrees.</returns>
    public double ToDouble()
    {
        return (Degrees +
                Minutes / 60d +
                Seconds / 3600d +
                SubSeconds / (3600d * SubSecondsUnit)) * (Negative ? -1 : 1);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is GpsPosition pos && Equals(pos);
    }

    /// <inheritdoc />
    public bool Equals(GpsPosition other)
    {
        return Degrees == other.Degrees &&
               Minutes == other.Minutes &&
               Seconds == other.Seconds &&
               SubSeconds == other.SubSeconds &&
               Negative == other.Negative;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Degrees.GetHashCode();
            hashCode = (hashCode * 397) ^ Minutes.GetHashCode();
            hashCode = (hashCode * 397) ^ Seconds.GetHashCode();
            hashCode = (hashCode * 397) ^ SubSeconds.GetHashCode();
            hashCode = (hashCode * 397) ^ Negative.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Formats the coordinate as degrees, minutes, seconds, and subseconds separated by periods.</summary>
    /// <returns>The formatted coordinate.</returns>
    public override string ToString()
    {
        string sign = Negative ? "-" : "";
        return $"{sign}{Degrees}.{Minutes}.{Seconds}.{SubSeconds}";
    }

    /// <summary>Parses the period-delimited representation returned by <see cref="ToString" />.</summary>
    /// <param name="val">The coordinate text.</param>
    /// <returns>The parsed coordinate, or <see langword="null" /> when the format is not recognized.</returns>
    public static GpsPosition? FromString(string val)
    {
        if (!string.IsNullOrWhiteSpace(val))
        {
            string[] list = val.Split('.');
            if (list.Length == 4)
            {
                int deg = int.Parse(list[0]);
                int min = int.Parse(list[1]);
                int sec = int.Parse(list[2]);
                int subSec = int.Parse(list[3]);
                bool neg = deg < 0;
                return new(Math.Abs(deg), min, sec, subSec, neg);
            }
        }

        return null;
    }

    /// <summary>Creates a coordinate from signed decimal degrees.</summary>
    /// <param name="coordinate">The coordinate in decimal degrees.</param>
    /// <returns>The converted coordinate.</returns>
    public static GpsPosition FromDoublePosition(double coordinate)
    {
        double sec = Math.Abs(coordinate) * 3600d;
        var deg = (int)(sec / 3600d);
        sec = sec - deg * 3600d;
        var min = (int)(sec / 60d);
        sec = sec - min * 60d;

        return new(
            deg,
            min,
            (int)sec,
            (int)((sec - (int)sec) * SubSecondsUnit),
            coordinate < 0);
    }

    /// <summary>Deserializes a coordinate from JSON.</summary>
    /// <param name="jsonString">The JSON object to deserialize.</param>
    /// <returns>The coordinate, or <see langword="null" /> for null or empty input.</returns>
    public static GpsPosition? FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Determines whether two coordinates are equal.</summary>
    public static bool operator ==(GpsPosition lhs, GpsPosition rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two coordinates are not equal.</summary>
    public static bool operator !=(GpsPosition lhs, GpsPosition rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>Adds two coordinates in decimal-degree space.</summary>
    public static GpsPosition operator +(GpsPosition pos1, GpsPosition pos2)
    {
        // Convert both positions to their double representation
        var pos1Value = pos1.ToDouble();
        var pos2Value = pos2.ToDouble();

        // Subtract the second position from the first
        double resultValue = pos1Value + pos2Value;

        // Convert the result back to a GpsPosition
        return FromDoublePosition(resultValue);
    }

    /// <summary>Subtracts the second coordinate from the first in decimal-degree space.</summary>
    public static GpsPosition operator -(GpsPosition pos1, GpsPosition pos2)
    {
        // Convert both positions to their double representation
        var pos1Value = pos1.ToDouble();
        var pos2Value = pos2.ToDouble();

        // Subtract the second position from the first
        double resultValue = pos1Value - pos2Value;

        // Convert the result back to a GpsPosition
        return FromDoublePosition(resultValue);
    }

    /// <summary>Gets the absolute whole degrees.</summary>
    public int Degrees => _degrees;

    /// <summary>Gets the whole minutes.</summary>
    public int Minutes => _minutes;

    /// <summary>Gets the whole seconds.</summary>
    public int Seconds => _seconds;

    /// <summary>Gets the ten-thousandths of a second.</summary>
    public int SubSeconds => _subSeconds;

    /// <summary>Gets whether the coordinate is negative.</summary>
    public bool Negative => _negative;

#endregion

#region Internal

    internal static GpsPosition? FromJson(JObject jsonObject)
    {
        if (jsonObject != null)
        {
            return new(
                (int)jsonObject["degrees"],
                (int)jsonObject["minutes"],
                (int)jsonObject["seconds"],
                (int)jsonObject["sub_seconds"],
                (bool)jsonObject["negative"]
            );
        }

        return null;
    }

    internal JObject ToJson()
    {
        var obj = new JObject
        {
            ["degrees"] = Degrees,
            ["minutes"] = Minutes,
            ["seconds"] = Seconds,
            ["sub_seconds"] = SubSeconds,
            ["negative"] = Negative
        };

        return obj;
    }

    internal string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

#endregion

#region Private

    private const double SubSecondsUnit = 10000;

#endregion
}