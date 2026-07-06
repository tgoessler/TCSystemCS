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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.Gps.Tests;

[TestFixture]
public class TakeoutRecordReaderTests
{
#region Public

    [Test]
    public async Task ReadAsync_ValidRecordsJson_DeserializesLocations()
    {
        TakeoutRecords records = await ReadRecordsAsync(SingleLocationJson);

        Assert.That(records, Is.Not.Null);
        Assert.That(records.Locations, Has.Count.EqualTo(1));

        TakeoutLocation location = records.Locations.Single();
        Assert.That(location.Timestamp, Is.EqualTo(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)));
        Assert.That(location.Accuracy, Is.EqualTo(5));
        Assert.That(location.DeviceTag, Is.EqualTo(42));
        Assert.That(location.FormFactor, Is.EqualTo(TakeoutRecords.FormFactorPhone));
        AssertGpsPoint(location.GpsPoint, 47.1234567, 15.1234567, 365);
    }

#endregion

#region Private

    private const string SingleLocationJson = """
                                              {
                                                "locations": [
                                                  {
                                                    "timestamp": "2024-01-01T10:00:00Z",
                                                    "latitudeE7": 471234567,
                                                    "longitudeE7": 151234567,
                                                    "accuracy": 5,
                                                    "altitude": 365,
                                                    "deviceTag": 42,
                                                    "formFactor": "PHONE"
                                                  }
                                                ]
                                              }
                                              """;

    private static async Task<TakeoutRecords> ReadRecordsAsync(string json)
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return await TakeoutRecordReader.ReadAsync(stream);
    }

    private static void AssertGpsPoint(GpsPoint gpsPoint, double latitude, double longitude, int altitude)
    {
        Assert.That(gpsPoint.IsSet, Is.True);
        Assert.That(gpsPoint.Latitude.Value.ToDouble(), Is.EqualTo(latitude).Within(0.0000001));
        Assert.That(gpsPoint.Longitude.Value.ToDouble(), Is.EqualTo(longitude).Within(0.0000001));
        Assert.That(gpsPoint.Altitude.Value.RawValue, Is.EqualTo(altitude));
    }

#endregion
}
