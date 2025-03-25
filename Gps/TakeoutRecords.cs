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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#endregion

namespace TCSystem.Gps;

public class TakeoutRecords
{
#region Public

    public const string FormFactorPhone = "PHONE";
    public const string FormFactorTablet = "TABLET";
    public const string FormFactorDesktop = "DESKTOP";
    public const string FormFactorUnknown = "";

    public static TakeoutLocation FindNearestLocation(TakeoutLocation[] locations, DateTime timestamp)
    {
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

    public TakeoutLocation FindNearestLocation(DateTime timestamp, string formFactor)
    {
        return FindNearestLocation(GetFilteredLocations(formFactor), timestamp);
    }

    public TakeoutLocation[] GetFilteredLocations(string formFactor)
    {
        return _locations.Where(l => l.FormFactor.Length == 0 || l.FormFactor.Equals(formFactor)).ToArray();
    }

    public IReadOnlyList<TakeoutLocation> Locations => _locations;

#endregion

#region Private

    [JsonInclude]
    [JsonPropertyName("locations")]
    private TakeoutLocation[] _locations = [];

#endregion
}