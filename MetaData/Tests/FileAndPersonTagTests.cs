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
using System.Linq;
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class FileAndPersonTagTests
{
    [Test]
    public void EqualsTest()
    {
        FileAndPersonTag fileAndPersonTag = TestData.FileAndPersonTag1;
        Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTag1), Is.True);
        Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTag2), Is.False);
        Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTagZero), Is.False);
        Assert.That(fileAndPersonTag.Equals(null), Is.False);
        Assert.That(fileAndPersonTag, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FileAndPersonTagTest()
    {
        FileAndPersonTag fpt = TestData.FileAndPersonTag1;
        Assert.That(fpt.FileName, Is.EqualTo("file1"));
        Assert.That(fpt.PersonTag, Is.EqualTo(TestData.PersonTag1));

        // null filename defaults to empty string
        FileAndPersonTag zero = TestData.FileAndPersonTagZero;
        Assert.That(zero.FileName, Is.EqualTo(string.Empty));
    }

    [Test]
    public void FromJsonStringArrayTest()
    {
        var items = new[] { TestData.FileAndPersonTag1, TestData.FileAndPersonTag2 };
        string json = FileAndPersonTag.ToJsonStringArray(items);
        var parsed = FileAndPersonTag.FromJsonStringArray(json).ToArray();
        Assert.That(parsed.Length, Is.EqualTo(2));
        Assert.That(parsed[0], Is.EqualTo(TestData.FileAndPersonTag1));
        Assert.That(parsed[1], Is.EqualTo(TestData.FileAndPersonTag2));

        // Empty string returns empty
        var empty = FileAndPersonTag.FromJsonStringArray("").ToArray();
        Assert.That(empty.Length, Is.EqualTo(0));
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(FileAndPersonTag d)
        {
            return d.ToJsonString();
        }

        Func<string, FileAndPersonTag> fromJson = FileAndPersonTag.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.FileAndPersonTag1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.FileAndPersonTag2, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.FileAndPersonTagZero, ToJson, fromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        FileAndPersonTag data1 = TestData.FileAndPersonTag1;
        var copyOfData1 = new FileAndPersonTag(data1.FileName, data1.PersonTag);

        TestUtil.GetHashCodeTest(TestData.FileAndPersonTagZero, TestData.FileAndPersonTag1,
            TestData.FileAndPersonTag2, copyOfData1);
    }

    [Test]
    public void ToJsonStringArrayTest()
    {
        var items = new[] { TestData.FileAndPersonTag1, TestData.FileAndPersonTag2 };
        string json = FileAndPersonTag.ToJsonStringArray(items);
        Assert.That(json, Is.Not.Empty);
        Assert.That(json, Does.StartWith("["));
    }

    [Test]
    public void ToJsonStringTest()
    {
        string json = TestData.FileAndPersonTag1.ToJsonString();
        Assert.That(json, Is.Not.Empty);
    }

    [Test]
    public void ToStringTest()
    {
        string str = TestData.FileAndPersonTag1.ToString();
        Assert.That(str, Is.Not.Empty);
    }
}