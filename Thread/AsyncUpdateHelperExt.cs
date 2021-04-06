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

using System;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread
{
    public static class AsyncUpdateHelperExt
    {
#region Public

        public static async Task<IDisposable> BeginUpdateScopeAsync(this IAsyncUpdateHelper asyncUpdateHelper)
        {
            await asyncUpdateHelper.BeginUpdateAsync();
            return new AsyncUpdateScope(asyncUpdateHelper);
        }

        public static async Task<IDisposable> WaitScopeAsync(this IAsyncUpdateHelper asyncUpdateHelper)
        {
            await asyncUpdateHelper.WaitAsync();
            return new AsyncUpdateScope(asyncUpdateHelper);
        }

#endregion
    }
}