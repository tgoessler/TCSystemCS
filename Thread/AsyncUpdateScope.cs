﻿// *******************************************************************************
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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;

#endregion

namespace TCSystem.Thread
{
    internal readonly struct AsyncUpdateScope : IDisposable
    {
#region Public

        public AsyncUpdateScope(IAsyncUpdateHelper asyncUpdateHelper)
        {
            _asyncUpdateHelper = asyncUpdateHelper;
        }

        public void Dispose()
        {
            _asyncUpdateHelper.EndUpdate();
        }

#endregion

#region Private

        private readonly IAsyncUpdateHelper _asyncUpdateHelper;

#endregion
    }
}