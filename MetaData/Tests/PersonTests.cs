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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
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
    public void InvalidateIdTest()
    {
        Person invalidated = TestData.Person1.InvalidateId();
        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Name, Is.EqualTo(TestData.Person1.Name));
        Assert.That(invalidated.EmailDigest, Is.EqualTo(TestData.Person1.EmailDigest));
        Assert.That(invalidated.LiveId, Is.EqualTo(TestData.Person1.LiveId));
        Assert.That(invalidated.SourceId, Is.EqualTo(TestData.Person1.SourceId));
    }

    [Test]
    public void PersonTest()
    {
        Person person = TestData.Person1;
        Assert.That(person.Id, Is.EqualTo(1));
        Assert.That(person.Name, Is.EqualTo("Thomas"));
        Assert.That(person.EmailDigest, Is.EqualTo("thomas@email.com"));
        Assert.That(person.LiveId, Is.EqualTo("123"));
        Assert.That(person.SourceId, Is.EqualTo("456"));
        Assert.That(person.IsValid, Is.True);
        Assert.That(person.AllAttributesDefined, Is.True);

        Person empty = TestData.PersonZero;
        Assert.That(empty.IsValid, Is.False);
        Assert.That(empty.AllAttributesDefined, Is.False);
    }

    [Test]
    public void ToJsonStringTest()
    {
        string json = TestData.Person1.ToJsonString();
        Assert.That(json, Is.Not.Empty);
    }

    [Test]
    public void ToStringTest()
    {
        string str = TestData.Person1.ToString();
        Assert.That(str, Is.Not.Empty);
    }
}