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
    public class GpsPointTests
    {
        [Test]
        public void EqualsTest()
        {
            var point = TestData.GpsPointZero;
            Assert.That(point.Equals(TestData.GpsPointZero), Is.True);
            Assert.That(point.Equals(TestData.GpsPoint1), Is.False);
            Assert.That(point.Equals(TestData.GpsPoint2), Is.False);
            Assert.That(point.Equals(null), Is.False);
            Assert.That(point, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(GpsPoint d) => d.ToJsonString();
            Func<string, GpsPoint> fromJson = GpsPoint.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.GpsPoint1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.GpsPoint2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.GpsPointZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.GpsPoint1;
            var copyOfData1 = new GpsPoint(data1.Latitude, data1.Longitude, data1.Altitude);

            TestUtil.GetHashCodeTest(TestData.GpsPointZero, TestData.GpsPoint1,
                TestData.GpsPoint2, copyOfData1);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.That(TestData.GpsPointZero.ToString(), Is.Not.EqualTo(""));
        }
    }
}