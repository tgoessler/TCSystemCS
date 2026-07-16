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

/// <summary>Identifies the detector used to find a face.</summary>
public enum FaceMode
{
    /// <summary>No detector is specified.</summary>
    Undefined = 0,

    /// <summary>The Dlib frontal-face detector was used.</summary>
    DlibFront = 1,

    /// <summary>The Dlib convolutional neural network detector was used.</summary>
    DlibCnn = 2
}