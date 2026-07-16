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

/// <summary>Provides queries over image-processing flags.</summary>
public static class ProcessingInfosExt
{
#region Public

    /// <summary>Determines whether every configured frontal and CNN face detection has completed.</summary>
    public static bool AreAllFaceDetectionsDone(this ProcessingInfos processingInfos)
    {
        return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 &&
               (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0 &&
               (processingInfos & ProcessingInfos.DlibCnnFaceDetection1000) != 0 &&
               (processingInfos & ProcessingInfos.DlibCnnFaceDetection2000) != 0;
    }

    /// <summary>Determines whether both configured frontal-face detections have completed.</summary>
    public static bool AreAllFrontalFaceDetectionsDone(this ProcessingInfos processingInfos)
    {
        return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 &&
               (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0;
    }

    /// <summary>Determines whether any frontal-face detection has completed.</summary>
    public static bool IsFrontalFaceDetection(this ProcessingInfos processingInfos)
    {
        return (processingInfos & ProcessingInfos.DlibFrontalFaceDetection2000) != 0 ||
               (processingInfos & ProcessingInfos.DlibFrontalFaceDetection3000) != 0;
    }

    /// <summary>Determines whether any CNN face detection has completed.</summary>
    public static bool IsCnnFaceDetection(this ProcessingInfos processingInfos)
    {
        return (processingInfos & ProcessingInfos.DlibCnnFaceDetection1000) != 0 ||
               (processingInfos & ProcessingInfos.DlibCnnFaceDetection2000) != 0;
    }

#endregion
}