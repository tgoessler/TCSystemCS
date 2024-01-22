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
using System.Linq;
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public class PersonTagTests : DBSetup
{
    [Test]
    public void GetNumPersons()
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
    public void GetNumFaces()
    {
        Assert.That(DB.GetNumFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DB.GetNumFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DB.GetNumFaces(), Is.EqualTo(2));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumPersons(), Is.EqualTo(3));
    }

    [Test]
    public void GetNumAutoDetectedFaces()
    {
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(2));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetNumAutoDetectedFaces(), Is.EqualTo(2));
    }

    [Test]
    public void GetAllPersonNamesLike()
    {
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(1));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(1));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(2));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DB.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);
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
        data = Image.ChangePersonTagVisible(data, personTag, true);
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

    [Test]
    public void GetAllFaceInfos()
    {
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(0));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(0));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(1));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(2));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(1));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(2));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetAllFaceInfos(true).Count, Is.EqualTo(2));
        Assert.That(DB.GetAllFaceInfos(false).Count, Is.EqualTo(3));
    }

    [Test]
    public void GetPersonFromName()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DB.GetPersonFromName(TestData.Person1.Name).InvalidateId(), Is.EqualTo(TestData.Person1));
        Assert.That(DB.GetPersonFromName(TestData.Person2.Name), Is.Null);

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DB.GetPersonFromName(TestData.Person1.Name).InvalidateId(), Is.EqualTo(TestData.Person1));
        Assert.That(DB.GetPersonFromName(TestData.Person2.Name).InvalidateId(), Is.EqualTo(TestData.Person2));
    }
}