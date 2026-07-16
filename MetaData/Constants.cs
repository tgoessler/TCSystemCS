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

namespace TCSystem.MetaData;

/// <summary>Defines identifier sentinel values used by metadata models and storage.</summary>
public static class Constants
{
#region Public

    /// <summary>The identifier assigned to an object that is invalid.</summary>
    public const long InvalidId = -1L;

    /// <summary>The reserved database identifier for an empty person.</summary>
    public const long EmptyPersonId = 1;

    /// <summary>The reserved database identifier for an empty location.</summary>
    public const long EmptyLocationId = 1;

#endregion
}