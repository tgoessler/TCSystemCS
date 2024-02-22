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

using System.IO;
using NUnit.Framework;

#endregion

namespace TCSystem.MetaDataDB.Tests;

[TestFixture]
public class ConverterTests
{
    [TearDown]
    public void DeInitTestDB()
    {
        if (_db1 != null)
        {
            Factory.Destroy(ref _db1);
            File.Delete(_dbFileName1);
        }

        if (_db2 != null)
        {
            Factory.Destroy(ref _db2);
            File.Delete(_dbFileName2);
        }
    }

    [Test]
    public void ConvertDBTest()
    {
        string[] files =
        {
            "MetaData2-v11.db",
            "MetaData2-v12.db",
        };

        string testDataDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData");

        foreach (string file in files)
        {
            string fileName = Path.Combine(testDataDir, file);
            _dbFileName1 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _dbFileName2 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            File.Copy(fileName, _dbFileName1);

            _db1 = Factory.CreateReadWrite(_dbFileName1);
            _db2 = Factory.CreateReadWrite(_dbFileName2);

            IDB2Converter converter = Factory.CreateConverter();
            converter.Convert(_db1, _db2);

            DeInitTestDB();
        }
    }

    private IDB2 _db1;
    private IDB2 _db2;
    private string _dbFileName1;
    private string _dbFileName2;
}