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
using NUnit.Framework;
using TCSystem.MetaData;

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public class DataTests : DBSetup
{
    [Test]
    public void GetAllYears()
    {
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(2));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(3));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(3));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(2));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image11.FileName);
        Assert.That(DBReadOnly.GetAllYears().Count, Is.EqualTo(0));

    }

        [Test]
    public void GetNumFilesOfYear()
    {
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(0));
        

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(1));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfYear(new DateTime(1900, 1, 1)), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.DateZero), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfYear(TestData.Date2), Is.EqualTo(0));
    }

    [Test]
    public void GetFilesOfYearYears()
    {
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(1));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(1));

        DB.RemoveMetaData(TestData.ImageZero.FileName);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.DateZero).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date1).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfYear(TestData.Date2).Count, Is.EqualTo(0));
    }

    [Test]
    public void GetOrientation()
    {
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetOrientation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Orientation));
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetOrientation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Orientation));
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetOrientation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Orientation));
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetOrientation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image11.FileName), Is.EqualTo(TestData.Image11.Orientation));
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));

        DB.AddMetaData(TestData.Image21, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetOrientation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image11.FileName), Is.EqualTo(TestData.Image11.Orientation));
        Assert.That(DBReadOnly.GetOrientation(TestData.Image21.FileName), Is.EqualTo(TestData.Image21.Orientation));
        Assert.That(DBReadOnly.GetOrientation("XXX"), Is.EqualTo(OrientationMode.Undefined));
    }
}