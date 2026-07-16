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
using System.Text.Json.Serialization;
using TCSystem.MetaData;

#endregion

namespace TCSystem.Gps;

/// <summary>
///     Represents one location record from a Google Takeout location-history export.
/// </summary>
public struct TakeoutLocation
{
#region Public

    /// <summary>
    ///     Initializes an empty location for JSON deserialization.
    /// </summary>
    public TakeoutLocation() { }

    /// <summary>
    ///     Returns a readable representation of the location record.
    /// </summary>
    /// <returns>The timestamp, GPS point, accuracy, form factor, and device tag.</returns>
    public override string ToString()
    {
        return $"Timestamp: {Timestamp}, GpsPoint: {GpsPoint}, Accuracy: {Accuracy}, FormFactor: {FormFactor}, DeviceTag: {DeviceTag}";
    }

    /// <summary>
    ///     Gets the reported location accuracy in meters.
    /// </summary>
    [JsonIgnore]
    public int Accuracy => _accuracy;

    /// <summary>
    ///     Gets the time at which the location was recorded.
    /// </summary>
    [JsonIgnore]
    public DateTime Timestamp => _timestamp;

    /// <summary>
    ///     Gets the Takeout device identifier associated with the record.
    /// </summary>
    [JsonIgnore]
    public int DeviceTag => _deviceTag;

    /// <summary>
    ///     Gets the device form factor reported by Takeout.
    /// </summary>
    [JsonIgnore]
    public string FormFactor => _formFactor;

    /// <summary>
    ///     Gets the latitude, longitude, and optional altitude as a metadata GPS point.
    /// </summary>
    [JsonIgnore]
    public GpsPoint GpsPoint => new(GpsPosition.FromDoublePosition(_latitude / 10000000.0),
        GpsPosition.FromDoublePosition(_longitude / 10000000.0),
        _altitude != 0 ? new FixedPoint32(_altitude) : null);

#endregion

#region Private

    [JsonInclude]
    [JsonPropertyName("accuracy")]
    private int _accuracy = 0;

    [JsonInclude]
    [JsonPropertyName("timestamp")]
    private DateTime _timestamp = default;

    [JsonInclude]
    [JsonPropertyName("altitude")]
    private int _altitude = 0;

    [JsonInclude]
    [JsonPropertyName("latitudeE7")]
    private int _latitude = 0;

    [JsonInclude]
    [JsonPropertyName("longitudeE7")]
    private int _longitude = 0;

    [JsonInclude]
    [JsonPropertyName("deviceTag")]
    private int _deviceTag = 0;

    [JsonInclude]
    [JsonPropertyName("formFactor")]
    private string _formFactor = string.Empty;

#endregion
}