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
using static TCSystem.MetaData.Tests.TestData;

#endregion

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public sealed class LocationTests : DBSetup
{
    [Test]
    public void GetAllLocationsLike()
    {
        Assert.That(DBReadOnly.GetAllLocationsLike().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address.Undefined), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllLocationsLike().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address.Undefined), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllLocationsLike().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address.Undefined), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address1), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Contains(Address1), Is.True);

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllLocationsLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address.Undefined), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address1), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike().Contains(Address2), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllLocationsLike("Steier").Contains(Address1), Is.True);
        Assert.That(DBReadOnly.GetAllLocationsLike("Kroa").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllLocationsLike("Kroa").Contains(Address2), Is.True);
    }

    [Test]
    public void GetFilesOfAddress()
    {
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Contains(TestData.FileName2), Is.True);

        DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Contains(TestData.FileName2), Is.True);

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.Image11.FileName), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Contains(TestData.FileName2), Is.True);

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.Image11.FileName), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Contains(TestData.Image11.FileName), Is.True);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image11.FileName);
        Assert.That(DBReadOnly.GetFilesOfAddress(Address1, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfAddress(Address2, true).Count, Is.EqualTo(0));
    }

    [Test]
    public void GetLocation()
    {
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetLocation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Location));
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetLocation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Location));
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetLocation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Location));
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetLocation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image11.FileName), Is.EqualTo(TestData.Image11.Location));
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));

        DB.AddMetaData(TestData.Image21, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetLocation(TestData.ImageZero.FileName), Is.EqualTo(TestData.ImageZero.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image1.FileName), Is.EqualTo(TestData.Image1.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image2.FileName), Is.EqualTo(TestData.Image2.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image11.FileName), Is.EqualTo(TestData.Image11.Location));
        Assert.That(DBReadOnly.GetLocation(TestData.Image21.FileName), Is.EqualTo(TestData.Image21.Location));
        Assert.That(DBReadOnly.GetLocation("XXX"), Is.EqualTo(null));
    }

    [Test]
    public void GetNumFilesOfAddress()
    {
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));

        DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image11, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image11.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));
    }

    [Test]
    public void GetNumLocations()
    {
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(1));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(2));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(3));

        DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(3));
    }

    [Test]
    public void RemoveAddress()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(3));

        // this now should remove the tags because we use write access
        Assert.That(DB.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DB.GetNumFilesOfAddress(Address2, true), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(2));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(2));

        // this now should remove the tags because we use write access
        Assert.That(DB.GetNumFilesOfAddress(Address1, true), Is.EqualTo(0));
        Assert.That(DB.GetNumFilesOfAddress(Address2, true), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumLocations(), Is.EqualTo(1));
    }
}