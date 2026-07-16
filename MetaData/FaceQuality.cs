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

/// <summary>Describes the quality assigned to a detected face.</summary>
public enum FaceQuality
{
    /// <summary>The face has normal quality.</summary>
    Normal = 0,

    /// <summary>The face has poor quality.</summary>
    Poor = 1,

    /// <summary>The face has good quality.</summary>
    Good = 2
}