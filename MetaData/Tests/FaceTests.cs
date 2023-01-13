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
    public class FaceTests
    {
        [Test]
        public void EqualsTest()
        {
            var face = TestData.Face1;
            Assert.That(face.Equals(TestData.Face1), Is.True);
            Assert.That(face.Equals(TestData.Face2), Is.False);
            Assert.That(face.Equals(TestData.FaceZero), Is.False);
            Assert.That(face.Equals(null), Is.False);
            Assert.That(face, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FaceTest()
        {
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
            var data1 = TestData.Face1;
            var copyOfData1 = new Face(data1.Id, data1.Rectangle, data1.FaceMode, data1.Visible, data1.FaceDescriptor);

            TestUtil.GetHashCodeTest(TestData.FaceZero, TestData.Face1,
                TestData.Face2, copyOfData1);
        }

        [Test]
        public void InvalidateIdTest()
        {
        }
    }
}