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

using System;
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class PersonTests
{
    [Test]
    public void EqualsTest()
    {
        Person person = TestData.Person1;
        Assert.That(person.Equals(TestData.Person1), Is.True);
        Assert.That(person.Equals(TestData.Person2), Is.False);
        Assert.That(person.Equals(TestData.PersonZero), Is.False);
        Assert.That(person.Equals(null), Is.False);
        Assert.That(person, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(Person d)
        {
            return d.ToJsonString();
        }

        Func<string, Person> fromJson = Person.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.Person1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.Person2, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.PersonZero, ToJson, fromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        Person data1 = TestData.Person1;
        var copyOfData1 = new Person(data1.Id, data1.Name, data1.EmailDigest, data1.LiveId, data1.SourceId);

        TestUtil.GetHashCodeTest(TestData.PersonZero, TestData.Person1,
            TestData.Person2, copyOfData1);
    }

    [Test]
    public void InvalidateIdTest() { }

    [Test]
    public void PersonTest() { }

    [Test]
    public void ToJsonStringTest() { }

    [Test]
    public void ToStringTest() { }
}