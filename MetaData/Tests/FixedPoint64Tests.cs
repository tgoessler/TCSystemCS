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
            Assert.Fail();
        }

        [Test]
        public void EqualsTest1()
        {
            Assert.Fail();
        }

        [Test]
        public void FixedPoint64Test()
        {
            Assert.Fail();
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
            Assert.Fail();
        }

        [Test]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}