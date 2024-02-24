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
using System.Linq;
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public class FilesTests : DBSetup
{
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

    [Test]
    public void GetAllFileAndModifiedDates()
    {
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(0));

        DateTimeOffset modifiedZero = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(TestData.ImageZero, modifiedZero);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));

        DateTimeOffset modified1 = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(TestData.Image1, modified1);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.Image1.FileName], Is.EqualTo(modified1));

        DateTimeOffset modified2 = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(TestData.Image2, modified2);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.Image1.FileName], Is.EqualTo(modified1));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.Image2.FileName], Is.EqualTo(modified2));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.Image2.FileName], Is.EqualTo(modified2));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(0));
    }

    [Test]
    public void GetAllProcessingInformation()
    {
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].FileName, Is.EqualTo(TestData.ImageZero.FileName));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo,
            Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image1.FileName).ProcessingInfo,
            Is.EqualTo(TestData.Image1.ProcessingInfos));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo,
            Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image1.FileName).ProcessingInfo,
            Is.EqualTo(TestData.Image1.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image2.FileName).ProcessingInfo,
            Is.EqualTo(TestData.Image2.ProcessingInfos));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo,
            Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image2.FileName).ProcessingInfo,
            Is.EqualTo(TestData.Image2.ProcessingInfos));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].FileName, Is.EqualTo(TestData.ImageZero.FileName));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(0));
    }
}