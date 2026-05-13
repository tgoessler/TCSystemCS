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
using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace TCSystem.Util.Tests;

[TestFixture]
public class AsyncExtTests
{
    [Test]
    public async Task ForEachAsync_EmptyCollection_DoesNothing()
    {
        var results = new List<int>();
        await new List<int>().ForEachAsync(async item =>
        {
            await Task.Yield();
            results.Add(item);
        });

        Assert.That(results, Is.Empty);
    }

    [Test]
    public async Task ForEachAsync_SingleItem_ProcessesItem()
    {
        var results = new List<int>();
        await new[] { 42 }.ForEachAsync(async item =>
        {
            await Task.Yield();
            results.Add(item);
        });

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo(42));
    }

    [Test]
    public async Task ForEachAsync_MultipleItems_ProcessesAllInOrder()
    {
        var results = new List<int>();
        await new[] { 1, 2, 3, 4, 5 }.ForEachAsync(async item =>
        {
            await Task.Yield();
            results.Add(item);
        });

        Assert.That(results, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
    }

    [Test]
    public async Task ForEachAsync_WithTransformation_AppliesActionToEachItem()
    {
        var results = new List<string>();
        await new[] { "a", "b", "c" }.ForEachAsync(async item =>
        {
            await Task.Yield();
            results.Add(item.ToUpperInvariant());
        });

        Assert.That(results, Is.EqualTo(new[] { "A", "B", "C" }));
    }

    [Test]
    public async Task ForEachAsync_ProcessesSequentially()
    {
        var order = new List<int>();
        await new[] { 1, 2, 3 }.ForEachAsync(async item =>
        {
            await Task.Delay(10);
            order.Add(item);
        });

        Assert.That(order, Is.EqualTo(new[] { 1, 2, 3 }));
    }
}
