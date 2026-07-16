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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class UtilTests
{
    [Test]
    public void GetValueOrDefault_EmptyDictionary_ReturnsDefault()
    {
        var dict = new Dictionary<string, string>();
        Assert.That(dict.GetValueOrDefault("key", "default"), Is.EqualTo("default"));
    }

    [Test]
    public void GetValueOrDefault_ExistingKey_ReturnsValue()
    {
        var dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.That(dict.GetValueOrDefault("key", 0), Is.EqualTo(42));
    }

    [Test]
    public void GetValueOrDefault_MissingKey_ReturnsDefault()
    {
        var dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.That(dict.GetValueOrDefault("missing", -1), Is.EqualTo(-1));
    }

    [Test]
    public void HasMinimalDifference_DifferentValues_ReturnsFalse()
    {
        Assert.That(Util.HasMinimalDifference(1.0, 2.0), Is.False);
    }

    [Test]
    public void HasMinimalDifference_OppositeSign_ReturnsFalse()
    {
        Assert.That(Util.HasMinimalDifference(1.0, -1.0), Is.False);
    }

    [Test]
    public void HasMinimalDifference_PositiveAndNegativeZero_ReturnsTrue()
    {
        Assert.That(Util.HasMinimalDifference(0.0, -0.0), Is.True);
    }

    [Test]
    public void HasMinimalDifference_SameValues_ReturnsTrue()
    {
        Assert.That(Util.HasMinimalDifference(1.0, 1.0), Is.True);
    }

    [Test]
    public void HasMinimalDifference_VeryCloseValues_ReturnsTrue()
    {
        Assert.That(Util.HasMinimalDifference(1.0, 1.0 + double.Epsilon), Is.True);
    }

    [Test]
    public void HasMinimalDifference_Zeros_ReturnsTrue()
    {
        Assert.That(Util.HasMinimalDifference(0.0, 0.0), Is.True);
    }

    [Test]
    public void IsSupportedFileType_JpegExtension_ReturnsTrue()
    {
        Assert.That(Util.IsSupportedFileType("photo.jpeg"), Is.True);
    }

    [Test]
    public void IsSupportedFileType_JpgExtension_ReturnsTrue()
    {
        Assert.That(Util.IsSupportedFileType("photo.jpg"), Is.True);
    }

    [Test]
    public void IsSupportedFileType_NoExtension_ReturnsFalse()
    {
        Assert.That(Util.IsSupportedFileType("photo"), Is.False);
    }

    [Test]
    public void IsSupportedFileType_PngExtension_ReturnsFalse()
    {
        Assert.That(Util.IsSupportedFileType("photo.png"), Is.False);
    }

    [Test]
    public void IsSupportedFileType_RecycleBinCaseInsensitive_ReturnsFalse()
    {
        Assert.That(Util.IsSupportedFileType(@"C:\$recycle.bin\photo.jpg"), Is.False);
    }

    [Test]
    public void IsSupportedFileType_RecycleBinPath_ReturnsFalse()
    {
        Assert.That(Util.IsSupportedFileType(@"C:\$RECYCLE.BIN\photo.jpg"), Is.False);
    }

    [Test]
    public void IsSupportedFileType_UpperCaseJpg_ReturnsFalse()
    {
        Assert.That(Util.IsSupportedFileType("photo.JPG"), Is.True);
    }

    [Test]
    public void SortPersonTags_EmptyCollection_ReturnsEmpty()
    {
        List<PersonTag> sorted = Util.SortPersonTags(Array.Empty<PersonTag>()).ToList();
        Assert.That(sorted, Is.Empty);
    }

    [Test]
    public void SortPersonTags_SingleItem_ReturnsSameItem()
    {
        PersonTag[] tags = [TestData.PersonTag1];
        List<PersonTag> sorted = Util.SortPersonTags(tags).ToList();

        Assert.That(sorted, Has.Count.EqualTo(1));
        Assert.That(sorted[0], Is.EqualTo(TestData.PersonTag1));
    }

    [Test]
    public void SortPersonTags_SortsAlphabeticallyWithEmptyLast()
    {
        var emptyPerson = new Person(3, "", "", "", "");
        var emptyTag = new PersonTag(emptyPerson, TestData.FaceZero);

        PersonTag[] tags = [TestData.PersonTag2, emptyTag, TestData.PersonTag1];
        List<PersonTag> sorted = Util.SortPersonTags(tags).ToList();

        Assert.That(sorted[0].Person.Name, Is.EqualTo("Sabine"));
        Assert.That(sorted[1].Person.Name, Is.EqualTo("Thomas"));
        Assert.That(sorted[2].Person.Name, Is.EqualTo(""));
    }

    [Test]
    public void SupportedFileTypes_ContainsJpgAndJpeg()
    {
        List<string> types = Util.SupportedFileTypes.ToList();
        Assert.That(types, Does.Contain(".jpg"));
        Assert.That(types, Does.Contain(".jpeg"));
    }

    [Test]
    public void Trim_ToMinutes_TruncatesSeconds()
    {
        var date = new DateTimeOffset(2024, 6, 15, 10, 30, 45, 500, TimeSpan.Zero);
        DateTimeOffset trimmed = date.Trim(TimeSpan.TicksPerMinute);

        Assert.That(trimmed.Second, Is.EqualTo(0));
        Assert.That(trimmed.Millisecond, Is.EqualTo(0));
        Assert.That(trimmed.Minute, Is.EqualTo(30));
    }

    [Test]
    public void Trim_ToSeconds_TruncatesMilliseconds()
    {
        var date = new DateTimeOffset(2024, 6, 15, 10, 30, 45, 500, TimeSpan.Zero);
        DateTimeOffset trimmed = date.Trim(TimeSpan.TicksPerSecond);

        Assert.That(trimmed.Millisecond, Is.EqualTo(0));
        Assert.That(trimmed.Second, Is.EqualTo(45));
    }
}