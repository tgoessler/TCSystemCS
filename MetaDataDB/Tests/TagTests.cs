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
public class TagTests : DBSetup
{
    [Test]
    public void AddFileTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        var data = DBReadOnly.GetMetaData(TestData.Image1.FileName);

        data = Image.AddTag(data, "Hello1");

        DB.AddMetaData(data, DateTimeOffset.Now);
        AssertImageDataNotEqual(DBReadOnly.GetMetaData(data.FileName), data);

        data = Image.AddTag(data, "Hello2");
        data = Image.AddTag(data, "Hello3");

        DB.AddMetaData(data, DateTimeOffset.Now);
        AssertImageDataNotEqual(DBReadOnly.GetMetaData(data.FileName), data);
    }

    [Test]
    public void RemoveFileTag()
    {
        DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        var data = DBReadOnly.GetMetaData(TestData.Image2.FileName);

        data = Image.RemoveTag(data, TestData.Tag2);
        DB.AddMetaData(data, DateTimeOffset.Now);
        AssertImageDataNotEqual(DBReadOnly.GetMetaData(data.FileName), data);

        data = Image.RemoveTag(data, TestData.Tag3);
        DB.AddMetaData(data, DateTimeOffset.Now);
        AssertImageDataNotEqual(DBReadOnly.GetMetaData(data.FileName), data);

        data = Image.RemoveTag(data, TestData.Tag1);
        DB.AddMetaData(data, DateTimeOffset.Now);
        AssertImageDataNotEqual(DBReadOnly.GetMetaData(data.FileName), data);
    }

    [Test]
    public void GetNumTags()
    {
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllTagsLike().Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetAllTagsLike("01").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("02").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("03").Count, Is.EqualTo(0));

        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllTagsLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllTagsLike("01").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("02").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("03").Count, Is.EqualTo(1));

        DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllTagsLike().Count, Is.EqualTo(3));
        Assert.That(DBReadOnly.GetAllTagsLike("01").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("02").Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetAllTagsLike("03").Count, Is.EqualTo(1));
    }

    [Test]
    public void GetTags()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        var tags = DBReadOnly.GetAllTagsLike().OrderBy(s => s).ToArray();
        Assert.That(tags[0], Is.EqualTo(TestData.Tag1));
        Assert.That(tags[1], Is.EqualTo(TestData.Tag2));

        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        tags = DBReadOnly.GetAllTagsLike().OrderBy(s => s).ToArray();
        Assert.That(tags[0], Is.EqualTo(TestData.Tag1));
        Assert.That(tags[1], Is.EqualTo(TestData.Tag2));
        Assert.That(tags[2], Is.EqualTo(TestData.Tag3));

        DB.AddMetaData(data, DateTimeOffset.Now);
        tags = DBReadOnly.GetAllTagsLike("02").ToArray();
        Assert.That(tags[0], Is.EqualTo(TestData.Tag2));
    }

    [Test]
    public void GetNumFilesOfTag()
    {
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(0));

        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(1));

        data = DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(2));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(1));

        DB.RemoveMetaData(data.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag3), Is.EqualTo(0));
    }

    [Test]
    public void GetFilesOfTag()
    {
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(0));

        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(0));

        var data = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);

        data = DB.AddMetaData(data, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(2));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName2), Is.True);

        DB.RemoveMetaData(data.FileName);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(1));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Contains(TestData.FileName1), Is.True);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(0));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag1).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag2).Count, Is.EqualTo(0));
        Assert.That(DBReadOnly.GetFilesOfTag(TestData.Tag3).Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveTag()
    {
        DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(1));
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(2));

        DB.RemoveMetaData(TestData.Image1.FileName);
        Assert.That(DBReadOnly.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(0));
        Assert.That(DBReadOnly.GetNumTags(), Is.EqualTo(2));

        // this now should remove the tags because we use write access
        Assert.That(DB.GetNumFilesOfTag(TestData.Tag1), Is.EqualTo(0));
        Assert.That(DB.GetNumTags(), Is.EqualTo(1));
        Assert.That(DB.GetNumFilesOfTag(TestData.Tag2), Is.EqualTo(0));
        Assert.That(DB.GetNumTags(), Is.EqualTo(0));
    }
}