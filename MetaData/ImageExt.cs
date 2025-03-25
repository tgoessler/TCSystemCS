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

using System.Collections.Generic;
using System.Linq;

#endregion

namespace TCSystem.MetaData;

public static class ImageExt
{
#region Public

    public static Image InvalidateId(this Image image)
    {
        PersonTag[] personTags = image.PersonTags.Select(InvalidateId).ToArray();
        return new(Constants.InvalidId, image.FileName, image.ProcessingInfos,
            image.Width, image.Height, image.Orientation,
            image.DateTaken, image.Title, image.Location,
            personTags, image.Tags);
    }

    public static Person InvalidateId(this Person person)
    {
        return new(Constants.InvalidId, person.Name, person.EmailDigest, person.LiveId, person.SourceId);
    }

    public static Face InvalidateId(this Face face)
    {
        return new(Constants.InvalidId, face.Rectangle, face.FaceMode, face.Visible, face.FaceDescriptor);
    }

    public static PersonTag InvalidateId(this PersonTag personTag)
    {
        return new(personTag.Person.InvalidateId(), personTag.Face.InvalidateId());
    }

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