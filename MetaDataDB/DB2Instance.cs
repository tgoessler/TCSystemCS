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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

// ReSharper disable InconsistentlySynchronizedField

[assembly: InternalsVisibleTo("TCPhotoGalleryConvertDB")]

namespace TCSystem.MetaDataDB
{
    internal sealed class DB2Instance : DB2Constants
    {
#region Internal

        internal void Open(string fileName)
        {
            Log.Instance.Info($"Open Database '{fileName}'");

            _fileName = fileName;
            if (!OpenDataBaseFile())
            {
                Log.Instance.Fatal($"Open Database {fileName}' failed");
                throw new InvalidProgramException($"Open Database '{fileName}' failed");
            }

            CreateTableIndex();
            ReadDbVersion();

            using (var transaction = WriteDbVersion())
            {
                CreateTableFiles(transaction);
                CreateTablePersons(transaction);
                CreateTableTags(transaction);
                CreateTableLocation(transaction);

                CreateTableFileData(transaction);
                CreateTableFileFaces(transaction);
                CreateTableFileTags(transaction);
                CreateTableFileLocation(transaction);

                transaction.Commit();
            }

            Log.Instance.Info($"Open Database '{fileName}' done.");
        }

        internal void Close()
        {
            CloseDataBaseFile();
        }

        internal SqliteTransaction BeginTransaction()
        {
            _numTransactions++;
            if (_numTransactions % 100 == 99 && !_unsafeModeEnabled)
            {
                Log.Instance.Info("Reopening Database");
                CloseDataBaseFile();
                if (OpenDataBaseFile())
                {
                    Log.Instance.Info("Reopening Database done");
                }
                else
                {
                    Log.Instance.Fatal("Reopening Database failed");
                }
            }

            return Connection.BeginTransaction();
        }

        internal void EnableUnsafeMode()
        {
            if (!_unsafeModeEnabled)
            {
                _unsafeModeEnabled = true;
                CloseDataBaseFile();
                OpenDataBaseFile();
            }
        }

        internal void EnableDefaultMode()
        {
            if (_unsafeModeEnabled)
            {
                _unsafeModeEnabled = false;
                CloseDataBaseFile();
                OpenDataBaseFile();
                VacuumDb();
            }
        }

        internal SqliteConnection Connection { get; private set; }

#endregion

#region Private

        private bool OpenDataBaseFile()
        {
            Connection = new SqliteConnection($"Filename={_fileName}");
            Connection.Open();

            if (_unsafeModeEnabled)
            {
                using (var command = new SqliteCommand
                {
                    Connection = Connection
                })
                {
                    command.CommandText = "PRAGMA synchronous = OFF;";
                    command.ExecuteNonQuery();

                    command.CommandText = "PRAGMA journal_mode = OFF;";
                    command.ExecuteNonQuery();
                }
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection
            })
            {
                const int size = 1024 * 1024 * 100;
                command.CommandText = $"PRAGMA cache_size = {size};";
                command.ExecuteNonQuery();

                command.CommandText = "PRAGMA temp_store = MEMORY;";
                command.ExecuteNonQuery();

                command.CommandText = "PRAGMA locking_mode = EXCLUSIVE;";
                command.ExecuteNonQuery();
            }

            return Connection != null;
        }

        private void CloseDataBaseFile()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }

        private void CreateTableFileTags(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand

            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileTags}" +
                              "( " +
                              $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                              $"{IdTagId} INTEGER NOT NULL " +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_tags ON {TableFileTags}({IdFileId});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTableFileLocation(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand

            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileLocations}" +
                              "( " +
                              $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                              $"{IdLocationId} INTEGER NOT NULL, " +
                              $"{IdLatitude} TEXT, " +
                              $"{IdLongitude} TEXT, " +
                              $"{IdAltitude} TEXT " +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_locations ON {TableFileLocations}({IdFileId});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void ReadDbVersion()
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                CommandText = $"SELECT {IdValue} FROM {TableKeyValues} WHERE {IdKey}='Version';"
            })
            {
                _dbVersion = null;
                try
                {
                    _dbVersion = command.ExecuteScalar() as string;
                }
                catch (Exception)
                {
                    // no version until now written
                }
            }
        }

        private void CreateTableIndex()
        {
            using (var transaction = BeginTransaction())
            {
                using (var command = new SqliteCommand
                {
                    Connection = Connection,
                    Transaction = transaction,
                    CommandText = $"CREATE TABLE IF NOT EXISTS {TableKeyValues}({IdKey} TEXT NOT NULL UNIQUE, {IdValue} TEXT NOT NULL);"
                })
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private void CreateTableFiles(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableFiles}" +
                              "( " +
                              $"{IdFileId} INTEGER PRIMARY KEY, " +
                              $"{IdFileName} TEXT NOT NULL UNIQUE, " +
                              $"{IdDateModified} TEXT, " +
                              $"{IdProcessingInfo} INTEGER" +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_filename_to_fileid ON {TableFiles} ({IdFileName});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTableFileData(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileData} " +
                              "( " +
                              $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                              $"{IdWidth} INTEGER, " +
                              $"{IdHeight} INTEGER, " +
                              $"{IdOrientation} INTEGER, " +
                              $"{IdDateTaken} TEXT" +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_fileid_to_data ON {TableFileData}({IdFileId});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTablePersons(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TablePersons}" +
                              "( " +
                              $"{IdPersonId} INTEGER PRIMARY KEY, " +
                              $"{IdName} TEXT NOT NULL UNIQUE, " +
                              $"{IdEmailDigest} TEXT, " +
                              $"{IdLiveId} TEXT, " +
                              $"{IdSourceId} TEXT " +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS index_name_to_person_id ON " + TablePersons + "(Name);"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTableFileFaces(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableFileFaces}" +
                              "( " +
                              $"{IdFaceId} INTEGER PRIMARY KEY, " +
                              $"{IdFileId} REFERENCES {TableFiles}({IdFileId}) ON DELETE CASCADE, " +
                              $"{IdPersonId} INTEGER, " +
                              $"{IdRectangleX} INTEGER, " +
                              $"{IdRectangleY} INTEGER, " +
                              $"{IdRectangleW} INTEGER, " +
                              $"{IdRectangleH} INTEGER, " +
                              $"{IdFaceMode} INTEGER DEFAULT {(int) FaceMode.Undefined}, " +
                              $"{IdFaceDescriptor} BLOB " +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE INDEX IF NOT EXISTS index_fileid_to_faces ON  {TableFileFaces}({IdFileId});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTableTags(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand

            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableTags}" +
                              "(" +
                              $"{IdTagId} INTEGER PRIMARY KEY, " +
                              $"{IdTag} TEXT" +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS index_tag_to_tag_id ON {TableTags}({IdTag});"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateTableLocation(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = $"CREATE TABLE IF NOT EXISTS {TableLocations} " +
                              "( " +
                              $"{IdLocationId} INTEGER PRIMARY KEY, " +
                              $"{IdCountry} TEXT, " +
                              $"{IdProvince} TEXT, " +
                              $"{IdCity} TEXT, " +
                              $"{IdStreet} TEXT " +
                              ");"
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void VacuumDb()
        {
            Log.Instance.Info("Optimizing database");
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                CommandText = "VACUUM;"
            })
            {
                command.ExecuteNonQuery();
            }

            Log.Instance.Info("Optimizing database done");
        }

        private SqliteTransaction WriteDbVersion()
        {
            SqliteTransaction transaction;

            if (_dbVersion == null)
            {
                transaction = BeginTransaction();

                ExecuteNonQuery($"INSERT INTO {TableKeyValues}({IdKey}, {IdValue}) VALUES('Version', '{Version}');",
                    transaction);
                _dbVersion = Version;
            }
            else if (_dbVersion == Version10)
            {
                transaction = BeginTransaction();

                ExecuteNonQuery($"ALTER TABLE {TableFileFaces} ADD {IdFaceMode} INTEGER DEFAULT {(int) FaceMode.Undefined};",
                    transaction);
                ExecuteNonQuery($"UPDATE {TableFiles} SET {IdProcessingInfo}={(long) ProcessingInfo.None};",
                    transaction);

                UpdateDbVersion(transaction);
            }
            else if (_dbVersion != Version)
            {
                CloseDataBaseFile();
                OpenDataBaseFile();

                transaction = BeginTransaction();

                foreach (var tableName in _dataTableNames)
                {
                    ExecuteNonQuery($"DROP TABLE IF EXISTS {tableName};",
                        transaction);
                }

                UpdateDbVersion(transaction);
            }
            else
            {
                transaction = BeginTransaction();
            }

            return transaction;
        }

        private void UpdateDbVersion(SqliteTransaction transaction)
        {
            _dbVersion = Version;
            ExecuteNonQuery($"UPDATE {TableKeyValues} SET {IdValue}='{Version}' WHERE {IdKey}='Version';",
                transaction);
        }

        private void ExecuteNonQuery(string commandText, SqliteTransaction transaction = null)
        {
            using (var command = new SqliteCommand
            {
                Connection = Connection,
                Transaction = transaction,
                CommandText = commandText
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private bool _unsafeModeEnabled;

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

        private string _dbVersion;
        private int _numTransactions;
        private string _fileName = "MetaData2.db";

#endregion
    }
}