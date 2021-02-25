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
    public class RectangleTests
    {
        [Test]
        public void EqualsTest()
        {
            var rectangle = TestData.Rectangle1;
            Assert.That(rectangle.Equals(TestData.Rectangle1), Is.True);
            Assert.That(rectangle.Equals(TestData.Rectangle2), Is.False);
            Assert.That(rectangle.Equals(TestData.RectangleZero), Is.False);
            Assert.That(rectangle.Equals(null), Is.False);
            Assert.That(rectangle, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FromFloatTest()
        {
            
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(Rectangle d) => d.ToJsonString();
            Func<string, Rectangle> fromJson = Rectangle.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Rectangle1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Rectangle2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.RectangleZero, ToJson, fromJson);
        }

        [Test]
        public void FromRawValuesTest()
        {
            
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.Rectangle1;
            var copyOfData1 = new Rectangle(data1.X, data1.Y, data1.W, data1.H);

            TestUtil.GetHashCodeTest(TestData.RectangleZero, TestData.Rectangle1,
                TestData.Rectangle2, copyOfData1);
        }

        [Test]
        public void RectangleTest()
        {
            
        }

        [Test]
        public void ToStringTest()
        {
            
        }
    }
}