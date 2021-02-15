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
using System.Threading;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;
using TCSystem.Thread;

#endregion

namespace TCSystem.MetaDataDB
{
    public sealed class DB2 : IDB2
    {
#region Public

        public void EnableUnsafeMode()
        {
            using (_lock.Lock())
            {
                _instance.EnableUnsafeMode();
            }
        }

        public void EnableDefaultMode()
        {
            using (_lock.Lock())
            {
                _instance.EnableDefaultMode();
            }
        }

        public long GetNumFiles()
        {
            using (_lock.Lock())
            {
                return _files.GetNumFiles();
            }
        }

        public long GetNumTags()
        {
            using (_lock.Lock())
            {
                return _tags.GetNumTags(null);
            }
        }

        public long GetNumPersons()
        {
            using (_lock.Lock())
            {
                return _persons.GetNumPersons(null);
            }
        }

        public long GetNumLocations()
        {
            using (_lock.Lock())
            {
                return _locations.GetNumLocations();
            }
        }

        public long GetNumFaces()
        {
            using (_lock.Lock())
            {
                return _persons.GetNumFaces();
            }
        }

        public long GetNumAutoDetectedFaces()
        {
            using (_lock.Lock())
            {
                return _persons.GetNumAutoDetectedFaces();
            }
        }

        public IList<string> GetAllFilesLike(string filter = null)
        {
            using (_lock.Lock())
            {
                return _files.GetAllFilesLike(filter);
            }
        }

        public IList<string> GetAllTagsLike(string filter = null)
        {
            using (_lock.Lock())
            {
                return _tags.GetAllTagsLike(filter);
            }
        }

        public IList<Address> GetAllLocationsLike(string filter = null)
        {
            using (_lock.Lock())
            {
                return _locations.GetAllLocationsLike(filter);
            }
        }

        public IList<string> GetAllPersonNamesLike(string filter = null)
        {
            using (_lock.Lock())
            {
                return _persons.GetAllPersonNamesLike(filter);
            }
        }

        public IList<DateTimeOffset> GetAllYears()
        {
            using (_lock.Lock())
            {
                return _data.GetAllYears();
            }
        }

        public IList<(string FileName, ProcessingInfo ProcessingInfo)> GetAllProcessingInformation()
        {
            using (_lock.Lock())
            {
                return _files.GetAllProcessingInformation();
            }
        }

        public IDictionary<string, DateTimeOffset> GetAllFileAndModifiedDates()
        {
            using (_lock.Lock())
            {
                return _files.GetAllFileAndModifiedDates();
            }
        }

        public IList<FaceInfo> GetAllFaceInfos()
        {
            using (_lock.Lock())
            {
                return _persons.GetAllFaceInfos();
            }
        }

        public Image GetMetaData(string fileName)
        {
            using (_lock.Lock())
            {
                Image image = null;
                var fileId = _files.GetFileId(fileName, null);
                if (fileId != Constants.InvalidId)
                {
                    image = GetMetaData(fileId, null);
                }

                return image;
            }
        }

        public Location GetLocation(string fileName)
        {
            using (_lock.Lock())
            {
                Location location = null;
                var fileId = _files.GetFileId(fileName, null);
                if (fileId != Constants.InvalidId)
                {
                    location = _locations.GetLocation(fileId, null);
                }

                return location;
            }
        }

        public DateTimeOffset GetDateModified(string fileName)
        {
            using (_lock.Lock())
            {
                return _files.GetDateModified(fileName);
            }
        }

        public OrientationMode GetOrientation(string fileName)
        {
            using (_lock.Lock())
            {
                return _data.GetOrientation(fileName);
            }
        }

        public Image AddMetaData(Image data, DateTimeOffset dateModified)
        {
            Image oldData = null;
            using (_lock.Lock())
            {
                using (var transaction = _instance.BeginTransaction())
                {
                    var fileId = _files.GetFileId(data.FileName, transaction);
                    // to make sure that we only store the information once per file
                    if (fileId != Constants.InvalidId)
                    {
                        oldData = GetMetaData(fileId, transaction);
                        _files.RemoveFile(fileId, transaction);
                    }

                    fileId = _files.AddFile(data, dateModified, transaction);
                    _persons.AddPersonTags(fileId, data.PersonTags, transaction);
                    _tags.AddTags(fileId, data.Tags, transaction);
                    _locations.AddLocation(fileId, data.Location, transaction);
                    _data.AddMetaData(fileId, data, transaction);

                    // get really save data for calling changed callback with correct data
                    data = GetMetaData(fileId, transaction);

                    transaction.Commit();
                }
            }

            // call event handler new meta data was added
            if (oldData != null)
            {
                MetaDataChanged?.Invoke((data, oldData));
            }
            else
            {
                MetaDataAdded?.Invoke(data);
            }

            return data;
        }

        public void RemoveMetaData(string fileName)
        {
            Image data = null;
            using (_lock.Lock())
            {
                using (var transaction = _instance.BeginTransaction())
                {
                    var fileId = _files.GetFileId(fileName, transaction);
                    // to make sure that we only store the information once per file
                    if (fileId != Constants.InvalidId)
                    {
                        data = GetMetaData(fileId, transaction);
                        _files.RemoveFile(fileId, transaction);
                    }

                    transaction.Commit();
                }
            }

            if (data != null)
            {
                MetaDataRemoved?.Invoke(data);
            }
        }

        public void RemoveAllFilesOfFolder(string folder)
        {
            using (_lock.Lock())
            {
                _files.RemoveAllFilesOfFolder(folder);
            }

            MetaDataRemoved?.Invoke(null);
        }

        public Person GetPersonFromName(string name)
        {
            using (_lock.Lock())
            {
                return _persons.GetPersonFromName(name, null);
            }
        }

        public long GetPersonIdFromName(string name)
        {
            using (_lock.Lock())
            {
                return _persons.GetPersonId(name, null);
            }
        }

        public Person GetPersonFromId(long personId)
        {
            using (_lock.Lock())
            {
                return _persons.GetPersonFromId(personId, null);
            }
        }

        public FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId)
        {
            using (_lock.Lock())
            {
                return _persons.GetFileAndPersonTagFromFaceId(faceId);
            }
        }

        public long GetNumFilesOfTag(string tag)
        {
            using (_lock.Lock())
            {
                var num = _tags.GetNumFilesOfTag(tag);
                if (num == 0)
                {
                    using (var transaction = _instance.BeginTransaction())
                    {
                        _tags.RemoveTag(tag, transaction);
                        transaction.Commit();
                    }
                }

                return num;
            }
        }

        public long GetNumFilesOfYear(DateTimeOffset year)
        {
            using (_lock.Lock())
            {
                return _data.GetNumFilesOfYear(year);
            }
        }

        public long GetNumFilesOfPerson(string person)
        {
            using (_lock.Lock())
            {
                var num = _persons.GetNumFilesOfPerson(person);
                if (num == 0)
                {
                    using (var transaction = _instance.BeginTransaction())
                    {
                        _persons.RemovePerson(person, transaction);
                        transaction.Commit();
                    }
                }

                return num;
            }
        }

        public long GetNumFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
        {
            using (_lock.Lock())
            {
                var num = _locations.GetNumFilesOfAddress(address, useProvinceAlsoIfEmpty);
                if (num == 0)
                {
                    using (var transaction = _instance.BeginTransaction())
                    {
                        _locations.RemoveAddress(address, transaction);
                        transaction.Commit();
                    }
                }

                return num;
            }
        }

        public long GetNumFilesOfFolder(string folder)
        {
            using (_lock.Lock())
            {
                return _files.GetNumAllFilesLike(folder);
            }
        }

        public IList<string> GetFilesOfTag(string tag)
        {
            using (_lock.Lock())
            {
                return _tags.GetFilesOfTag(tag);
            }
        }

        public IList<string> GetFilesOfYear(DateTimeOffset year)
        {
            using (_lock.Lock())
            {
                return _data.GetFilesOfYear(year);
            }
        }

        public IList<string> GetFilesOfPerson(string person)
        {
            using (_lock.Lock())
            {
                return _persons.GetFilesOfPerson(person);
            }
        }

        public IList<string> GetFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
        {
            using (_lock.Lock())
            {
                return _locations.GetFilesOfAddress(address, useProvinceAlsoIfEmpty);
            }
        }

        public IList<string> GetFilesOfFolder(string folder)
        {
            using (_lock.Lock())
            {
                return _files.GetAllFilesLike(folder);
            }
        }

        public IList<string> SearchForFiles(string searchFilter)
        {
            using (_lock.Lock())
            {
                return _files.SearchForFiles(searchFilter);
            }
        }

        public IList<FileAndPersonTag> GetFileAndPersonTagsOfPerson(string name)
        {
            using (_lock.Lock())
            {
                return _persons.GetFileAndPersonTags(name);
            }
        }

        public event Action<Image> MetaDataAdded;
        public event Action<Image> MetaDataRemoved;
        public event Action<(Image NewData, Image OldData)> MetaDataChanged;

#endregion

#region Internal

        internal DB2(string fileName)
        {
            using (_lock.Lock())
            {
                _instance.Open(fileName);

                _files.Instance = _instance;
                _persons.Instance = _instance;
                _tags.Instance = _instance;
                _locations.Instance = _instance;
                _data.Instance = _instance;

                using (var transaction = _instance.BeginTransaction())
                {
                    // add an empty person first
                    if (_persons.GetNumPersons(transaction) == 0)
                    {
                        var id = _persons.AddPerson(new Person(Constants.InvalidId, "", "", "", ""), transaction);
                        if (id != Constants.EmptyPersonId)
                        {
                            Log.Instance.Error($"Empty person id not {Constants.EmptyPersonId}, id ={id}");
                        }
                    }

                    // add an empty address first
                    if (_locations.GetNumLocations(transaction) == 0)
                    {
                        var id = _locations.AddLocation(Location.NoLocation, transaction);
                        if (id != Constants.EmptyLocationId)
                        {
                            Log.Instance.Error($"Empty location id not {Constants.EmptyLocationId}, id ={id}");
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        internal void Close()
        {
            using (_lock.Lock())
            {
                _instance.Close();
            }
        }

#endregion

#region Private

        private Image GetMetaData(long fileId, SqliteTransaction transaction)
        {
            var personTags = _persons.GetPersonTags(fileId, transaction);
            var tags = _tags.GetTags(fileId, transaction);
            var location = _locations.GetLocation(fileId, transaction);
            var image = _data.GetMetaData(fileId, location, personTags, tags, transaction);
            return image;
        }


        private readonly DB2Instance _instance = new DB2Instance();
        private readonly DB2Files _files = new DB2Files();
        private readonly DB2Persons _persons = new DB2Persons();
        private readonly DB2Tags _tags = new DB2Tags();
        private readonly DB2Locations _locations = new DB2Locations();
        private readonly DB2Data _data = new DB2Data();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

#endregion
    }
}