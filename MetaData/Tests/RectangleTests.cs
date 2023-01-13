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
        public void RectangleCenterTest()
        {
            Assert.That(TestData.RectangleZero.Center, Is.EqualTo((TestData.FixedPoint32Zero, TestData.FixedPoint32Zero)));

            var rect = Rectangle.FromRawValues(-10, -5, 30, 40);
            Assert.That(rect.Center, Is.EqualTo((new FixedPoint32(5), new FixedPoint32(15))));
            
            rect = Rectangle.FromRawValues(10, 5, 30, 40);
            Assert.That(rect.Center, Is.EqualTo((new FixedPoint32(25), new FixedPoint32(25))));
        }

        [Test]
        public void RectangleDiameterTest()
        {
            Assert.That(TestData.RectangleZero.Diameter, Is.EqualTo(TestData.FixedPoint32Zero));

            var rect = Rectangle.FromRawValues(-10, -5, 30, 40);
            Assert.That(rect.Diameter, Is.EqualTo(new FixedPoint32(50)));
            
            rect = Rectangle.FromRawValues(10, 5, 30, 40);
            Assert.That(rect.Diameter, Is.EqualTo(new FixedPoint32(50)));
        }

        [Test]
        public void RectangleContainsTest()
        {
            var rect1 = Rectangle.FromRawValues(-10, -5, 30, 40);
            var rect2 = Rectangle.FromRawValues(10, 5, 30, 40);
            var rect3 = Rectangle.FromRawValues(-10, -5, 29, 39);
            var rect4 = Rectangle.FromRawValues(11, 6, 29, 39);

            Assert.That(TestData.RectangleZero.Contains(rect1), Is.False);
            Assert.That(TestData.RectangleZero.Contains(rect2), Is.False);

            Assert.That(rect1.Contains(TestData.RectangleZero), Is.True);
            Assert.That(rect1.Contains(TestData.RectangleZero), Is.True);

            Assert.That(rect2.Contains(TestData.RectangleZero), Is.False);
            Assert.That(rect2.Contains(TestData.RectangleZero), Is.False);

            Assert.That(rect1.Contains(rect1), Is.True);
            Assert.That(rect1.Contains(rect1), Is.True);

            Assert.That(rect2.Contains(rect2), Is.True);
            Assert.That(rect2.Contains(rect2), Is.True);

            Assert.That(rect1.Contains(rect2), Is.False);
            Assert.That(rect1.Contains(rect2), Is.False);

            Assert.That(rect2.Contains(rect1), Is.False);
            Assert.That(rect2.Contains(rect1), Is.False);

            Assert.That(rect1.Contains(rect3), Is.True);
            Assert.That(rect1.Contains(rect3), Is.True);

            Assert.That(rect2.Contains(rect4), Is.True);
            Assert.That(rect2.Contains(rect4), Is.True);

        }

        [Test]
        public void ToStringTest()
        {
            
        }
    }
}