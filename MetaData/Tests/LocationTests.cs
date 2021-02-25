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
    public class LocationTests
    {
        [Test]
        public void EqualsTest()
        {
            var location1 = TestData.Location1;
            Assert.That(location1.Equals(TestData.Location1), Is.True);
            Assert.That(location1.Equals(TestData.Location2), Is.False);
            Assert.That(location1.Equals(TestData.LocationZero), Is.False);
            Assert.That(location1.Equals(null), Is.False);
            Assert.That(location1, Is.Not.EqualTo(string.Empty));

            var location2 = new Location(location1.Address, location1.Point);
            Assert.That(location1, Is.EqualTo(location2));
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(Location d) => d.ToJsonString();
            Func<string, Location> fromJson = Location.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Location1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Location2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.LocationZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.Location1;
            var copyOfData1 = new Location(data1.Address, data1.Point);

            TestUtil.GetHashCodeTest(TestData.LocationZero, TestData.Location1,
                TestData.Location2, copyOfData1);
        }

        [Test]
        public void LocationTest()
        {
            Assert.That(TestData.LocationZero.IsAllSet, Is.False);
            Assert.That(TestData.LocationZero.IsSet, Is.False);

            Assert.That(TestData.Location1.IsAllSet, Is.True);
            Assert.That(TestData.Location1.IsSet, Is.True);

            var location = new Location(TestData.Location1.Address, null);
            Assert.That(location.IsAllSet, Is.False);
            Assert.That(location.IsSet, Is.True);

            location = new Location(null, TestData.Location1.Point);
            Assert.That(location.IsAllSet, Is.False);
            Assert.That(location.IsSet, Is.True);
        }
    }
}