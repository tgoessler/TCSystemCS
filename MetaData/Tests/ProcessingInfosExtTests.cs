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

using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class ProcessingInfosExtTests
{
    [Test]
    public void AreAllFaceDetectionsDone_None_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.None.AreAllFaceDetectionsDone(), Is.False);
    }

    [Test]
    public void AreAllFaceDetectionsDone_OnlyFrontal_ReturnsFalse()
    {
        const ProcessingInfos infos = ProcessingInfos.DlibFrontalFaceDetection2000 |
                                      ProcessingInfos.DlibFrontalFaceDetection3000;
        Assert.That(infos.AreAllFaceDetectionsDone(), Is.False);
    }

    [Test]
    public void AreAllFaceDetectionsDone_OnlyCnn_ReturnsFalse()
    {
        const ProcessingInfos infos = ProcessingInfos.DlibCnnFaceDetection1000 |
                                      ProcessingInfos.DlibCnnFaceDetection2000;
        Assert.That(infos.AreAllFaceDetectionsDone(), Is.False);
    }

    [Test]
    public void AreAllFaceDetectionsDone_AllSet_ReturnsTrue()
    {
        const ProcessingInfos infos = ProcessingInfos.DlibFrontalFaceDetection2000 |
                                      ProcessingInfos.DlibFrontalFaceDetection3000 |
                                      ProcessingInfos.DlibCnnFaceDetection1000 |
                                      ProcessingInfos.DlibCnnFaceDetection2000;
        Assert.That(infos.AreAllFaceDetectionsDone(), Is.True);
    }

    [Test]
    public void AreAllFaceDetectionsDone_AllSetPlusClassification_ReturnsTrue()
    {
        const ProcessingInfos infos = ProcessingInfos.DlibFrontalFaceDetection2000 |
                                      ProcessingInfos.DlibFrontalFaceDetection3000 |
                                      ProcessingInfos.DlibCnnFaceDetection1000 |
                                      ProcessingInfos.DlibCnnFaceDetection2000 |
                                      ProcessingInfos.DlibImageClassification600;
        Assert.That(infos.AreAllFaceDetectionsDone(), Is.True);
    }

    [Test]
    public void AreAllFrontalFaceDetectionsDone_None_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.None.AreAllFrontalFaceDetectionsDone(), Is.False);
    }

    [Test]
    public void AreAllFrontalFaceDetectionsDone_OnlyOne_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.DlibFrontalFaceDetection2000.AreAllFrontalFaceDetectionsDone(), Is.False);
        Assert.That(ProcessingInfos.DlibFrontalFaceDetection3000.AreAllFrontalFaceDetectionsDone(), Is.False);
    }

    [Test]
    public void AreAllFrontalFaceDetectionsDone_BothSet_ReturnsTrue()
    {
        const ProcessingInfos infos = ProcessingInfos.DlibFrontalFaceDetection2000 |
                                      ProcessingInfos.DlibFrontalFaceDetection3000;
        Assert.That(infos.AreAllFrontalFaceDetectionsDone(), Is.True);
    }

    [Test]
    public void IsFrontalFaceDetection_None_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.None.IsFrontalFaceDetection(), Is.False);
    }

    [Test]
    public void IsFrontalFaceDetection_Frontal2000_ReturnsTrue()
    {
        Assert.That(ProcessingInfos.DlibFrontalFaceDetection2000.IsFrontalFaceDetection(), Is.True);
    }

    [Test]
    public void IsFrontalFaceDetection_Frontal3000_ReturnsTrue()
    {
        Assert.That(ProcessingInfos.DlibFrontalFaceDetection3000.IsFrontalFaceDetection(), Is.True);
    }

    [Test]
    public void IsFrontalFaceDetection_CnnOnly_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.DlibCnnFaceDetection1000.IsFrontalFaceDetection(), Is.False);
    }

    [Test]
    public void IsCnnFaceDetection_None_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.None.IsCnnFaceDetection(), Is.False);
    }

    [Test]
    public void IsCnnFaceDetection_Cnn1000_ReturnsTrue()
    {
        Assert.That(ProcessingInfos.DlibCnnFaceDetection1000.IsCnnFaceDetection(), Is.True);
    }

    [Test]
    public void IsCnnFaceDetection_Cnn2000_ReturnsTrue()
    {
        Assert.That(ProcessingInfos.DlibCnnFaceDetection2000.IsCnnFaceDetection(), Is.True);
    }

    [Test]
    public void IsCnnFaceDetection_FrontalOnly_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.DlibFrontalFaceDetection2000.IsCnnFaceDetection(), Is.False);
    }

    [Test]
    public void IsCnnFaceDetection_ClassificationOnly_ReturnsFalse()
    {
        Assert.That(ProcessingInfos.DlibImageClassification600.IsCnnFaceDetection(), Is.False);
    }
}
