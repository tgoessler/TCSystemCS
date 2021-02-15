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
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB
{
    internal sealed class DB2Tags : DB2Constants
    {
#region Public

        public long GetNumTags(SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT COUNT({IdTagId}) FROM {TableTags};"
            })
            {
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public IList<string> GetAllTagsLike(string filter)
        {
            const string filterTagsCommand = "WHERE " + IdTag + " LIKE @Filter ";

            var filterCommand = filter != null ? filterTagsCommand : "";
            if (filter != null)
            {
                filter = "%" + filter.Replace(' ', '%') + "%";
            }

            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT {IdTag} FROM {TableTags} {filterCommand} ORDER by {IdTag} ASC;"
            })
            {
                command.Parameters.AddWithValue("@Filter", filter);
                using (var reader = command.ExecuteReader())
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
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"SELECT {TableTags}.{IdTag} FROM {TableFileTags} " +
                              $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId} = {TableFileTags}.{IdTagId} " +
                              $"WHERE {IdFileId}=@{IdFileId};"
            })
            {
                command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                using (var reader = command.ExecuteReader())
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

        public void AddTags(long fileId, IReadOnlyList<string> tags, SqliteTransaction transaction)
        {
            foreach (var tag in tags)
            {
                AddTag(fileId, tag, transaction);
            }
        }

        public IList<string> GetFilesOfTag(string tag)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT {TableFiles}.{IdFileName} " +
                              $"FROM {TableFileTags} " +
                              $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId}={TableFileTags}.{IdTagId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileTags}.{IdFileId} " +
                              $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileTags}.{IdFileId} " +
                              $"WHERE {TableTags}.{IdTag} LIKE @{IdTag} ESCAPE '?' " +
                              $"ORDER by {TableFileData}.{IdDateTaken} DESC;"
            })
            {
                command.Parameters.AddWithValue($"@{IdTag}", tag + "%");
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

        public long GetNumFilesOfTag(string tag)
        {
            using (var command = new SqliteCommand
            {
                Connection = Instance.Connection,
                CommandText = $"SELECT DISTINCT COUNT({TableFiles}.{IdFileName}) " +
                              $"FROM {TableFileTags} " +
                              $"    INNER JOIN {TableTags} ON {TableTags}.{IdTagId}={TableFileTags}.{IdTagId} " +
                              $"    INNER JOIN {TableFiles} ON {TableFiles}.{IdFileId}={TableFileTags}.{IdFileId} " +
                              $"    INNER JOIN {TableFileData} ON {TableFileData}.{IdFileId}={TableFileTags}.{IdFileId} " +
                              $"WHERE {TableTags}.{IdTag} LIKE @{IdTag} ESCAPE '?' " +
                              $"ORDER by {TableFileData}.{IdDateTaken} DESC;"
            })
            {
                command.Parameters.AddWithValue($"@{IdTag}", tag + "%");
                var result = command.ExecuteScalar();
                return (long?) result ?? 0;
            }
        }

        public void RemoveTag(string tag, SqliteTransaction transaction)
        {
            var id = GetTagId(tag, transaction);
            if (id != Constants.InvalidId)
            {
                using (var command = new SqliteCommand
                {
                    Transaction = transaction,
                    Connection = Instance.Connection,
                    CommandText = $"DELETE From {TableFileTags} WHERE {IdTagId}=@{IdTagId};"
                })
                {
                    command.Parameters.AddWithValue($"@{IdTagId}", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public DB2Instance Instance;

#endregion

#region Private

        private long GetTagId(string tag, SqliteTransaction transaction)
        {
            using (var command = new SqliteCommand
            {
                Transaction = transaction,
                Connection = Instance.Connection,
                CommandText = $"SELECT {IdTagId} FROM {TableTags} WHERE {IdTag}=@{IdTag}"
            })
            {
                command.Parameters.AddWithValue($"@{IdTag}", tag);
                var result = command.ExecuteScalar();
                return (long?) result ?? Constants.InvalidId;
            }
        }

        private long AddTag(string tag, SqliteTransaction transaction)
        {
            var tagId = Constants.InvalidId;
            if (tag.Length > 0)
            {
                tagId = GetTagId(tag, transaction);

                if (tagId == Constants.InvalidId)
                {
                    using (var command = new SqliteCommand
                    {
                        Transaction = transaction,
                        Connection = Instance.Connection,
                        CommandText = $"INSERT INTO {TableTags} ({IdTag}) VALUES(@{IdTag});"
                    })
                    {
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
            var tagId = AddTag(tag, transaction);
            if (tagId != Constants.InvalidId)
            {
                using (var command = new SqliteCommand
                {
                    Transaction = transaction,
                    Connection = Instance.Connection,
                    CommandText = $"INSERT INTO {TableFileTags} ({IdFileId}, {IdTagId}) VALUES(@{IdFileId}, @{IdTagId});"
                })
                {
                    command.Parameters.AddWithValue($"@{IdFileId}", fileId);
                    command.Parameters.AddWithValue($"@{IdTagId}", tagId);
                    command.ExecuteNonQuery();
                }
            }
        }

#endregion
    }
}