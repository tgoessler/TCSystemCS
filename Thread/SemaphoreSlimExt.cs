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
//  see https://github.com/ThE-TiGeR/TCSystemCS for details.
//  Copyright (C) 2003 - 2024 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.Thread
{
    public static class SemaphoreSlimExt
    {
#region Public

        public static IDisposable Lock(this SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return new SemaphoreSlimLock(semaphore);
        }

        public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new SemaphoreSlimLock(semaphore);
        }

#endregion
    }
}