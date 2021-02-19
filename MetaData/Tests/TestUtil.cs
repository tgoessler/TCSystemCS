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

using System;
using NUnit.Framework;

namespace TCSystem.MetaData.Tests
{
    public static class TestUtil
    {
        public static void FromJsonStringTest<TData>(TData data1, Func<TData, string> toJson, Func<string, TData> fromJson)
        {
            var jsonString = toJson(data1);
            var jsonData = fromJson(jsonString);
            Assert.That(jsonData, Is.EqualTo(data1));
        }
    }
}