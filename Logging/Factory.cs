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
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

#endregion

namespace TCSystem.Logging;

/// <summary>
///     Creates loggers and manages the process-wide Serilog configuration.
/// </summary>
public static class Factory
{
#region Public

    /// <summary>
    ///     Specifies the sinks enabled by the default logging configuration.
    /// </summary>
    [Flags]
    public enum LoggingOptions
    {
        /// <summary>Write log events to a rolling file.</summary>
        File = 1,

        /// <summary>Write log events to the attached debugger.</summary>
        Debugger = 2,

        /// <summary>Write log events to the console.</summary>
        Console = 4,
    }

    /// <summary>
    ///     Creates a logger whose source context is the supplied type.
    /// </summary>
    /// <param name="type">The type used as the source context.</param>
    /// <returns>A logger for <paramref name="type" />.</returns>
    public static Logger GetLogger(Type type)
    {
        return new LoggerSerilog(type);
    }

    /// <summary>
    ///     Initializes process-wide logging without a file path.
    /// </summary>
    /// <param name="options">The default sinks to enable.</param>
    /// <param name="configure">An optional callback that can extend the Serilog configuration.</param>
    /// <returns><see langword="true" /> when initialization succeeds.</returns>
    /// <remarks>Initialization is reference-counted; balance each call with <see cref="DeInitLogging" />.</remarks>
    public static bool InitLogging(LoggingOptions options, Action<LoggerConfiguration> configure = null)
    {
        return InternalInitLogging(options, null, 0, 0, configure);
    }

    /// <summary>
    ///     Initializes process-wide logging with rolling-file settings.
    /// </summary>
    /// <param name="options">The default sinks to enable.</param>
    /// <param name="loggingFile">The output path used when <see cref="LoggingOptions.File" /> is enabled.</param>
    /// <param name="maxFiles">The maximum number of retained rolling files.</param>
    /// <param name="maxFileSizeKb">The size limit of each file in kilobytes.</param>
    /// <param name="configure">An optional callback that can extend the Serilog configuration.</param>
    /// <returns><see langword="true" /> when initialization succeeds.</returns>
    /// <remarks>Initialization is reference-counted; balance each call with <see cref="DeInitLogging" />.</remarks>
    public static bool InitLogging(LoggingOptions options, string loggingFile, int maxFiles = 2, int maxFileSizeKb = 1024,
                                   Action<LoggerConfiguration> configure = null)
    {
        return InternalInitLogging(options, loggingFile, maxFiles, maxFileSizeKb, configure);
    }

    /// <summary>
    ///     Releases one logging initialization reference and flushes all sinks when the final reference is released.
    /// </summary>
    public static void DeInitLogging()
    {
        if (--_initCount == 0)
        {
            Log.Instance.Info("Logging stopped");
            Serilog.Log.CloseAndFlush();
        }
    }

#endregion

#region Internal

    internal static event Action LoggingInitialized;

    internal static bool IsLoggingInitialized { get; private set; }

#endregion

#region Private

    private const string OutputTemplate = "[{Timestamp:o}, {Level:u3}] {SourceContext}({ThreadId}): {Message:lj}{NewLine}{Exception}";

    private static bool InternalInitLogging(LoggingOptions options, string loggingFile, int maxFiles, int maxFileSizeKb,
                                            Action<LoggerConfiguration> configure)
    {
        if (_initCount++ != 0)
        {
            Log.Instance.Info($"Logging init already done, _initCount={_initCount}'");
            return true;
        }

        LoggerConfiguration loggerConfiguration = CreateDefaultLoggerConfiguration();
        loggerConfiguration = AddDebuggerLoggerConfiguration(options, loggerConfiguration);
        loggerConfiguration = AddConsoleLoggerConfiguration(options, loggerConfiguration);
        loggerConfiguration = AddFileLoggerConfiguration(options, loggingFile, maxFiles, maxFileSizeKb, loggerConfiguration);

        configure?.Invoke(loggerConfiguration);

        Serilog.Log.Logger = loggerConfiguration.CreateLogger();

        IsLoggingInitialized = true;
        LoggingInitialized?.Invoke();
        LoggingInitialized = null;

        Log.Instance.Info("Logging started");
        return true;
    }

    private static LoggerConfiguration CreateDefaultLoggerConfiguration()
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
#if DEBUG
            .MinimumLevel.Debug();
#else
            .MinimumLevel.Information();
#endif
        return loggerConfiguration;
    }

    private static LoggerConfiguration AddFileLoggerConfiguration(LoggingOptions options, string loggingFile, int maxFiles,
                                                                  int maxFileSizeKb,
                                                                  LoggerConfiguration loggerConfiguration)
    {
        if ((options & LoggingOptions.File) == LoggingOptions.File)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Async(a =>
            {
                a.File($"{loggingFile}",
                    fileSizeLimitBytes: maxFileSizeKb * 1024,
                    retainedFileCountLimit: maxFiles,
                    rollOnFileSizeLimit: true,
                    outputTemplate: OutputTemplate);
            });
        }

        return loggerConfiguration;
    }

    private static LoggerConfiguration AddConsoleLoggerConfiguration(LoggingOptions options, LoggerConfiguration loggerConfiguration)
    {
        if ((options & LoggingOptions.Console) == LoggingOptions.Console)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Console(outputTemplate: OutputTemplate, theme: AnsiConsoleTheme.Literate);
        }

        return loggerConfiguration;
    }

    private static LoggerConfiguration AddDebuggerLoggerConfiguration(LoggingOptions options, LoggerConfiguration loggerConfiguration)
    {
        if ((options & LoggingOptions.Debugger) == LoggingOptions.Debugger)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Debug(outputTemplate: OutputTemplate);
        }

        return loggerConfiguration;
    }

    private static int _initCount;

#endregion
}