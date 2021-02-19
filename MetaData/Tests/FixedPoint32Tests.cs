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
            Assert.Fail();
        }

        [Test]
        public void EqualsTest1()
        {
            Assert.Fail();
        }

        [Test]
        public void FixedPoint32Test()
        {
            Assert.Fail();
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
            Assert.Fail();
        }

        [Test]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}