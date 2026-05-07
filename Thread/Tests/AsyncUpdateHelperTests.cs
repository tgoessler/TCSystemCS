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

using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class AsyncUpdateHelperTests
{
    [SetUp]
    public void SetUp()
    {
        _helper = Factory.CreateAsyncUpdateHelper();
    }

    [Test]
    public void InitialState_ShouldStop_IsFalse()
    {
        Assert.That(_helper.ShouldStop, Is.False);
    }

    [Test]
    public void InitialState_IsUpdatePending_IsFalse()
    {
        Assert.That(_helper.IsUpdatePending, Is.False);
    }

    [Test]
    public async Task BeginUpdateAsync_ThenEndUpdate_Succeeds()
    {
        await _helper.BeginUpdateAsync();
        Assert.That(_helper.ShouldStop, Is.False);
        Assert.That(_helper.IsUpdatePending, Is.False);
        _helper.EndUpdate();
    }

    [Test]
    public async Task WaitAsync_ThenEndUpdate_Succeeds()
    {
        await _helper.WaitAsync();
        Assert.That(_helper.ShouldStop, Is.False);
        Assert.That(_helper.IsUpdatePending, Is.False);
        _helper.EndUpdate();
    }

    [Test]
    public async Task ShouldStop_TrueWhileSecondBeginUpdatePending()
    {
        // Acquire the lock so that a second BeginUpdateAsync will block.
        await _helper.BeginUpdateAsync();

        // Start a second update in the background — it will block until the semaphore is released.
        Task secondUpdate = _helper.BeginUpdateAsync();

        // Give the second task a moment to block on the semaphore.
        await Task.Delay(50);
        Assert.That(_helper.ShouldStop, Is.True);

        // Release so the second update can acquire.
        _helper.EndUpdate();
        await secondUpdate;
        Assert.That(_helper.ShouldStop, Is.False);

        // Release the second update.
        _helper.EndUpdate();
    }

    [Test]
    public async Task IsUpdatePending_TrueWhileSecondWaitPending()
    {
        // Acquire the lock so that WaitAsync will block.
        await _helper.BeginUpdateAsync();

        Task waitTask = _helper.WaitAsync();

        await Task.Delay(50);
        Assert.That(_helper.IsUpdatePending, Is.True);

        _helper.EndUpdate();
        await waitTask;
        Assert.That(_helper.IsUpdatePending, Is.False);
        _helper.EndUpdate();
    }

    [Test]
    public async Task SequentialBeginEndUpdate_WorksMultipleTimes()
    {
        for (int i = 0; i < 5; i++)
        {
            await _helper.BeginUpdateAsync();
            _helper.EndUpdate();
        }

        Assert.That(_helper.ShouldStop, Is.False);
        Assert.That(_helper.IsUpdatePending, Is.False);
    }

    private IAsyncUpdateHelper _helper;
}
