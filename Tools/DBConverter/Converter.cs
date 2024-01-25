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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TCSystem.MetaData;
using TCSystem.MetaDataDB;
using Factory = TCSystem.Logging.Factory;

#endregion

namespace TCSystem.Tools.DBConverter;

internal sealed class Converter
{
#region Public

    public void Start(IDB2Read from, IDB2 to)
    {
        _fromDB = from;
        _toDB = to;

        _fromDB.EnableUnsafeMode();
        _toDB.EnableUnsafeMode();

        ConvertDB();

        _fromDB.EnableDefaultMode();
        _toDB.EnableDefaultMode();
    }

#endregion

#region Private

    private static void Main(string[] args)
    {
        Factory.InitLogging(Factory.LoggingOptions.Debugger | Factory.LoggingOptions.Console);
        if (args.Length < 2)
        {
            Log.Instance.Fatal("Wrong number of arguments\n filename_from filename_to is required");
        }

        IDB2 db1Write = null;
        IDB2Read db1 = null;
        IDB2 db2 = null;
        string fileToConvert = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            File.Delete(fileToConvert);
            File.Delete(args[1]);

            File.Copy(args[0], fileToConvert);

            db1Write = MetaDataDB.Factory.CreateReadWrite(fileToConvert); // just to update version
            MetaDataDB.Factory.Destroy(ref db1Write);

            db1 = MetaDataDB.Factory.CreateRead(fileToConvert);
            db2 = MetaDataDB.Factory.CreateReadWrite(args[1]);

            var converter = new Converter();
            converter.Start(db1, db2);
        }
        catch (Exception e)
        {
            Log.Instance.Fatal("Received exception", e);
        }
        finally
        {
            MetaDataDB.Factory.Destroy(ref db1Write);
            MetaDataDB.Factory.Destroy(ref db1);
            MetaDataDB.Factory.Destroy(ref db2);

            File.Delete(fileToConvert);
            File.Delete(fileToConvert + "-shm");
            File.Delete(fileToConvert + "-wal");
        }

        Factory.DeInitLogging();
    }

    private void ConvertDB()
    {
        Log.Instance.Info("ConvertDB ...");

        try
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            IList<string> files = _fromDB.GetAllFilesLike();
            Log.Instance.Info($"Converting old database format to new format: {files.Count} Entries");

            foreach (string fileName in files)
            {
                Image originalData = ConvertFile(fileName);
                CheckConvertedData(fileName, originalData);
            }

            stopWatch.Stop();
            Log.Instance.Info($"ConvertDB done in {stopWatch.ElapsedMilliseconds}ms.");


            Log.Instance.Info("Checking DB");
            stopWatch.Start();

            CheckDbCounters();
            CheckData();

            stopWatch.Stop();
            Log.Instance.Info($"Checking DB done in {stopWatch.ElapsedMilliseconds}ms.");
        }
        catch (Exception e)
        {
            Log.Instance.Error("Failed converting database", e);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void CheckData()
    {
        Log.Instance.Info("Checking data");

        CheckYears();
        CheckTags();
        CheckPersons();
        CheckLocations();
        CheckFaces(true);
        CheckFaces(false);
        CheckPersonTags(true);
        CheckPersonTags(false);
        CheckFiles();
    }

    private void CheckFiles()
    {
        Log.Instance.Info("Checking files");

        IList<string> files1 = _fromDB.GetAllFilesLike();
        IList<string> files2 = _toDB.GetAllFilesLike();
        string[] files3 = files1.Except(files2).ToArray();
        if (files3.Length > 0)
        {
            throw new InvalidProgramException($"List of all Files is different {string.Join(",", files3)}");
        }

        IList<(string FileName, ProcessingInfos ProcessingInfo)> proc1 = _fromDB.GetAllProcessingInformation();
        IList<(string FileName, ProcessingInfos ProcessingInfo)> proc2 = _toDB.GetAllProcessingInformation();
        (string FileName, ProcessingInfos ProcessingInfo)[] proc3 = proc1.Except(proc2).ToArray();
        if (proc3.Length > 0)
        {
            throw new InvalidProgramException($"List of all processing info is different {string.Join(",", proc3)}");
        }


        IDictionary<string, DateTimeOffset> fmd1 = _fromDB.GetAllFileAndModifiedDates();
        IDictionary<string, DateTimeOffset> fmd2 = _toDB.GetAllFileAndModifiedDates();
        KeyValuePair<string, DateTimeOffset>[] fmd3 = fmd1.Except(fmd2).ToArray();
        if (fmd3.Length > 0)
        {
            throw new InvalidProgramException($"List of all file and modified dates is different {string.Join(",", fmd3)}");
        }

        foreach (string fileName in files1)
        {
            DateTimeOffset date1 = _fromDB.GetDateModified(fileName);
            DateTimeOffset date2 = _toDB.GetDateModified(fileName);
            if (date1 != date2)
            {
                throw new InvalidProgramException($"Date of {fileName} not equal. date1={date1}, date2={date2}");
            }

            OrientationMode o1 = _fromDB.GetOrientation(fileName);
            OrientationMode o2 = _toDB.GetOrientation(fileName);
            if (o1 != o2)
            {
                throw new InvalidProgramException($"Orientation of {fileName} not equal. o1={o1}, o2={o2}");
            }

            Location l1 = _fromDB.GetLocation(fileName);
            Location l2 = _toDB.GetLocation(fileName);
            if (!(ReferenceEquals(l1, l2) || l1.Equals(l2)))
            {
                throw new InvalidProgramException($"Location of {fileName} not equal. l1={l1}, l2={l2}");
            }
        }

        foreach (string folder in files1.Select(Path.GetDirectoryName).Distinct())
        {
            CheckFolder(folder);
        }
    }

    private void CheckFolder(string folder)
    {
        Log.Instance.Info($"Checking folder {folder}");

        long num1 = _fromDB.GetNumFilesOfFolder(folder);
        long num2 = _toDB.GetNumFilesOfFolder(folder);
        if (num1 != num2)
        {
            throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
        }

        IList<string> files1 = _fromDB.GetFilesOfFolder(folder);
        IList<string> files2 = _toDB.GetFilesOfFolder(folder);
        string[] files3 = files1.Except(files2).ToArray();
        if (files3.Length > 0)
        {
            throw new InvalidProgramException($"List of Files of ${folder} is different {string.Join(",", files3)}");
        }
    }

    private void CheckYears()
    {
        Log.Instance.Info("Checking years");

        IList<DateTimeOffset> years1 = _fromDB.GetAllYears();
        IList<DateTimeOffset> years2 = _toDB.GetAllYears();
        DateTimeOffset[] years3 = years1.Except(years2).ToArray();
        if (years3.Length > 0)
        {
            throw new InvalidProgramException($"List of all years is different {string.Join(",", years3.Select(y => y.ToString()))}");
        }

        foreach (DateTimeOffset year in years1)
        {
            long num1 = _fromDB.GetNumFilesOfYear(year);
            long num2 = _toDB.GetNumFilesOfYear(year);
            if (num1 != num2)
            {
                throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
            }

            IList<string> files1 = _fromDB.GetFilesOfYear(year);
            IList<string> files2 = _toDB.GetFilesOfYear(year);
            string[] files3 = files1.Except(files2).ToArray();
            if (files3.Length > 0)
            {
                throw new InvalidProgramException($"List of Files of Year={year} is different {string.Join(",", files3)}");
            }
        }
    }

    private void CheckPersons()
    {
        Log.Instance.Info("Checking persons");

        IList<string> personNames1 = _fromDB.GetAllPersonNamesLike();
        IList<string> personNames2 = _toDB.GetAllPersonNamesLike();
        string[] personNames3 = personNames1.Except(personNames2).ToArray();
        if (personNames3.Length > 0)
        {
            Log.Instance.Warn($"List of all Person Names is different {string.Join(",", personNames3)}");
        }

        foreach (string personName in personNames1)
        {
            long personId1 = _fromDB.GetPersonIdFromName(personName);

            Person p1 = _fromDB.GetPersonFromId(personId1).InvalidateId();
            Person p2 = _toDB.GetPersonFromName(personName).InvalidateId();
            if (!p1.Equals(p2))
            {
                throw new InvalidProgramException($"Persons are different, '{p1}' != '{p2}''");
            }

            long num1 = _fromDB.GetNumFilesOfPerson(personName);
            long num2 = _toDB.GetNumFilesOfPerson(personName);
            if (num1 != num2)
            {
                throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
            }

            IList<string> files1 = _fromDB.GetFilesOfPerson(personName);
            IList<string> files2 = _toDB.GetFilesOfPerson(personName);
            string[] files3 = files1.Except(files2).ToArray();
            if (files3.Length > 0)
            {
                throw new InvalidProgramException($"List of Files of Person={personName} is different {string.Join(",", files3)}");
            }
        }
    }

    private void CheckFaces(bool visibleOnly)
    {
        Log.Instance.Info($"Checking faces visibleOnly={visibleOnly}");

        IList<FaceInfo> faceInfos1 = _fromDB.GetAllFaceInfos(visibleOnly);
        IList<FaceInfo> faceInfos2 = _toDB.GetAllFaceInfos(visibleOnly);
        if (faceInfos1.Count != faceInfos2.Count)
        {
            throw new InvalidProgramException("Length of all Face Infos is different");
        }
    }

    private void CheckPersonTags(bool visibleOnly)
    {
        Log.Instance.Info($"Checking person tags visibleOnly={visibleOnly}");

        IList<string> personNames1 = _fromDB.GetAllPersonNamesLike();
        foreach (string name in personNames1)
        {
            IEnumerable<FileAndPersonTag> personTags1 = _fromDB.GetFileAndPersonTagsOfPerson(name, visibleOnly)
                .Select(f => new FileAndPersonTag(f.FileName, f.PersonTag.InvalidateId()));
            IEnumerable<FileAndPersonTag> personTags2 = _toDB.GetFileAndPersonTagsOfPerson(name, visibleOnly)
                .Select(f => new FileAndPersonTag(f.FileName, f.PersonTag.InvalidateId()));
            FileAndPersonTag[] personTags3 = personTags1.Except(personTags2).ToArray();
            if (personTags3.Length > 0)
            {
                throw new InvalidProgramException(
                    $"List of all person tags is different for person {name}: {string.Join(",", personTags3.Select(p => p.ToString()))}");
            }
        }
    }

    private void CheckTags()
    {
        Log.Instance.Info("Checking tags");

        IList<string> tags1 = _fromDB.GetAllTagsLike();
        IList<string> tags2 = _toDB.GetAllTagsLike();
        string[] tags3 = tags1.Except(tags2).ToArray();
        if (tags3.Length > 0)
        {
            Log.Instance.Warn($"List of all Tags is different {string.Join(",", tags3)}");
        }

        foreach (string tag in tags1)
        {
            long num1 = _fromDB.GetNumFilesOfTag(tag);
            long num2 = _toDB.GetNumFilesOfTag(tag);
            if (num1 != num2)
            {
                throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
            }

            IList<string> files1 = _fromDB.GetFilesOfTag(tag);
            IList<string> files2 = _toDB.GetFilesOfTag(tag);
            string[] files3 = files1.Except(files2).ToArray();
            if (files3.Length > 0)
            {
                throw new InvalidProgramException($"List of Files of Tag={tag} is different {string.Join(",", files3)}");
            }
        }
    }

    private void CheckLocations()
    {
        Log.Instance.Info("Checking locations");

        IList<Address> locations1 = _fromDB.GetAllLocationsLike();
        IList<Address> locations2 = _toDB.GetAllLocationsLike();
        Address[] locations3 = locations1.Except(locations2).ToArray();
        if (locations3.Length > 0)
        {
            Log.Instance.Warn($"List of all Locations is different {string.Join(",", locations3.Select(l => l.ToString()))}");
        }

        foreach (Address location in locations1)
        {
            if (location.IsSet)
            {
                long num1 = _fromDB.GetNumFilesOfAddress(location, true);
                long num2 = _toDB.GetNumFilesOfAddress(location, true);
                if (num1 != num2)
                {
                    throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
                }

                IList<string> files1 = _fromDB.GetFilesOfAddress(location, true);
                IList<string> files2 = _toDB.GetFilesOfAddress(location, true);
                string[] files3 = files1.Except(files2).ToArray();
                if (files3.Length > 0)
                {
                    throw new InvalidProgramException($"List of Files of Location={location} is different {string.Join(",", files3)}");
                }
            }
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void CheckDbCounters()
    {
        Log.Instance.Info("Checking counters");

        if (_fromDB.GetNumFiles() != _toDB.GetNumFiles())
        {
            throw new InvalidProgramException($"Num files different {_fromDB.GetNumFiles()} != {_toDB.GetNumFiles()}");
        }

        if (_fromDB.GetNumTags() != _toDB.GetNumTags())
        {
            Log.Instance.Warn($"Num tags different {_fromDB.GetNumTags()} != {_toDB.GetNumTags()}");
        }

        int add = _fromDB.Version == "1.0" ? 1 : 0;
        if (_fromDB.GetNumPersons() + add != _toDB.GetNumPersons())
        {
            Log.Instance.Warn($"Num persons different {_fromDB.GetNumPersons()} != {_toDB.GetNumPersons()}");
        }

        if (_fromDB.GetNumLocations() != _toDB.GetNumLocations())
        {
            Log.Instance.Warn($"Num locations different {_fromDB.GetNumLocations()} != {_toDB.GetNumLocations()}");
        }

        if (_fromDB.GetNumFaces() != _toDB.GetNumFaces())
        {
            throw new InvalidProgramException($"Num faces different {_fromDB.GetNumFaces()} != {_toDB.GetNumFaces()}");
        }

        if (_fromDB.GetNumAutoDetectedFaces() != _toDB.GetNumAutoDetectedFaces())
        {
            throw new InvalidProgramException(
                $"Num auto detected faces different {_fromDB.GetNumAutoDetectedFaces()} != {_toDB.GetNumAutoDetectedFaces()}");
        }
    }

    private void CheckConvertedData(string file, Image originalData)
    {
        Image convertedData = _toDB.GetMetaData(file);
        convertedData = convertedData.InvalidateId();
        if (!originalData.Equals(convertedData))
        {
            throw new InvalidProgramException($"Converting data of {file} failed\n\n{originalData}\n\n{convertedData}");
        }
    }

    private Image ConvertFile(string file)
    {
        Image data = _fromDB.GetMetaData(file);
        data = data.InvalidateId();
        DateTimeOffset dateModified = _fromDB.GetDateModified(file);

        _toDB.AddMetaData(data, dateModified);

        return data;
    }

    private IDB2Read _fromDB;
    private IDB2 _toDB;

#endregion
}