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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.Logging
{
    /// <summary>
    ///     Abstract class for logging data do the configured log target.
    ///     This class is not an interface to support conditional compilation
    /// </summary>
    public abstract class Logger
    {
#region Public

        [Conditional("DEBUG")]
        public abstract void Debug(string message);

        [Conditional("DEBUG")]
        public abstract void Debug(string message, Exception exception);

        public abstract void Info(string message);
        public abstract void Info(string message, Exception exception);

        public abstract void Warn(string message);
        public abstract void Warn(string message, Exception exception);

        public abstract void Error(string message);
        public abstract void Error(string message, Exception exception);

        public abstract void Fatal(string message);
        public abstract void Fatal(string message, Exception exception);
        public abstract bool IsDebugEnabled { get; }
        public abstract bool IsInfoEnabled { get; }
        public abstract bool IsWarnEnabled { get; }
        public abstract bool IsErrorEnabled { get; }
        public abstract bool IsFatalEnabled { get; }

#endregion
    }
}