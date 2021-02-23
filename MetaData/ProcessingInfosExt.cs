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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

namespace TCSystem.MetaData
{
    public static class ProcessingInfosExt
    {
#region Public

        public static bool AreAllFaceDetectionsDone(this ProcessingInfos processingInfos)
        {
            return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 &&
                   (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0 &&
                   (processingInfos & ProcessingInfos.DlibCnnFaceDetection1000) != 0 &&
                   (processingInfos & ProcessingInfos.DlibCnnFaceDetection2000) != 0;
        }

        public static bool AreAllFrontalFaceDetectionsDone(this ProcessingInfos processingInfos)
        {
            return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 &&
                   (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0;
        }

        public static bool IsFrontalFaceDetection(this ProcessingInfos processingInfos)
        {
            return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 ||
                   (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0;
        }

        public static bool IsCnnFaceDetection(this ProcessingInfos processingInfos)
        {
            return (processingInfos & ProcessingInfos.DlibCnnFaceDetection1000) != 0 ||
                   (processingInfos & ProcessingInfos.DlibCnnFaceDetection2000) != 0;
        }

#endregion
    }
}