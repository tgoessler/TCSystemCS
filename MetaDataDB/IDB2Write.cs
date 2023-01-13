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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.MetaDataDB
{
    public interface IDB2Write
    {
#region Public

        Image AddMetaData(Image data, DateTimeOffset dateModified);
        void RemoveMetaData(string fileName);
        void RemoveAllFilesOfFolder(string folder);

        event Action<Image> MetaDataAdded;
        event Action<Image> MetaDataRemoved;
        event Action<(Image NewData, Image OldData)> MetaDataChanged;
#endregion
    }
}