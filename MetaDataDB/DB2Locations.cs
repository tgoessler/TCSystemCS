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

using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2Locations : DB2Constants
{
#region Public

    public DB2Locations(DB2Instance instance)
    {
        _instance = instance;
    }

    public IList<string> GetFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            SetupGetFilesOfAddressCommand(command, address, useProvinceAlsoIfEmpty, false);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                var files = new List<string>();
                while (reader.Read())
                {
                    files.Add(reader.GetString(0));
                }

                return files;
            }
        }
    }

    public long GetNumLocations(SqliteTransaction transaction = null)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT COUNT({IdLocationId}) FROM {TableLocations};";
            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public IList<Address> GetAllLocationsLike(string filter)
    {
        const string filterLocationCommand = "WHERE " +
                                             IdCountry + " LIKE @Filter OR " +
                                             IdProvince + " LIKE @Filter OR " +
                                             IdCity + " LIKE @Filter OR " +
                                             IdStreet + " LIKE @Filter";

        string filterCommand = filter != null ? filterLocationCommand : "";
        if (filter != null)
        {
            filter = "%" + filter.Replace(' ', '%') + "% ";
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT DISTINCT {IdCountry}, {IdProvince}, {IdCity}, {IdStreet} " +
                                  $"FROM {TableLocations} {filterCommand} " +
                                  $"ORDER by {IdCountry} ASC, {IdProvince} ASC, {IdCity} ASC, {IdStreet} ASC;";
            command.Parameters.AddWithValue("@Filter", filter);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                var tags = new List<Address>();
                while (reader.Read())
                {
                    tags.Add(ReadAddress(reader, 0));
                }

                return tags;
            }
        }
    }

    public Location GetLocation(long fileId, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText =
                $"SELECT {TableLocations}.{IdCountry}, {TableLocations}.{IdProvince}, {TableLocations}.{IdCity}, {TableLocations}.{IdStreet}, " +
                $"{IdLatitude}, {IdLongitude}, {IdAltitude} " +
                $"FROM {TableFileLocations} " +
                $"    INNER JOIN {TableLocations} ON {TableLocations}.{IdLocationId} = {TableFileLocations}.{IdLocationId} " +
                $"WHERE {IdFileId}=@{IdFileId};";
            command.Parameters.AddWithValue($"@{IdFileId}", fileId);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                Location location = null;
                if (reader.Read())
                {
                    FixedPoint32? altitude = null;
                    if (reader.GetString(6).Length > 0)
                    {
                        altitude = new FixedPoint32(reader.GetInt32(6));
                    }

                    location = new(
                        ReadAddress(reader, 0),
                        new(
                            GpsPosition.FromString(reader.GetString(4)),
                            GpsPosition.FromString(reader.GetString(5)),
                            altitude)
                    );
                    if (!location.IsSet)
                    {
                        location = null;
                    }
                }

                return location;
            }
        }
    }

    public long AddLocation(Location location, SqliteTransaction transaction, bool force = false)
    {
        long locationId = location == null || location.Equals(Location.NoLocation) ?
            Constants.EmptyLocationId :
            GetLocationId(location.Address, transaction);

        if ((force || locationId == Constants.InvalidId) && location != null)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"INSERT INTO {TableLocations} " +
                                      $"( {IdCountry},  {IdProvince},  {IdCity},  {IdStreet}) " +
                                      "VALUES " +
                                      $"(@{IdCountry}, @{IdProvince}, @{IdCity}, @{IdStreet});";
                command.Parameters.AddWithValue($"@{IdCountry}", location.Address.Country);
                command.Parameters.AddWithValue($"@{IdProvince}", location.Address.Province);
                command.Parameters.AddWithValue($"@{IdCity}", location.Address.City);
                command.Parameters.AddWithValue($"@{IdStreet}", location.Address.Street);

                command.ExecuteNonQuery();
            }

            locationId = GetLocationId(location.Address, transaction);
        }

        return locationId;
    }

    public void SetLocation(long fileId, Location newLocation, Location oldLocation, SqliteTransaction transaction)
    {
        long newLocationId = AddLocation(newLocation, transaction);
        if (oldLocation == null)
        {
            AddLocation(fileId, newLocation, newLocationId, transaction);
        }
        else
        {
            UpdateLocation(fileId, newLocation, newLocationId, transaction);
        }
    }

    public long GetNumFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            SetupGetFilesOfAddressCommand(command, address, useProvinceAlsoIfEmpty, true);

            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public void RemoveAddress(Address address, SqliteTransaction transaction)
    {
        long id = GetLocationId(address, transaction);
        if (id != Constants.InvalidId)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"DELETE From {TableLocations} WHERE {IdLocationId}=@{IdLocationId};";
                command.Parameters.AddWithValue($"@{IdLocationId}", id);
                command.ExecuteNonQuery();
            }
        }
    }

#endregion

#region Private

    private void AddLocation(long fileId, Location newLocation, long newLocationId, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"INSERT INTO {TableFileLocations} " +
                                  $"( {IdFileId}, {IdLocationId}, {IdLatitude}, {IdLongitude}, {IdAltitude}) " +
                                  "VALUES " +
                                  $"(@{IdFileId}, @{IdLocationId}, @{IdLatitude}, @{IdLongitude}, @{IdAltitude});";
            AddLocationParameters(fileId, newLocation, command, newLocationId);

            command.ExecuteNonQuery();
        }
    }

    private void UpdateLocation(long fileId, Location newLocation, long newLocationId, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"UPDATE {TableFileLocations} " +
                                  $"SET {IdFileId}=@{IdFileId}," +
                                  $"    {IdLocationId}=@{IdLocationId}," +
                                  $"    {IdLatitude}=@{IdLatitude}," +
                                  $"    {IdLongitude}=@{IdLongitude} ," +
                                  $"    {IdAltitude}=@{IdAltitude} " +
                                  $"WHERE {IdFileId}=@{IdFileId};";
            AddLocationParameters(fileId, newLocation, command, newLocationId);

            command.ExecuteNonQuery();
        }
    }

    private static void AddLocationParameters(long fileId, Location location, SqliteCommand command, long locationId)
    {
        command.Parameters.AddWithValue($"@{IdFileId}", fileId);
        command.Parameters.AddWithValue($"@{IdLocationId}", locationId);
        command.Parameters.AddWithValue($"@{IdLatitude}", location?.Point.Latitude?.ToString() ?? "");
        command.Parameters.AddWithValue($"@{IdLongitude}", location?.Point.Longitude?.ToString() ?? "");
        string altitudeString = location?.Point.Altitude != null ?
            location.Point.Altitude.Value.RawValue.ToString(CultureInfo.InvariantCulture) :
            "";
        command.Parameters.AddWithValue($"@{IdAltitude}", altitudeString);
    }

    private static void SetupGetFilesOfAddressCommand(SqliteCommand command, Address address, bool useProvinceAlsoIfEmpty, bool countOnly)
    {
        bool isEmptyAddress = address.Equals(Address.Undefined);

        command.CommandText =
            (countOnly ?
                $"SELECT DISTINCT COUNT({TableFiles}.{IdFileName}) FROM {TableFileLocations} " :
                $"SELECT DISTINCT {TableFiles}.{IdFileName} FROM {TableFileLocations} ") +
            $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileLocations}.{IdFileId} " +
            $"    INNER JOIN {TableLocations} ON {TableLocations}.{IdLocationId}={TableFileLocations}.{IdLocationId} " +
            $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileLocations}.{IdFileId} " +
            "WHERE ";

        var whereCommands = new List<string>();
        if (address.Country.Length > 0 || isEmptyAddress)
        {
            whereCommands.Add($"{TableLocations}.{IdCountry}=@{IdCountry}");
            command.Parameters.AddWithValue($"@{IdCountry}", address.Country);
        }

        if (address.Province.Length > 0 || useProvinceAlsoIfEmpty || isEmptyAddress)
        {
            whereCommands.Add($"{TableLocations}.{IdProvince}=@{IdProvince}");
            command.Parameters.AddWithValue($"@{IdProvince}", address.Province);
        }

        if (address.City.Length > 0 || isEmptyAddress)
        {
            whereCommands.Add($"{TableLocations}.{IdCity}=@{IdCity}");
            command.Parameters.AddWithValue($"@{IdCity}", address.City);
        }

        if (address.Street.Length > 0 || isEmptyAddress)
        {
            whereCommands.Add($"{TableLocations}.{IdStreet}=@{IdStreet}");
            command.Parameters.AddWithValue($"@{IdStreet}", address.Street);
        }

        command.CommandText += string.Join(" AND ", whereCommands) +
                               (countOnly ? ";" : $" ORDER by {TableFileData}.{IdDateTaken} DESC;");
    }

    private long GetLocationId(Address address, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT {IdLocationId} FROM {TableLocations} WHERE " +
                                  $"{IdCountry}=@{IdCountry} AND {IdProvince}=@{IdProvince} AND {IdCity}=@{IdCity} AND {IdStreet}=@{IdStreet};";
            command.Parameters.AddWithValue($"@{IdCountry}", address.Country);
            command.Parameters.AddWithValue($"@{IdProvince}", address.Province);
            command.Parameters.AddWithValue($"@{IdCity}", address.City);
            command.Parameters.AddWithValue($"@{IdStreet}", address.Street);

            object result = command.ExecuteScalar();
            return (long?)result ?? Constants.InvalidId;
        }
    }

    private static Address ReadAddress(SqliteDataReader reader, int startIndex)
    {
        return new(
            reader.GetString(startIndex + 0),
            reader.GetString(startIndex + 1),
            reader.GetString(startIndex + 2),
            reader.GetString(startIndex + 3));
    }

    private readonly DB2Instance _instance;

#endregion
}