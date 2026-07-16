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
public class PersonTagTests
{
    [Test]
    public void EqualsTest()
    {
        PersonTag personTag = TestData.PersonTag1;
        Assert.That(personTag.Equals(TestData.PersonTag1), Is.True);
        Assert.That(personTag.Equals(TestData.PersonTag2), Is.False);
        Assert.That(personTag.Equals(TestData.PersonTagZero), Is.False);
        Assert.That(personTag.Equals(null), Is.False);
        Assert.That(personTag, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void FromJsonStringTest()
    {
        string ToJson(PersonTag d)
        {
            return d.ToJsonString();
        }

        Func<string, PersonTag> fromJson = PersonTag.FromJsonString;

        TestUtil.FromJsonStringTest(TestData.PersonTag1, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.PersonTag2, ToJson, fromJson);
        TestUtil.FromJsonStringTest(TestData.PersonTagZero, ToJson, fromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        PersonTag data1 = TestData.PersonTag1;
        var copyOfData1 = new PersonTag(data1.Person, data1.Face);

        TestUtil.GetHashCodeTest(TestData.PersonTagZero, TestData.PersonTag1,
            TestData.PersonTag2, copyOfData1);
    }

    [Test]
    public void InvalidateIdsTest()
    {
        PersonTag invalidated = TestData.PersonTag1.InvalidateId();
        Assert.That(invalidated.Person.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Face.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Person.Name, Is.EqualTo(TestData.PersonTag1.Person.Name));
    }

    [Test]
    public void PersonTagTest()
    {
        PersonTag tag = TestData.PersonTag1;
        Assert.That(tag.Person, Is.EqualTo(TestData.Person1));
        Assert.That(tag.Face, Is.EqualTo(TestData.Face1));
    }

    [Test]
    public void ToStringTest()
    {
        var str = TestData.PersonTag1.ToString();
        Assert.That(str, Is.Not.Empty);
    }
}