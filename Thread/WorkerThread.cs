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

        public void StopThread(TimeSpan? timeOut=null)
        {
            if (_thread is { IsAlive: true })
            {
                Log.Instance.Info($"Stopping Thread '{_thread.Name}'");

                using (_semaphoreSlim.Lock())
                {
                    _workerActions.Clear();
                    _running = false;
                    _cancellationTokenSource.Cancel();
                    _event.Set();
                }

                if (timeOut == null)
                {
                    _thread.Join();
                }
                else
                {
                    _thread.Join(timeOut.Value);
                }

                Log.Instance.Info($"Stopping Thread '{_thread.Name} done'");
            }
        }

        public int NumOpenActions
        {
            get
            {
                using (_semaphoreSlim.Lock())
                {
                    return _workerActions.Count;
                }
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

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public event Action OnInitThread;
        public event Action OnDeInitThread;
        public event Action<bool> IdleEvent;

#endregion

#region Private

        private void RunThread()
        {
            (Action Action, string Message)? action = null;

            GoToIdle(false);
            while (_running)
            {
                try
                {
                    action = GetNextAction();
                    if (action == null)
                    {
                        GoToIdle(true);

                        _event.WaitOne();

                        GoToIdle(false);
                    }
                    else
                    {
                        ExecuteAction(action.Value);
                    }
                }
                catch (ThreadAbortException e)
                {
                    if (_isBusy)
                    {
                        Log.Instance.Error($"Error executing {action?.Message}", e);
                    }

                    _running = false;
                }
                catch (Exception e)
                {
                    Log.Instance.Error($"Error executing {action?.Message}", e);
                }
            }

            if (_init)
            {
                OnDeInitThread?.Invoke();
                _init = false;
            }

        }

        private void ExecuteAction((Action Action, string Message) action)
        {
            try
            {
                if (!_init)
                {
                    OnInitThread?.Invoke();
                    _init = true;
                }

                action.Action();
            }
            catch (OperationCanceledException)
            {
                Log.Instance.Info($"Thread '{_thread.Name}' canceled");
            }
            catch (Exception e)
            {
                Log.Instance.Error($"Thread '{_thread.Name}' failed.", e);
            }
        }

        private void GoToIdle(bool idle)
        {
            _isBusy = !idle;
            IdleEvent?.Invoke(idle);
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
        private bool _init = false;
        private readonly System.Threading.Thread _thread;
        private readonly SemaphoreSlim _semaphoreSlim = new(1);
        private readonly EventWaitHandle _event = new(false, EventResetMode.AutoReset);
        private readonly Queue<(Action Action, string Message)> _workerActions = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isBusy;

#endregion
    }
}