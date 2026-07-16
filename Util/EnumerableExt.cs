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
using System.Linq;

#endregion

namespace TCSystem.Util;

/// <summary>Provides additional operations for enumerable sequences.</summary>
public static class EnumerableExt
{
#region Public

    /// <summary>Returns a deferred sequence ordered by pseudo-random keys.</summary>
    /// <typeparam name="TYpe">The item type.</typeparam>
    /// <param name="values">The items to reorder.</param>
    /// <returns>A sequence in pseudo-random order.</returns>
    public static IEnumerable<TYpe> OrderRandom<TYpe>(this IEnumerable<TYpe> values)
    {
        return values.OrderBy(_ => _sRandom.Next());
    }

#endregion

#region Private

    private static readonly Random _sRandom = new();

#endregion
}