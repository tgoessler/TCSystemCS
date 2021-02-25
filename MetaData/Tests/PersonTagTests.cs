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
    public class PersonTagTests
    {
        [Test]
        public void EqualsTest()
        {
            var personTag = TestData.PersonTag1;
            Assert.That(personTag.Equals(TestData.PersonTag1), Is.True);
            Assert.That(personTag.Equals(TestData.PersonTag2), Is.False);
            Assert.That(personTag.Equals(TestData.PersonTagZero), Is.False);
            Assert.That(personTag.Equals(null), Is.False);
            Assert.That(personTag, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(PersonTag d) => d.ToJsonString();
            Func<string, PersonTag> fromJson = PersonTag.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.PersonTag1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PersonTag2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PersonTagZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            var data1 = TestData.PersonTag1;
            var copyOfData1 = new PersonTag(data1.Person, data1.Face);

            TestUtil.GetHashCodeTest(TestData.PersonTagZero, TestData.PersonTag1,
                TestData.PersonTag2, copyOfData1);
        }

        [Test]
        public void InvalidateIdsTest()
        {
            
        }

        [Test]
        public void PersonTagTest()
        {
            
        }

        [Test]
        public void ToStringTest()
        {
            
        }
    }
}