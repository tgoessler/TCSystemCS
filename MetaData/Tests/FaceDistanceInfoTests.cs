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
public class FaceDistanceInfoTests
{
    [Test]
    public void EqualsTest()
    {
        FaceDistanceInfo faceDistanceInfo = TestData.FaceDistanceInfo1;
        Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfo1), Is.True);
        Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfo2), Is.False);
        Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfoZero), Is.False);
        Assert.That(faceDistanceInfo.Equals(null), Is.False);
        Assert.That(faceDistanceInfo, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FromJsonStringArrayTest()
    {
        var items = new[] { TestData.FaceDistanceInfo1, TestData.FaceDistanceInfo2 };
        string json = "[" + string.Join(",", items.Select(i => i.ToJsonString())) + "]";
        var parsed = FaceDistanceInfo.FromJsonStringArray(json).ToArray();
        Assert.That(parsed.Length, Is.EqualTo(2));
        Assert.That(parsed[0], Is.EqualTo(TestData.FaceDistanceInfo1));
        Assert.That(parsed[1], Is.EqualTo(TestData.FaceDistanceInfo2));

        // Empty/null string returns empty
        var empty = FaceDistanceInfo.FromJsonStringArray("").ToArray();
        Assert.That(empty.Length, Is.EqualTo(0));
    }

    [Test]
    public void FromJsonStringTest()
    {
        FaceDistanceInfo original = TestData.FaceDistanceInfo1;
        string json = original.ToJsonString();
        FaceDistanceInfo parsed = FaceDistanceInfo.FromJsonString(json);
        Assert.That(parsed, Is.EqualTo(original));
    }

    [Test]
    public void GetHashCodeTest()
    {
        FaceDistanceInfo data1 = TestData.FaceDistanceInfo1;
        var copyOfData1 = new FaceDistanceInfo(data1.FaceId1, data1.FaceId1, data1.Distance);

        TestUtil.GetHashCodeTest(TestData.FaceDistanceInfoZero, TestData.FaceDistanceInfo1,
            TestData.FaceDistanceInfo2, copyOfData1);
    }

    [Test]
    public void ToJsonStringTest()
    {
        string ToJson(FaceDistanceInfo d)
        {
            return d.ToJsonString();
        }

        Func<string, FaceDistanceInfo> fromJson = FaceDistanceInfo.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.FaceDistanceInfo1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.FaceDistanceInfo2, ToJson, fromJson);
    }
}