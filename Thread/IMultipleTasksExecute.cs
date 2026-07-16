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

#endregion

namespace TCSystem.Thread;

/// <summary>Executes actions with a bounded degree of concurrency.</summary>
public interface IMultipleTasksExecute
{
#region Public

    /// <summary>Queues an action after a concurrency slot becomes available.</summary>
    void ExecuteCommand(Action action);

    /// <summary>Queues an action after a concurrency slot becomes available, observing cancellation while waiting.</summary>
    void ExecuteCommand(Action action, CancellationToken token);

    /// <summary>Blocks until every queued action has completed.</summary>
    void WaitAllDone();

    /// <summary>Blocks until every queued action has completed or cancellation is requested.</summary>
    void WaitAllDone(CancellationToken token);

#endregion
}