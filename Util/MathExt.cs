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

namespace TCSystem.Util;

/// <summary>Provides integer mathematics helpers.</summary>
public static class MathExt
{
#region Public

    /// <summary>Rounds a non-negative integer up to the next power of two.</summary>
    /// <param name="x">The value to round.</param>
    /// <returns>The next power of two; zero for negative values and for overflow beyond the positive <see cref="int" /> range.</returns>
    public static int ToNextPowerOf2(this int x)
    {
        if (x < 0)
        {
            return 0;
        }

        --x;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x + 1;
    }

#endregion
}