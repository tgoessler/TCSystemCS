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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB
{
    internal sealed class DB2Data : DB2Constants
    {
#region Public

        public DB2Data(DB2Instance instance)
        {
            _instance = instance;
        }

        public Image GetMetaData(long fileId, Location location, IReadOnlyList<PersonTag> personTags,
                                 IReadOnlyList<string> tags, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = _instance.Connection,
                CommandText = $"SELECT {TableFiles}.{IdFileName}, {TableFiles}.{IdProcessingInfo}, {IdWidth}, {IdHeight}, {IdOrientation}, {IdDateTaken} " +
                              $"FROM {TableFileData} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileData}.{IdFileId} " +
                              $"WHERE {TableFileData}.{IdFileId}=@{IdFileId};"
            })
            {
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    Image image = null;
                    if (reader.HasRows && reader.Read())
                    {
                        image = new Image
                        (
                            fileId,
                            reader.GetString(0),
                            (ProcessingInfos)reader.GetInt64(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3),
                            (OrientationMode)reader.GetInt32(4),
                            reader.GetDateTimeOffset(5),
                            null,
                            location,
                            personTags,
                            tags
                        );
                    }

                    return image;
                }
            }
        }

        public void AddMetaData(long fileId, Image data, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = _instance.Connection,
                CommandText = $"INSERT INTO {TableFileData} VALUES " +
                              "(" +
                              $"@{IdFileId}, " +
                              $"@{IdWidth}, " +
                              $"@{IdHeight}, " +
                              $"@{IdOrientation}, " +
                              $"@{IdDateTaken}" +
                              ");"
            })
            {
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                command.Parameters.AddWithValue($"@{IdWidth}", data.Width);
                command.Parameters.AddWithValue($"@{IdHeight}", data.Height);
                command.Parameters.AddWithValue($"@{IdOrientation}", (int)data.Orientation);
                command.Parameters.AddWithValue($"@{IdDateTaken}", data.DateTaken.ToString("s"));
                command.ExecuteNonQuery();
            }
        }

        public IList<DateTimeOffset> GetAllYears()
        {
            using (var command = new SqliteCommand
            {
                Connection = _instance.Connection,
                CommandText = $"SELECT DISTINCT {IdDateTaken} FROM {TableFileData} ORDER by {IdDateTaken} DESC;"
            })
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var years = new List<DateTimeOffset>();
                    while (reader.Read())
                    {
                        DateTimeOffset year = reader.GetDateTimeOffset(0);
                        if (year != Image.InvalidDateTaken)
                        {
                            year = new DateTimeOffset(new DateTime(year.Year, 1, 1));
                        }

                        if (!years.Contains(year))
                        {
                            years.Add(year);
                        }
                    }

                    return years;
                }
            }
        }

        public IList<string> GetFilesOfYear(DateTimeOffset year)
        {
            using (var command = new SqliteCommand
            {
                Connection = _instance.Connection
            })
            {
                SetupGetFilesOfYearCommand(command, year, false);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var fileNames = new List<string>();
                    while (reader.Read())
                    {
                        fileNames.Add(reader.GetString(0));
                    }

                    return fileNames;
                }
            }
        }

        public OrientationMode GetOrientation(string fileName)
        {
            using (var command = new SqliteCommand
            {
                Connection = _instance.Connection,
                CommandText = $"SELECT {IdOrientation} FROM {TableFileData} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileData}.{IdFileId} " +
                              $"WHERE {TableFiles}.{IdFileName}=@{IdFileName};"
            })
            {
                command.Parameters.AddWithValue($"{IdFileName}", fileName);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        return (OrientationMode)reader.GetInt32(0);
                    }

                    return OrientationMode.Normal;
                }
            }
        }

        public long GetNumFilesOfYear(DateTimeOffset year)
        {
            using (var command = new SqliteCommand
            {
                Connection = _instance.Connection
            })
            {
                SetupGetFilesOfYearCommand(command, year, true);

                object result = command.ExecuteScalar();
                return (long?)result ?? 0;
            }
        }

#endregion

#region Private

        private static void SetupGetFilesOfYearCommand(SqliteCommand command, DateTimeOffset year, bool countOnly)
        {
            string filter = year == Image.InvalidDateTaken
                ? $"WHERE {TableFileData}.{IdDateTaken}=@Date "
                : $"WHERE {TableFileData}.{IdDateTaken} BETWEEN @StartDate AND @EndDate ";

            command.CommandText = countOnly ? $"SELECT DISTINCT COUNT({IdFileName}) " : $"SELECT DISTINCT {IdFileName} ";
            command.CommandText += $"FROM {TableFiles} " +
                                   $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFiles}.{IdFileId} " +
                                   filter;
            command.CommandText += countOnly ? ";" : $"ORDER by {TableFileData}.{IdDateTaken} DESC;";

            if (year == Image.InvalidDateTaken)
            {
                command.Parameters.AddWithValue("@Date", year.ToString("s"));
            }
            else
            {
                var startDate = new DateTimeOffset(new DateTime(year.Year, 1, 1, 0, 0, 0));
                var endDate = new DateTimeOffset(new DateTime(year.Year, 12, 31, 23, 59, 59));
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
            }
        }

        private readonly DB2Instance _instance;

#endregion
    }
}