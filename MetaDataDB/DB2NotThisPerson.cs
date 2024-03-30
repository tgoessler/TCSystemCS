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
using Microsoft.Data.Sqlite;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2NotThisPerson(DB2Instance instance) : DB2Constants
{
#region Public

    public void AddNotThisPerson(long faceId, long personId, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"INSERT INTO {TableNotThisPerson} ({IdFaceId}, {IdPersonId}) VALUES (@{IdFaceId}, @{IdPersonId});";
            command.Parameters.AddWithValue($"@{IdFaceId}", faceId);
            command.Parameters.AddWithValue($"@{IdPersonId}", personId);

            command.ExecuteNonQuery();
        }
    }

    public IDictionary<long, IList<long>> GetNotThisPersonInformation()
    {
        using (var command = new SqliteCommand())
        {
            command.Connection = _instance.Connection;
            command.CommandText = $"SELECT {IdFaceId}, {IdPersonId} FROM {TableNotThisPerson};";

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                var notThisPersonInformation = new Dictionary<long, IList<long>>();

                while (reader.HasRows && reader.Read())
                {
                    long faceId = reader.GetInt64(0);
                    long personId = reader.GetInt64(1);
                    if (!notThisPersonInformation.TryGetValue(faceId, out IList<long> personIds))
                    {
                        notThisPersonInformation[faceId] = [personId];
                    }
                    else
                    {
                        personIds.Add(personId);
                    }
                }

                return notThisPersonInformation;
            }
        }
    }

    public void RemoveNotThisPerson(long faceId, SqliteTransaction transaction)
    {
        using (var command = new SqliteCommand())
        {
            command.Transaction = transaction;
            command.Connection = _instance.Connection;
            command.CommandText = $"DELETE FROM {TableNotThisPerson} WHERE {IdFaceId} = @{IdFaceId};";
            command.Parameters.AddWithValue($"@{IdFaceId}", faceId);

            command.ExecuteNonQuery();
        }
    }

#endregion

#region Private

    private readonly DB2Instance _instance = instance;

#endregion
}