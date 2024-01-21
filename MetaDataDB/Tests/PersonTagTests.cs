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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
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
public class PersonTagTests : DBSetup
{
    [Test]
    public void NumPersons()
    {
        Assert.That(DB.GetNumPersons(), Is.EqualTo(1));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(2));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(3));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(3));
    }

    [Test]
    public void AddPersonTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        var data = DB.GetMetaData(TestData.Image1.FileName);
        data = Image.AddPersonTag(data, TestData.PersonTag2);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DB.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DB.GetMetaData(TestData.Image1.FileName));
        Assert.That(data.PersonTags[0].Face.Id, Is.EqualTo(DB.GetMetaData(TestData.Image1.FileName).PersonTags[0].Face.Id));
    }

    [Test]
    public void RemovePersonTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        var data = DB.GetMetaData(TestData.Image1.FileName);
        data = Image.RemovePersonTag(data, TestData.PersonTag1);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DB.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DB.GetMetaData(TestData.Image1.FileName));
    }

    [Test]
    public void ChangeFaceToNewFace()
    {
        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        var personTag = data.PersonTags[1];
        data = Image.RemovePersonTag(data, personTag);
        data = Image.AddPersonTag(data, new PersonTag(personTag.Person, TestData.Face3));
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DB.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DB.GetMetaData(TestData.Image2.FileName));
    }

    [Test]
    public void ChangePersonTagVisible()
    {
        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        var personTag = data.PersonTags[1];
        data = Image.ChangePersonTagVisible(data, personTag, false);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DB.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DB.GetMetaData(TestData.Image2.FileName));
    }

    [Test]
    public void ChangePerson()
    {
        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        var personTag = data.PersonTags[1];
        data = Image.RemovePersonTag(data, personTag);
        data = Image.AddPersonTag(data, new PersonTag(TestData.Person3, personTag.Face));
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DB.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DB.GetMetaData(TestData.Image2.FileName));
    }

}