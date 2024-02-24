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
public class PersonTagTests : DBSetup
{
    [Test]
    public void AddPersonTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        Image data = DBReadOnly.GetMetaData(TestData.Image1.FileName);
        data = Image.AddPersonTag(data, TestData.PersonTag2);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image1.FileName));
        Assert.That(data.PersonTags[0].Face.Id, Is.EqualTo(DBReadOnly.GetMetaData(TestData.Image1.FileName).PersonTags[0].Face.Id));
    }

    [Test]
    public void ChangeFaceToNewFace()
    {
        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        PersonTag personTag = data.PersonTags[1];
        data = Image.RemovePersonTag(data, personTag);
        data = Image.AddPersonTag(data, new(personTag.Person, TestData.Face3));
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image2.FileName));
    }

    [Test]
    public void ChangePerson()
    {
        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        PersonTag personTag = data.PersonTags[1];
        data = Image.RemovePersonTag(data, personTag);
        data = Image.AddPersonTag(data, new(TestData.Person3, personTag.Face));
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image2.FileName));
    }

    [Test]
    public void ChangePersonTagVisible()
    {
        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

        PersonTag personTag = data.PersonTags[1];
        data = Image.ChangePersonTagVisible(data, personTag, true);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image2.FileName));
    }

    [Test]
    public void GetAllFaceInfos()
    {
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(2));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllFaceInfos(true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllFaceInfos(false).Count, Is.EqualTo(3));
    }

    [Test]
    public void GetAllPersonNamesLike()
    {
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == ""), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike().FirstOrDefault(n => n == TestData.Person2.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("Tho").FirstOrDefault(n => n == TestData.Person1.Name), Is.Not.Null);
        Assert.That(DBReadOnly.GetAllPersonNamesLike("abc").FirstOrDefault(n => n == ""), Is.Null);
    }

    [Test]
    public void GetFileAndPersonTagFromFaceId()
    {
        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[0].Face.Id, false).PersonTag, Is.EqualTo(data.PersonTags[0]));
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[0].Face.Id, false).FileName, Is.EqualTo(data.FileName));
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[1].Face.Id, false).PersonTag, Is.EqualTo(data.PersonTags[1]));
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[1].Face.Id, false).FileName, Is.EqualTo(data.FileName));

        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[0].Face.Id, true).PersonTag, Is.EqualTo(data.PersonTags[0]));
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[0].Face.Id, true).FileName, Is.EqualTo(data.FileName));
        Assert.That(DBReadOnly.GetFileAndPersonTagFromFaceId(data.PersonTags[1].Face.Id, true), Is.Null);
    }

    [Test]
    public void GetFileAndPersonTags()
    {
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));

        data = DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));

        DB.RemoveMetaData(data.FileName);
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person1.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person2.Name, false).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, true).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFileAndPersonTagsOfPerson(TestData.Person3.Name, false).Count, Is.EqualTo(0));
    }

    [Test]
    public void GetFilesOfPerson()
    {
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName2), Is.True);

        data = DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName2), Is.True);

        DB.RemoveMetaData(data.FileName);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person1.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person2.Name).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfPerson(TestData.Person3.Name).Count, Is.EqualTo(0));
    }

    [Test]
    public void GetNumAutoDetectedFaces()
    {
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(2));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumAutoDetectedFaces(), Is.EqualTo(2));
    }

    [Test]
    public void GetNumFaces()
    {
        Assert.That(DBReadOnly.GetNumFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFaces(), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFaces(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFaces(), Is.EqualTo(2));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));
    }

    [Test]
    public void GetNumFilesOfPerson()
    {
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));

        Image data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));

        DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person3.Name), Is.EqualTo(0));
    }

    [Test]
    public void GetNumPersons()
    {
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(1));

        DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(1));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(2));

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));
    }

    [Test]
    public void GetPersonFromId()
    {
        Image data1 = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetPersonFromId(data1.PersonTags[0].Person.Id).InvalidateId(), Is.EqualTo(TestData.Person1));

        Image data2 = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(data2, Is.EqualTo(data2));
        Assert.That(DBReadOnly.GetPersonFromId(data2.PersonTags[0].Person.Id).InvalidateId(), Is.EqualTo(TestData.Person1));
        Assert.That(DBReadOnly.GetPersonFromId(data2.PersonTags[1].Person.Id).InvalidateId(), Is.EqualTo(TestData.Person2));
    }

    [Test]
    public void GetPersonFromName()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetPersonFromName(TestData.Person1.Name).InvalidateId(), Is.EqualTo(TestData.Person1));
        Assert.That(DBReadOnly.GetPersonFromName(TestData.Person2.Name), Is.Null);

        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetPersonFromName(TestData.Person1.Name).InvalidateId(), Is.EqualTo(TestData.Person1));
        Assert.That(DBReadOnly.GetPersonFromName(TestData.Person2.Name).InvalidateId(), Is.EqualTo(TestData.Person2));
    }

    [Test]
    public void RemovePerson()
    {
        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(1));

        DB.RemoveMetaData(TestData.Image2.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumPersons(), Is.EqualTo(3));

        // this now should remove the tags because we use write access
        Assert.That(DB.GetNumFilesOfPerson(TestData.Person1.Name), Is.EqualTo(0));
        Assert.That(DB.GetNumPersons(), Is.EqualTo(2));
        Assert.That(DB.GetNumFilesOfPerson(TestData.Person2.Name), Is.EqualTo(0));
        Assert.That(DB.GetNumPersons(), Is.EqualTo(1));
    }

    [Test]
    public void RemovePersonTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        Image data = DBReadOnly.GetMetaData(TestData.Image1.FileName);
        data = Image.RemovePersonTag(data, TestData.PersonTag1);
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image1.FileName));
    }

    [Test]
    public void UpdatePerson()
    {
        Image data = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

        Person p = data.PersonTags[0].Person;
        data = Image.ChangePerson(data, new(p.Id, p.Name, "new@email.com", p.LiveId, p.SourceId));
        DB.AddMetaData(data, DateTimeOffset.Now);

        Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        Assert.That(data.PersonTags[0].Person.Id, Is.EqualTo(DBReadOnly.GetMetaData(TestData.Image1.FileName).PersonTags[0].Person.Id));
        AssertImageDataNotEqual(data, DBReadOnly.GetMetaData(TestData.Image1.FileName));
    }
}