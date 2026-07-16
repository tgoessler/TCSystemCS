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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#endregion

namespace TCSystem.Gps;

/// <summary>
///     Contains Google Takeout location-history records and provides filtering and nearest-record lookup.
/// </summary>
public class TakeoutRecords
{
#region Public

    /// <summary>The Takeout form-factor value for phones.</summary>
    public const string FormFactorPhone = "PHONE";

    /// <summary>The Takeout form-factor value for tablets.</summary>
    public const string FormFactorTablet = "TABLET";

    /// <summary>The Takeout form-factor value for desktop devices.</summary>
    public const string FormFactorDesktop = "DESKTOP";

    /// <summary>The value used when Takeout does not specify a form factor.</summary>
    public const string FormFactorUnknown = "";

    /// <summary>
    ///     Finds the record whose timestamp is closest to the requested timestamp.
    /// </summary>
    /// <param name="locations">Locations sorted in ascending timestamp order.</param>
    /// <param name="timestamp">The timestamp to locate.</param>
    /// <returns>The nearest location. Equidistant records resolve to the earlier record.</returns>
    /// <exception cref="ArgumentException"><paramref name="locations" /> is empty.</exception>
    public static TakeoutLocation FindNearestLocation(TakeoutLocation[] locations, DateTime timestamp)
    {
        if (locations.Length == 0)
        {
            throw new ArgumentException("At least one location is required.", nameof(locations));
        }

        var left = 0;
        int right = locations.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (locations[mid].Timestamp == timestamp)
            {
                return locations[mid];
            }

            if (locations[mid].Timestamp < timestamp)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        if (right < 0)
        {
            return locations[left];
        }

        if (left >= locations.Length)
        {
            return locations[right];
        }

        TimeSpan diffLeft = locations[left].Timestamp - timestamp;
        TimeSpan diffRight = timestamp - locations[right].Timestamp;

        return diffLeft < diffRight ? locations[left] : locations[right];
    }

    /// <summary>
    ///     Finds the filtered location whose timestamp is closest to the requested timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp to locate.</param>
    /// <param name="formFactor">The requested Takeout form factor.</param>
    /// <returns>The nearest matching location.</returns>
    /// <exception cref="ArgumentException">No records match <paramref name="formFactor" />.</exception>
    public TakeoutLocation FindNearestLocation(DateTime timestamp, string formFactor)
    {
        return FindNearestLocation(GetFilteredLocations(formFactor), timestamp);
    }

    /// <summary>
    ///     Gets records for a form factor, including records for which Takeout did not specify a form factor.
    /// </summary>
    /// <param name="formFactor">The Takeout form factor to include.</param>
    /// <returns>The matching records sorted in ascending timestamp order.</returns>
    public TakeoutLocation[] GetFilteredLocations(string formFactor)
    {
        return _locations
            .Where(l => l.FormFactor.Length == 0 || l.FormFactor.Equals(formFactor))
            .OrderBy(l => l.Timestamp)
            .ToArray();
    }

    /// <summary>
    ///     Gets all deserialized location records in their source order.
    /// </summary>
    public IReadOnlyList<TakeoutLocation> Locations => _locations;

#endregion

#region Private

    [JsonInclude]
    [JsonPropertyName("locations")]
    private TakeoutLocation[] _locations = [];

#endregion
}