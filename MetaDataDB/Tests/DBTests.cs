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
using NUnit.Framework;

#endregion

namespace TCSystem.MetaDataDB.Tests
{
    [TestFixture]
    public sealed class DBTests : DBSetup
    {
        [Test]
        public void AddMetaData()
        {
            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

            var data = DB.GetMetaData(TestData.ImageZero.FileName);
            AsserImageDataNotEqual(TestData.ImageZero, data);

            data = DB.GetMetaData(TestData.Image1.FileName);
            AsserImageDataNotEqual(TestData.Image1, data);

            data = DB.GetMetaData(TestData.Image2.FileName);
            AsserImageDataNotEqual(TestData.Image2, data);
        }
    }
}