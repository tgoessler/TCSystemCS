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
using System.Threading.Tasks;

#endregion

namespace TCSystem.Util;

/// <summary>Provides asynchronous operations for enumerable sequences.</summary>
public static class AsyncExt
{
#region Public

    /// <summary>Asynchronously invokes an action for each item in sequence.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The items to process.</param>
    /// <param name="action">The asynchronous action to invoke for each item.</param>
    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        foreach (T item in source)
        {
            await action(item);
        }
    }

#endregion
}