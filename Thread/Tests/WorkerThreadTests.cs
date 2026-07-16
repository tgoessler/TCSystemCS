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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable AccessToDisposedClosure

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class WorkerThreadTests
{
    [SetUp]
    public void SetUp()
    {
        _worker = Factory.CreateWorkerThread("TestWorkerThread", ThreadPriority.Normal);
    }

    [TearDown]
    public void TearDown()
    {
        _worker.StopThread(TimeSpan.FromSeconds(5));
    }

    [Test]
    public void CancellationToken_IsValid()
    {
        CancellationToken token = _worker.CancellationToken;
        Assert.That(token.CanBeCanceled, Is.True);
        Assert.That(token.IsCancellationRequested, Is.False);
    }

    [Test]
    public void ClearOpenCommands_RemovesQueuedActions()
    {
        // Hold the worker busy so queued commands stay in the queue
        using var holdEvent = new ManualResetEventSlim(false);
        using var workerBusy = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            workerBusy.Set();
            holdEvent.Wait(TimeSpan.FromSeconds(5));
        });

        // Wait until worker is executing the blocking action
        workerBusy.Wait(TimeSpan.FromSeconds(5));

        // Queue two more actions
        _worker.ExecuteCommand(() => { });
        _worker.ExecuteCommand(() => { });
        Assert.That(_worker.NumOpenActions, Is.EqualTo(2));

        _worker.ClearOpenCommands();
        Assert.That(_worker.NumOpenActions, Is.EqualTo(0));

        // Unblock worker
        holdEvent.Set();
    }

    [Test]
    public async Task ClearOpenCommandsAsync_RemovesQueuedActions()
    {
        using var holdEvent = new ManualResetEventSlim(false);
        using var workerBusy = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            workerBusy.Set();
            holdEvent.Wait(TimeSpan.FromSeconds(5));
        });

        workerBusy.Wait(TimeSpan.FromSeconds(5));

        _worker.ExecuteCommand(() => { });
        _worker.ExecuteCommand(() => { });

        await _worker.ClearOpenCommandsAsync();
        Assert.That(_worker.NumOpenActions, Is.EqualTo(0));

        holdEvent.Set();
    }

    [Test]
    public void ExecuteCommand_RunsAction()
    {
        var executed = false;
        using var done = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            executed = true;
            done.Set();
        });

        Assert.That(done.Wait(TimeSpan.FromSeconds(5)), Is.True, "Action did not execute in time");
        Assert.That(executed, Is.True);
    }

    [Test]
    public void ExecuteCommand_WithMessage_RunsAction()
    {
        var executed = false;
        using var done = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            executed = true;
            done.Set();
        }, "TestMessage");

        Assert.That(done.Wait(TimeSpan.FromSeconds(5)), Is.True);
        Assert.That(executed, Is.True);
    }

    [Test]
    public async Task ExecuteCommandAsync_RunsAction()
    {
        var executed = false;
        using var done = new ManualResetEventSlim(false);

        await _worker.ExecuteCommandAsync(() =>
        {
            executed = true;
            done.Set();
        });

        Assert.That(done.Wait(TimeSpan.FromSeconds(5)), Is.True);
        Assert.That(executed, Is.True);
    }

    [Test]
    public async Task ExecuteCommandAsync_WithMessage_RunsAction()
    {
        var executed = false;
        using var done = new ManualResetEventSlim(false);

        await _worker.ExecuteCommandAsync(() =>
        {
            executed = true;
            done.Set();
        }, "TestMessageAsync");

        Assert.That(done.Wait(TimeSpan.FromSeconds(5)), Is.True);
        Assert.That(executed, Is.True);
    }

    [Test]
    public void IdleEvent_FiredWhenWorkerBecomesIdle()
    {
        var idleFired = false;
        using var idleEvent = new ManualResetEventSlim(false);

        _worker.IdleEvent += idle =>
        {
            if (idle)
            {
                idleFired = true;
                idleEvent.Set();
            }
        };

        _worker.ExecuteCommand(() => { });

        Assert.That(idleEvent.Wait(TimeSpan.FromSeconds(5)), Is.True);
        Assert.That(idleFired, Is.True);
    }

    [Test]
    public void IsBusy_TrueWhileExecuting()
    {
        using var holdEvent = new ManualResetEventSlim(false);
        using var workerBusy = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            workerBusy.Set();
            holdEvent.Wait(TimeSpan.FromSeconds(5));
        });

        workerBusy.Wait(TimeSpan.FromSeconds(5));
        Assert.That(_worker.IsBusy, Is.True);

        holdEvent.Set();
    }

    [Test]
    public void NumOpenActions_ReturnsCorrectCount()
    {
        // Block the worker thread so actions accumulate in the queue
        using var holdEvent = new ManualResetEventSlim(false);
        using var workerBusy = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            workerBusy.Set();
            holdEvent.Wait(TimeSpan.FromSeconds(5));
        });

        workerBusy.Wait(TimeSpan.FromSeconds(5));

        _worker.ExecuteCommand(() => { });
        _worker.ExecuteCommand(() => { });
        Assert.That(_worker.NumOpenActions, Is.EqualTo(2));

        holdEvent.Set();
    }

    [Test]
    public void OnDeInitThread_CalledAfterStop()
    {
        var deinitCalled = false;
        using var done = new ManualResetEventSlim(false);

        _worker.OnDeInitThread += () =>
        {
            deinitCalled = true;
            done.Set();
        };

        // Execute at least one action so init runs
        using var firstAction = new ManualResetEventSlim(false);
        _worker.ExecuteCommand(() => firstAction.Set());
        firstAction.Wait(TimeSpan.FromSeconds(5));

        _worker.StopThread(TimeSpan.FromSeconds(5));

        Assert.That(done.Wait(TimeSpan.FromSeconds(5)), Is.True);
        Assert.That(deinitCalled, Is.True);
    }

    [Test]
    public void OnInitThread_CalledBeforeFirstAction()
    {
        var initCalled = false;
        using var done = new ManualResetEventSlim(false);

        _worker.OnInitThread += () => initCalled = true;
        _worker.ExecuteCommand(() => done.Set());

        done.Wait(TimeSpan.FromSeconds(5));
        Assert.That(initCalled, Is.True);
    }

    [Test]
    public void StopThread_StopsExecution()
    {
        var count = 0;
        using var done = new ManualResetEventSlim(false);

        _worker.ExecuteCommand(() =>
        {
            count++;
            done.Set();
        });

        done.Wait(TimeSpan.FromSeconds(5));
        _worker.StopThread(TimeSpan.FromSeconds(5));

        // After stop no further actions should run — verify count stays at 1
        Assert.That(count, Is.EqualTo(1));
    }

    private IWorkerThread _worker;
}