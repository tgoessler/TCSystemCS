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
//  Copyright (C) 2003 - 2024 Thomas Goessler. All Rights Reserved.
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
    public class AddressTests
    {
        [Test]
        public void AddressTest()
        {
            var address1 = TestData.Address1;
            var address2 = new Address(address1.Country, address1.Province, address1.City, address1.Street);

            Assert.That(address1.Country, Is.EqualTo(address2.Country));
            Assert.That(address1.Province, Is.EqualTo(address2.Province));
            Assert.That(address1.City, Is.EqualTo(address2.City));
            Assert.That(address1.Street, Is.EqualTo(address2.Street));
        }

        [Test]
        public void EqualsTest()
        {
            var address1 = TestData.Address1;
            Assert.That(address1.Equals(null), Is.False);
            Assert.That(address1.Equals(TestData.Address1), Is.True);
            Assert.That(address1.Equals(null), Is.False);
            Assert.That(address1, Is.Not.EqualTo(string.Empty));

            var address2 = TestData.Address2;
            Assert.That(address1, !Is.EqualTo(address2));

            address2 = new Address(address1.Country, address1.Province, address1.City, address1.Street);
            Assert.That(address1, Is.EqualTo(address2));

            address2 = new Address("", address1.Province, address1.City, address1.Street);
            Assert.That(address1, !Is.EqualTo(address2));

            address2 = new Address(address1.Country, "", address1.City, address1.Street);
            Assert.That(address1, !Is.EqualTo(address2));

            address2 = new Address(address1.Country, address1.Province, "", address1.Street);
            Assert.That(address1, !Is.EqualTo(address2));

            address2 = new Address(address1.Country, address1.Province, address1.City);
            Assert.That(address1, !Is.EqualTo(address2));
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(Address d) => d.ToJsonString();
            Func<string, Address> fromJson = Address.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Address1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Address2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.AddressZero, ToJson, fromJson);
            TestUtil.FromJsonStringTest(Address.NotFound, ToJson, fromJson);
            TestUtil.FromJsonStringTest(Address.Undefined, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.Address1;
            var copyOfData1 = new Address(data1.Country, data1.Province, data1.City, data1.Street);

            TestUtil.GetHashCodeTest(TestData.AddressZero, TestData.Address1,
                TestData.Address2, copyOfData1);
        }
    }
}