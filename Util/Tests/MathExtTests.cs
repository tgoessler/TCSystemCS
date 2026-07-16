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

using NUnit.Framework;

#endregion

namespace TCSystem.Util.Tests;

[TestFixture]
public class MathExtTests
{
    [Test]
    public void ToNextPowerOf2_LargeValues()
    {
        Assert.That(65536.ToNextPowerOf2(), Is.EqualTo(65536));
        Assert.That(65537.ToNextPowerOf2(), Is.EqualTo(131072));
    }

    [Test]
    public void ToNextPowerOf2_NegativeValue_ReturnsZero()
    {
        Assert.That((-1).ToNextPowerOf2(), Is.EqualTo(0));
        Assert.That((-100).ToNextPowerOf2(), Is.EqualTo(0));
    }

    [Test]
    public void ToNextPowerOf2_NonPowerOf2_ReturnsNextPower()
    {
        Assert.That(3.ToNextPowerOf2(), Is.EqualTo(4));
        Assert.That(5.ToNextPowerOf2(), Is.EqualTo(8));
        Assert.That(7.ToNextPowerOf2(), Is.EqualTo(8));
        Assert.That(9.ToNextPowerOf2(), Is.EqualTo(16));
        Assert.That(17.ToNextPowerOf2(), Is.EqualTo(32));
        Assert.That(100.ToNextPowerOf2(), Is.EqualTo(128));
        Assert.That(1000.ToNextPowerOf2(), Is.EqualTo(1024));
    }

    [Test]
    public void ToNextPowerOf2_One_ReturnsOne()
    {
        Assert.That(1.ToNextPowerOf2(), Is.EqualTo(1));
    }

    [Test]
    public void ToNextPowerOf2_PowerOf2_ReturnsSameValue()
    {
        Assert.That(2.ToNextPowerOf2(), Is.EqualTo(2));
        Assert.That(4.ToNextPowerOf2(), Is.EqualTo(4));
        Assert.That(8.ToNextPowerOf2(), Is.EqualTo(8));
        Assert.That(16.ToNextPowerOf2(), Is.EqualTo(16));
        Assert.That(1024.ToNextPowerOf2(), Is.EqualTo(1024));
    }

    [Test]
    public void ToNextPowerOf2_Zero_ReturnsZero()
    {
        Assert.That(0.ToNextPowerOf2(), Is.EqualTo(0));
    }
}