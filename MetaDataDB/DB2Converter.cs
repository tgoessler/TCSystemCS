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
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2Converter : IDB2Converter
{
#region Public

    public void Convert(IDB2Read from, IDB2 to)
    {
        _fromDB = from;
        _toDB = to;

        _fromDB.EnableUnsafeMode();
        _toDB.EnableUnsafeMode();

        Convert();

        _fromDB.EnableDefaultMode();
        _toDB.EnableDefaultMode();
    }

#endregion

#region Private

    private void Convert()
    {
        try
        {
            Log.Instance.Info("Converting database ...");

            IList<string> files = _fromDB.GetAllFilesLike();
            Log.Instance.Debug($"Converting old database format to new format: {files.Count} Entries");

            foreach (string fileName in files)
            {
                ConvertFile(fileName);
            }

            Log.Instance.Info("Converting database done.");
        }
        catch (Exception e)
        {
            Log.Instance.Error("Failed converting database", e);
        }
    }

    private void ConvertFile(string file)
    {
        Image data = _fromDB.GetMetaData(file);
        data = data.InvalidateId();
        DateTimeOffset dateModified = _fromDB.GetDateModified(file);

        _toDB.AddMetaData(data, dateModified);
    }

    private IDB2Read _fromDB;
    private IDB2 _toDB;

#endregion
}