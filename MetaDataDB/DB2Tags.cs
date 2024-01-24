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
using System.Linq;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB
{
    internal sealed class DB2Tags : DB2Constants
    {
#region Public

        public DB2Tags(DB2Instance instance)
        {
            _instance = instance;
        }

        public long GetNumTags(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand())
            {
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT COUNT({IdTagId}) FROM {TableTags};";
                object result = command.ExecuteScalar();
                return (long?)result ?? 0;
            }
        }

        public IList<string> GetAllTagsLike(string filter)
        {
            const string filterTagsCommand = "WHERE " + IdTag + " LIKE @Filter ";

            string filterCommand = filter != null ? filterTagsCommand : "";
            if (filter != null)
            {
                filter = "%" + filter.Replace(' ', '%') + "%";
            }

            using (var command = new SqliteCommand())
            {
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT DISTINCT {IdTag} FROM {TableTags} {filterCommand} ORDER by {IdTag} ASC;";
                command.Parameters.AddWithValue("@Filter", filter);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var tags = new List<string>();
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }

                    return tags;
                }
            }
        }


        public IReadOnlyList<string> GetTags(long fileId, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT {TableTags}.{IdTag} FROM {TableFileTags} " +
                                      $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId} = {TableFileTags}.{IdTagId} " +
                                      $"WHERE {IdFileId}=@{IdFileId};";
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var tags = new List<string>();
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }

                    return tags;
                }
            }
        }

        public void SetTags(long fileId, IReadOnlyList<string> newTags, IReadOnlyList<string> oldTags, SqliteTransaction transaction)
        {
            foreach (string tag in newTags)
            {
                if (!oldTags.Contains(tag))
                {
                    AddTag(fileId, tag, transaction);
                }
            }

            foreach (string tag in oldTags)
            {
                if (!newTags.Contains(tag))
                {
                    RemoveTag(fileId, tag, transaction);
                }
            }

        }

        public IList<string> GetFilesOfTag(string tag)
        {
            using (var command = new SqliteCommand())
            {
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT DISTINCT {TableFiles}.{IdFileName} " +
                                      $"FROM {TableFileTags} " +
                                      $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId}={TableFileTags}.{IdTagId} " +
                                      $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileTags}.{IdFileId} " +
                                      $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileTags}.{IdFileId} " +
                                      $"WHERE {TableTags}.{IdTag} LIKE @{IdTag} ESCAPE '?' " +
                                      $"ORDER by {TableFileData}.{IdDateTaken} DESC;";
                command.Parameters.AddWithValue($"@{IdTag}", tag + "%");
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

        public long GetNumFilesOfTag(string tag)
        {
            using (var command = new SqliteCommand())
            {
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT DISTINCT COUNT({TableFiles}.{IdFileName}) " +
                                      $"FROM {TableFileTags} " +
                                      $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId}={TableFileTags}.{IdTagId} " +
                                      $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileTags}.{IdFileId} " +
                                      $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileTags}.{IdFileId} " +
                                      $"WHERE {TableTags}.{IdTag} LIKE @{IdTag} ESCAPE '?' " +
                                      $"ORDER by {TableFileData}.{IdDateTaken} DESC;";
                command.Parameters.AddWithValue($"@{IdTag}", tag + "%");
                object result = command.ExecuteScalar();
                return (long?)result ?? 0;
            }
        }

        public void RemoveTag(string tag, SqliteTransaction transaction)
        {
            long id = GetTagId(tag, transaction);
            if (id != Constants.InvalidId)
            {
                using (var command = new SqliteCommand())
                {
                    command.Transaction = transaction;
                    command.Connection = _instance.Connection;
                    command.CommandText = $"DELETE From {TableTags} WHERE {IdTagId}=@{IdTagId};";
                    command.Parameters.AddWithValue($"@{IdTagId}", id);
                    command.ExecuteNonQuery();
                }
            }
        }

#endregion

#region Private

        private long GetTagId(string tag, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand())
            {
                command.Transaction = transaction;
                command.Connection = _instance.Connection;
                command.CommandText = $"SELECT {IdTagId} FROM {TableTags} WHERE {IdTag}=@{IdTag}";
                command.Parameters.AddWithValue($"@{IdTag}", tag);
                object result = command.ExecuteScalar();
                return (long?)result ?? Constants.InvalidId;
            }
        }

        private long AddTag(string tag, SqliteTransaction transaction)
        {
            long tagId = Constants.InvalidId;
            if (tag.Length > 0)
            {
                tagId = GetTagId(tag, transaction);

                if (tagId == Constants.InvalidId)
                {
                    using (var command = new SqliteCommand())
                    {
                        command.Transaction = transaction;
                        command.Connection = _instance.Connection;
                        command.CommandText = $"INSERT INTO {TableTags} ({IdTag}) VALUES(@{IdTag});";
                        command.Parameters.AddWithValue($"@{IdTag}", tag);
                        command.ExecuteNonQuery();
                    }

                    tagId = GetTagId(tag, transaction);
                }
            }

            return tagId;
        }

        private void AddTag(long fileId, string tag, SqliteTransaction transaction)
        {
            long tagId = AddTag(tag, transaction);
            if (tagId != Constants.InvalidId)
            {
                using (var command = new SqliteCommand())
                {
                    command.Transaction = transaction;
                    command.Connection = _instance.Connection;
                    command.CommandText = $"INSERT INTO {TableFileTags} ({IdFileId}, {IdTagId}) VALUES(@{IdFileId}, @{IdTagId});";
                    AddTagParameters(fileId, tagId, command);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void RemoveTag(long fileId, string tag, SqliteTransaction transaction)
        {
            long tagId = GetTagId(tag, transaction);
            if (tagId != Constants.InvalidId)
            {
                using (var command = new SqliteCommand())
                {
                    command.Transaction = transaction;
                    command.Connection = _instance.Connection;
                    command.CommandText = $"DELETE FROM {TableFileTags} WHERE {IdFileId}=@{IdFileId} AND {IdTagId}=@{IdTagId};";
                    AddTagParameters(fileId, tagId, command);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void AddTagParameters(long fileId, long tagId, SqliteCommand command)
        {
            command.Parameters.AddWithValue($"@{IdFileId}", fileId);
            command.Parameters.AddWithValue($"@{IdTagId}", tagId);
        }

        private readonly DB2Instance _instance;

#endregion
    }
}