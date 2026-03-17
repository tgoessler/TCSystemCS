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

public sealed class FaceInfo(long _fileId, long _faceId, long _personId, FaceMode _faceMode, FaceQuality _faceQuality,
                             IReadOnlyList<FixedPoint64> _faceDescriptor) : IEquatable<FaceInfo>
{
#region Public

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as FaceInfo, EqualsImp);
    }

    public bool Equals(FaceInfo other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

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

    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    public static FaceInfo FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public static string ToJsonStringArray(IEnumerable<FaceInfo> faceInfos)
    {
        var array = new JArray(faceInfos.Select(fi => fi.ToJson()));
        return array.ToString(Formatting.None);
    }

    public long FileId => _fileId;
    public long FaceId => _faceId;
    public long PersonId => _personId == Constants.EmptyPersonId ? Constants.InvalidId : _personId;
    public FaceMode FaceMode => _faceMode;
    public FaceQuality FaceQuality => _faceQuality;
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