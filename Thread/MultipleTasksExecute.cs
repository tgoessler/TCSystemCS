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
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread;

internal sealed class MultipleTasksExecute(int maxNumberOfTasks) : IMultipleTasksExecute
{
#region Public

    public void ExecuteCommand(Action action)
    {
        ExecuteCommandInternal(action, null);
    }

    public void ExecuteCommand(Action action, CancellationToken token)
    {
        ExecuteCommandInternal(action, token);
    }

    public void WaitAllDone()
    {
        WaitAllDoneInternal(null);
    }

    public void WaitAllDone(CancellationToken token)
    {
        WaitAllDoneInternal(token);
    }

    #endregion

    #region Private
    private void ExecuteCommandInternal(Action action, CancellationToken? token)
    {
        if (token.HasValue)
        {
            _semaphore.Wait(token.Value);
        }
        else
        {
            _semaphore.Wait();
        }

        Task.Run(() =>
        {
            try
            {
                action();
            }
            finally
            {
                _semaphore.Release();
            }
        });
    }

    public void WaitAllDoneInternal(CancellationToken? token)
    {
        while(_semaphore.CurrentCount != maxNumberOfTasks)
        {   
            if (token.HasValue)
            {
                Task.Delay(100).Wait(token.Value);
            }
            else
            {
                Task.Delay(100).Wait();
            }
            
        }
    }

    private readonly SemaphoreSlim _semaphore = new(maxNumberOfTasks, maxNumberOfTasks);

#endregion
}