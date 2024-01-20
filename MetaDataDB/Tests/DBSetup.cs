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

using System.IO;
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests
{
    public class DBSetup
    {
#region Public

        [SetUp]
        public void InitTestDB()
        {
            var dbFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _db = Factory.CreateReadWrite(dbFileName);
        }

        [TearDown]
        public static void DeInitTestDB()
        {
            Factory.Destroy(ref _db);
            var dbFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Delete(dbFileName);
        }

        public static void AsserImageDataNotEqual(Image data1, Image data2)
        {
            Assert.That(data1.InvalidateId(), Is.EqualTo(data2.InvalidateId()));
        }

        public static IDB2 DB => _db;

#endregion

#region Private

        private static IDB2 _db;

#endregion
    }
}