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
            var faceDistanceInfo = TestData.FaceDistanceInfo1;
            Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfo1), Is.True);
            Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfo2), Is.False);
            Assert.That(faceDistanceInfo.Equals(TestData.FaceDistanceInfoZero), Is.False);
            Assert.That(faceDistanceInfo.Equals(null), Is.False);
            Assert.That(faceDistanceInfo, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FromJsonStringArrayTest()
        {
        }

        [Test]
        public void FromJsonStringTest()
        {
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.FaceDistanceInfo1;
            var copyOfData1 = new FaceDistanceInfo(data1.FaceId1, data1.FaceId1, data1.Distance);

            TestUtil.GetHashCodeTest(TestData.FaceDistanceInfoZero, TestData.FaceDistanceInfo1,
                TestData.FaceDistanceInfo2, copyOfData1);
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