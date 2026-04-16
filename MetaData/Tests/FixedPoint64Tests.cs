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
public class FixedPoint64Tests
{
    [Test]
    public void EqualsTest()
    {
        FixedPoint64 fixedPoint64 = TestData.FixedPoint641;
        Assert.That(fixedPoint64.Equals(TestData.FixedPoint641), Is.True);
        Assert.That(fixedPoint64.Equals(TestData.FixedPoint642), Is.False);
        Assert.That(fixedPoint64.Equals(TestData.FixedPoint64Zero), Is.False);
        Assert.That(fixedPoint64.Equals(null), Is.False);
        Assert.That(fixedPoint64, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FixedPoint64Test()
    {
        // Constructor from raw long
        var fp = new FixedPoint64(100L);
        Assert.That(fp.RawValue, Is.EqualTo(100L));

        // Constructor from double: value * (1L << 32)
        var fpDouble = new FixedPoint64(1.0);
        Assert.That(fpDouble.RawValue, Is.EqualTo(1L << 32));
        Assert.That(fpDouble.Value, Is.EqualTo(1.0).Within(1e-9));

        var fpNeg = new FixedPoint64(-2.0);
        Assert.That(fpNeg.Value, Is.EqualTo(-2.0).Within(1e-9));

        // Zero
        Assert.That(TestData.FixedPoint64Zero.Value, Is.EqualTo(0.0));

        // Operators
        var fp641Copy = new FixedPoint64(TestData.FixedPoint641.RawValue);
        Assert.That(TestData.FixedPoint641 == fp641Copy, Is.True);
        Assert.That(TestData.FixedPoint641 != TestData.FixedPoint642, Is.True);
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(FixedPoint64 d)
        {
            return d.ToJsonString();
        }

        Func<string, FixedPoint64> fromJson = FixedPoint64.FromJsonString;

        TestUtil.FromJsonStringTest(new(10), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(-10), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(0), ToJson, fromJson);

        TestUtil.FromJsonStringTest(new(1.23), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(-1.71), ToJson, fromJson);
        TestUtil.FromJsonStringTest(new(0.0), ToJson, fromJson);
    }


    [Test]
    public void GetHashCodeTest()
    {
        FixedPoint64 data1 = TestData.FixedPoint641;
        var copyOfData1 = new FixedPoint64(data1.RawValue);

        TestUtil.GetHashCodeTest(TestData.FixedPoint64Zero, TestData.FixedPoint641,
            TestData.FixedPoint642, copyOfData1);
    }

    [Test]
    public void ToStringTest()
    {
        string s = new FixedPoint64(1.5).ToString();
        Assert.That(s, Is.Not.Empty);
        Assert.That(s, Does.Contain("."));
    }
}