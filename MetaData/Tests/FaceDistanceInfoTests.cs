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
    public class FaceDistanceInfoTests
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
        public void FromJsonStringArrayTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FromJsonStringTest()
        {
            Assert.Fail();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToJsonStringTest()
        {
            string ToJson(FaceDistanceInfo d) => d.ToJsonString();
            Func<string, FaceDistanceInfo> fromJson = FaceDistanceInfo.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.FaceDistanceInfo1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.FaceDistanceInfo2, ToJson, fromJson);
        }
    }
}