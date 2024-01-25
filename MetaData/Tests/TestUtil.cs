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
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

public static class TestUtil
{
#region Public

    public static void FromJsonStringTest<TData>(TData data1, Func<TData, string> toJson, Func<string, TData> fromJson)
    {
        string jsonString = toJson(data1);
        TData jsonData = fromJson(jsonString);
        Assert.That(jsonData, Is.EqualTo(data1));
    }

    public static void GetHashCodeTest<TData>(TData dataZero, TData data1, TData data2, TData copyOfData1)
    {
        Assert.That(dataZero.GetHashCode(), !Is.EqualTo(data1.GetHashCode()));
        Assert.That(dataZero.GetHashCode(), !Is.EqualTo(data2.GetHashCode()));
        Assert.That(data1.GetHashCode(), !Is.EqualTo(data2.GetHashCode()));

        Assert.That(data1.GetHashCode(), Is.EqualTo(copyOfData1.GetHashCode()));
    }

#endregion
}