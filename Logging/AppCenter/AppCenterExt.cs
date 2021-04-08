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
using Serilog;
using Serilog.Configuration;

#endregion

namespace TCSystem.Logging.AppCenter
{
    public static class AppCenterExt
    {
#region Public

        public static LoggerConfiguration AppCenter(this LoggerSinkConfiguration loggerConfiguration,
                                                        bool async= true, IFormatProvider formatProvider = null)
        {
            return async ?
                loggerConfiguration.Async(l => l.Sink(new AppCenterSink(formatProvider))) :
                loggerConfiguration.Sink(new AppCenterSink(formatProvider));
        }

        public static LoggerConfiguration AppCenter(this LoggerConfiguration loggerConfiguration, bool async=true, IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.WriteTo.AppCenter(async, formatProvider);
        }

#endregion
    }
}