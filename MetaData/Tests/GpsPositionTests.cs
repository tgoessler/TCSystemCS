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
    public class GpsPositionTests
    {
        [Test]
        public void EqualsTest()
        {
            var point1 = TestData.PositionZero;
            var point2 = point1;
            Assert.That(point1.Equals(null), Is.False);
            Assert.That(point1, Is.Not.EqualTo(TestData.Address1));
            Assert.That(point1.Equals(point2), Is.True);

            point2 = new GpsPosition(0, 0, 0, 0, false);
            Assert.That(point1, Is.EqualTo(point2));

            point2 = new GpsPosition(1, 0, 0, 0, false);
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = new GpsPosition(0, 1, 0, 0, false);
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = new GpsPosition(0, 0, 1, 0, false);
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = new GpsPosition(0, 0, 0, 1, false);
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = new GpsPosition(0, 0, 0, 0, true);
            Assert.That(point1, Is.Not.EqualTo(point2));
        }

        [Test]
        public void FromDoublePositionTest()
        {
            var point1 = TestData.Position1;
            var pos = point1.ToDouble();
            var point2 = GpsPosition.FromDoublePosition(pos);
            Assert.That(point1, Is.EqualTo(point2));

            point1 = TestData.Position2;
            pos = point1.ToDouble();
            point2 = GpsPosition.FromDoublePosition(pos);
            Assert.That(point1, Is.EqualTo(point2));
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(GpsPosition d) => d.ToJsonString();
            Func<string, GpsPosition> fromJson = GpsPosition.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Position1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Position2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PositionZero, ToJson, fromJson);
        }

        [Test]
        public void FromStringTest()
        {
            var point1 = TestData.Position1;
            var jsonString = point1.ToString();
            var point2 = GpsPosition.FromString(jsonString);
            Assert.That(point1, Is.EqualTo(point2));

            point1 = TestData.Position2;
            jsonString = point1.ToString();
            point2 = GpsPosition.FromString(jsonString);
            Assert.That(point1, Is.EqualTo(point2));

            point2 = GpsPosition.FromString("fsdf");
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = GpsPosition.FromString("");
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = GpsPosition.FromString(null);
            Assert.That(point1, Is.Not.EqualTo(point2));
        }

        [Test]
        public void GetHashCodeTest()
        {
            var point1 = TestData.Position1;
            var point2 = new GpsPosition(TestData.Position1.Degrees,
                TestData.Position1.Minutes,
                TestData.Position1.Seconds,
                TestData.Position1.SubSeconds,
                TestData.Position1.Negative);
            Assert.That(point1.GetHashCode(), Is.EqualTo(point2.GetHashCode()));

            point2 = TestData.Position2;
            Assert.That(point1.GetHashCode(), Is.Not.EqualTo(point2.GetHashCode()));
        }

        [Test]
        public void GpsPositionTest()
        {
            var point = TestData.Position2;
            Assert.That(point.Degrees, Is.EqualTo(47));
            Assert.That(point.Minutes, Is.EqualTo(4));
            Assert.That(point.Seconds, Is.EqualTo(15));
            Assert.That(point.SubSeconds, Is.EqualTo(16));
            Assert.That(point.Negative, Is.EqualTo(true));

            point = TestData.Position1;
            Assert.That(point.Negative, Is.EqualTo(false));
        }
    }
}