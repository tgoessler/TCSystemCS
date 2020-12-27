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
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Serilog.Core;
using Serilog.Events;

#endregion

namespace TCSystem.Logging
{
    internal sealed class AppCenterSink : ILogEventSink
    {
#region Public

        public AppCenterSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            if ((logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal) &&
                Analytics.IsEnabledAsync().GetAwaiter().GetResult())
            {
                string message = logEvent.RenderMessage(_formatProvider);
                Analytics.TrackEvent("LogEntry",
                    new Dictionary<string, string> {{logEvent.ToString(), message}});
            }
        }

#endregion

#region Private

        private readonly IFormatProvider _formatProvider;

#endregion
    }
}