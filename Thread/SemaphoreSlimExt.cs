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

#endregion

namespace TCSystem.Thread;

/// <summary>Provides disposable lock scopes for <see cref="SemaphoreSlim" />.</summary>
public static class SemaphoreSlimExt
{
#region Public

    /// <summary>Waits for the semaphore and returns a scope that releases it when disposed.</summary>
    public static IDisposable Lock(this SemaphoreSlim semaphore)
    {
        semaphore.Wait();
        return new SemaphoreSlimLock(semaphore);
    }

    /// <summary>Asynchronously waits for the semaphore and returns a scope that releases it when disposed.</summary>
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        return new SemaphoreSlimLock(semaphore);
    }

#endregion
}