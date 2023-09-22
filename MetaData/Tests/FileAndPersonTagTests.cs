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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
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
            var fileAndPersonTag = TestData.FileAndPersonTag1;
            Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTag1), Is.True);
            Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTag2), Is.False);
            Assert.That(fileAndPersonTag.Equals(TestData.FileAndPersonTagZero), Is.False);
            Assert.That(fileAndPersonTag.Equals(null), Is.False);
            Assert.That(fileAndPersonTag, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FileAndPersonTagTest()
        {
        }

        [Test]
        public void FromJsonStringArrayTest()
        {
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
            var data1 = TestData.FileAndPersonTag1;
            var copyOfData1 = new FileAndPersonTag(data1.FileName, data1.PersonTag);

            TestUtil.GetHashCodeTest(TestData.FileAndPersonTagZero, TestData.FileAndPersonTag1,
                TestData.FileAndPersonTag2, copyOfData1);
        }

        [Test]
        public void ToJsonStringArrayTest()
        {
        }

        [Test]
        public void ToJsonStringTest()
        {
        }

        [Test]
        public void ToStringTest()
        {
        }
    }
}