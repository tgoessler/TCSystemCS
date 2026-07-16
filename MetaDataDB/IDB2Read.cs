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
//  see https://github.com/tgoessler/TCSystemCS for details.
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
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB;

/// <summary>Defines read and query operations for a metadata database.</summary>
public interface IDB2Read
{
#region Public

    /// <summary>Gets the number of stored files.</summary>
    long GetNumFiles();

    /// <summary>Gets the number of distinct tags.</summary>
    long GetNumTags();

    /// <summary>Gets the number of stored people.</summary>
    long GetNumPersons();

    /// <summary>Gets the number of stored locations.</summary>
    long GetNumLocations();

    /// <summary>Gets the number of stored faces.</summary>
    long GetNumFaces();

    /// <summary>Gets the number of automatically detected faces.</summary>
    long GetNumAutoDetectedFaces();

    /// <summary>Gets file names matching an optional SQL-like filter.</summary>
    IList<string> GetAllFilesLike(string filter = null);

    /// <summary>Gets tags matching an optional SQL-like filter.</summary>
    IList<string> GetAllTagsLike(string filter = null);

    /// <summary>Gets matching tags ordered by the newest associated file.</summary>
    IList<string> GetAllTagsLikeOrderByNewestFile(string filter = null);

    /// <summary>Gets addresses matching an optional SQL-like filter.</summary>
    IList<Address> GetAllAddressesLike(string filter = null);

    /// <summary>Gets person names matching an optional SQL-like filter.</summary>
    IList<string> GetAllPersonNamesLike(string filter = null);

    /// <summary>Gets all stored locations.</summary>
    IList<Location> GetAllLocations();

    /// <summary>Gets the distinct years represented by image dates.</summary>
    IList<DateTimeOffset> GetAllYears();

    /// <summary>Gets processing information for every stored file.</summary>
    IList<(string FileName, ProcessingInfos ProcessingInfo)> GetAllProcessingInformation();

    /// <summary>Gets every stored file and its modification date.</summary>
    IDictionary<string, DateTimeOffset> GetAllFileAndModifiedDates();

    /// <summary>Gets flattened face information, optionally restricted to visible faces.</summary>
    IList<FaceInfo> GetAllFaceInfos(bool visibleOnly);

    /// <summary>Gets the metadata for a file, or <see langword="null" /> when it is absent.</summary>
    Image GetMetaData(string fileName);

    /// <summary>Gets the location associated with a file.</summary>
    Location GetLocation(string fileName);

    /// <summary>Gets the stored modification date for a file.</summary>
    DateTimeOffset GetDateModified(string fileName);

    /// <summary>Gets the stored orientation for a file.</summary>
    OrientationMode GetOrientation(string fileName);

    /// <summary>Gets a person by name.</summary>
    Person GetPersonFromName(string name);

    /// <summary>Gets a person's identifier by name.</summary>
    long GetPersonIdFromName(string name);

    /// <summary>Gets a person by identifier.</summary>
    Person GetPersonFromId(long personId);

    /// <summary>Gets the file and person tag associated with a face identifier.</summary>
    FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId, bool visibleOnly);

    /// <summary>Gets the number of files associated with a tag.</summary>
    long GetNumFilesOfTag(string tag);

    /// <summary>Gets the number of files taken in a year.</summary>
    long GetNumFilesOfYear(DateTimeOffset year);

    /// <summary>Gets the number of files associated with a person.</summary>
    long GetNumFilesOfPerson(string person);

    /// <summary>Gets the number of files matching an address.</summary>
    long GetNumFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty);

    /// <summary>Gets the number of files directly contained in a folder.</summary>
    long GetNumFilesOfFolder(string folder);

    /// <summary>Gets files associated with a tag.</summary>
    IList<string> GetFilesOfTag(string tag);

    /// <summary>Gets files taken in a year.</summary>
    IList<string> GetFilesOfYear(DateTimeOffset year);

    /// <summary>Gets files associated with a person.</summary>
    IList<string> GetFilesOfPerson(string person);

    /// <summary>Gets files matching an address.</summary>
    IList<string> GetFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty);

    /// <summary>Gets files directly contained in a folder.</summary>
    IList<string> GetFilesOfFolder(string folder);

    /// <summary>Searches file metadata using the supplied filter text.</summary>
    IList<string> SearchForFiles(string searchFilter);

    /// <summary>Gets file and person-tag associations for a person.</summary>
    IList<FileAndPersonTag> GetFileAndPersonTagsOfPerson(string name, bool visibleOnly);

    /// <summary>Gets, for each face identifier, the person identifiers rejected as matches.</summary>
    /// <returns>A dictionary from face identifiers to rejected person identifiers.</returns>
    IDictionary<long, IList<long>> GetNotThisPersonInformation();

    /// <summary>Enables faster SQLite settings that reduce durability guarantees.</summary>
    void EnableUnsafeMode();

    /// <summary>Restores the default durable SQLite settings.</summary>
    void EnableDefaultMode();

    /// <summary>Gets the metadata database schema version.</summary>
    string Version { get; }

#endregion
}