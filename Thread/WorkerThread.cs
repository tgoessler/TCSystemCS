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
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread
{
    internal sealed class WorkerThread : IWorkerThread
    {
#region Public

        public WorkerThread(string name, ThreadPriority priority)
        {
            _thread = new System.Threading.Thread(RunThread)
            {
                Name = name,
                Priority = priority
            };
            _running = true;
            _thread.Start();
        }

        public void ExecuteCommand(Action action)
        {
            ExecuteCommand(action, null);
        }

        public void ExecuteCommand(Action action, string message)
        {
            using (_semaphoreSlim.Lock())
            {
                _workerActions.Enqueue((action, message));
                _event.Set();
            }
        }

        public void ClearOpenCommands()
        {
            using (_semaphoreSlim.Lock())
            {
                _workerActions.Clear();
            }
        }

        public async Task ExecuteCommandAsync(Action action)
        {
            await ExecuteCommandAsync(action, null);
        }

        public async Task ExecuteCommandAsync(Action action, string message)
        {
            using (await _semaphoreSlim.LockAsync())
            {
                _workerActions.Enqueue((action, message));
                _event.Set();
            }
        }

        public async Task ClearOpenCommandsAsync()
        {
            using (await _semaphoreSlim.LockAsync())
            {
                _workerActions.Clear();
            }
        }

        public void StopThread()
        {
            if (_thread != null && _thread.IsAlive)
            {
                Log.Instance.Info($"Stopping Thread '{_thread.Name}'");

                using (_semaphoreSlim.Lock())
                {
                    ClearOpenCommands();
                    _running = false;
                    _event.Set();
                }

                _thread.Join();

                Log.Instance.Info($"Stopping Thread '{_thread.Name} done'");
            }
        }

        public bool IsBusy
        {
            get
            {
                using (_semaphoreSlim.Lock())
                {
                    return _isBusy || _workerActions.Count > 0;
                }
            }
        }

#endregion

#region Private

        private void RunThread()
        {
            (Action Action, string Message)? action = null;

            _isBusy = true;
            while (_running)
            {
                try
                {
                    action = GetNextAction();
                    if (action == null)
                    {
                        _isBusy = false;
                        _event.WaitOne();
                        _isBusy = true;
                    }
                    else
                    {
                        action.Value.Action();
                    }
                }
                catch (Exception e)
                {
                    Log.Instance.Error($"Error executing {action?.Message}", e);
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }

        private (Action Action, string Message)? GetNextAction()
        {
            using (_semaphoreSlim.Lock())
            {
                if (_workerActions.Count > 0)
                {
                    return _workerActions.Dequeue();
                }
            }

            return null;
        }

        private bool _running;
        private readonly System.Threading.Thread _thread;
        private readonly SemaphoreSlim _semaphoreSlim = new(1);
        private readonly EventWaitHandle _event = new(false, EventResetMode.AutoReset);
        private readonly Queue<(Action Action, string Message)> _workerActions = new();
        private bool _isBusy;

#endregion
    }
}