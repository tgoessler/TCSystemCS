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
    public class FaceTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FaceTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(Face d) => d.ToJsonString();
            Func<string, Face> fromJson = Face.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Face1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Face2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.FaceZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void InvalidateIdTest()
        {
            Assert.Fail();
        }
    }
}