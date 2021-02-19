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
    public class PersonTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void FromJsonStringTest()
        {
            string ToJson(Person d) => d.ToJsonString();
            Func<string, Person> fromJson = Person.FromJsonString;

            TestUtil.FromJsonStringTest(TestData.Person1, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.Person2, ToJson, fromJson);
            TestUtil.FromJsonStringTest(TestData.PersonZero, ToJson, fromJson);
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void InvalidateIdTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PersonTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToJsonStringTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}