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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public sealed class FileAndPersonTag
    {
#region Public

        public FileAndPersonTag(string fileName, PersonTag personTag)
        {
            FileName = fileName;
            PersonTag = personTag;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is FileAndPersonTag fileAndPersonTag && Equals(fileAndPersonTag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FileName.GetHashCode();
                hashCode = (hashCode * 397) ^ PersonTag.GetHashCode();
                return hashCode;
            }
        }

        public string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

        public override string ToString()
        {
            return ToJson().ToString(Formatting.Indented);
        }

        public static FileAndPersonTag FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public static string ToJsonStringArray(IList<FileAndPersonTag> fileAndPersonTags)
        {
            var array = new JArray(fileAndPersonTags.Select(fpt => fpt.ToJson()));
            return array.ToString(Formatting.None);
        }

        public static IEnumerable<FileAndPersonTag> FromJsonStringArray(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            var array = JArray.Parse(jsonString);
            return array.Select(v => FromJson((JObject) v));
        }

        public string FileName { get; }
        public PersonTag PersonTag { get; }

#endregion

#region Internal

        internal static FileAndPersonTag FromJson(JObject jsonObject)
        {
            return new FileAndPersonTag(
                (string) jsonObject["file_name"],
                PersonTag.FromJson((JObject) jsonObject["person_tag"])
            );
        }

        internal JObject ToJson()
        {
            var obj = new JObject
            {
                ["file_name"] = FileName,
                ["person_tag"] = PersonTag?.ToJson()
            };

            return obj;
        }

#endregion

#region Private

        private bool Equals(FileAndPersonTag other)
        {
            return string.Equals(FileName, other.FileName, StringComparison.InvariantCulture) &&
                   Equals(PersonTag, other.PersonTag);
        }

#endregion
    }
}