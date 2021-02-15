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

        public async void Emit(LogEvent logEvent)
        {
            if ((logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal) &&
                await Analytics.IsEnabledAsync())
            {
                var logData = new Dictionary<string, string>
                {
                    {"Message", logEvent.RenderMessage(_formatProvider)},
                    {"TimeStamp", logEvent.Timestamp.ToString()},
                    {"Level", logEvent.Level.ToString()},
                };

                if (logEvent.Exception != null)
                {
                    logData["Exception"] = logEvent.Exception.ToString();
                }

                if (logEvent.Properties != null)
                {
                    foreach (var keyValuePair in logEvent.Properties)
                    {
                        logData[keyValuePair.Key] = keyValuePair.Value.ToString();
                    }
                }

                Analytics.TrackEvent("LogEntry", logData);
            }
        }

#endregion

#region Private

        private readonly IFormatProvider _formatProvider;

#endregion
    }
}