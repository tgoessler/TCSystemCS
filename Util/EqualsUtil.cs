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
using System.Runtime.CompilerServices;

#endregion

namespace TCSystem.Util;

public static class EqualsUtil
{
#region Public

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