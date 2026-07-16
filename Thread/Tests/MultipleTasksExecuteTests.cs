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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable AccessToDisposedClosure

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class MultipleTasksExecuteTests
{
    [Test]
    public void ExecuteCommand_LimitsParallelism()
    {
        const int maxParallel = 2;
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(maxParallel);

        var currentlyRunning = 0;
        var maxObserved = 0;
        var syncLock = new object();

        using var holdEvent = new ManualResetEventSlim(false);
        const int taskCount = 6;

        for (var i = 0; i < taskCount; i++)
        {
            executor.ExecuteCommand(() =>
            {
                int running = Interlocked.Increment(ref currentlyRunning);
                lock (syncLock)
                {
                    if (running > maxObserved)
                    {
                        maxObserved = running;
                    }
                }

                // Simulate some work while holding the slot
                holdEvent.Wait(TimeSpan.FromMilliseconds(50));
                Interlocked.Decrement(ref currentlyRunning);
            });
        }

        holdEvent.Set();
        executor.WaitAllDone();

        Assert.That(maxObserved, Is.LessThanOrEqualTo(maxParallel));
    }

    [Test]
    public void ExecuteCommand_MultipleActions_AllRun()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(3);
        var results = new List<int>();
        var lockObj = new object();

        for (var i = 0; i < 10; i++)
        {
            int value = i;
            executor.ExecuteCommand(() =>
            {
                lock (lockObj)
                {
                    results.Add(value);
                }
            });
        }

        executor.WaitAllDone();

        Assert.That(results.Count, Is.EqualTo(10));
    }

    [Test]
    public void ExecuteCommand_RunsAction()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(2);
        var executed = false;

        executor.ExecuteCommand(() => executed = true);
        executor.WaitAllDone();

        Assert.That(executed, Is.True);
    }

    [Test]
    public void ExecuteCommand_WithCancellationToken_RunsAction()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(2);
        var executed = false;
        using var cts = new CancellationTokenSource();

        executor.ExecuteCommand(() => executed = true, cts.Token);
        executor.WaitAllDone();

        Assert.That(executed, Is.True);
    }

    [Test]
    public void WaitAllDone_CancelledToken_ThrowsOperationCanceled()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(1);
        using var cts = new CancellationTokenSource();

        // Block all slots
        using var holdEvent = new ManualResetEventSlim(false);
        executor.ExecuteCommand(() => holdEvent.Wait(TimeSpan.FromSeconds(5)));

        // Wait a bit for the slot to be acquired
        Task.Delay(20).Wait();

        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => executor.WaitAllDone(cts.Token));

        holdEvent.Set();
        executor.WaitAllDone();
    }

    [Test]
    public void WaitAllDone_WaitsForAllTasksToComplete()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(4);
        var counter = 0;

        for (var i = 0; i < 8; i++)
        {
            executor.ExecuteCommand(() => Interlocked.Increment(ref counter));
        }

        executor.WaitAllDone();
        Assert.That(counter, Is.EqualTo(8));
    }

    [Test]
    public void WaitAllDone_WithCancellationToken_WaitsForAllTasksToComplete()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(3);
        var counter = 0;
        using var cts = new CancellationTokenSource();

        for (var i = 0; i < 6; i++)
        {
            executor.ExecuteCommand(() => Interlocked.Increment(ref counter));
        }

        executor.WaitAllDone(cts.Token);
        Assert.That(counter, Is.EqualTo(6));
    }
}