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
using System.Threading.Tasks;
using NUnit.Framework;

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class AsyncUpdateHelperExtTests
{
    [SetUp]
    public void SetUp()
    {
        _helper = Factory.CreateAsyncUpdateHelper();
    }

    [Test]
    public async Task BeginUpdateScopeAsync_Dispose_ReleasesLock()
    {
        IDisposable scope = await _helper.BeginUpdateScopeAsync();

        // While scope is held, a new BeginUpdateAsync should block — ShouldStop == false only after acquire
        Assert.That(_helper.ShouldStop, Is.False);

        // Dispose must release the internal lock
        scope.Dispose();

        // After dispose another acquire should succeed immediately
        await _helper.BeginUpdateAsync();
        _helper.EndUpdate();
    }

    [Test]
    public async Task WaitScopeAsync_Dispose_ReleasesLock()
    {
        IDisposable scope = await _helper.WaitScopeAsync();

        Assert.That(_helper.IsUpdatePending, Is.False);

        scope.Dispose();

        // After dispose another acquire should succeed immediately
        await _helper.WaitAsync();
        _helper.EndUpdate();
    }

    [Test]
    public async Task BeginUpdateScopeAsync_UsedWithUsing_ReleasesOnExit()
    {
        IDisposable scope = await _helper.BeginUpdateScopeAsync();
        // Lock held
        scope.Dispose();

        // Lock released; next acquire must be immediate
        await _helper.BeginUpdateAsync();
        _helper.EndUpdate();
    }

    private IAsyncUpdateHelper _helper;
}
