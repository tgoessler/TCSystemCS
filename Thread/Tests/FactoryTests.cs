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
using NUnit.Framework;

#endregion

namespace TCSystem.Thread.Tests;

[TestFixture]
public class FactoryTests
{
    [Test]
    public void CreateAsyncUpdateHelper_ReturnsNonNull()
    {
        IAsyncUpdateHelper helper = Factory.CreateAsyncUpdateHelper();
        Assert.That(helper, Is.Not.Null);
    }

    [Test]
    public void CreateMultipleTasksExecute_ReturnsNonNull()
    {
        IMultipleTasksExecute executor = Factory.CreateMultipleTasksExecute(2);
        Assert.That(executor, Is.Not.Null);
    }

    [Test]
    public void CreateWorkerThread_ReturnsNonNull()
    {
        IWorkerThread worker = Factory.CreateWorkerThread("TestThread", ThreadPriority.Normal);
        Assert.That(worker, Is.Not.Null);
        worker.StopThread();
    }
}