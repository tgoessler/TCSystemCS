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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
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

namespace TCSystem.MetaData
{
    public sealed class FaceInfo : IEquatable<FaceInfo>
    {
#region Public

        public FaceInfo(long fileId, long faceId, long personId, FaceMode faceMode, IEnumerable<FixedPoint64> faceDescriptor)
        {
            FileId = fileId;
            FaceId = faceId;
            PersonId = personId == Constants.EmptyPersonId ? Constants.InvalidId : personId;
            FaceMode = faceMode;
            FaceDescriptor = (faceDescriptor ?? Array.Empty<FixedPoint64>()).ToArray();
        }

        public override bool Equals(object obj)
        {
            return EqualsUtil.Equals(this, obj as FaceInfo, EqualsImp);
        }

        public bool Equals(FaceInfo other)
        {
            return EqualsUtil.Equals(this, other, EqualsImp);
        }

        private bool EqualsImp(FaceInfo other)
        {
            return FileId == other.FileId &&
                   FaceId == other.FaceId &&
                   PersonId == other.PersonId &&
                   FaceMode == other.FaceMode &&
                   FaceDescriptor.SequenceEqual(other.FaceDescriptor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FileId.GetHashCode();
                hashCode *= 397 ^ FaceId.GetHashCode();
                hashCode *= 397 ^ PersonId.GetHashCode();
                hashCode *= 397 ^ FaceMode.GetHashCode();
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

        public static string ToJsonStringArray(IList<FaceInfo> faceInfos)
        {
            var array = new JArray(faceInfos.Select(fi => fi.ToJson()));
            return array.ToString(Formatting.None);
        }

        public long FileId { get; }
        public long FaceId { get; }
        public long PersonId { get; }
        public FaceMode FaceMode { get; }
        public IReadOnlyCollection<FixedPoint64> FaceDescriptor { get; }

#endregion

#region Private

        private static FaceInfo FromJson(JObject jsonObject)
        {
            var fdJson = (JArray) jsonObject["face_descriptor"];
            return new FaceInfo(
                (long) jsonObject["file_id"],
                (long) jsonObject["face_id"],
                (long) jsonObject["person_id"],
                (FaceMode) (long) jsonObject["face_mode"],
                fdJson != null && fdJson.Count > 0 ? fdJson.Select(v => new FixedPoint64((double) v)) : null);
        }

        private JObject ToJson()
        {
            var obj = new JObject
            {
                ["file_id"] = FileId,
                ["face_id"] = FaceId,
                ["person_id"] = PersonId,
                ["face_mode"] = (long) FaceMode,
                ["face_descriptor"] = new JArray(FaceDescriptor.Select(v => v.Value))
            };

            return obj;
        }

#endregion
    }
}