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
public class FaceTests
{
    [Test]
    public void EqualsTest()
    {
        Face face = TestData.Face1;
        Assert.That(face.Equals(TestData.Face1), Is.True);
        Assert.That(face.Equals(TestData.Face2), Is.False);
        Assert.That(face.Equals(TestData.FaceZero), Is.False);
        Assert.That(face.Equals(null), Is.False);
        Assert.That(face, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FaceTest()
    {
        Face face = TestData.Face1;
        Assert.That(face.Id, Is.EqualTo(1));
        Assert.That(face.FaceMode, Is.EqualTo(FaceMode.DlibFront));
        Assert.That(face.FaceQuality, Is.EqualTo(FaceQuality.Good));
        Assert.That(face.Visible, Is.True);
        Assert.That(face.IsFrontFace, Is.True);
        Assert.That(face.HasFaceDescriptor, Is.True);
        Assert.That(face.FaceDescriptor.Count, Is.EqualTo(128));

        Face zero = TestData.FaceZero;
        Assert.That(zero.IsFrontFace, Is.False);
        Assert.That(zero.HasFaceDescriptor, Is.False);

        // Face descriptor with wrong length is discarded
        var wrongDesc = new FixedPoint64[5];
        var faceWithWrongDesc = new Face(1, TestData.Rectangle1, FaceMode.DlibFront, FaceQuality.Normal, true, wrongDesc);
        Assert.That(faceWithWrongDesc.HasFaceDescriptor, Is.False);
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(Face d)
        {
            return d.ToJsonString();
        }

        Func<string, Face> fromJson = Face.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.Face1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.Face2, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.FaceZero, ToJson, fromJson);

        foreach (FaceQuality faceQuality in new[] { FaceQuality.Unusable, FaceQuality.Excellent })
        {
            var face = new Face(TestData.FaceZero.Id, TestData.FaceZero.Rectangle, TestData.FaceZero.FaceMode, faceQuality,
                TestData.FaceZero.Visible, TestData.FaceZero.FaceDescriptor);
            TestUtil.FromJsonStringTest(face, ToJson, fromJson);
        }
    }

    [Test]
    public void GetHashCodeTest()
    {
        Face data1 = TestData.Face1;
        var copyOfData1 = new Face(data1.Id, data1.Rectangle, data1.FaceMode, data1.FaceQuality, data1.Visible, data1.FaceDescriptor);

        TestUtil.GetHashCodeTest(TestData.FaceZero, TestData.Face1,
            TestData.Face2, copyOfData1);
    }

    [Test]
    public void InvalidateIdTest()
    {
        Face invalidated = TestData.Face1.InvalidateId();
        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Rectangle, Is.EqualTo(TestData.Face1.Rectangle));
        Assert.That(invalidated.FaceMode, Is.EqualTo(TestData.Face1.FaceMode));
        Assert.That(invalidated.FaceQuality, Is.EqualTo(TestData.Face1.FaceQuality));
        Assert.That(invalidated.Visible, Is.EqualTo(TestData.Face1.Visible));
    }
}