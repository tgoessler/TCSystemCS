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
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests;

public class DBSetup
{
#region Public

    [SetUp]
    public void InitTestDB()
    {
        _dbFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _db = Factory.CreateReadWrite(_dbFileName);
        _dbReadOnly = Factory.CreateRead(_dbFileName);
    }

    [TearDown]
    public void DeInitTestDB()
    {
        Factory.Destroy(ref _db);
        Factory.Destroy(ref _dbReadOnly);
        File.Delete(_dbFileName);
    }

#endregion

#region Protected

    protected static void AssertImageDataNotEqual(Image data1, Image data2)
    {
        data1 = data1.InvalidateId();
        data2 = data2.InvalidateId();

        Assert.That(data1.Tags, Is.EquivalentTo(data2.Tags));
        Assert.That(data1.PersonTags, Is.EquivalentTo(data2.PersonTags));
        Assert.That(data1.Location, Is.EqualTo(data2.Location));
        Assert.That(data1.DateTaken, Is.EqualTo(data2.DateTaken));
        Assert.That(data1, Is.EqualTo(data2));
    }

    protected IDB2 DB => _db;
    protected IDB2Read DBReadOnly => _dbReadOnly;

#endregion

#region Private

    private IDB2 _db;
    private IDB2Read _dbReadOnly;
    private string _dbFileName;

#endregion
}