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

using System;
using System.Linq;
using NUnit.Framework;
using TCSystem.MetaData;

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public class FilesTests : DBSetup
{
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
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image1.FileName).ProcessingInfo, Is.EqualTo(TestData.Image1.ProcessingInfos));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image1.FileName).ProcessingInfo, Is.EqualTo(TestData.Image1.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image2.FileName).ProcessingInfo, Is.EqualTo(TestData.Image2.ProcessingInfos));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.ImageZero.FileName).ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));
        Assert.That(DBReadOnly.GetAllProcessingInformation().FirstOrDefault(v => v.FileName == TestData.Image2.FileName).ProcessingInfo, Is.EqualTo(TestData.Image2.ProcessingInfos));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].FileName, Is.EqualTo(TestData.ImageZero.FileName));
        Assert.That(DBReadOnly.GetAllProcessingInformation()[0].ProcessingInfo, Is.EqualTo(TestData.ImageZero.ProcessingInfos));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetAllProcessingInformation().Count, Is.EqualTo(0));
    }

    [Test]
    public void GetAllFileAndModifiedDates()
    {
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(0));

        var modifiedZero = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(TestData.ImageZero, modifiedZero);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));

        var modified1 = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
        DB.AddMetaData(TestData.Image1, modified1);
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.ImageZero.FileName], Is.EqualTo(modifiedZero));
        Assert.That(DBReadOnly.GetAllFileAndModifiedDates()[TestData.Image1.FileName], Is.EqualTo(modified1));

        var modified2 = DateTimeOffset.Now.Trim(TimeSpan.TicksPerSecond);
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
}