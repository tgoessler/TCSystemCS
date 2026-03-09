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

public static class Factory
{
#region Public

    public static IWorkerThread CreateWorkerThread(string name, ThreadPriority priority)
    {
        return new WorkerThread(name, priority);
    }

    public static IAsyncUpdateHelper CreateAsyncUpdateHelper()
    {
        return new AsyncUpdateHelper();
    }

    public static IMultipleTasksExecute CreateMultipleTasksExecute(int maxNumberOfTasks)
    {
        return new MultipleTasksExecute(maxNumberOfTasks);
    }

    #endregion
}