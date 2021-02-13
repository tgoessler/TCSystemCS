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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
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
    public enum ProcessingInfo : long
    {
        None = 0,

        DlibFrontalFaceDetection2000 = 1,
        DlibFrontalFaceDetection3000 = 1 << 1,

        DlibCnnFaceDetection1000 = 1 << 10,
        DlibCnnFaceDetection2000 = 1 << 11,

        DlibImageClassification600 = 1 << 20
    }

    public static class ProcessingInfoUtil
    {
#region Public

        public static bool AreAllFaceDetectionsDone(this ProcessingInfo processingInfo)
        {
            return (processingInfo & ProcessingInfo.DlibFrontalFaceDetection2000) != 0 &&
                   (processingInfo & ProcessingInfo.DlibFrontalFaceDetection3000) != 0 &&
                   (processingInfo & ProcessingInfo.DlibCnnFaceDetection1000) != 0 &&
                   (processingInfo & ProcessingInfo.DlibCnnFaceDetection2000) != 0;
        }

        public static bool AreAllFrontalFaceDetectionsDone(this ProcessingInfo processingInfo)
        {
            return (processingInfo & ProcessingInfo.DlibFrontalFaceDetection2000) != 0 &&
                   (processingInfo & ProcessingInfo.DlibFrontalFaceDetection3000) != 0;
        }

        public static bool IsFrontalFaceDetection(this ProcessingInfo processingInfo)
        {
            return (processingInfo & ProcessingInfo.DlibFrontalFaceDetection2000) != 0 ||
                   (processingInfo & ProcessingInfo.DlibFrontalFaceDetection3000) != 0;
        }

        public static bool IsCnnFaceDetection(this ProcessingInfo processingInfo)
        {
            return (processingInfo & ProcessingInfo.DlibCnnFaceDetection1000) != 0 ||
                   (processingInfo & ProcessingInfo.DlibCnnFaceDetection2000) != 0;
        }

#endregion
    }
}