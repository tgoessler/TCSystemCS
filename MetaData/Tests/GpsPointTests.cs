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
    public class GpsPointTests
    {
        [Test]
        public void EqualsTest()
        {
            var point = TestData.PointZero;
            Assert.That(point.Equals(TestData.PointZero), Is.True);
            Assert.That(point.Equals(TestData.Point1), Is.False);
            Assert.That(point.Equals(TestData.Point2), Is.False);
            Assert.That(point.Equals(null), Is.False);
        }

        [Test]
        public void FromJsonStringTest()
        {
            var jsonString = TestData.PointZero.ToJsonString();
            var point = GpsPoint.FromJsonString(jsonString);
            Assert.That(point.Equals(TestData.PointZero), Is.True);

            jsonString = TestData.Point1.ToJsonString();
            point = GpsPoint.FromJsonString(jsonString);
            Assert.That(point.Equals(TestData.Point1), Is.True);

            jsonString = TestData.Point2.ToJsonString();
            point = GpsPoint.FromJsonString(jsonString);
            Assert.That(point.Equals(TestData.Point2), Is.True);
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.That(TestData.PointZero.GetHashCode(), Is.Not.EqualTo(TestData.Point1.GetHashCode()));

            var point = new GpsPoint(TestData.Point1.Latitude, TestData.Point1.Longitude, TestData.Point1.Altitude);
            Assert.That(point.GetHashCode(), Is.EqualTo(TestData.Point1.GetHashCode()));
        }

        [Test]
        public void ToStringTest()
        {
            Assert.That(TestData.PointZero.ToString(), Is.Not.EqualTo(""));
        }
    }
}