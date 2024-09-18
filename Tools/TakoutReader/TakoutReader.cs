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
using System.IO;
using System.Threading.Tasks;
using TCSystem.Gps;
using TCSystem.MetaData;
using TCSystem.MetaDataDB;
using Factory = TCSystem.Logging.Factory;

#endregion

namespace TCSystem.Tools.TakeoutReader;

public static class Reader
{
#region Private

    private static async Task Main(string[] args)
    {
        try
        {
            Factory.InitLogging(Factory.LoggingOptions.File, "takeout.log", maxFileSizeKb:10*1024);

            await using Stream stream = File.OpenRead(args[0]);

            Log.Instance.Info($"Reading file {args[0]}");
            TakeoutRecords records = await TakeoutRecordReader.ReadAsync(stream);
            TakeoutLocation[] takeOutLocations = records.GetFilteredLocations(TakeoutRecords.FormFactorPhone);

            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            var numLocationsFound = 0;
            IDB2Read db = MetaDataDB.Factory.CreateRead(args[1]);
            IList<string> files = db.GetAllFilesLike();
            foreach (string file in files)
            {
                Image metaData = db.GetMetaData(file);
                if (metaData.Location is not { IsSet: true })
                {
                    DateTime dateTime = metaData.DateTaken.ToUniversalTime().UtcDateTime;
                    if (Path.GetFileName(file).StartsWith("SON", StringComparison.Ordinal) &&
                        localTimeZone.IsDaylightSavingTime(metaData.DateTaken))
                    {
                        dateTime = dateTime.AddHours(1);
                    }

                    TakeoutLocation takeoutLocation = TakeoutRecords.FindNearestLocation(takeOutLocations, dateTime);
                    GpsPoint gps = takeoutLocation.GpsPoint;

                    double diff = Math.Abs((dateTime - takeoutLocation.Timestamp).TotalHours);
                    if (diff < 1)
                    {
                        Log.Instance.Info($"file = {file}");
                        Log.Instance.Info($"DateTaken = {dateTime}");
                        Log.Instance.Info($"TakeoutTimestamp = {takeoutLocation.Timestamp}");
                        Log.Instance.Info($"difference = {(int)(diff * 60.0)} min");
                        Log.Instance.Info($"takeoutLocation = {takeoutLocation}");
                        if (gps.Latitude != null && gps.Longitude != null)
                        {
                            string lat = $"{gps.Latitude.Value.ToDouble():##.######}".Replace(',', '.');
                            string lon = $"{gps.Longitude.Value.ToDouble():##.######}".Replace(',', '.');
                            Log.Instance.Info($"location = {lat}, {lon}");
                        }

                        numLocationsFound++;
                    }
                }
            }

            Log.Instance.Info($"numLocationsFound = {numLocationsFound}");
        }
        catch (Exception e)
        {
            Log.Instance.Fatal("Received exception", e);
        }
        finally
        {
            Factory.DeInitLogging();
        }
    }

#endregion
}