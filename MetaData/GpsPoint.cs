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
//  Copyright (C) 2003 - 2025 Thomas Goessler. All Rights Reserved.
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

/// <summary>
///     Represents a GPS point with latitude, longitude, and optional altitude.
/// </summary>
public sealed class GpsPoint(GpsPosition? _latitude = null, GpsPosition? _longitude = null, FixedPoint32? _altitude = null) : IEquatable<GpsPoint>
{
#region Public

    /// <summary>
    ///     Initializes a new instance of the <see cref="GpsPoint" /> class with specified latitude, longitude, and altitude.
    /// </summary>
    /// <param name="latitude">The latitude of the GPS point.</param>
    /// <param name="longitude">The longitude of the GPS point.</param>
    /// <param name="altitude">The altitude of the GPS point in meters.</param>
    public GpsPoint(GpsPosition latitude, GpsPosition longitude, float altitude)
        : this(latitude, longitude, new FixedPoint32(altitude)) { }

    /// <summary>
    ///     Determines whether the specified object is equal to the current GPS point.
    /// </summary>
    /// <param name="obj">The object to compare with the current GPS point.</param>
    /// <returns>True if the specified object is equal to the current GPS point; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as GpsPoint, EqualsImp);
    }

    /// <summary>
    ///     Determines whether the specified GPS point is equal to the current GPS point.
    /// </summary>
    /// <param name="other">The GPS point to compare with the current GPS point.</param>
    /// <returns>True if the specified GPS point is equal to the current GPS point; otherwise, false.</returns>
    public bool Equals(GpsPoint other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <summary>
    ///     Returns the hash code for the current GPS point.
    /// </summary>
    /// <returns>A hash code for the current GPS point.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Latitude?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Longitude?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Altitude?.GetHashCode() ?? 0;
            return hashCode;
        }
    }

    /// <summary>
    ///     Returns a string representation of the GPS point.
    /// </summary>
    /// <returns>A string in the format "Latitude:Longitude:Altitude".</returns>
    public override string ToString()
    {
        return $"{Latitude}:{Longitude}:{Altitude}";
    }

    /// <summary>
    ///     Creates a <see cref="GpsPoint" /> instance from a JSON string.
    /// </summary>
    /// <param name="jsonString">The JSON string representing the GPS point.</param>
    /// <returns>A <see cref="GpsPoint" /> instance or null if the JSON string is null or empty.</returns>
    public static GpsPoint FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>
    ///     Calculates the distance between two GPS points using the Haversine formula in kilometers.
    /// </summary>
    /// <param name="point1">The starting GPS point.</param>
    /// <param name="point2">The ending GPS point.</param>
    /// <returns>The distance in kilometers, or NaN if either point is not set.</returns>
    public static double operator -(GpsPoint point1, GpsPoint point2)
    {
        if (point1 is not { IsSet: true } || point2 is not { IsSet: true })
        {
            return double.NaN;
        }

        // ReSharper disable PossibleInvalidOperationException
        var lat1 = point1.Latitude.Value.ToDouble();
        var lon1 = point1.Longitude.Value.ToDouble();
        var lat2 = point2.Latitude.Value.ToDouble();
        var lon2 = point2.Longitude.Value.ToDouble();
        // ReSharper restore PossibleInvalidOperationException

        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        const double earthRadius = 6378;

        return earthRadius * c;
    }

    /// <summary>
    ///     Gets the latitude of the GPS point.
    /// </summary>
    public GpsPosition? Latitude => _latitude;

    /// <summary>
    ///     Gets the longitude of the GPS point.
    /// </summary>
    public GpsPosition? Longitude => _longitude;

    /// <summary>
    ///     Gets the altitude of the GPS point.
    /// </summary>
    public FixedPoint32? Altitude => _altitude;

    /// <summary>
    ///     Gets a value indicating whether both latitude and longitude are set.
    /// </summary>
    public bool IsSet => Longitude.HasValue && Latitude.HasValue;

    /// <summary>
    ///     Gets an undefined GPS point.
    /// </summary>
    public static GpsPoint Undefined { get; } = new();

#endregion

#region Internal

    /// <summary>
    ///     Creates a <see cref="GpsPoint" /> instance from a JSON object.
    /// </summary>
    /// <param name="jsonObject">The JSON object representing the GPS point.</param>
    /// <returns>A <see cref="GpsPoint" /> instance.</returns>
    internal static GpsPoint FromJson(JObject jsonObject)
    {
        JToken altitudeJson = jsonObject["altitude"];
        FixedPoint32? altitude = null;
        if (altitudeJson != null)
        {
            altitude = FixedPoint32.FromJson(altitudeJson);
        }

        return new(
            GpsPosition.FromJson((JObject)jsonObject["latitude"]),
            GpsPosition.FromJson((JObject)jsonObject["longitude"]),
            altitude);
    }

    /// <summary>
    ///     Converts the GPS point to a JSON object.
    /// </summary>
    /// <returns>A JSON object representing the GPS point.</returns>
    internal JObject ToJson()
    {
        var obj = new JObject();
        if (Latitude.HasValue)
        {
            obj["latitude"] = Latitude.Value.ToJson();
        }

        if (Longitude.HasValue)
        {
            obj["longitude"] = Longitude.Value.ToJson();
        }

        if (Altitude.HasValue)
        {
            obj["altitude"] = Altitude.Value.ToJson();
        }

        return obj;
    }

    /// <summary>
    ///     Converts the GPS point to a JSON string.
    /// </summary>
    /// <returns>A JSON string representing the GPS point.</returns>
    internal string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

#endregion

#region Private

    /// <summary>
    ///     Compares the current GPS point with another GPS point for equality.
    /// </summary>
    /// <param name="other">The GPS point to compare with.</param>
    /// <returns>True if the GPS points are equal; otherwise, false.</returns>
    private bool EqualsImp(GpsPoint other)
    {
        return Equals(Latitude, other.Latitude) &&
               Equals(Longitude, other.Longitude) &&
               Equals(Altitude, other.Altitude);
    }

#endregion
}