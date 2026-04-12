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

using NUnit.Framework;

#endregion

namespace TCSystem.Util.Tests;

[TestFixture]
public class EqualsUtilTests
{
    [Test]
    public void Equals_BothSameReference_ReturnsTrue()
    {
        var obj = new TestClass("test");
        Assert.That(EqualsUtil.Equals(obj, obj, _ => false), Is.True);
    }

    [Test]
    public void Equals_SecondIsNull_ReturnsFalse()
    {
        var obj = new TestClass("test");
        Assert.That(EqualsUtil.Equals(obj, null, _ => true), Is.False);
    }

    [Test]
    public void Equals_DifferentObjectsEqualsReturnsTrue_ReturnsTrue()
    {
        var obj1 = new TestClass("test");
        var obj2 = new TestClass("test");
        Assert.That(EqualsUtil.Equals(obj1, obj2, other => other.Value == obj1.Value), Is.True);
    }

    [Test]
    public void Equals_DifferentObjectsEqualsReturnsFalse_ReturnsFalse()
    {
        var obj1 = new TestClass("test1");
        var obj2 = new TestClass("test2");
        Assert.That(EqualsUtil.Equals(obj1, obj2, other => other.Value == obj1.Value), Is.False);
    }

    private sealed class TestClass(string value)
    {
        public string Value { get; } = value;
    }
}
