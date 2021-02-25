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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.MetaData.Tests
{
    [TestFixture]
    public class FixedPoint32Tests
    {
        [Test]
        public void EqualsTest()
        {
            var fixedPoint32 = TestData.FixedPoint321;
            Assert.That(fixedPoint32.Equals(TestData.FixedPoint321), Is.True);
            Assert.That(fixedPoint32.Equals(TestData.FixedPoint322), Is.False);
            Assert.That(fixedPoint32.Equals(TestData.FixedPoint32Zero), Is.False);
            Assert.That(fixedPoint32.Equals(null), Is.False);
            Assert.That(fixedPoint32, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FixedPoint32Test()
        {
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(FixedPoint32 d) => d.ToJsonString();
            Func<string, FixedPoint32> fromJson = FixedPoint32.FromJsonString;

            TestUtil.FromJsonStringTest(new FixedPoint32(10), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint32(-10), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint32(0), ToJson, fromJson);

            TestUtil.FromJsonStringTest(new FixedPoint32(1.23f), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint32(-1.71f), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint32(0.0f), ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.FixedPoint321;
            var copyOfData1 = new FixedPoint32(data1.RawValue);

            TestUtil.GetHashCodeTest(TestData.FixedPoint32Zero, TestData.FixedPoint321,
                TestData.FixedPoint322, copyOfData1);
        }

        [Test]
        public void ToStringTest()
        {
        }
    }
}