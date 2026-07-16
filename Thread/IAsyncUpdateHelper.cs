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

using System.Threading.Tasks;

#endregion

namespace TCSystem.Thread;

/// <summary>Coordinates mutually exclusive updates and exposes whether update work is waiting.</summary>
public interface IAsyncUpdateHelper
{
#region Public

    /// <summary>Marks a stop-requesting update as pending and asynchronously acquires the update lock.</summary>
    Task BeginUpdateAsync();

    /// <summary>Marks an ordinary update as pending and asynchronously acquires the update lock.</summary>
    Task WaitAsync();

    /// <summary>Releases the update lock.</summary>
    void EndUpdate();

    /// <summary>Gets whether a stop-requesting update is waiting to acquire the lock.</summary>
    bool ShouldStop { get; }

    /// <summary>Gets whether any update is waiting to acquire the lock.</summary>
    bool IsUpdatePending { get; }

#endregion
}