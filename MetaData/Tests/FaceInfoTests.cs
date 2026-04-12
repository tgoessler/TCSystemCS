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
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class FaceInfoTests
{
    [Test]
    public void EqualsTest()
    {
        FaceInfo faceInfo = TestData.FaceInfo1;
        Assert.That(faceInfo.Equals(TestData.FaceInfo1), Is.True);
        Assert.That(faceInfo.Equals(TestData.FaceInfo2), Is.False);
        Assert.That(faceInfo.Equals(TestData.FaceInfoZero), Is.False);
        Assert.That(faceInfo.Equals(null), Is.False);
        Assert.That(faceInfo, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FaceInfoTest()
    {
        FaceInfo fi = TestData.FaceInfo1;
        Assert.That(fi.FileId, Is.EqualTo(1));
        Assert.That(fi.FaceId, Is.EqualTo(2));
        Assert.That(fi.PersonId, Is.EqualTo(3));
        Assert.That(fi.FaceMode, Is.EqualTo(FaceMode.DlibCnn));
        Assert.That(fi.FaceQuality, Is.EqualTo(FaceQuality.Normal));
        Assert.That(fi.FaceDescriptor, Is.Not.Null);

        // PersonId == EmptyPersonId should map to InvalidId
        var fi2 = new FaceInfo(1, 2, Constants.EmptyPersonId, FaceMode.DlibFront, FaceQuality.Good, null);
        Assert.That(fi2.PersonId, Is.EqualTo(Constants.InvalidId));
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(FaceInfo d)
        {
            return d.ToJsonString();
        }

        Func<string, FaceInfo> fromJson = FaceInfo.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.FaceInfo1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.FaceInfo2, ToJson, fromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        FaceInfo data1 = TestData.FaceInfo1;
        var copyOfData1 = new FaceInfo(data1.FileId, data1.FaceId,
            data1.PersonId, data1.FaceMode, data1.FaceQuality, data1.FaceDescriptor);

        TestUtil.GetHashCodeTest(TestData.FaceInfoZero, TestData.FaceInfo1,
            TestData.FaceInfo2, copyOfData1);
    }

    [Test]
    public void ToJsonStringArrayTest()
    {
        var faceInfos = new[] { TestData.FaceInfo1, TestData.FaceInfo2 };
        string json = FaceInfo.ToJsonStringArray(faceInfos);
        Assert.That(json, Is.Not.Empty);
        Assert.That(json, Does.StartWith("["));
    }

    [Test]
    public void ToJsonStringTest()
    {
        string json = TestData.FaceInfo1.ToJsonString();
        Assert.That(json, Is.Not.Empty);
    }
}