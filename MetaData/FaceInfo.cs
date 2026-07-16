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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData;

/// <summary>Provides flattened face, file, and person information returned by metadata database queries.</summary>
/// <param name="_fileId">The file identifier.</param>
/// <param name="_faceId">The face identifier.</param>
/// <param name="_personId">The associated person identifier.</param>
/// <param name="_faceMode">The detector that found the face.</param>
/// <param name="_faceQuality">The face quality classification.</param>
/// <param name="_faceDescriptor">The optional face descriptor.</param>
public sealed class FaceInfo(long _fileId, long _faceId, long _personId, FaceMode _faceMode,
                             FaceQuality _faceQuality,
                             IReadOnlyList<FixedPoint64> _faceDescriptor) : IEquatable<FaceInfo>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as FaceInfo, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(FaceInfo other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = FileId.GetHashCode();
            hashCode *= 397 ^ FaceId.GetHashCode();
            hashCode *= 397 ^ PersonId.GetHashCode();
            hashCode *= 397 ^ FaceMode.GetHashCode();
            hashCode *= 397 ^ FaceQuality.GetHashCode();
            return FaceDescriptor.Aggregate(hashCode, (current, fixedPoint64) => (current * 397) ^ fixedPoint64.GetHashCode());
        }
    }

    /// <summary>Serializes the information to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Serializes the information to compact JSON.</summary>
    /// <returns>The serialized value.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Deserializes face information from JSON.</summary>
    /// <param name="jsonString">The JSON to deserialize.</param>
    /// <returns>The information, or <see langword="null" /> for null or empty input.</returns>
    public static FaceInfo FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Serializes a sequence of face information to a compact JSON array.</summary>
    /// <param name="faceInfos">The values to serialize.</param>
    /// <returns>The serialized JSON array.</returns>
    public static string ToJsonStringArray(IEnumerable<FaceInfo> faceInfos)
    {
        var array = new JArray(faceInfos.Select(fi => fi.ToJson()));
        return array.ToString(Formatting.None);
    }

    /// <summary>Gets the file identifier.</summary>
    public long FileId => _fileId;

    /// <summary>Gets the face identifier.</summary>
    public long FaceId => _faceId;

    /// <summary>
    ///     Gets the person identifier, mapping the reserved empty-person identifier to <see cref="Constants.InvalidId" />
    ///     .
    /// </summary>
    public long PersonId => _personId == Constants.EmptyPersonId ? Constants.InvalidId : _personId;

    /// <summary>Gets the detector that found the face.</summary>
    public FaceMode FaceMode => _faceMode;

    /// <summary>Gets the face quality classification.</summary>
    public FaceQuality FaceQuality => _faceQuality;

    /// <summary>Gets the face descriptor, or an empty list when none is available.</summary>
    public IReadOnlyList<FixedPoint64> FaceDescriptor => _faceDescriptor ?? Array.Empty<FixedPoint64>();

#endregion

#region Private

    private bool EqualsImp(FaceInfo other)
    {
        return FileId == other.FileId &&
               FaceId == other.FaceId &&
               PersonId == other.PersonId &&
               FaceMode == other.FaceMode &&
               FaceQuality == other.FaceQuality &&
               FaceDescriptor.SequenceEqual(other.FaceDescriptor);
    }

    private static FaceInfo FromJson(JObject jsonObject)
    {
        var fdJson = (JArray)jsonObject["face_descriptor"];
        return new(
            (long)jsonObject["file_id"],
            (long)jsonObject["face_id"],
            (long)jsonObject["person_id"],
            (FaceMode)(long)jsonObject["face_mode"],
            (FaceQuality)(long)jsonObject["face_quality"],
            fdJson is { Count: > 0 } ? fdJson.Select(v => new FixedPoint64((double)v)).ToArray() : null);
    }

    private JObject ToJson()
    {
        var obj = new JObject
        {
            ["file_id"] = FileId,
            ["face_id"] = FaceId,
            ["person_id"] = PersonId,
            ["face_mode"] = (long)FaceMode,
            ["face_quality"] = (long)FaceQuality,
            ["face_descriptor"] = new JArray(FaceDescriptor.Select(v => v.Value))
        };

        return obj;
    }

#endregion
}