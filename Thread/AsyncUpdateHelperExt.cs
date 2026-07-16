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
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread;

/// <summary>Provides disposable scopes for asynchronous update coordination.</summary>
public static class AsyncUpdateHelperExt
{
#region Public

    /// <summary>Acquires a stop-requesting update scope that releases the helper when disposed.</summary>
    public static async Task<IDisposable> BeginUpdateScopeAsync(this IAsyncUpdateHelper asyncUpdateHelper)
    {
        await asyncUpdateHelper.BeginUpdateAsync();
        return new AsyncUpdateScope(asyncUpdateHelper);
    }

    /// <summary>Acquires an ordinary update scope that releases the helper when disposed.</summary>
    public static async Task<IDisposable> WaitScopeAsync(this IAsyncUpdateHelper asyncUpdateHelper)
    {
        await asyncUpdateHelper.WaitAsync();
        return new AsyncUpdateScope(asyncUpdateHelper);
    }

#endregion
}