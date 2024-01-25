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
using Serilog;
using Serilog.Core;
using Serilog.Events;

#endregion

namespace TCSystem.Logging;

internal sealed class LoggerSerilog : Logger
{
#region Public

    public LoggerSerilog(Type type)
    {
        string name = type.Namespace;
        _logger = Serilog.Log.Logger.ForContext(Constants.SourceContextPropertyName, name);
        if (!Factory.IsLoggingInitialized)
        {
            Factory.LoggingInitialized += () => _logger = Serilog.Log.Logger.ForContext(Constants.SourceContextPropertyName, name);
        }
    }

    public override void Debug(string message)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger.Write(LogEventLevel.Debug, message);
        }
    }

    public override void Debug(string message, Exception exception)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger.Write(LogEventLevel.Debug, exception, message);
        }
    }

    public override void Info(string message)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger.Write(LogEventLevel.Information, message);
        }
    }

    public override void Info(string message, Exception exception)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger.Write(LogEventLevel.Information, exception, message);
        }
    }

    public override void Warn(string message)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger.Write(LogEventLevel.Warning, message);
        }
    }

    public override void Warn(string message, Exception exception)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger.Write(LogEventLevel.Warning, exception, message);
        }
    }

    public override void Error(string message)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger.Write(LogEventLevel.Error, message);
        }
    }

    public override void Error(string message, Exception exception)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger.Write(LogEventLevel.Error, exception, message);
        }
    }

    public override void Fatal(string message)
    {
        if (_logger.IsEnabled(LogEventLevel.Fatal))
        {
            _logger.Write(LogEventLevel.Fatal, message);
        }
    }

    public override void Fatal(string message, Exception exception)
    {
        if (_logger.IsEnabled(LogEventLevel.Fatal))
        {
            _logger.Write(LogEventLevel.Fatal, exception, message);
        }
    }

    public override bool IsDebugEnabled => _logger.IsEnabled(LogEventLevel.Debug);
    public override bool IsInfoEnabled => _logger.IsEnabled(LogEventLevel.Information);
    public override bool IsWarnEnabled => _logger.IsEnabled(LogEventLevel.Warning);
    public override bool IsErrorEnabled => _logger.IsEnabled(LogEventLevel.Error);
    public override bool IsFatalEnabled => _logger.IsEnabled(LogEventLevel.Fatal);

#endregion

#region Private

    private ILogger _logger;

#endregion
}