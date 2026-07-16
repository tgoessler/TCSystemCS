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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

/// <summary>Defines mutation operations and change notifications for a metadata database.</summary>
public interface IDB2Write
{
#region Public

    /// <summary>Adds or updates image metadata and returns the persisted representation.</summary>
    Image AddMetaData(Image newMetaData, DateTimeOffset dateModified);

    /// <summary>Removes the metadata associated with a file.</summary>
    void RemoveMetaData(string fileName);

    /// <summary>Removes metadata for all files directly contained in a folder.</summary>
    void RemoveAllFilesOfFolder(string folder);

    /// <summary>Records that a detected face does not belong to a person.</summary>
    void AddNotThisPerson(Face face, Person person);

    /// <summary>Occurs after metadata is added.</summary>
    event Action<Image> MetaDataAdded;

    /// <summary>Occurs after metadata is removed.</summary>
    event Action<Image> MetaDataRemoved;

    /// <summary>Occurs after existing metadata is changed.</summary>
    event Action<(Image NewData, Image OldData)> MetaDataChanged;

#endregion
}