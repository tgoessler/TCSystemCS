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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

#endregion

namespace TCSystem.Util.Tests;

[TestFixture]
public class EnumerableExtTests
{
    [Test]
    public void OrderRandom_EmptyCollection_ReturnsEmpty()
    {
        IEnumerable<int> result = new List<int>().OrderRandom();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void OrderRandom_PreservesAllElements()
    {
        int[] source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        IEnumerable<int> result = source.OrderRandom();

        Assert.That(result.OrderBy(x => x), Is.EqualTo(source));
    }

    [Test]
    public void OrderRandom_PreservesCount()
    {
        int[] source = [1, 2, 3, 4, 5];
        IEnumerable<int> result = source.OrderRandom();

        Assert.That(result.Count(), Is.EqualTo(source.Length));
    }

    [Test]
    public void OrderRandom_SingleElement_ReturnsSameElement()
    {
        IEnumerable<int> result = new[] { 42 }.OrderRandom();
        Assert.That(result.Single(), Is.EqualTo(42));
    }

    [Test]
    public void OrderRandom_WithDuplicates_PreservesAllElements()
    {
        int[] source = [1, 1, 2, 2, 3, 3];
        List<int> result = source.OrderRandom().ToList();

        Assert.That(result.Count, Is.EqualTo(source.Length));
        Assert.That(result.OrderBy(x => x), Is.EqualTo(source.OrderBy(x => x)));
    }

    [Test]
    public void OrderRandom_WithStrings_PreservesAllElements()
    {
        string[] source = ["alpha", "beta", "gamma", "delta"];
        List<string> result = source.OrderRandom().ToList();

        Assert.That(result.Count, Is.EqualTo(source.Length));
        Assert.That(result.OrderBy(x => x), Is.EqualTo(source.OrderBy(x => x)));
    }
}