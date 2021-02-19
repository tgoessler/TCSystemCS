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
            Assert.That(location1.Equals(null), Is.False);
            Assert.That(location1, Is.Not.EqualTo(TestData.Address1));
            Assert.That(location1.Equals(TestData.Location1), Is.True);

            var location2 = TestData.Location2;
            Assert.That(location1, !Is.EqualTo(location2));

            location2 = new Location(location1.Address, location1.Point);
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
            Assert.That(TestData.LocationZero.GetHashCode(), Is.Not.EqualTo(TestData.Location1.GetHashCode()));

            var location = new Location(TestData.Location1.Address, TestData.Location1.Point);
            Assert.That(location.GetHashCode(), Is.EqualTo(TestData.Location1.GetHashCode()));
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
            Assert.That(location.IsSet, Is.False);

            location = new Location(null, TestData.Location1.Point);
            Assert.That(location.IsAllSet, Is.False);
            Assert.That(location.IsSet, Is.False);
        }
    }
}