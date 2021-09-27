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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread
{
    internal sealed class AsyncUpdateHelper : IAsyncUpdateHelper
    {
#region Public

        public async Task BeginUpdateAsync()
        {
            Interlocked.Increment(ref _numPendingStops);
            Interlocked.Increment(ref _numPendingUpdates);

            await _semaphore.WaitAsync();

            Interlocked.Decrement(ref _numPendingStops);
            Interlocked.Decrement(ref _numPendingUpdates);
        }

        public async Task WaitAsync()
        {
            Interlocked.Increment(ref _numPendingUpdates);

            await _semaphore.WaitAsync();

            Interlocked.Decrement(ref _numPendingUpdates);
        }

        public void EndUpdate()
        {
            _semaphore.Release();
        }

        public bool ShouldStop => Interlocked.Read(ref _numPendingStops) > 0;
        public bool IsUpdatePending => Interlocked.Read(ref _numPendingUpdates) > 0;

#endregion

#region Private

        private long _numPendingStops;
        private long _numPendingUpdates;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

#endregion
    }
}