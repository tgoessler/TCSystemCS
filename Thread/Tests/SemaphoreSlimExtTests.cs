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

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class SemaphoreSlimExtTests
{
    [Test]
    public void Lock_AcquiresAndReleasesOnDispose()
    {
        using var sem = new SemaphoreSlim(1, 1);

        IDisposable lockHandle = sem.Lock();
        Assert.That(sem.CurrentCount, Is.EqualTo(0));

        lockHandle.Dispose();
        Assert.That(sem.CurrentCount, Is.EqualTo(1));
    }

    [Test]
    public async Task LockAsync_AcquiresAndReleasesOnDispose()
    {
        using var sem = new SemaphoreSlim(1, 1);

        IDisposable lockHandle = await sem.LockAsync();
        Assert.That(sem.CurrentCount, Is.EqualTo(0));

        lockHandle.Dispose();
        Assert.That(sem.CurrentCount, Is.EqualTo(1));
    }

    [Test]
    public void Lock_BlocksUntilDisposed()
    {
        using var sem = new SemaphoreSlim(1, 1);
        bool secondAcquired = false;

        IDisposable first = sem.Lock();

        // Try to acquire in background — will block until first is disposed.
        Task background = Task.Run(() =>
        {
            using IDisposable second = sem.Lock();
            secondAcquired = true;
        });

        System.Threading.Thread.Sleep(50);
        Assert.That(secondAcquired, Is.False);

        first.Dispose();
        background.Wait(TimeSpan.FromSeconds(2));
        Assert.That(secondAcquired, Is.True);
    }

    [Test]
    public async Task LockAsync_BlocksUntilDisposed()
    {
        using var sem = new SemaphoreSlim(1, 1);
        bool secondAcquired = false;

        IDisposable first = await sem.LockAsync();

        Task background = Task.Run(async () =>
        {
            using IDisposable second = await sem.LockAsync();
            secondAcquired = true;
        });

        await Task.Delay(50);
        Assert.That(secondAcquired, Is.False);

        first.Dispose();
        await background.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.That(secondAcquired, Is.True);
    }

    [Test]
    public void Lock_MultipleSequentialLocks_Work()
    {
        using var sem = new SemaphoreSlim(1, 1);

        for (int i = 0; i < 5; i++)
        {
            using IDisposable _ = sem.Lock();
            Assert.That(sem.CurrentCount, Is.EqualTo(0));
        }

        Assert.That(sem.CurrentCount, Is.EqualTo(1));
    }
}
