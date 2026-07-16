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

/// <summary>Copies and upgrades metadata between database instances.</summary>
public interface IDB2Converter
{
#region Public

    /// <summary>Copies all metadata from a source database to a destination database.</summary>
    /// <param name="from">The source database.</param>
    /// <param name="to">The destination database.</param>
    void Convert(IDB2Read from, IDB2 to);

#endregion
}