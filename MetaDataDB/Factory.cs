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

namespace TCSystem.MetaDataDB;

/// <summary>Creates and releases metadata database abstractions.</summary>
public static class Factory
{
#region Public

    /// <summary>Opens or creates a metadata database for reading and writing.</summary>
    public static IDB2 CreateReadWrite(string fileName)
    {
        return new DB2(fileName, false);
    }

    /// <summary>Opens an existing metadata database for read-only access.</summary>
    public static IDB2Read CreateRead(string fileName)
    {
        return new DB2(fileName, true);
    }

    /// <summary>Opens or creates a metadata database for write access.</summary>
    public static IDB2Write CreateWrite(string fileName)
    {
        return new DB2(fileName, false);
    }

    /// <summary>Closes a read/write database and sets the reference to <see langword="null" />.</summary>
    public static void Destroy(ref IDB2 db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

    /// <summary>Closes a read-only database and sets the reference to <see langword="null" />.</summary>
    public static void Destroy(ref IDB2Read db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

    /// <summary>Closes a write database and sets the reference to <see langword="null" />.</summary>
    public static void Destroy(ref IDB2Write db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

    /// <summary>Creates a metadata database converter.</summary>
    public static IDB2Converter CreateConverter()
    {
        return new DB2Converter();
    }

#endregion
}