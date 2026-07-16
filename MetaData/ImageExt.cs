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

using System.Collections.Generic;
using System.Linq;

#endregion

namespace TCSystem.MetaData;

/// <summary>Provides identifier-related operations for immutable metadata models.</summary>
public static class ImageExt
{
#region Public

    /// <summary>Returns an image whose file, person, and face identifiers are invalidated.</summary>
    public static Image InvalidateId(this Image image)
    {
        PersonTag[] personTags = image.PersonTags.Select(InvalidateId).ToArray();
        return new(Constants.InvalidId, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            personTags, image.Tags);
    }

    /// <summary>Returns a person whose identifier is invalidated.</summary>
    public static Person InvalidateId(this Person person)
    {
        return new(Constants.InvalidId, person.Name, person.EmailDigest, person.LiveId, person.SourceId);
    }

    /// <summary>Returns a face whose identifier is invalidated.</summary>
    public static Face InvalidateId(this Face face)
    {
        return new(Constants.InvalidId, face.Rectangle, face.FaceMode, face.FaceQuality, face.Visible, face.FaceDescriptor);
    }

    /// <summary>Returns a person tag whose person and face identifiers are invalidated.</summary>
    public static PersonTag InvalidateId(this PersonTag personTag)
    {
        return new(personTag.Person.InvalidateId(), personTag.Face.InvalidateId());
    }

    /// <summary>Finds a face by identifier in a sequence of person tags.</summary>
    /// <returns>The matching face, or <see langword="null" /> when no match exists or the identifier is invalid.</returns>
    public static Face GetFace(this IEnumerable<PersonTag> personTags, long faceId)
    {
        if (personTags != null && faceId != Constants.InvalidId)
        {
            return personTags.FirstOrDefault(p => p.Face.Id == faceId)?.Face;
        }

        return null;
    }

#endregion
}