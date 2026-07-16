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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class RectangleTests
{
    [Test]
    public void EqualsTest()
    {
        Rectangle rectangle = TestData.Rectangle1;
        Assert.That(rectangle.Equals(TestData.Rectangle1), Is.True);
        Assert.That(rectangle.Equals(TestData.Rectangle2), Is.False);
        Assert.That(rectangle.Equals(TestData.RectangleZero), Is.False);
        Assert.That(rectangle.Equals(null), Is.False);
        Assert.That(rectangle, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FromFloatTest()
    {
        Rectangle r = Rectangle.FromFloat(1.0f, 2.0f, 3.0f, 4.0f);
        Assert.That(r.X, Is.EqualTo(new FixedPoint32(1.0f)));
        Assert.That(r.Y, Is.EqualTo(new FixedPoint32(2.0f)));
        Assert.That(r.W, Is.EqualTo(new FixedPoint32(3.0f)));
        Assert.That(r.H, Is.EqualTo(new FixedPoint32(4.0f)));
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(Rectangle d)
        {
            return d.ToJsonString();
        }

        Func<string, Rectangle> fromJson = Rectangle.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.Rectangle1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.Rectangle2, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.RectangleZero, ToJson, fromJson);
    }

    [Test]
    public void FromRawValuesTest()
    {
        Rectangle r = Rectangle.FromRawValues(10, 20, 30, 40);
        Assert.That(r.X, Is.EqualTo(new FixedPoint32(10)));
        Assert.That(r.Y, Is.EqualTo(new FixedPoint32(20)));
        Assert.That(r.W, Is.EqualTo(new FixedPoint32(30)));
        Assert.That(r.H, Is.EqualTo(new FixedPoint32(40)));
        Assert.That(r.Left, Is.EqualTo(new FixedPoint32(10)));
        Assert.That(r.Top, Is.EqualTo(new FixedPoint32(20)));
        Assert.That(r.Right, Is.EqualTo(new FixedPoint32(40)));
        Assert.That(r.Bottom, Is.EqualTo(new FixedPoint32(60)));
    }

    [Test]
    public void GetHashCodeTest()
    {
        Rectangle data1 = TestData.Rectangle1;
        var copyOfData1 = new Rectangle(data1.X, data1.Y, data1.W, data1.H);

        TestUtil.GetHashCodeTest(TestData.RectangleZero, TestData.Rectangle1,
            TestData.Rectangle2, copyOfData1);
    }

    [Test]
    public void RectangleCenterTest()
    {
        Assert.That(TestData.RectangleZero.Center, Is.EqualTo((TestData.FixedPoint32Zero, TestData.FixedPoint32Zero)));

        Rectangle rect = Rectangle.FromRawValues(-10, -5, 30, 40);
        Assert.That(rect.Center, Is.EqualTo((new FixedPoint32(5), new FixedPoint32(15))));

        rect = Rectangle.FromRawValues(10, 5, 30, 40);
        Assert.That(rect.Center, Is.EqualTo((new FixedPoint32(25), new FixedPoint32(25))));
    }

    [Test]
    public void RectangleContainsTest()
    {
        Rectangle rect1 = Rectangle.FromRawValues(-10, -5, 30, 40);
        Rectangle rect2 = Rectangle.FromRawValues(10, 5, 30, 40);
        Rectangle rect3 = Rectangle.FromRawValues(-10, -5, 29, 39);
        Rectangle rect4 = Rectangle.FromRawValues(11, 6, 29, 39);

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
    public void RectangleDiameterTest()
    {
        Assert.That(TestData.RectangleZero.Diameter, Is.EqualTo(TestData.FixedPoint32Zero));

        Rectangle rect = Rectangle.FromRawValues(-10, -5, 30, 40);
        Assert.That(rect.Diameter, Is.EqualTo(new FixedPoint32(50)));

        rect = Rectangle.FromRawValues(10, 5, 30, 40);
        Assert.That(rect.Diameter, Is.EqualTo(new FixedPoint32(50)));

        rect = Rectangle.FromFloat(0, 0, 1, 1);
        Assert.That(rect.Diameter.Value, Is.EqualTo(Math.Sqrt(2)).Within(0.0001));
    }

    [Test]
    public void RectangleTest()
    {
        var r = new Rectangle(new(1), new(2), new(3), new(4));
        Assert.That(r.X, Is.EqualTo(new FixedPoint32(1)));
        Assert.That(r.Y, Is.EqualTo(new FixedPoint32(2)));
        Assert.That(r.W, Is.EqualTo(new FixedPoint32(3)));
        Assert.That(r.H, Is.EqualTo(new FixedPoint32(4)));
        var rCopy = new Rectangle(r.X, r.Y, r.W, r.H);
        Assert.That(r == rCopy, Is.True);
        Assert.That(r != TestData.RectangleZero, Is.True);
    }

    [Test]
    public void ToStringTest()
    {
        var str = TestData.Rectangle1.ToString();
        Assert.That(str, Is.Not.Empty);
        // Format is "{X}, {Y}, {W}, {H}"
        Assert.That(str, Does.Contain(","));
    }
}