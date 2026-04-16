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
public class FixedPoint32Tests
{
    [Test]
    public void EqualsTest()
    {
        FixedPoint32 fixedPoint32 = TestData.FixedPoint321;
        Assert.That(fixedPoint32.Equals(TestData.FixedPoint321), Is.True);
        Assert.That(fixedPoint32.Equals(TestData.FixedPoint322), Is.False);
        Assert.That(fixedPoint32.Equals(TestData.FixedPoint32Zero), Is.False);
        Assert.That(fixedPoint32.Equals(null), Is.False);
        Assert.That(fixedPoint32, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FixedPoint32Test()
    {
        // Constructor from raw int
        var fp = new FixedPoint32(100);
        Assert.That(fp.RawValue, Is.EqualTo(100));

        // Constructor from float: value * (1 << 16)
        var fpFloat = new FixedPoint32(1.0f);
        Assert.That(fpFloat.RawValue, Is.EqualTo(1 << 16));
        Assert.That(fpFloat.Value, Is.EqualTo(1.0f).Within(1e-4f));

        var fpNeg = new FixedPoint32(-2.0f);
        Assert.That(fpNeg.Value, Is.EqualTo(-2.0f).Within(1e-4f));

        // Zero
        Assert.That(TestData.FixedPoint32Zero.Value, Is.EqualTo(0.0f));

        // Comparison operators
        Assert.That(TestData.FixedPoint321 < TestData.FixedPoint322, Is.True);
        Assert.That(TestData.FixedPoint322 > TestData.FixedPoint321, Is.True);
        var fp1Copy = new FixedPoint32(TestData.FixedPoint321.RawValue);
        Assert.That(TestData.FixedPoint321 <= fp1Copy, Is.True);
        Assert.That(TestData.FixedPoint321 >= fp1Copy, Is.True);
        Assert.That(TestData.FixedPoint321 == fp1Copy, Is.True);
        Assert.That(TestData.FixedPoint321 != TestData.FixedPoint322, Is.True);
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(FixedPoint32 d)
        {
            return d.ToJsonString();
        }

        Func<string, FixedPoint32> fromJson = FixedPoint32.FromJsonString;

        TestUtil.FromJsonStringTest(new(10), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(-10), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(0), ToJson, fromJson);

        TestUtil.FromJsonStringTest(new(1.23f), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(-1.71f), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(0.0f), ToJson, fromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        FixedPoint32 data1 = TestData.FixedPoint321;
        var copyOfData1 = new FixedPoint32(data1.RawValue);

        TestUtil.GetHashCodeTest(TestData.FixedPoint32Zero, TestData.FixedPoint321,
            TestData.FixedPoint322, copyOfData1);
    }

    [Test]
    public void ToStringTest()
    {
        string s = new FixedPoint32(1.5f).ToString();
        Assert.That(s, Is.Not.Empty);
        // Should use invariant culture (dot as decimal separator)
        Assert.That(s, Does.Contain("."));
    }
}