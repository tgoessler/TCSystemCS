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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

internal sealed class DB2 : IDB2
{
#region Public

    public void EnableUnsafeMode()
    {
        _instance.EnableUnsafeMode();
    }

    public void EnableDefaultMode()
    {
        _instance.EnableDefaultMode();
    }

    public long GetNumFiles()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetNumFiles();
        }
    }

    public long GetNumTags()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Tags.GetNumTags(null);
        }
    }

    public long GetNumPersons()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetNumPersons(null);
        }
    }

    public long GetNumLocations()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Locations.GetNumLocations();
        }
    }

    public long GetNumFaces()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetNumFaces();
        }
    }

    public long GetNumAutoDetectedFaces()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetNumAutoDetectedFaces();
        }
    }

    public IList<string> GetAllFilesLike(string filter = null)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetAllFilesLike(filter);
        }
    }

    public IList<string> GetAllTagsLike(string filter = null)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Tags.GetAllTagsLike(filter);
        }
    }

    public IList<Address> GetAllLocationsLike(string filter = null)
    public IList<Address> GetAllAddressesLike(string filter = null)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Locations.GetAllAddressesLike(filter);
        }
    }

    public IList<string> GetAllPersonNamesLike(string filter = null)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetAllPersonNamesLike(filter);
        }
    }

    public IList<DateTimeOffset> GetAllYears()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Data.GetAllYears();
        }
    }

    public IList<Location> GetAllLocations()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Locations.GetAllLocations();
        }
    }

    public IList<(string FileName, ProcessingInfos ProcessingInfo)> GetAllProcessingInformation()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetAllProcessingInformation();
        }
    }

    public IDictionary<string, DateTimeOffset> GetAllFileAndModifiedDates()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetAllFileAndModifiedDates();
        }
    }

    public IList<FaceInfo> GetAllFaceInfos(bool visibleOnly)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetAllFaceInfos(visibleOnly);
        }
    }

    public Image GetMetaData(string fileName)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            Image image = null;
            long fileId = acquiredInstance.Instance.Files.GetFileId(fileName, null);
            if (fileId != Constants.InvalidId)
            {
                image = GetMetaData(fileId, acquiredInstance.Instance, null);
            }

            return image;
        }
    }

    public Location GetLocation(string fileName)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            Location location = null;
            long fileId = acquiredInstance.Instance.Files.GetFileId(fileName, null);
            if (fileId != Constants.InvalidId)
            {
                location = acquiredInstance.Instance.Locations.GetLocation(fileId, null);
            }

            return location;
        }
    }

    public DateTimeOffset GetDateModified(string fileName)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetDateModified(fileName);
        }
    }

    public OrientationMode GetOrientation(string fileName)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Data.GetOrientation(fileName);
        }
    }

    public Image AddMetaData(Image newMetaData, DateTimeOffset dateModified)
    {
        Image oldData = null;
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
            {
                DB2Files files = acquiredInstance.Instance.Files;
                long fileId = files.GetFileId(newMetaData.FileName, transaction);
                Image removedMetaData = null;

                // no id available should be added as new file
                if (newMetaData.Id == Constants.InvalidId)
                {
                    if (fileId != Constants.InvalidId)
                    {
                        removedMetaData = GetMetaData(fileId, acquiredInstance.Instance, transaction);
                        files.RemoveFile(fileId, transaction);
                    }
                }
                else
                {
                    if (fileId != newMetaData.Id)
                    {
                        Log.Instance.Fatal($"File Id is not matching {fileId} != {newMetaData.Id}");
                        throw new ArgumentOutOfRangeException(nameof(newMetaData));
                    }

                    oldData = GetMetaData(fileId, acquiredInstance.Instance, transaction);
                }

                fileId = files.SetFile(newMetaData, oldData, dateModified, transaction);
                acquiredInstance.Instance.Persons.SetPersonTags(fileId, newMetaData.PersonTags,
                    oldData?.PersonTags ?? Array.Empty<PersonTag>(), transaction);
                acquiredInstance.Instance.Tags.SetTags(fileId, newMetaData.Tags, oldData?.Tags ?? Array.Empty<string>(), transaction);
                acquiredInstance.Instance.Locations.SetLocation(fileId, newMetaData.Location, oldData?.Location, transaction);
                acquiredInstance.Instance.Data.SetMetaData(fileId, newMetaData, oldData, transaction);

                // get really save newMetaData for calling changed callback with correct newMetaData
                newMetaData = GetMetaData(fileId, acquiredInstance.Instance, transaction);

                transaction.Commit();

                if (removedMetaData != null)
                {
                    oldData = removedMetaData;
                }
            }
        }

        // call event handler new meta newMetaData was added
        if (oldData != null)
        {
            MetaDataChanged?.Invoke((newMetaData, oldData));
        }
        else
        {
            MetaDataAdded?.Invoke(newMetaData);
        }

        return newMetaData;
    }

    public void RemoveMetaData(string fileName)
    {
        Image data = null;
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
            {
                DB2Files files = acquiredInstance.Instance.Files;
                long fileId = files.GetFileId(fileName, transaction);
                // to make sure that we only store the information once per file
                if (fileId != Constants.InvalidId)
                {
                    data = GetMetaData(fileId, acquiredInstance.Instance, transaction);
                    files.RemoveFile(fileId, transaction);
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
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            acquiredInstance.Instance.Files.RemoveAllFilesOfFolder(folder);
        }

        MetaDataRemoved?.Invoke(null);
    }

    public Person GetPersonFromName(string name)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetPersonFromName(name, null);
        }
    }

    public long GetPersonIdFromName(string name)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetPersonId(name, null);
        }
    }

    public Person GetPersonFromId(long personId)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetPersonFromId(personId, null);
        }
    }

    public FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId, bool visibleOnly)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetFileAndPersonTagFromFaceId(faceId, visibleOnly);
        }
    }

    public long GetNumFilesOfTag(string tag)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            DB2Tags tags = acquiredInstance.Instance.Tags;
            long num = tags.GetNumFilesOfTag(tag);
            if (num == 0 && !acquiredInstance.Instance.ReadOnly)
            {
                using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
                {
                    tags.RemoveTag(tag, transaction);
                    transaction.Commit();
                }
            }

            return num;
        }
    }

    public long GetNumFilesOfYear(DateTimeOffset year)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Data.GetNumFilesOfYear(year);
        }
    }

    public long GetNumFilesOfPerson(string person)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            DB2Persons persons = acquiredInstance.Instance.Persons;
            long num = persons.GetNumFilesOfPerson(person);
            if (num == 0 && !acquiredInstance.Instance.ReadOnly)
            {
                using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
                {
                    persons.RemovePerson(person, transaction);
                    transaction.Commit();
                }
            }

            return num;
        }
    }

    public long GetNumFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            DB2Locations locations = acquiredInstance.Instance.Locations;
            long num = locations.GetNumFilesOfAddress(address, useProvinceAlsoIfEmpty);
            if (num == 0 && !acquiredInstance.Instance.ReadOnly)
            {
                using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
                {
                    locations.RemoveAddress(address, transaction);
                    transaction.Commit();
                }
            }

            return num;
        }
    }

    public long GetNumFilesOfFolder(string folder)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetNumAllFilesLike(folder);
        }
    }

    public IList<string> GetFilesOfTag(string tag)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Tags.GetFilesOfTag(tag);
        }
    }

    public IList<string> GetFilesOfYear(DateTimeOffset year)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Data.GetFilesOfYear(year);
        }
    }

    public IList<string> GetFilesOfPerson(string person)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetFilesOfPerson(person);
        }
    }

    public IList<string> GetFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Locations.GetFilesOfAddress(address, useProvinceAlsoIfEmpty);
        }
    }

    public IList<string> GetFilesOfFolder(string folder)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.GetAllFilesLike(folder);
        }
    }

    public IList<string> SearchForFiles(string searchFilter)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Files.SearchForFiles(searchFilter);
        }
    }

    public IList<FileAndPersonTag> GetFileAndPersonTagsOfPerson(string name, bool visibleOnly)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.Persons.GetFileAndPersonTags(name, visibleOnly);
        }
    }

    public void AddNotThisPerson(Face face, Person person)
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            using (SqliteTransaction transaction = acquiredInstance.Instance.BeginTransaction())
            {
                acquiredInstance.Instance.NotThisPerson.AddNotThisPerson(face.Id, person.Id, transaction);
                transaction.Commit();
            }
        }
    }

    public IDictionary<long, IList<long>> GetNotThisPersonInformation()
    {
        using (var acquiredInstance = new InstanceAcquire(this))
        {
            return acquiredInstance.Instance.NotThisPerson.GetNotThisPersonInformation();
        }
    }

    public event Action<Image> MetaDataAdded;
    public event Action<Image> MetaDataRemoved;
    public event Action<(Image NewData, Image OldData)> MetaDataChanged;

    public string Version => _instance.Version;

#endregion

#region Internal

    internal DB2(string fileName, bool readOnly)
    {
        _instance = new(fileName, readOnly);

        if (!readOnly)
        {
            // add an empty person first
            if (_instance.Persons.GetNumPersons(null) == 0)
            {
                using (SqliteTransaction transaction = _instance.BeginTransaction())
                {
                    long id = _instance.Persons.AddPerson(new(Constants.InvalidId, "", "", "", ""), transaction);
                    if (id != Constants.EmptyPersonId)
                    {
                        Log.Instance.Error($"Empty person id not {Constants.EmptyPersonId}, id ={id}");
                    }

                    transaction.Commit();
                }
            }

            // add an empty address first
            if (_instance.Locations.GetNumLocations() == 0)
            {
                using (SqliteTransaction transaction = _instance.BeginTransaction())
                {
                    long id = _instance.Locations.AddLocation(Location.NoLocation, transaction, true);
                    if (id != Constants.EmptyLocationId)
                    {
                        Log.Instance.Error($"Empty location id not {Constants.EmptyLocationId}, id ={id}");
                    }

                    transaction.Commit();
                }
            }
        }

        _instance.Connection.Close();
    }

    internal void Close()
    {
        while (_instances.TryPop(out DB2Instance instance))
        {
            instance.Close();
        }

        _instance.Close();
    }

#endregion

#region Private

    private static Image GetMetaData(long fileId, DB2Instance instance, SqliteTransaction transaction)
    {
        IReadOnlyList<PersonTag> personTags = instance.Persons.GetPersonTags(fileId, false, transaction);
        IReadOnlyList<string> tags = instance.Tags.GetTags(fileId, transaction);
        Location location = instance.Locations.GetLocation(fileId, transaction);
        Image image = instance.Data.GetMetaData(fileId, location, personTags, tags, transaction);
        return image;
    }

    private DB2Instance AcquireInstance()
    {
        if (!_instances.TryPop(out DB2Instance instance))
        {
            _totalCreatedConnections++;
            Log.Instance.Info($"Creating new connection _totalCreatedConnections={_totalCreatedConnections}");

            instance = _instance.Clone();
        }

        if (instance.Connection.State != ConnectionState.Open)
        {
            instance.Connection.Open();
        }

        return instance;
    }

    private void ReleaseInstance(DB2Instance instance)
    {
        if (!instance.ReadOnly && !instance.UnsafeModeEnabled)
        {
            instance.Connection.Close();
        }

        _instances.Push(instance);
    }

    private readonly struct InstanceAcquire(DB2 db2) : IDisposable
    {
#region Public

        public void Dispose()
        {
            _db2.ReleaseInstance(Instance);
        }

        public DB2Instance Instance { get; } = db2.AcquireInstance();

#endregion

#region Private

        private readonly DB2 _db2 = db2;

#endregion
    }

    private readonly DB2Instance _instance;
    private readonly ConcurrentStack<DB2Instance> _instances = new();
    private int _totalCreatedConnections;

#endregion
}