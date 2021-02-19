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
    public class PersonTagTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(PersonTag d) => d.ToJsonString();
            Func<string, PersonTag> fromJson = PersonTag.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.PersonTag1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PersonTag2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PersonTagZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void InvalidateIdsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PersonTagTest()
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