﻿// *******************************************************************************
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
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB
{
    public interface IDB2
    {
#region Public

        long GetNumFiles();
        long GetNumTags();
        long GetNumPersons();
        long GetNumLocations();
        long GetNumFaces();
        long GetNumAutoDetectedFaces();


        IList<string> GetAllFilesLike(string filter = null);
        IList<string> GetAllTagsLike(string filter = null);
        IList<Address> GetAllLocationsLike(string filter = null);
        IList<string> GetAllPersonNamesLike(string filter = null);
        IList<DateTimeOffset> GetAllYears();
        IList<(string FileName, ProcessingInfos ProcessingInfo)> GetAllProcessingInformation();
        IDictionary<string, DateTimeOffset> GetAllFileAndModifiedDates();
        IList<FaceInfo> GetAllFaceInfos();


        Image GetMetaData(string fileName);
        Location GetLocation(string fileName);
        DateTimeOffset GetDateModified(string fileName);
        OrientationMode GetOrientation(string fileName);
        Person GetPersonFromName(string name);
        long GetPersonIdFromName(string name);
        Person GetPersonFromId(long personId);
        FileAndPersonTag GetFileAndPersonTagFromFaceId(long faceId);


        Image AddMetaData(Image data, DateTimeOffset dateModified);
        void RemoveMetaData(string fileName);
        void RemoveAllFilesOfFolder(string folder);

        long GetNumFilesOfTag(string tag);
        long GetNumFilesOfYear(DateTimeOffset year);
        long GetNumFilesOfPerson(string person);
        long GetNumFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty);
        long GetNumFilesOfFolder(string folder);

        IList<string> GetFilesOfTag(string tag);
        IList<string> GetFilesOfYear(DateTimeOffset year);
        IList<string> GetFilesOfPerson(string person);
        IList<string> GetFilesOfAddress(Address address, bool useProvinceAlsoIfEmpty);
        IList<string> GetFilesOfFolder(string folder);

        IList<string> SearchForFiles(string searchFilter);

        IList<FileAndPersonTag> GetFileAndPersonTagsOfPerson(string name);

        void EnableUnsafeMode();
        void EnableDefaultMode();
        event Action<Image> MetaDataAdded;
        event Action<Image> MetaDataRemoved;
        event Action<(Image NewData, Image OldData)> MetaDataChanged;

#endregion
    }
}