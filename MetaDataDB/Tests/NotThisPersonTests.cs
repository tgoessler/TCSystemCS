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

[TestFixture]
internal sealed class NotThisPersonTests : DBSetup
{
    [Test]
    public void GetNotThisPersonInformation0()
    {
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(0));
    }

    [Test]
    public void AddNotThisPersonInformation()
    {

        var data1 = Image.AddPersonTag(TestData.Image1, new (MetaData.Tests.TestData.PersonZero, TestData.Face3));
        data1 = DB.AddMetaData(data1, DateTimeOffset.Now);
        var data2 = Image.AddPersonTag(TestData.Image2, new (MetaData.Tests.TestData.PersonZero, TestData.Face3));
        data2 = DB.AddMetaData(data2, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(0));

        DB.AddNotThisPerson(data1.PersonTags[1].Face, data2.PersonTags[0].Person);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));

        DB.AddNotThisPerson(data1.PersonTags[1].Face, data2.PersonTags[1].Person);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][1], Is.EqualTo(data2.PersonTags[1].Person.Id));

        DB.AddNotThisPerson(data2.PersonTags[2].Face, data1.PersonTags[0].Person);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][1], Is.EqualTo(data2.PersonTags[1].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id].Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id][0], Is.EqualTo(data1.PersonTags[0].Person.Id));

        DB.RemoveMetaData(data2.FileName);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][1], Is.EqualTo(data2.PersonTags[1].Person.Id));

        // these calls will remove person if no other file uses it
        DB.GetNumFilesOfPerson(TestData.Person1.Name);
        DB.GetNumFilesOfPerson(TestData.Person2.Name);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));

        DB.RemoveMetaData(data1.FileName);
        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveNotThisPersonInformation()
    {
        var data1 = Image.AddPersonTag(TestData.Image1, new(MetaData.Tests.TestData.PersonZero, TestData.Face3));
        data1 = DB.AddMetaData(data1, DateTimeOffset.Now);
        var data2 = Image.AddPersonTag(TestData.Image2, new(MetaData.Tests.TestData.PersonZero, TestData.Face3));
        data2 = DB.AddMetaData(data2, DateTimeOffset.Now);

        DB.AddNotThisPerson(data1.PersonTags[1].Face, data2.PersonTags[0].Person);
        DB.AddNotThisPerson(data1.PersonTags[1].Face, data2.PersonTags[1].Person);
        DB.AddNotThisPerson(data2.PersonTags[2].Face, data1.PersonTags[0].Person);

        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id].Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][0], Is.EqualTo(data2.PersonTags[0].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data1.PersonTags[1].Face.Id][1], Is.EqualTo(data2.PersonTags[1].Person.Id));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id].Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id][0], Is.EqualTo(data1.PersonTags[0].Person.Id));

        var pt = data1.PersonTags[1];
        data1 = Image.RemovePersonTag(data1, pt);
        data1 = Image.AddPersonTag(data1, new (TestData.Person3, pt.Face));
        DB.AddMetaData(data1, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id].Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNotThisPersonInformation()[data2.PersonTags[2].Face.Id][0], Is.EqualTo(data1.PersonTags[0].Person.Id));

        pt = data2.PersonTags[2];
        data2 = Image.RemovePersonTag(data2, pt);
        data2 = Image.AddPersonTag(data2, new (TestData.Person3, pt.Face));
        DB.AddMetaData(data2, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNotThisPersonInformation().Count, Is.EqualTo(0));
    }


}