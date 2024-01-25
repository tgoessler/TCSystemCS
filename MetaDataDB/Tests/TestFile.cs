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
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests;

public class TestFile : DBSetup
{
#region Public

    [Test]
    public void ChangeDateModified()
    {
        Image data = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        DateTimeOffset dateTimeNow = DateTimeOffset.Now;
        DateTimeOffset newDateModified = dateTimeNow.AddSeconds(5).Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(data, newDateModified);

        Assert.That(newDateModified, Is.EqualTo(DBReadOnly.GetDateModified(data.FileName)));
        Assert.That(data.Id, Is.EqualTo(DBReadOnly.GetMetaData(data.FileName).Id));
    }

    [Test]
    public void ChangeProcessingInfo()
    {
        Image data = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        data = Image.ChangeProcessingInfo(data, ProcessingInfos.DlibCnnFaceDetection1000 | ProcessingInfos.DlibCnnFaceDetection2000);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(data.ProcessingInfos, Is.EqualTo(DBReadOnly.GetMetaData(data.FileName).ProcessingInfos));
        Assert.That(data.Id, Is.EqualTo(DBReadOnly.GetMetaData(data.FileName).Id));
    }

#endregion
}