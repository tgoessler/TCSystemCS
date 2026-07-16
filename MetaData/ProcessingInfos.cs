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

#endregion

namespace TCSystem.MetaData;

/// <summary>Flags the image-processing operations that have been completed.</summary>
[Flags]
public enum ProcessingInfos : long
{
    /// <summary>No processing operation has been recorded.</summary>
    None = 0,

    /// <summary>Dlib frontal-face detection was run at a 2000-pixel scale.</summary>
    DlibFrontalFaceDetection2000 = 1,

    /// <summary>Dlib frontal-face detection was run at a 3000-pixel scale.</summary>
    DlibFrontalFaceDetection3000 = 1 << 1,

    /// <summary>Dlib CNN face detection was run at a 1000-pixel scale.</summary>
    DlibCnnFaceDetection1000 = 1 << 10,

    /// <summary>Dlib CNN face detection was run at a 2000-pixel scale.</summary>
    DlibCnnFaceDetection2000 = 1 << 11,

    /// <summary>Dlib image classification was run at a 600-pixel scale.</summary>
    DlibImageClassification600 = 1 << 20
}