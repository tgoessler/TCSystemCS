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

using System.Threading;

#endregion

namespace TCSystem.Thread;

/// <summary>Creates TCSystem concurrency helper implementations.</summary>
public static class Factory
{
#region Public

    /// <summary>Creates and starts a FIFO worker thread.</summary>
    public static IWorkerThread CreateWorkerThread(string name, ThreadPriority priority)
    {
        return new WorkerThread(name, priority);
    }

    /// <summary>Creates an asynchronous update coordinator.</summary>
    public static IAsyncUpdateHelper CreateAsyncUpdateHelper()
    {
        return new AsyncUpdateHelper();
    }

    /// <summary>Creates an executor limited to a maximum number of concurrent tasks.</summary>
    public static IMultipleTasksExecute CreateMultipleTasksExecute(int maxNumberOfTasks)
    {
        return new MultipleTasksExecute(maxNumberOfTasks);
    }

#endregion
}