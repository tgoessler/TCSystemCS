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

#endregion

namespace TCSystem.Gps.Tests;

[TestFixture]
public class TakeoutRecordsTests
{
    [Test]
    public void FindNearestLocation_EmptyLocations_ThrowsArgumentException()
    {
        Assert.That(() => TakeoutRecords.FindNearestLocation(Array.Empty<TakeoutLocation>(), DateTime.UtcNow),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("locations"));
    }

    [Test]
    public async Task FindNearestLocation_TimestampBetweenLocations_ReturnsClosestLocation()
    {
        TakeoutRecords records = await ReadRecordsAsync(MultipleLocationsJson);
        TakeoutLocation[] locations = records.GetFilteredLocations(TakeoutRecords.FormFactorPhone);

        TakeoutLocation nearest = TakeoutRecords.FindNearestLocation(locations, new(2024, 1, 1, 10, 45, 0, DateTimeKind.Utc));

        Assert.That(nearest.Timestamp, Is.EqualTo(new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public async Task GetFilteredLocations_ReturnsRequestedFormFactorAndUnknownOrderedByTimestamp()
    {
        TakeoutRecords records = await ReadRecordsAsync(MultipleLocationsJson);

        TakeoutLocation[] locations = records.GetFilteredLocations(TakeoutRecords.FormFactorPhone);

        Assert.That(locations, Has.Length.EqualTo(3));
        Assert.That(locations.Select(l => l.Timestamp), Is.EqualTo(new[]
        {
            new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
        }));
        Assert.That(locations.Select(l => l.FormFactor), Is.EqualTo(new[]
        {
            TakeoutRecords.FormFactorPhone,
            TakeoutRecords.FormFactorUnknown,
            TakeoutRecords.FormFactorPhone
        }));
    }

    private const string MultipleLocationsJson = """
                                                 {
                                                   "locations": [
                                                     {
                                                       "timestamp": "2024-01-01T11:00:00Z",
                                                       "latitudeE7": 471100000,
                                                       "longitudeE7": 151100000,
                                                       "formFactor": "PHONE"
                                                     },
                                                     {
                                                       "timestamp": "2024-01-01T08:00:00Z",
                                                       "latitudeE7": 470800000,
                                                       "longitudeE7": 150800000,
                                                       "formFactor": "TABLET"
                                                     },
                                                     {
                                                       "timestamp": "2024-01-01T10:00:00Z",
                                                       "latitudeE7": 471000000,
                                                       "longitudeE7": 151000000,
                                                       "formFactor": ""
                                                     },
                                                     {
                                                       "timestamp": "2024-01-01T09:00:00Z",
                                                       "latitudeE7": 470900000,
                                                       "longitudeE7": 150900000,
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
}