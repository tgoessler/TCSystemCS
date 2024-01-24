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

#endregion

namespace TCSystem.MetaData
{
    [Flags]
    public enum ProcessingInfos : long
    {
        None = 0,

        DlibFrontalFaceDetection2000 = 1,
        DlibFrontalFaceDetection3000 = 1 << 1,

        DlibCnnFaceDetection1000 = 1 << 10,
        DlibCnnFaceDetection2000 = 1 << 11,

        DlibImageClassification600 = 1 << 20
    }
}