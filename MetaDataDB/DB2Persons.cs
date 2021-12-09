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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB
{
    internal sealed class DB2Persons : DB2Constants
    {
#region Public

        public long GetNumPersons(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"SELECT COUNT({IdPersonId}) FROM {TablePersons};"
            })
            {
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public IList<string> GetAllPersonNamesLike(string filter)
        {
            const string filterNameCommand = "WHERE " + IdName + " LIKE @Filter ";

            var filterCommand = filter != null ? filterNameCommand : "";
            if (filter != null)
            {
                filter = "%" + filter.Replace(' ', '%') + "%";
            }

            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT {IdName} FROM {TablePersons} {filterCommand} ORDER by {IdName} ASC;"
            })
            {
                command.Parameters.AddWithValue("@Filter", filter);

                using (var reader = command.ExecuteReader())
                {
                    var names = new List<string>();
                    while (reader.Read())
                    {
                        names.Add(reader.GetString(0));
                    }

                    return names;
                }
            }
        }

        public Person GetPersonFromName(string name, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                //                             0                  1             2              3                4
                CommandText = $"SELECT {IdPersonId}, {IdName}, {IdEmailDigest}, {IdLiveId}, {IdSourceId}, {IdPersonId} " +
                              $"FROM {TablePersons} WHERE {IdName}=@{IdName};"
            })
            {
                command.Parameters.AddWithValue($"@{IdName}", name);
                using (var reader = command.ExecuteReader())
                {
                    Person person = null;
                    if (reader.Read())
                    {
                        person = ReadPerson(0, reader);
                    }

                    return person;
                }
            }
        }

        public Person GetPersonFromId(long personId, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                //                             0                  1             2              3                4
                CommandText = $"SELECT {IdPersonId}, {IdName}, {IdEmailDigest}, {IdLiveId}, {IdSourceId}, {IdPersonId} " +
                              $"FROM {TablePersons} WHERE {IdPersonId}={personId};"
            })
            {
                using (var reader = command.ExecuteReader())
                {
                    Person person = null;
                    if (reader.Read())
                    {
                        person = ReadPerson(0, reader);
                    }

                    return person;
                }
            }
        }

        public IReadOnlyList<PersonTag> GetPersonTags(long fileId, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText =
                    $"SELECT {TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                    $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdFaceDescriptor} " +
                    $"FROM {TableFileFaces} " +
                    $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId} = {TableFileFaces}.{IdPersonId} " +
                    $"WHERE {IdFileId}=@{IdFileId};"
            })
            {
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                using (var reader = command.ExecuteReader())
                {
                    var personTags = new List<PersonTag>();
                    while (reader.Read())
                    {
                        personTags.Add(ReadPersonTag(0, reader));
                    }

                    return personTags;
                }
            }
        }

        public void AddPersonTags(long fileId, IReadOnlyList<PersonTag> personTags, SqliteTransaction transaction)
        {
            foreach (var pt in personTags)
            {
                var personId = AddPerson(pt.Person, transaction);
                AddFace(fileId, personId, pt.Face, transaction);
            }
        }

        public long AddPerson(Person person, SqliteTransaction transaction)
        {
            var existingPerson = GetPersonFromName(person.Name, transaction);
            if (existingPerson == null)
            {
                using (var command = new SqliteCommand
                {
                    Transaction = transaction,
                    Connection = Instance.Connection,
                    CommandText = $"INSERT INTO {TablePersons} " +
                                  $"( {IdName},  {IdEmailDigest},  {IdLiveId},  {IdSourceId}) " +
                                  "VALUES " +
                                  $"(@{IdName}, @{IdEmailDigest}, @{IdLiveId}, @{IdSourceId});"
                })
                {
                    command.Parameters.AddWithValue($"@{IdName}", person.Name);
                    command.Parameters.AddWithValue($"@{IdEmailDigest}", person.EmailDigest);
                    command.Parameters.AddWithValue($"@{IdLiveId}", person.LiveId);
                    command.Parameters.AddWithValue($"@{IdSourceId}", person.SourceId);
                    command.ExecuteNonQuery();
                }

                return GetPersonId(person.Name, transaction);
            }

            if (!existingPerson.AllAttributesDefined && !person.Equals(existingPerson))
            {
                var updatedPerson = new Person(existingPerson.Id,
                    existingPerson.Name,
                    string.IsNullOrEmpty(existingPerson.EmailDigest) ? person.EmailDigest : existingPerson.EmailDigest,
                    string.IsNullOrEmpty(existingPerson.LiveId) ? person.LiveId : existingPerson.LiveId,
                    string.IsNullOrEmpty(existingPerson.SourceId) ? person.SourceId : existingPerson.SourceId
                );
                if (!updatedPerson.Equals(existingPerson))
                {
                    UpdatePerson(updatedPerson, transaction);
                }
            }

            return existingPerson.Id;
        }

        public IList<string> GetFilesOfPerson(string person)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT {TableFiles}.{IdFileName} " +
                              $"FROM {TableFileFaces} " +
                              $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"WHERE {TablePersons}.{IdName}=@{IdName} " +
                              $"ORDER by {TableFileData}.{IdDateTaken} DESC;"
            })
            {
                command.Parameters.AddWithValue($"@{IdName}", person);
                using (var reader = command.ExecuteReader())
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

        public long GetNumFilesOfPerson(string person)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT COUNT({TableFiles}.{IdFileName}) " +
                              $"FROM {TableFileFaces} " +
                              $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"WHERE {TablePersons}.{IdName}=@{IdName} " +
                              $"ORDER by {TableFileData}.{IdDateTaken} DESC;"
            })
            {
                command.Parameters.AddWithValue($"@{IdName}", person);
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public long GetNumFaces()
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT COUNT({IdFileId}) FROM {TableFileFaces};"
            })
            {
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public long GetNumAutoDetectedFaces()
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT COUNT({IdFileId}) FROM {TableFileFaces} WHERE {IdFaceMode} != {(int) FaceMode.Undefined};"
            })
            {
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public long GetPersonId(string name, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"SELECT {IdPersonId} FROM {TablePersons} WHERE {IdName}=@{IdName};"
            })
            {
                command.Parameters.AddWithValue($"@{IdName}", name);
                var result = command.ExecuteScalar();
                return (long?) result ?? Constants.InvalidId;
            }
        }

        public IList<FileAndPersonTag> GetFileAndPersonTags(string name)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = "SELECT " +
                              $"{TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                              $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdFaceDescriptor}, " +
                              $"{TableFiles}.{IdFileName} " +
                              $"FROM {TableFileFaces} " +
                              $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"WHERE {TablePersons}.{IdName}=@{IdName} " +
                              $"ORDER by {TableFileData}.{IdDateTaken} DESC;"
            })
            {
                command.Parameters.AddWithValue($"@{IdName}", name);
                using (var reader = command.ExecuteReader())
                {
                    var fileAndPersonTags = new List<FileAndPersonTag>();
                    while (reader.HasRows && reader.Read())
                    {
                        fileAndPersonTags.Add(new FileAndPersonTag(reader.GetString(12),
                            ReadPersonTag(0, reader)));
                    }

                    return fileAndPersonTags;
                }
            }
        }

        public void RemovePerson(string person, SqliteTransaction transaction)
        {
            var id = GetPersonId(person, transaction);
            if (id != Constants.InvalidId)
            {
                using (var command = new SqliteCommand
                {
                    Transaction = transaction,
                    Connection = Instance.Connection,
                    CommandText = $"DELETE From {TablePersons} WHERE {IdPersonId}=@{IdPersonId};"
                })
                {
                    command.Parameters.AddWithValue($"@{IdPersonId}", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public DB2Instance Instance;
        public DB2Persons(DB2Instance instance)
        {
            Instance = instance;
        }

#endregion

#region Internal

        internal IList<FaceInfo> GetAllFaceInfos()
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT {TableFiles}.{IdFileId}, {IdFaceId}, {TablePersons}.{IdPersonId}, {IdFaceMode}, {IdFaceDescriptor} " +
                              $"FROM {TableFileFaces} " +
                              $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"WHERE {IdFaceDescriptor} != \"\";"
            })
            {
                using (var reader = command.ExecuteReader())
                {
                    var faceInfos = new List<FaceInfo>();
                    while (reader.HasRows && reader.Read())
                    {
                        faceInfos.Add(new FaceInfo(reader.GetInt64(0),
                            reader.GetInt64(1),
                            reader.GetInt64(2),
                            (FaceMode) reader.GetInt64(3),
                            ReadFaceDescriptor(4, reader)));
                    }

                    return faceInfos;
                }
            }
        }

        internal FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = "SELECT " +
                              $"{TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                              $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdFaceDescriptor}, " +
                              $"{TableFiles}.{IdFileName} " +
                              $"FROM {TableFileFaces} " +
                              $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                              $"WHERE {IdFaceId}={faceId};"
            })
            {
                using (var reader = command.ExecuteReader())
                {
                    FileAndPersonTag fileAndPersonTag = null;
                    if (reader.HasRows && reader.Read())
                    {
                        fileAndPersonTag = new FileAndPersonTag(reader.GetString(12),
                            ReadPersonTag(0, reader));
                    }

                    return fileAndPersonTag;
                }
            }
        }

#endregion

#region Private

        private static PersonTag ReadPersonTag(int start, SqliteDataReader reader)
        {
            return new(ReadPerson(start, reader), ReadFace(start + 5, reader));
        }

        private static Face ReadFace(int start, SqliteDataReader reader)
        {
            var faceId = reader.GetInt32(start + 0);
            var rect = Rectangle.FromRawValues(
                reader.GetInt32(start + 1), // X
                reader.GetInt32(start + 2), // Y
                reader.GetInt32(start + 3), // W
                reader.GetInt32(start + 4) // H
            );

            Face face;
            if (rect.W.RawValue > 0 && rect.H.RawValue > 0)
            {
                var faceMode = (FaceMode) reader.GetInt32(start + 5);
                var faceDescriptor = ReadFaceDescriptor(start + 6, reader);
                face = new Face(faceId, rect, faceMode, faceDescriptor);
            }
            else
            {
                face = new Face(faceId, new Rectangle(new FixedPoint32(0),
                        new FixedPoint32(0),
                        new FixedPoint32(0),
                        new FixedPoint32(0)),
                    FaceMode.Undefined,
                    null);
            }

            return face;
        }

        private static IEnumerable<FixedPoint64> ReadFaceDescriptor(int start, SqliteDataReader reader)
        {
            var faceDescriptorString = reader.GetString(start);
            var faceDescriptor = faceDescriptorString.Length > 0
                ? faceDescriptorString.Split(',').Select(s => new FixedPoint64(long.Parse(s, CultureInfo.InvariantCulture)))
                : null;
            return faceDescriptor;
        }

        private static Person ReadPerson(int start, SqliteDataReader reader)
        {
            var person = new Person(
                reader.GetInt32(start + 0), // ID_PERSON_ID
                reader.GetString(start + 1), // ID_NAME
                reader.GetString(start + 2), // ID_EMAIL_DIGEST
                reader.GetString(start + 3), // ID_LIVE_ID
                reader.GetString(start + 4) // ID_SOURCE_ID
            );
            return person;
        }

        private void AddFace(long fileId, long personId, Face face, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"INSERT INTO {TableFileFaces} " + "" +
                              $"( {IdFileId},  {IdPersonId},  {IdRectangleX},  {IdRectangleY},  {IdRectangleW},  {IdRectangleH},  {IdFaceMode},  {IdFaceDescriptor})" +
                              "VALUES " +
                              $"(@{IdFileId}, @{IdPersonId}, @{IdRectangleX}, @{IdRectangleY}, @{IdRectangleW}, @{IdRectangleH}, @{IdFaceMode}, @{IdFaceDescriptor});"
            })
            {
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                command.Parameters.AddWithValue($"@{IdPersonId}", personId);
                command.Parameters.AddWithValue($"@{IdRectangleX}", face.Rectangle.X.RawValue);
                command.Parameters.AddWithValue($"@{IdRectangleY}", face.Rectangle.Y.RawValue);
                command.Parameters.AddWithValue($"@{IdRectangleW}", face.Rectangle.W.RawValue);
                command.Parameters.AddWithValue($"@{IdRectangleH}", face.Rectangle.H.RawValue);
                command.Parameters.AddWithValue($"@{IdFaceMode}", (int) face.FaceMode);
                var faceDescriptorStringValues = face.HasFaceDescriptor ? face.FaceDescriptor.Select(v => v.RawValue.ToString(CultureInfo.InvariantCulture)) : null;
                var faceDescriptorString = faceDescriptorStringValues != null ? string.Join(",", faceDescriptorStringValues) : "";
                command.Parameters.AddWithValue($"@{IdFaceDescriptor}", faceDescriptorString);
                command.ExecuteNonQuery();
            }
        }

        private void UpdatePerson(Person person, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"UPDATE {TablePersons} " +
                              $"SET {IdName}=@{IdName}," +
                              $"    {IdEmailDigest}=@{IdEmailDigest}," +
                              $"    {IdLiveId}=@{IdLiveId}," +
                              $"    {IdSourceId}=@{IdSourceId} " +
                              $"WHERE {IdPersonId}=@{IdPersonId};"
            })
            {
                command.Parameters.AddWithValue($"@{IdPersonId}", person.Id);
                command.Parameters.AddWithValue($"@{IdName}", person.Name);
                command.Parameters.AddWithValue($"@{IdEmailDigest}", person.EmailDigest);
                command.Parameters.AddWithValue($"@{IdLiveId}", person.LiveId);
                command.Parameters.AddWithValue($"@{IdSourceId}", person.SourceId);
                command.ExecuteNonQuery();
            }
        }

#endregion
    }
}