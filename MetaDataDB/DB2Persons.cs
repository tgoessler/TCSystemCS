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
using System.Linq;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2Persons : DB2Constants
{
#region Public

    public DB2Persons(DB2Instance instance)
    {
        _instance = instance;
    }

    public long GetNumPersons(SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT COUNT({IdPersonId}) FROM {TablePersons};";
            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public IList<string> GetAllPersonNamesLike(string filter)
    {
        const string filterNameCommand = "WHERE " + IdName + " LIKE @Filter ";

        string filterCommand = filter != null ? filterNameCommand : "";
        if (filter != null)
        {
            filter = "%" + filter.Replace(' ', '%') + "%";
        }

        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT DISTINCT {IdName} FROM {TablePersons} {filterCommand} ORDER by {IdName} ASC;";
            command.Parameters.AddWithValue("@Filter", filter);

            using (SqliteDataReader reader = command.ExecuteReader())
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
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection =
                _instance.Connection; //                             0                  1             2              3                4
            command.CommandText = $"SELECT {IdPersonId}, {IdName}, {IdEmailDigest}, {IdLiveId}, {IdSourceId}, {IdPersonId} " +
                                  $"FROM {TablePersons} WHERE {IdName}=@{IdName};";
            command.Parameters.AddWithValue($"@{IdName}", name);
            using (SqliteDataReader reader = command.ExecuteReader())
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
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection =
                _instance.Connection; //                             0                  1             2              3                4
            command.CommandText = $"SELECT {IdPersonId}, {IdName}, {IdEmailDigest}, {IdLiveId}, {IdSourceId}, {IdPersonId} " +
                                  $"FROM {TablePersons} WHERE {IdPersonId}={personId};";
            using (SqliteDataReader reader = command.ExecuteReader())
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

    public IReadOnlyList<PersonTag> GetPersonTags(long fileId, bool visibleOnly, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText =
                $"SELECT {TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdVisible}, {IdFaceDescriptor} " +
                $"FROM {TableFileFaces} " +
                $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId} = {TableFileFaces}.{IdPersonId} " +
                $"WHERE {IdFileId}=@{IdFileId}{AddVisibleWhere(visibleOnly)};";
            command.Parameters.AddWithValue($"@{IdFileId}", fileId);
            command.Parameters.AddWithValue($"@{IdVisible}", visibleOnly ? 1 : 0);
            using (SqliteDataReader reader = command.ExecuteReader())
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

    public void SetPersonTags(long fileId, IReadOnlyList<PersonTag> newPersonTags, IReadOnlyList<PersonTag> oldPersonTags,
                              SqliteTransaction transaction)
    {
        // update or add faces
        foreach (PersonTag pt in newPersonTags)
        {
            long personId = AddPerson(pt.Person, transaction);
            if (oldPersonTags.GetFace(pt.Face.Id) == null)
            {
                AddFace(fileId, personId, pt.Face, transaction);
            }
            else
            {
                UpdateFace(fileId, personId, pt.Face, transaction);
            }
        }

        // remove deleted faces
        foreach (long id in oldPersonTags
                     .Select(pt => pt.Face.Id)
                     .Where(id => newPersonTags.GetFace(id) == null))
        {
            RemoveFace(id, transaction);
        }
    }

    public long AddPerson(Person person, SqliteTransaction transaction)
    {
        Person existingPerson = GetPersonFromName(person.Name, transaction);
        if (existingPerson == null)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"INSERT INTO {TablePersons} " +
                                      $"( {IdName},  {IdEmailDigest},  {IdLiveId},  {IdSourceId}) " +
                                      "VALUES " +
                                      $"(@{IdName}, @{IdEmailDigest}, @{IdLiveId}, @{IdSourceId});";
                AddPersonParameters(person, command);
                command.ExecuteNonQuery();
            }

            return GetPersonId(person.Name, transaction);
        }

        if (!person.Equals(existingPerson))
        {
            var updatedPerson = new Person(existingPerson.Id,
                existingPerson.Name,
                !string.IsNullOrEmpty(person.EmailDigest) ? person.EmailDigest : existingPerson.EmailDigest,
                !string.IsNullOrEmpty(person.LiveId) ? person.LiveId : existingPerson.LiveId,
                !string.IsNullOrEmpty(person.SourceId) ? person.SourceId : existingPerson.SourceId
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
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT DISTINCT {TableFiles}.{IdFileName} " +
                                  $"FROM {TableFileFaces} " +
                                  $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                                  $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"WHERE {TablePersons}.{IdName}=@{IdName} " +
                                  $"ORDER by {TableFileData}.{IdDateTaken} DESC;";
            command.Parameters.AddWithValue($"@{IdName}", person);
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

    public long GetNumFilesOfPerson(string person)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT DISTINCT COUNT({TableFiles}.{IdFileName}) " +
                                  $"FROM {TableFileFaces} " +
                                  $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                                  $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"WHERE {TablePersons}.{IdName}=@{IdName} " +
                                  $"ORDER by {TableFileData}.{IdDateTaken} DESC;";
            command.Parameters.AddWithValue($"@{IdName}", person);
            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public long GetNumFaces()
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT COUNT({IdFileId}) FROM {TableFileFaces};";
            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public long GetNumAutoDetectedFaces()
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT COUNT({IdFileId}) FROM {TableFileFaces} WHERE {IdFaceMode} != {(int)FaceMode.Undefined};";
            object result = command.ExecuteScalar();
            return (long?)result ?? 0;
        }
    }

    public long GetPersonId(string name, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT {IdPersonId} FROM {TablePersons} WHERE {IdName}=@{IdName};";
            command.Parameters.AddWithValue($"@{IdName}", name);
            object result = command.ExecuteScalar();
            return (long?)result ?? Constants.InvalidId;
        }
    }

    public IList<FileAndPersonTag> GetFileAndPersonTags(string name, bool visibleOnly)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = "SELECT " +
                                  $"{TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                                  $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdVisible}, {IdFaceDescriptor}, " +
                                  $"{TableFiles}.{IdFileName} " +
                                  $"FROM {TableFileFaces} " +
                                  $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                                  $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"WHERE {TablePersons}.{IdName}=@{IdName}{AddVisibleWhere(visibleOnly)} " +
                                  $"ORDER by {TableFileData}.{IdDateTaken} DESC;";
            command.Parameters.AddWithValue($"@{IdName}", name);
            command.Parameters.AddWithValue($"@{IdVisible}", visibleOnly ? 1 : 0);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                var fileAndPersonTags = new List<FileAndPersonTag>();
                while (reader.HasRows && reader.Read())
                {
                    fileAndPersonTags.Add(new(reader.GetString(13),
                        ReadPersonTag(0, reader)));
                }

                return fileAndPersonTags;
            }
        }
    }

    public void RemovePerson(string person, SqliteTransaction transaction)
    {
        long id = GetPersonId(person, transaction);
        if (id != Constants.InvalidId)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"DELETE From {TablePersons} WHERE {IdPersonId}=@{IdPersonId};";
                command.Parameters.AddWithValue($"@{IdPersonId}", id);
                command.ExecuteNonQuery();
            }
        }
    }

    public IList<FaceInfo> GetAllFaceInfos(bool visibleOnly)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText =
                $"SELECT {TableFiles}.{IdFileId}, {IdFaceId}, {TablePersons}.{IdPersonId}, {IdFaceMode}, {IdFaceDescriptor} " +
                $"FROM {TableFileFaces} " +
                $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                $"WHERE {IdFaceDescriptor} != \"\"{AddVisibleWhere(visibleOnly)};";
            command.Parameters.AddWithValue($"@{IdVisible}", visibleOnly ? 1 : 0);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                var faceInfos = new List<FaceInfo>();
                while (reader.HasRows && reader.Read())
                {
                    faceInfos.Add(new(reader.GetInt64(0),
                        reader.GetInt64(1),
                        reader.GetInt64(2),
                        (FaceMode)reader.GetInt64(3),
                        ReadFaceDescriptor(4, reader)));
                }

                return faceInfos;
            }
        }
    }

    public FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId, bool visibleOnly)
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = "SELECT " +
                                  $"{TablePersons}.{IdPersonId}, {TablePersons}.{IdName}, {TablePersons}.{IdEmailDigest}, {TablePersons}.{IdLiveId}, {TablePersons}.{IdSourceId}, " +
                                  $"{IdFaceId}, {IdRectangleX}, {IdRectangleY}, {IdRectangleW}, {IdRectangleH}, {IdFaceMode}, {IdVisible}, {IdFaceDescriptor}, " +
                                  $"{TableFiles}.{IdFileName} " +
                                  $"FROM {TableFileFaces} " +
                                  $"    INNER JOIN {TablePersons} ON {TablePersons}.{IdPersonId}={TableFileFaces}.{IdPersonId} " +
                                  $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileFaces}.{IdFileId} " +
                                  $"WHERE {IdFaceId}={faceId}{AddVisibleWhere(visibleOnly)};";
            command.Parameters.AddWithValue($"@{IdVisible}", visibleOnly ? 1 : 0);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                FileAndPersonTag fileAndPersonTag = null;
                if (reader.HasRows && reader.Read())
                {
                    fileAndPersonTag = new(reader.GetString(13),
                        ReadPersonTag(0, reader));
                }

                return fileAndPersonTag;
            }
        }
    }

#endregion

#region Private

    private static string AddVisibleWhere(bool visibleOnly)
    {
        return visibleOnly ? $" AND {IdVisible}=@{IdVisible}" : "";
    }

    private static PersonTag ReadPersonTag(int start, SqliteDataReader reader)
    {
        return new(ReadPerson(start, reader), ReadFace(start + 5, reader));
    }

    private static Face ReadFace(int start, SqliteDataReader reader)
    {
        int faceId = reader.GetInt32(start + 0);
        Rectangle rect = Rectangle.FromRawValues(
            reader.GetInt32(start + 1), // X
            reader.GetInt32(start + 2), // Y
            reader.GetInt32(start + 3), // W
            reader.GetInt32(start + 4) // H
        );

        Face face;
        if (rect.W.RawValue > 0 && rect.H.RawValue > 0)
        {
            var faceMode = (FaceMode)reader.GetInt32(start + 5);
            bool visible = reader.GetInt32(start + 6) == 1;
            IEnumerable<FixedPoint64> faceDescriptor = ReadFaceDescriptor(start + 7, reader);
            face = new(faceId, rect, faceMode, visible, faceDescriptor);
        }
        else
        {
            face = new(faceId, new(new(0),
                    new(0),
                    new(0),
                    new(0)),
                FaceMode.Undefined,
                false,
                null);
        }

        return face;
    }

    private static IEnumerable<FixedPoint64> ReadFaceDescriptor(int start, SqliteDataReader reader)
    {
        string faceDescriptorString = reader.GetString(start);
        IEnumerable<FixedPoint64> faceDescriptor = faceDescriptorString.Length > 0 ?
            faceDescriptorString.Split(',').Select(s => new FixedPoint64(long.Parse(s, CultureInfo.InvariantCulture))) :
            null;
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
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"INSERT INTO {TableFileFaces} " + "" +
                                  $"( {IdFileId},  {IdPersonId},  {IdRectangleX},  {IdRectangleY},  {IdRectangleW},  {IdRectangleH},  {IdFaceMode},  {IdVisible}, {IdFaceDescriptor})" +
                                  "VALUES " +
                                  $"(@{IdFileId}, @{IdPersonId}, @{IdRectangleX}, @{IdRectangleY}, @{IdRectangleW}, @{IdRectangleH}, @{IdFaceMode}, @{IdVisible}, @{IdFaceDescriptor});";
            AddFaceParameters(fileId, personId, face, command);
            command.ExecuteNonQuery();
        }
    }

    private void UpdateFace(long fileId, long personId, Face face, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"UPDATE {TableFileFaces} " + "" +
                                  $"SET {IdFileId}=@{IdFileId}," +
                                  $"    {IdPersonId}=@{IdPersonId}," +
                                  $"    {IdRectangleX}=@{IdRectangleX}," +
                                  $"    {IdRectangleY}=@{IdRectangleY}," +
                                  $"    {IdRectangleW}=@{IdRectangleW}," +
                                  $"    {IdRectangleH}=@{IdRectangleH}," +
                                  $"    {IdFaceMode}=@{IdFaceMode}," +
                                  $"    {IdVisible}=@{IdVisible}," +
                                  $"    {IdFaceDescriptor}=@{IdFaceDescriptor} " +
                                  $"WHERE {IdFaceId}=@{IdFaceId};";

            AddFaceParameters(fileId, personId, face, command);
            command.ExecuteNonQuery();
        }
    }

    private void RemoveFace(long id, SqliteTransaction transaction)
    {
        if (id != Constants.InvalidId)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"DELETE From {TableFileFaces} WHERE {IdFaceId}=@{IdFaceId};";
                command.Parameters.AddWithValue($"@{IdFaceId}", id);
                command.ExecuteNonQuery();
            }
        }
    }

    private static void AddFaceParameters(long fileId, long personId, Face face, SqliteCommand command)
    {
        command.Parameters.AddWithValue($"@{IdFileId}", fileId);
        command.Parameters.AddWithValue($"@{IdPersonId}", personId);
        command.Parameters.AddWithValue($"@{IdFaceId}", face.Id);
        command.Parameters.AddWithValue($"@{IdRectangleX}", face.Rectangle.X.RawValue);
        command.Parameters.AddWithValue($"@{IdRectangleY}", face.Rectangle.Y.RawValue);
        command.Parameters.AddWithValue($"@{IdRectangleW}", face.Rectangle.W.RawValue);
        command.Parameters.AddWithValue($"@{IdRectangleH}", face.Rectangle.H.RawValue);
        command.Parameters.AddWithValue($"@{IdFaceMode}", (int)face.FaceMode);
        command.Parameters.AddWithValue($"@{IdVisible}", face.Visible ? 1 : 0);
        IEnumerable<string> faceDescriptorStringValues = face.HasFaceDescriptor ?
            face.FaceDescriptor.Select(v => v.RawValue.ToString(CultureInfo.InvariantCulture)) :
            null;
        string faceDescriptorString = faceDescriptorStringValues != null ? string.Join(",", faceDescriptorStringValues) : "";
        command.Parameters.AddWithValue($"@{IdFaceDescriptor}", faceDescriptorString);
    }

    private void UpdatePerson(Person person, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"UPDATE {TablePersons} " +
                                  $"SET {IdName}=@{IdName}," +
                                  $"    {IdEmailDigest}=@{IdEmailDigest}," +
                                  $"    {IdLiveId}=@{IdLiveId}," +
                                  $"    {IdSourceId}=@{IdSourceId} " +
                                  $"WHERE {IdPersonId}=@{IdPersonId};";
            AddPersonParameters(person, command);
            command.ExecuteNonQuery();
        }
    }

    private static void AddPersonParameters(Person person, SqliteCommand command)
    {
        command.Parameters.AddWithValue($"@{IdPersonId}", person.Id);
        command.Parameters.AddWithValue($"@{IdName}", person.Name);
        command.Parameters.AddWithValue($"@{IdEmailDigest}", person.EmailDigest);
        command.Parameters.AddWithValue($"@{IdLiveId}", person.LiveId);
        command.Parameters.AddWithValue($"@{IdSourceId}", person.SourceId);
    }

    private readonly DB2Instance _instance;

#endregion
}