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
//  Copyright (C) 2003 - 2020 Thomas Goessler. All Rights Reserved.
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

// ReSharper disable CognitiveComplexity

#endregion

namespace TCSystem.Tools.DBConverter
{
    internal sealed class Converter
    {
#region Public

        public void Start(IDB2 from, IDB2 to, IEnumerable<string> folders)
        {
            _fromDB = from;
            _toDB = to;
            _folders = folders.ToList();

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

            File.Delete(args[1]);

            var db1 = MetaDataDB.Factory.Create(args[0]);
            var db2 = MetaDataDB.Factory.Create(args[1]);

            try
            {
                var converter = new Converter();
                converter.Start(db1, db2, args.Skip(2));
            }
            catch (Exception e)
            {
                Log.Instance.Fatal("Received exception", e);
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

                var files = _fromDB.GetAllFilesLike();
                Log.Instance.Info($"Converting old database format to new format: {files.Count}");

                foreach (var fileName in files)
                {
                    // ReSharper disable once UnusedVariable
                    var originalData = ConvertFile(fileName);
                    CheckConvertedData(fileName, originalData);
                }

                CheckDbCounters();
                CheckData();

                stopWatch.Stop();
                Log.Instance.Info($"ConvertDB done in {stopWatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception e)
            {
                Log.Instance.Error("Failed converting database", e);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void CheckData()
        {
            CheckFiles();
            CheckYears();
            CheckTags();
            CheckPersons();
            CheckLocations();
        }

        private void CheckFiles()
        {
            var files1 = _fromDB.GetAllFilesLike();
            var files2 = _toDB.GetAllFilesLike();
            var files3 = files1.Except(files2).ToArray();
            if (files3.Length > 0)
            {
                throw new InvalidProgramException($"List of all Files is different {string.Join(",", files3)}");
            }

            var proc1 = _fromDB.GetAllProcessingInformation();
            var proc2 = _toDB.GetAllProcessingInformation();
            var proc3 = proc1.Except(proc2).ToArray();
            if (proc3.Length > 0)
            {
                throw new InvalidProgramException($"List of all processing info is different {string.Join(",", proc3)}");
            }


            var fmd1 = _fromDB.GetAllFileAndModifiedDates();
            var fmd2 = _toDB.GetAllFileAndModifiedDates();
            var fmd3 = fmd1.Except(fmd2).ToArray();
            if (fmd3.Length > 0)
            {
                throw new InvalidProgramException($"List of all file and modified dates is different {string.Join(",", fmd3)}");
            }

            foreach (var fileName in files1)
            {
                var date1 = _fromDB.GetDateModified(fileName);
                var date2 = _toDB.GetDateModified(fileName);
                if (date1 != date2)
                {
                    throw new InvalidProgramException($"Date of {fileName} not equal. date1={date1}, date2={date2}");
                }

                var o1 = _fromDB.GetOrientation(fileName);
                var o2 = _toDB.GetOrientation(fileName);
                if (o1 != o2)
                {
                    throw new InvalidProgramException($"Orientation of {fileName} not equal. o1={o1}, o2={o2}");
                }
            }

            foreach (var folder in _folders)
            {
                CheckFolder(folder);
            }
        }

        private void CheckFolder(string folder)
        {
            var num1 = _fromDB.GetNumFilesOfFolder(folder);
            var num2 = _toDB.GetNumFilesOfFolder(folder);
            if (num1 != num2)
            {
                throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
            }

            var files1 = _fromDB.GetFilesOfFolder(folder);
            var files2 = _toDB.GetFilesOfFolder(folder);
            var files3 = files1.Except(files2).ToArray();
            if (files3.Length > 0)
            {
                throw new InvalidProgramException($"List of Files of ${folder} is different {string.Join(",", files3)}");
            }


            foreach (var subFolder in Directory.EnumerateDirectories(folder))
            {
                CheckFolder(subFolder);
            }
        }

        private void CheckYears()
        {
            var years1 = _fromDB.GetAllYears();
            var years2 = _toDB.GetAllYears();
            var years3 = years1.Except(years2).ToArray();
            if (years3.Length > 0)
            {
                throw new InvalidProgramException($"List of all years is different {string.Join(",", years3.Select(y => y.ToString()))}");
            }

            foreach (var year in years1)
            {
                var num1 = _fromDB.GetNumFilesOfYear(year);
                var num2 = _toDB.GetNumFilesOfYear(year);
                if (num1 != num2)
                {
                    throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
                }

                var files1 = _fromDB.GetFilesOfYear(year);
                var files2 = _toDB.GetFilesOfYear(year);
                var files3 = files1.Except(files2).ToArray();
                if (files3.Length > 0)
                {
                    throw new InvalidProgramException($"List of Files of Year={year} is different {string.Join(",", files3)}");
                }
            }
        }

        private void CheckPersons()
        {
            var personNames1 = _fromDB.GetAllPersonNamesLike();
            var personNames2 = _toDB.GetAllPersonNamesLike();
            var personNames3 = personNames1.Except(personNames2).ToArray();
            if (personNames3.Length > 0)
            {
                Log.Instance.Warn($"List of all Person Names is different {string.Join(",", personNames3)}");
            }

            foreach (var personName in personNames1)
            {
                var p1 = _fromDB.GetPersonFromName(personName);
                var p2 = _fromDB.GetPersonFromName(personName);
                if (!p1.Equals(p2))
                {
                    throw new InvalidProgramException($"Persons are different, '{p1}' != '{p2}''");
                }

                var num1 = _fromDB.GetNumFilesOfPerson(personName);
                var num2 = _toDB.GetNumFilesOfPerson(personName);
                if (num1 != num2)
                {
                    throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
                }

                var files1 = _fromDB.GetFilesOfPerson(personName);
                var files2 = _toDB.GetFilesOfPerson(personName);
                var files3 = files1.Except(files2).ToArray();
                if (files3.Length > 0)
                {
                    throw new InvalidProgramException($"List of Files of Person={personName} is different {string.Join(",", files3)}");
                }
            }
        }

        private void CheckTags()
        {
            var tags1 = _fromDB.GetAllTagsLike();
            var tags2 = _toDB.GetAllTagsLike();
            var tags3 = tags1.Except(tags2).ToArray();
            if (tags3.Length > 0)
            {
                Log.Instance.Warn($"List of all Tags is different {string.Join(",", tags3)}");
            }

            foreach (var tag in tags1)
            {
                var num1 = _fromDB.GetNumFilesOfTag(tag);
                var num2 = _toDB.GetNumFilesOfTag(tag);
                if (num1 != num2)
                {
                    throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
                }

                var files1 = _fromDB.GetFilesOfTag(tag);
                var files2 = _toDB.GetFilesOfTag(tag);
                var files3 = files1.Except(files2).ToArray();
                if (files3.Length > 0)
                {
                    throw new InvalidProgramException($"List of Files of Tag={tag} is different {string.Join(",", files3)}");
                }
            }
        }

        private void CheckLocations()
        {
            var locations1 = _fromDB.GetAllLocationsLike();
            var locations2 = _toDB.GetAllLocationsLike();
            var locations3 = locations1.Except(locations2).ToArray();
            if (locations3.Length > 0)
            {
                Log.Instance.Warn($"List of all Locations is different {string.Join(",", locations3.Select(l => l.ToString()))}");
            }

            foreach (var location in locations1)
            {
                if (location.IsSet)
                {
                    var num1 = _fromDB.GetNumFilesOfAddress(location, true);
                    var num2 = _toDB.GetNumFilesOfAddress(location, true);
                    if (num1 != num2)
                    {
                        throw new InvalidProgramException($"Num files do not match {num1} != {num2}");
                    }

                    var files1 = _fromDB.GetFilesOfAddress(location, true);
                    var files2 = _toDB.GetFilesOfAddress(location, true);
                    var files3 = files1.Except(files2).ToArray();
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
            if (_fromDB.GetNumFiles() != _toDB.GetNumFiles())
            {
                throw new InvalidProgramException($"Num files different {_fromDB.GetNumFiles()} != {_toDB.GetNumFiles()}");
            }

            if (_fromDB.GetNumTags() != _toDB.GetNumTags())
            {
                Log.Instance.Warn($"Num tags different {_fromDB.GetNumTags()} != {_toDB.GetNumTags()}");
            }

            if (_fromDB.GetNumPersons() + 1 != _toDB.GetNumPersons())
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
                throw new InvalidProgramException($"Num auto detected faces different {_fromDB.GetNumAutoDetectedFaces()} != {_toDB.GetNumAutoDetectedFaces()}");
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void CheckConvertedData(string file, Image originalData)
        {
            var convertedData = _toDB.GetMetaData(file);
            convertedData = Image.InvalidateIds(convertedData);
            if (!originalData.Equals(convertedData))
            {
                throw new InvalidProgramException($"Converting data of {file} failed\n\n{originalData}\n\n{convertedData}");
            }
        }

        private Image ConvertFile(string file)
        {
            var data = _fromDB.GetMetaData(file);
            data = Image.InvalidateIds(data);
            var dateModified = _fromDB.GetDateModified(file);

            _toDB.AddMetaData(data, dateModified);

            return data;
        }

        private IDB2 _fromDB;
        private IDB2 _toDB;
        private IList<string> _folders;

#endregion
    }
}