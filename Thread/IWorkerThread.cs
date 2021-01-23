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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread
{
    public interface IWorkerThread
    {
#region Public

        void ExecuteCommand(Action action);
        Task ExecuteCommandAsync(Action action);
        void ExecuteCommand(Action action, string message);
        Task ExecuteCommandAsync(Action action, string message);
        void ClearOpenCommands();
        Task ClearOpenCommandsAsync();
        void StopThread();
        bool IsBusy { get; }

#endregion
    }
}