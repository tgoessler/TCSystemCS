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
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread;

/// <summary>Represents a dedicated worker thread that executes queued actions in FIFO order.</summary>
public interface IWorkerThread
{
#region Public

    /// <summary>Queues an action for execution.</summary>
    void ExecuteCommand(Action action);

    /// <summary>Queues an action with a message used when logging failures.</summary>
    void ExecuteCommand(Action action, string message);

    /// <summary>Removes queued actions that have not started.</summary>
    void ClearOpenCommands();

    /// <summary>Asynchronously acquires the queue lock and queues an action.</summary>
    Task ExecuteCommandAsync(Action action);

    /// <summary>Asynchronously acquires the queue lock and queues an action with a diagnostic message.</summary>
    Task ExecuteCommandAsync(Action action, string message);

    /// <summary>Asynchronously acquires the queue lock and removes actions that have not started.</summary>
    Task ClearOpenCommandsAsync();

    /// <summary>Stops the worker, clears pending actions, and waits for the thread to terminate.</summary>
    void StopThread(TimeSpan? timeOut = null);

    /// <summary>Occurs on the worker thread before its first action executes.</summary>
    event Action OnInitThread;

    /// <summary>Occurs on the worker thread when it terminates after initialization.</summary>
    event Action OnDeInitThread;

    /// <summary>Occurs when the worker enters or leaves the idle state; the argument is <see langword="true" /> when idle.</summary>
    event Action<bool> IdleEvent;

    /// <summary>Gets the number of actions waiting in the queue.</summary>
    int NumOpenActions { get; }

    /// <summary>Gets whether an action is executing or queued.</summary>
    bool IsBusy { get; }

    /// <summary>Gets the token canceled when the worker is stopped.</summary>
    CancellationToken CancellationToken { get; }

#endregion
}