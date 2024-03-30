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
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2Instance : DB2Constants
{
#region Public

    public DB2Instance(string fileName, bool readOnly)
        : this(fileName, readOnly, false)
    {
        try
        {
            Log.Instance.Info($"Open Database '{fileName}:readOnly={readOnly}'");

            _fileName = fileName;
            ReadOnly = readOnly;
            CreateConnection();

            if (readOnly)
            {
                ReadDbVersion();
            }
            else
            {
                CreateTableIndex();
                ReadDbVersion();

                using (SqliteTransaction transaction = WriteDbVersion())
                {
                    CreateTableFiles(transaction);
                    CreateTablePersons(transaction);
                    CreateTableTags(transaction);
                    CreateTableLocation(transaction);

                    CreateTableFileData(transaction);
                    CreateTableFileFaces(transaction);
                    CreateTableFileTags(transaction);
                    CreateTableFileLocation(transaction);

                    CreateTableNotThisPerson(transaction);

                    transaction.Commit();
                }
            }

            Log.Instance.Info($"Open Database '{fileName}:readOnly={readOnly}' done.");
        }
        catch (Exception e)
        {
            Log.Instance.Fatal($"Failed open DB {fileName}:readOnly={readOnly}", e);
            throw;
        }
    }

    public void Close()
    {
        DestroyConnection();
    }

    public DB2Instance Clone()
    {
        return new(_fileName, ReadOnly, Version, UnsafeModeEnabled);
    }

    public SqliteTransaction BeginTransaction()
    {
        _numTransactions++;
        if (_numTransactions % 100 == 99 && !UnsafeModeEnabled)
        {
            Log.Instance.Info("Reopening Database");
            DestroyConnection();
            CreateConnection();
        }

        return Connection.BeginTransaction();
    }

    public void EnableUnsafeMode()
    {
        if (!UnsafeModeEnabled)
        {
            UnsafeModeEnabled = true;
            DestroyConnection();
            CreateConnection();
        }
    }

    public void EnableDefaultMode()
    {
        if (UnsafeModeEnabled)
        {
            UnsafeModeEnabled = false;
            DestroyConnection();
            CreateConnection();
            if (!ReadOnly)
            {
                VacuumDb();
            }
        }
    }

    public SqliteConnection Connection { get; private set; }
    public string Version { get; private set; }
    public bool ReadOnly { get; }
    public bool UnsafeModeEnabled { get; private set; }

    public DB2Data Data { get; }
    public DB2Files Files { get; }
    public DB2Locations Locations { get; }
    public DB2Persons Persons { get; }
    public DB2Tags Tags { get; }
    public DB2NotThisPerson NotThisPerson { get; }

#endregion

#region Private

    private DB2Instance(string fileName, bool readOnly, bool unsafeModeEnabled)
    {
        _fileName = fileName;
        UnsafeModeEnabled = unsafeModeEnabled;
        ReadOnly = readOnly;

        Data = new(this);
        Files = new(this);
        Locations = new(this);
        Persons = new(this);
        Tags = new(this);
        NotThisPerson = new(this);
    }

    private DB2Instance(string fileName, bool readOnly, string version, bool unsafeModeEnabled)
        : this(fileName, readOnly, unsafeModeEnabled)
    {
        Version = version;

        CreateConnection();
    }

    private void CreateConnection()
    {
        try
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = _fileName,
                Mode = ReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWriteCreate
            };
            Log.Instance.Info($"Creating connection {builder.ConnectionString}");

            Connection = new(builder.ConnectionString);
            Connection.Open();

            ApplyDbSetting();
        }
        catch (Exception e)
        {
            Log.Instance.Fatal($"Creating connection {_fileName}' failed", e);
            Connection = null;
        }

        if (Connection == null)
        {
            Log.Instance.Fatal($"Creating connection {_fileName}' failed");
            throw new InvalidProgramException($"Open connection '{_fileName}' failed");
        }

        Log.Instance.Info("Creating connection done");
    }

    private void ApplyDbSetting()
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            if (UnsafeModeEnabled)
            {
                command.CommandText += "PRAGMA synchronous = OFF;";
            }
            else
            {
                command.CommandText += "PRAGMA synchronous = NORMAL;";
            }

            const int size = 1024 * 1024 * 100;
            command.CommandText += $"PRAGMA cache_size = {size};";
            command.CommandText += "PRAGMA temp_store = MEMORY;";
            command.CommandText += "PRAGMA locking_mode = NORMAL;";

            if (!ReadOnly)
            {
                command.CommandText += "PRAGMA journal_mode = WAL;";
            }

            command.ExecuteNonQuery();
        }
    }

    private void DestroyConnection()
    {
        if (Connection != null)
        {
            Log.Instance.Info("Destroying connection");

            SqliteConnection.ClearPool(Connection);
            Connection.Close();
            Connection.Dispose();
            Connection = null;
            Log.Instance.Info("Destroying connection done");
        }
    }

    private void CreateTableFileTags(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileTags}" +
                                  "( " +
                                  $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                                  $"{IdTagId} INTEGER NOT NULL REFERENCES {TableTags}({IdTagId}) ON DELETE CASCADE" +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_tags ON {TableFileTags}({IdFileId});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableFileLocation(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileLocations}" +
                                  "( " +
                                  $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                                  $"{IdLocationId} INTEGER NOT NULL, " +
                                  $"{IdLatitude} TEXT, " +
                                  $"{IdLongitude} TEXT, " +
                                  $"{IdAltitude} TEXT " +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_locations ON {TableFileLocations}({IdFileId});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableNotThisPerson(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableNotThisPerson} " +
                                  "( " +
                                  $"{IdFaceId} REFERENCES {TableFileFaces}({IdFaceId}) ON DELETE CASCADE, " +
                                  $"{IdPersonId} REFERENCES {TablePersons}({IdPersonId}) ON DELETE CASCADE" +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_tags ON {TableFileTags}({IdFileId});";
            command.ExecuteNonQuery();
        }
    }

    private void ReadDbVersion()
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.CommandText = $"SELECT {IdValue} FROM {TableKeyValues} WHERE {IdKey}='Version';";
            Version = null;
            try
            {
                Version = command.ExecuteScalar() as string;
            }
            catch (Exception)
            {
                // no version until now written
            }
        }
    }

    private void CreateTableIndex()
    {
        using (SqliteTransaction transaction = BeginTransaction())
        {
            using (var command = new SqliteCommand())
            {
                command.Connection = Connection;
                command.Transaction = transaction;
                command.CommandText =
                    $"CREATE TABLE IF NOT EXISTS {TableKeyValues}({IdKey} TEXT NOT NULL UNIQUE, {IdValue} TEXT NOT NULL);";
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    private void CreateTableFiles(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableFiles}" +
                                  "( " +
                                  $"{IdFileId} INTEGER PRIMARY KEY, " +
                                  $"{IdFileName} TEXT NOT NULL UNIQUE, " +
                                  $"{IdDateModified} TEXT, " +
                                  $"{IdProcessingInfo} INTEGER" +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_filename_to_fileid ON {TableFiles} ({IdFileName});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableFileData(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileData} " +
                                  "( " +
                                  $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                                  $"{IdWidth} INTEGER, " +
                                  $"{IdHeight} INTEGER, " +
                                  $"{IdOrientation} INTEGER, " +
                                  $"{IdDateTaken} TEXT" +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_fileid_to_data ON {TableFileData}({IdFileId});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTablePersons(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TablePersons}" +
                                  "( " +
                                  $"{IdPersonId} INTEGER PRIMARY KEY, " +
                                  $"{IdName} TEXT NOT NULL UNIQUE, " +
                                  $"{IdEmailDigest} TEXT, " +
                                  $"{IdLiveId} TEXT, " +
                                  $"{IdSourceId} TEXT " +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS index_name_to_person_id ON " + TablePersons + "(Name);";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableFileFaces(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileFaces}" +
                                  "( " +
                                  $"{IdFaceId} INTEGER PRIMARY KEY, " +
                                  $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                                  $"{IdPersonId} INTEGER, " +
                                  $"{IdRectangleX} INTEGER, " +
                                  $"{IdRectangleY} INTEGER, " +
                                  $"{IdRectangleW} INTEGER, " +
                                  $"{IdRectangleH} INTEGER, " +
                                  $"{IdFaceMode} INTEGER DEFAULT {(int)FaceMode.Undefined}, " +
                                  $"{IdVisible} INTEGER DEFAULT 1, " +
                                  $"{IdFaceDescriptor} BLOB " +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_faces ON  {TableFileFaces}({IdFileId});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableTags(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableTags}" +
                                  "(" +
                                  $"{IdTagId} INTEGER PRIMARY KEY, " +
                                  $"{IdTag} TEXT" +
                                  ");";
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_tag_to_tag_id ON {TableTags}({IdTag});";
            command.ExecuteNonQuery();
        }
    }

    private void CreateTableLocation(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableLocations} " +
                                  "( " +
                                  $"{IdLocationId} INTEGER PRIMARY KEY, " +
                                  $"{IdCountry} TEXT, " +
                                  $"{IdProvince} TEXT, " +
                                  $"{IdCity} TEXT, " +
                                  $"{IdStreet} TEXT " +
                                  ");";
            command.ExecuteNonQuery();
        }
    }

    private void VacuumDb()
    {
        Log.Instance.Info("Optimizing database");
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.CommandText = "VACUUM;";
            command.ExecuteNonQuery();
        }

        Log.Instance.Info("Optimizing database done");
    }

    private SqliteTransaction WriteDbVersion()
    {
        SqliteTransaction transaction = null;

        if (Version == null)
        {
            transaction = BeginTransaction();

            ExecuteNonQuery($"INSERT INTO {TableKeyValues}({IdKey}, {IdValue}) VALUES('Version', '{CurrentVersion}');", transaction);
            Version = CurrentVersion;
        }

        if (Version == Version10)
        {
            transaction ??= BeginTransaction();

            ExecuteNonQuery($"ALTER TABLE {TableFileFaces} ADD {IdFaceMode} INTEGER DEFAULT {(int)FaceMode.Undefined};", transaction);
            ExecuteNonQuery($"UPDATE {TableFiles} SET {IdProcessingInfo}={(long)ProcessingInfos.None};", transaction);

            UpdateDbVersion(transaction, Version11);
        }

        if (Version == Version11)
        {
            transaction ??= BeginTransaction();

            ExecuteNonQuery($"ALTER TABLE {TableFileFaces} ADD {IdVisible} INTEGER DEFAULT 1;", transaction);

            UpdateDbVersion(transaction, Version12);
        }

        if (Version != CurrentVersion)
        {
            transaction?.Dispose();

            DestroyConnection();
            CreateConnection();

            transaction = BeginTransaction();

            foreach (string tableName in _dataTableNames)
            {
                ExecuteNonQuery($"DROP TABLE IF EXISTS {tableName};", transaction);
            }

            UpdateDbVersion(transaction, CurrentVersion);
        }

        return transaction ?? BeginTransaction();
    }

    private void UpdateDbVersion(SqliteTransaction transaction, string version)
    {
        Version = version;
        ExecuteNonQuery($"UPDATE {TableKeyValues} SET {IdValue}='{Version}' WHERE {IdKey}='Version';", transaction);
    }

    private void ExecuteNonQuery(string commandText, SqliteTransaction transaction = null)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = Connection;
            command.Transaction = transaction;
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }

    private readonly IReadOnlyList<string> _dataTableNames = new[]
    {
        TableFiles,
        TablePersons,
        TableTags,
        TableLocations,
        TableFileData,
        TableFileFaces,
        TableFileTags,
        TableFileLocations
    };

    private int _numTransactions;
    private readonly string _fileName;

#endregion
}