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

namespace TCSystem.MetaDataDB;

public static class Factory
{
#region Public

    public static IDB2 CreateReadWrite(string fileName)
    {
        return new DB2(fileName, false);
    }

    public static IDB2Read CreateRead(string fileName)
    {
        return new DB2(fileName, true);
    }

    public static IDB2Write CreateWrite(string fileName)
    {
        return new DB2(fileName, false);
    }

    public static void Destroy(ref IDB2 db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

    public static void Destroy(ref IDB2Read db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

    public static void Destroy(ref IDB2Write db)
    {
        if (db is DB2 db2)
        {
            db2.Close();
        }

        db = null;
    }

#endregion
}