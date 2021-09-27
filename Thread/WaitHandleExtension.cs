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
    internal static class WaitHandleExt
    {
#region Public

        public static async Task WaitOneAsync(this WaitHandle waitHandle)
        {
            await Task.Run(waitHandle.WaitOne);
        }

        public static async Task WaitOneAsync(this WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            await Task.Run(waitHandle.WaitOne, cancellationToken);
        }

#endregion
    }
}