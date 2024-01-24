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

using NUnit.Framework;
using System;

#endregion

namespace TCSystem.MetaData.Tests
{
    [TestFixture]
    public class FixedPoint64Tests
    {
        [Test]
        public void EqualsTest()
        {
            var fixedPoint64 = TestData.FixedPoint641;
            Assert.That(fixedPoint64.Equals(TestData.FixedPoint641), Is.True);
            Assert.That(fixedPoint64.Equals(TestData.FixedPoint642), Is.False);
            Assert.That(fixedPoint64.Equals(TestData.FixedPoint64Zero), Is.False);
            Assert.That(fixedPoint64.Equals(null), Is.False);
            Assert.That(fixedPoint64, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FixedPoint64Test()
        {
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(FixedPoint64 d) => d.ToJsonString();
            Func<string, FixedPoint64> fromJson = FixedPoint64.FromJsonString;

            TestUtil.FromJsonStringTest(new FixedPoint64(10), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint64(-10), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint64(0), ToJson, fromJson);

            TestUtil.FromJsonStringTest(new FixedPoint64(1.23), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint64(-1.71), ToJson, fromJson);
            TestUtil.FromJsonStringTest(new FixedPoint64(0.0), ToJson, fromJson);
        }


        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.FixedPoint641;
            var copyOfData1 = new FixedPoint64(data1.RawValue);

            TestUtil.GetHashCodeTest(TestData.FixedPoint64Zero, TestData.FixedPoint641,
                TestData.FixedPoint642, copyOfData1);
        }

        [Test]
        public void ToStringTest()
        {
        }
    }
}