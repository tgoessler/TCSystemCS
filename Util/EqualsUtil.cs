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
using System.Runtime.CompilerServices;

#endregion

namespace TCSystem.Util;

/// <summary>Provides a common null-safe equality pattern for reference-type models.</summary>
public static class EqualsUtil
{
#region Public

    /// <summary>Compares references first, then invokes a value-comparison callback when required.</summary>
    /// <typeparam name="TData">The reference type being compared.</typeparam>
    /// <param name="d1">The current object.</param>
    /// <param name="d2">The other object.</param>
    /// <param name="equals">The callback that compares <paramref name="d1" /> with <paramref name="d2" />.</param>
    /// <returns><see langword="true" /> when the references or values are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<TData>(TData d1, TData d2, Func<TData, bool> equals) where TData : class
    {
        if (d2 is null)
        {
            return false;
        }

        return ReferenceEquals(d1, d2) || equals(d2);
    }

#endregion
}