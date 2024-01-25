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

#endregion

namespace TCSystem.Thread;

internal readonly struct SemaphoreSlimLock : IDisposable
{
#region Public

    public void Dispose()
    {
        _semaphore?.Release();
    }

#endregion

#region Internal

    internal SemaphoreSlimLock(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

#endregion

#region Private

    private readonly SemaphoreSlim _semaphore;

#endregion
}