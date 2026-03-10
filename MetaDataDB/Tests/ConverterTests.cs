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
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TCSystem.MetaData;

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
    public void CheckAddresses([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        IList<Address> addresses1 = _db1.GetAllAddressesLike();
        IList<Address> addresses2 = _db2.GetAllAddressesLike();
        Assert.That(addresses1.Count, Is.GreaterThanOrEqualTo(addresses2.Count));

        foreach (Address address in addresses1)
        {
            if (address.IsSet)
            {
                long num1 = _db1.GetNumFilesOfAddress(address, true);
                long num2 = _db2.GetNumFilesOfAddress(address, true);
                Assert.That(num1, Is.EqualTo(num2));

                IList<string> files1 = _db1.GetFilesOfAddress(address, true);
                IList<string> files2 = _db2.GetFilesOfAddress(address, true);
                string[] files3 = files1.Except(files2).ToArray();
                Assert.That(files3.Length, Is.EqualTo(0));
            }
        }
    }

    [Test]
    public void CheckFaces([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName, [Values(true, false)] bool visibleOnly)
    {
        ConvertDB(fileName);

        IList<FaceInfo> faceInfos1 = _db1.GetAllFaceInfos(visibleOnly);
        IList<FaceInfo> faceInfos2 = _db2.GetAllFaceInfos(visibleOnly);
        Assert.That(faceInfos1.Count, Is.GreaterThanOrEqualTo(faceInfos2.Count));
    }

    [Test]
    public void CheckFiles([Values("MetaData2-v11.db", "MetaData2-v12.db")] string dbFileName)
    {
        ConvertDB(dbFileName);

        IList<string> files1 = _db1.GetAllFilesLike();
        IList<string> files2 = _db2.GetAllFilesLike();
        string[] files3 = files1.Except(files2).ToArray();
        Assert.That(files3.Count, Is.EqualTo(0));

        IList<(string FileName, ProcessingInfos ProcessingInfo)> proc1 = _db1.GetAllProcessingInformation();
        IList<(string FileName, ProcessingInfos ProcessingInfo)> proc2 = _db2.GetAllProcessingInformation();
        (string FileName, ProcessingInfos ProcessingInfo)[] proc3 = proc1.Except(proc2).ToArray();
        Assert.That(proc3.Count, Is.EqualTo(0));


        IDictionary<string, DateTimeOffset> fmd1 = _db1.GetAllFileAndModifiedDates();
        IDictionary<string, DateTimeOffset> fmd2 = _db2.GetAllFileAndModifiedDates();
        // ReSharper disable once UsageOfDefaultStructEquality
        KeyValuePair<string, DateTimeOffset>[] fmd3 = fmd1.Except(fmd2).ToArray();
        Assert.That(fmd3.Count, Is.EqualTo(0));

        foreach (string fileName in files1)
        {
            DateTimeOffset date1 = _db1.GetDateModified(fileName);
            DateTimeOffset date2 = _db2.GetDateModified(fileName);
            Assert.That(date1, Is.EqualTo(date2));

            OrientationMode o1 = _db1.GetOrientation(fileName);
            OrientationMode o2 = _db2.GetOrientation(fileName);
            Assert.That(o1, Is.EqualTo(o2));

            Location l1 = _db1.GetLocation(fileName);
            Location l2 = _db2.GetLocation(fileName);
            Assert.That(l1, Is.EqualTo(l2));

            CheckConvertedData(fileName);
        }

        foreach (string folder in files1.Select(Path.GetDirectoryName).Distinct())
        {
            CheckFolder(folder);
        }
    }

    [Test]
    public void CheckLocations([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        IList<Location> locations1 = _db1.GetAllLocations();
        IList<Location> locations2 = _db2.GetAllLocations();
        Assert.That(locations1.Count, Is.GreaterThanOrEqualTo(locations2.Count));
    }

    [Test]
    public void CheckPersons([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        IList<string> personNames1 = _db1.GetAllPersonNamesLike();
        IList<string> personNames2 = _db2.GetAllPersonNamesLike();
        Assert.That(personNames1.Count, Is.GreaterThanOrEqualTo(personNames2.Count));

        foreach (string personName in personNames1)
        {
            long num1 = _db1.GetNumFilesOfPerson(personName);
            long num2 = _db2.GetNumFilesOfPerson(personName);
            Assert.That(num1, Is.EqualTo(num2));
            if (num1 == 0)
            {
                continue;
            }

            long personId1 = _db1.GetPersonIdFromName(personName);

            Person p1 = _db1.GetPersonFromId(personId1).InvalidateId();
            Person p2 = _db2.GetPersonFromName(personName).InvalidateId();
            Assert.That(p1, Is.EqualTo(p2));


            IList<string> files1 = _db1.GetFilesOfPerson(personName);
            IList<string> files2 = _db2.GetFilesOfPerson(personName);
            string[] files3 = files1.Except(files2).ToArray();
            Assert.That(files3.Length, Is.EqualTo(0));
        }
    }

    [Test]
    public void CheckPersonTags([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName, [Values(true, false)] bool visibleOnly)
    {
        ConvertDB(fileName);

        IList<string> personNames1 = _db1.GetAllPersonNamesLike();
        foreach (string name in personNames1)
        {
            IEnumerable<FileAndPersonTag> personTags1 = _db1.GetFileAndPersonTagsOfPerson(name, visibleOnly)
                .Select(f => new FileAndPersonTag(f.FileName, f.PersonTag.InvalidateId()));
            IEnumerable<FileAndPersonTag> personTags2 = _db2.GetFileAndPersonTagsOfPerson(name, visibleOnly)
                .Select(f => new FileAndPersonTag(f.FileName, f.PersonTag.InvalidateId()));
            FileAndPersonTag[] personTags3 = personTags1.Except(personTags2).ToArray();
            if (personTags3.Length > 0)
            {
                Assert.That(personTags3.Length, Is.EqualTo(0));
            }
            Assert.That(personTags3.Length, Is.EqualTo(0));
        }
    }

    [Test]
    public void CheckTags([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        IList<string> tags1 = _db1.GetAllTagsLike();
        IList<string> tags2 = _db2.GetAllTagsLike();
        Assert.That(tags1.Count, Is.GreaterThanOrEqualTo(tags2.Count));

        foreach (string tag in tags1)
        {
            long num1 = _db1.GetNumFilesOfTag(tag);
            long num2 = _db2.GetNumFilesOfTag(tag);
            Assert.That(num1, Is.EqualTo(num2));

            IList<string> files1 = _db1.GetFilesOfTag(tag);
            IList<string> files2 = _db2.GetFilesOfTag(tag);
            string[] files3 = files1.Except(files2).ToArray();
            Assert.That(files3.Length, Is.EqualTo(0));
        }
    }

    [Test]
    public void CheckYears([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        IList<DateTimeOffset> years1 = _db1.GetAllYears();
        IList<DateTimeOffset> years2 = _db2.GetAllYears();
        DateTimeOffset[] years3 = years1.Except(years2).ToArray();
        Assert.That(years3.Count, Is.EqualTo(0));

        foreach (DateTimeOffset year in years1)
        {
            long num1 = _db1.GetNumFilesOfYear(year);
            long num2 = _db2.GetNumFilesOfYear(year);
            Assert.That(num1, Is.EqualTo(num2));

            IList<string> files1 = _db1.GetFilesOfYear(year);
            IList<string> files2 = _db2.GetFilesOfYear(year);
            string[] files3 = files1.Except(files2).ToArray();
            Assert.That(files3.Length, Is.EqualTo(0));
        }
    }

    [Test]
    public void TestDbCounters([Values("MetaData2-v11.db", "MetaData2-v12.db")] string fileName)
    {
        ConvertDB(fileName);

        Assert.That(_db1.GetNumFiles(), Is.EqualTo(_db2.GetNumFiles()));
        Assert.That(_db1.GetNumTags(), Is.GreaterThanOrEqualTo(_db2.GetNumTags()));
        Assert.That(_db1.GetNumPersons(), Is.GreaterThanOrEqualTo(_db2.GetNumPersons()));
        Assert.That(_db1.GetNumLocations(), Is.GreaterThanOrEqualTo(_db2.GetNumLocations()));
        Assert.That(_db1.GetNumFaces(), Is.EqualTo(_db2.GetNumFaces()));
        Assert.That(_db1.GetNumAutoDetectedFaces(), Is.EqualTo(_db2.GetNumAutoDetectedFaces()));
    }

    private void ConvertDB(string fileName)
    {
        fileName = Path.Combine(TestDataDirectory, fileName);
        _dbFileName1 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _dbFileName2 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        if (!File.Exists(fileName))
        {
            Assert.Ignore("DB file not available");
        }
        File.Copy(fileName, _dbFileName1);

        _db1 = Factory.CreateReadWrite(_dbFileName1);
        _db2 = Factory.CreateReadWrite(_dbFileName2);

        IDB2Converter converter = Factory.CreateConverter();
        converter.Convert(_db1, _db2);
    }

    private void CheckFolder(string folder)
    {
        long num1 = _db1.GetNumFilesOfFolder(folder);
        long num2 = _db2.GetNumFilesOfFolder(folder);
        Assert.That(num1, Is.EqualTo(num2));

        IList<string> files1 = _db1.GetFilesOfFolder(folder);
        IList<string> files2 = _db2.GetFilesOfFolder(folder);
        string[] files3 = files1.Except(files2).ToArray();
        Assert.That(files3.Length, Is.EqualTo(0));
    }

    private void CheckConvertedData(string file)
    {
        Image originalData = _db1.GetMetaData(file).InvalidateId();
        Image convertedData = _db2.GetMetaData(file).InvalidateId();
        Assert.That(originalData, Is.EqualTo(convertedData));
    }


    private string TestDataDirectory => Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData");

    private IDB2 _db1;
    private IDB2 _db2;
    private string _dbFileName1;
    private string _dbFileName2;
}