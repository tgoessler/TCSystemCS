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
using System.Text.Json.Serialization;
using TCSystem.MetaData;

#endregion

namespace TCSystem.Gps;

public struct TakeoutLocation
{
#region Public

    public TakeoutLocation() { }

    public override string ToString()
    {
        return $"Timestamp: {Timestamp}, GpsPoint: {GpsPoint}, Accuracy: {Accuracy}, FormFactor: {FormFactor}, DeviceTag: {DeviceTag}";
    }

    [JsonIgnore]
    public int Accuracy => _accuracy;

    [JsonIgnore]
    public DateTime Timestamp => _timestamp;

    [JsonIgnore]
    public int DeviceTag => _deviceTag;

    [JsonIgnore]
    public string FormFactor => _formFactor;

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