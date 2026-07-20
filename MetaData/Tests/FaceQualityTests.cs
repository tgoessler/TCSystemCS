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
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class FaceQualityTests
{
    [Test]
    public void EnumContractTest()
    {
        string[] names = Enum.GetNames<FaceQuality>();

        Assert.That(names, Has.Length.EqualTo(5));
        Assert.That(names, Is.EquivalentTo(new[]
        {
            nameof(FaceQuality.Normal),
            nameof(FaceQuality.Poor),
            nameof(FaceQuality.Good),
            nameof(FaceQuality.Unusable),
            nameof(FaceQuality.Excellent)
        }));
        Assert.That((int)FaceQuality.Normal, Is.EqualTo(0));
        Assert.That((int)FaceQuality.Poor, Is.EqualTo(1));
        Assert.That((int)FaceQuality.Good, Is.EqualTo(2));
        Assert.That((int)FaceQuality.Unusable, Is.EqualTo(3));
        Assert.That((int)FaceQuality.Excellent, Is.EqualTo(4));
    }
}
