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
    public class FileAndPersonTagTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FileAndPersonTagTest()
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
            string ToJson(FileAndPersonTag d) => d.ToJsonString();
            Func<string, FileAndPersonTag> fromJson = FileAndPersonTag.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.FileAndPersonTag1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.FileAndPersonTag2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.FileAndPersonTagZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToJsonStringArrayTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToJsonStringTest()
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