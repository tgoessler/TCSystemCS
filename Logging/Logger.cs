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
using System.Diagnostics;

#endregion

namespace TCSystem.Logging;

/// <summary>
///     Defines logging operations for the configured targets.
/// </summary>
/// <remarks>This is an abstract class so debug calls can use conditional compilation.</remarks>
public abstract class Logger
{
#region Public

    /// <summary>Writes a debug-level message when the caller is compiled with <c>DEBUG</c>.</summary>
    /// <param name="message">The message to write.</param>
    [Conditional("DEBUG")]
    public abstract void Debug(string message);

    /// <summary>Writes a debug-level message and exception when the caller is compiled with <c>DEBUG</c>.</summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The associated exception.</param>
    [Conditional("DEBUG")]
    public abstract void Debug(string message, Exception exception);

    /// <summary>Writes an information-level message.</summary>
    /// <param name="message">The message to write.</param>
    public abstract void Info(string message);

    /// <summary>Writes an information-level message and exception.</summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The associated exception.</param>
    public abstract void Info(string message, Exception exception);

    /// <summary>Writes a warning-level message.</summary>
    /// <param name="message">The message to write.</param>
    public abstract void Warn(string message);

    /// <summary>Writes a warning-level message and exception.</summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The associated exception.</param>
    public abstract void Warn(string message, Exception exception);

    /// <summary>Writes an error-level message.</summary>
    /// <param name="message">The message to write.</param>
    public abstract void Error(string message);

    /// <summary>Writes an error-level message and exception.</summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The associated exception.</param>
    public abstract void Error(string message, Exception exception);

    /// <summary>Writes a fatal-level message.</summary>
    /// <param name="message">The message to write.</param>
    public abstract void Fatal(string message);

    /// <summary>Writes a fatal-level message and exception.</summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The associated exception.</param>
    public abstract void Fatal(string message, Exception exception);

    /// <summary>Gets whether debug-level logging is enabled.</summary>
    public abstract bool IsDebugEnabled { get; }

    /// <summary>Gets whether information-level logging is enabled.</summary>
    public abstract bool IsInfoEnabled { get; }

    /// <summary>Gets whether warning-level logging is enabled.</summary>
    public abstract bool IsWarnEnabled { get; }

    /// <summary>Gets whether error-level logging is enabled.</summary>
    public abstract bool IsErrorEnabled { get; }

    /// <summary>Gets whether fatal-level logging is enabled.</summary>
    public abstract bool IsFatalEnabled { get; }

#endregion
}