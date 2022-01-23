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
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TCSystem.Util
{
    public static class EnumerableExt
    {
#region Public

        public static IEnumerable<TYpe> OrderRandom<TYpe>(this IEnumerable<TYpe> values)
        {
            return values.OrderBy(_ => _sRandom.Next());
        }

#endregion

#region Private

        private static readonly Random _sRandom = new Random();

#endregion
    }
}