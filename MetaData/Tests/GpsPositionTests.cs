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

#endregion

namespace TCSystem.MetaData.Tests
{
    [TestFixture]
    public class GpsPositionTests
    {
        [Test]
        public void EqualsTest()
        {
            var point1 = TestData.TestPositionZero;
            var point2 = point1;
            Assert.That(point1.Equals(null), Is.False);
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
            var point1 = TestData.TestPosition1;
            var pos = point1.ToDouble();
            var point2 = GpsPosition.FromDoublePosition(pos);
            Assert.That(point1, Is.EqualTo(point2));

            point1 = TestData.TestPosition2;
            pos = point1.ToDouble();
            point2 = GpsPosition.FromDoublePosition(pos);
            Assert.That(point1, Is.EqualTo(point2));
        }

        [Test]
        public void FromJsonStringTest()
        {
            var point1 = TestData.TestPosition1;
            var jsonString = point1.ToJsonString();
            var point2 = GpsPosition.FromJsonString(jsonString);
            Assert.That(point1, Is.EqualTo(point2));

            point1 = TestData.TestPosition2;
            jsonString = point1.ToJsonString();
            point2 = GpsPosition.FromJsonString(jsonString);
            Assert.That(point1, Is.EqualTo(point2));

            point2 = GpsPosition.FromJsonString(null);
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = GpsPosition.FromJsonString("");
            Assert.That(point1, Is.Not.EqualTo(point2));

            point2 = GpsPosition.FromJson(null);
            Assert.That(point1, Is.Not.EqualTo(point2));
        }

        [Test]
        public void FromStringTest()
        {
            var point1 = TestData.TestPosition1;
            var jsonString = point1.ToString();
            var point2 = GpsPosition.FromString(jsonString);
            Assert.That(point1, Is.EqualTo(point2));

            point1 = TestData.TestPosition2;
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
            var point1 = TestData.TestPosition1;
            var point2 = new GpsPosition(TestData.TestPosition1.Degrees, 
                TestData.TestPosition1.Minutes,
                TestData.TestPosition1.Seconds,
                TestData.TestPosition1.SubSeconds,
                TestData.TestPosition1.Negative);
            Assert.That(point1.GetHashCode(), Is.EqualTo(point2.GetHashCode()));

            point2 = TestData.TestPosition2;
            Assert.That(point1.GetHashCode(), Is.Not.EqualTo(point2.GetHashCode()));
        }

        [Test]
        public void GpsPositionTest()
        {
            var point = TestData.TestPosition2;
            Assert.That(point.Degrees, Is.EqualTo(47));
            Assert.That(point.Minutes, Is.EqualTo(4));
            Assert.That(point.Seconds, Is.EqualTo(15));
            Assert.That(point.SubSeconds, Is.EqualTo(16));
            Assert.That(point.Negative, Is.EqualTo(true));

            point = TestData.TestPosition1;
            Assert.That(point.Negative, Is.EqualTo(false));
        }
    }
}