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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using Serilog;
using Serilog.Configuration;

#endregion

namespace TCSystem.Logging
{
    internal static class AppCenterSinkExtensions
    {
#region Public

        public static LoggerConfiguration AppCenterSink(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new AppCenterSink(formatProvider));
        }

#endregion
    }
}