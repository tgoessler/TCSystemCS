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

namespace TCSystem.MetaDataDB
{
    internal class DB2Constants
    {
#region Protected

        protected const string Version = "1.1";
        protected const string Version10 = "1.0";

        protected const string TableKeyValues = "KeyValues";
        protected const string TableFiles = "Files";
        protected const string TablePersons = "Persons";
        protected const string TableTags = "Tags";
        protected const string TableLocations = "Locations";
        protected const string TableFileData = "FileData";
        protected const string TableFileFaces = "FileFaces";
        protected const string TableFileTags = "FileTags";
        protected const string TableFileLocations = "FileLocations";

        protected const string IdKey = "Key";
        protected const string IdValue = "Value";

        protected const string IdFileId = "FileId";
        protected const string IdTagId = "TagId";
        protected const string IdPersonId = "PersonId";
        protected const string IdLocationId = "LocationId";
        protected const string IdFaceId = "FaceId";

        protected const string IdFileName = "FileName";
        protected const string IdDateModified = "DateModified";
        protected const string IdProcessingInfo = "ProcessingInfo";

        protected const string IdTag = "Tag";

        protected const string IdCountry = "Country";
        protected const string IdProvince = "Province";
        protected const string IdCity = "City";
        protected const string IdStreet = "Street";
        protected const string IdLatitude = "Latitude";
        protected const string IdLongitude = "Longitude";
        protected const string IdAltitude = "Altitude";

        protected const string IdName = "Name";
        protected const string IdEmailDigest = "EmailDigest";
        protected const string IdLiveId = "LiveId";
        protected const string IdSourceId = "SourceId";

        protected const string IdRectangleX = "RectangleX";
        protected const string IdRectangleY = "RectangleY";
        protected const string IdRectangleW = "RectangleW";
        protected const string IdRectangleH = "RectangleH";
        protected const string IdFaceMode = "FaceMode";
        protected const string IdFaceDescriptor = "FaceDescriptor";

        protected const string IdWidth = "Width";
        protected const string IdHeight = "Height";
        protected const string IdDateTaken = "DateTaken";
        protected const string IdOrientation = "Orientation";

        protected const char EscapeCharacter = '?';
        protected const string LikeEscape = "ESCAPE '?'";

        protected const string FilterDate = "Date";
        protected const string FilterFaceId = "FaceId";
        protected const string FilterFile = "File";
        protected const string FilterLocation = "Location";
        protected const string FilterName = "Name";
        protected const string FilterTag = "Tag";
        protected const string FilterNumPersons = "NumPersons";

        protected static readonly string[] _filters =
        {
            FilterDate,
            FilterFaceId,
            FilterFile,
            FilterLocation,
            FilterName,
            FilterTag,
            FilterNumPersons
        };

#endregion
    }
}