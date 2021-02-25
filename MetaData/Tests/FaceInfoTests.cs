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
    public class FaceInfoTests
    {
        [Test]
        public void EqualsTest()
        {
            var faceInfo = TestData.FaceInfo1;
            Assert.That(faceInfo.Equals(TestData.FaceInfo1), Is.True);
            Assert.That(faceInfo.Equals(TestData.FaceInfo2), Is.False);
            Assert.That(faceInfo.Equals(TestData.FaceInfoZero), Is.False);
            Assert.That(faceInfo.Equals(null), Is.False);
            Assert.That(faceInfo, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FaceInfoTest()
        {
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(FaceInfo d) => d.ToJsonString();
            Func<string, FaceInfo> fromJson = FaceInfo.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.FaceInfo1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.FaceInfo2, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.FaceInfo1;
            var copyOfData1 = new FaceInfo(data1.FileId, data1.FaceId, 
                data1.PersonId, data1.FaceMode, data1.FaceDescriptor);

            TestUtil.GetHashCodeTest(TestData.FaceInfoZero, TestData.FaceInfo1,
                TestData.FaceInfo2, copyOfData1);
        }

        [Test]
        public void ToJsonStringArrayTest()
        {
        }

        [Test]
        public void ToJsonStringTest()
        {
        }
    }
}