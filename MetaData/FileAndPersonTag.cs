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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData;

public sealed class FileAndPersonTag : IEquatable<FileAndPersonTag>
{
#region Public

    public FileAndPersonTag(string fileName, PersonTag personTag)
    {
        FileName = fileName ?? string.Empty;
        PersonTag = personTag;
    }

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as FileAndPersonTag, EqualsImp);
    }

    public bool Equals(FileAndPersonTag other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = FileName.GetHashCode();
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

    public static string ToJsonStringArray(IEnumerable<FileAndPersonTag> fileAndPersonTags)
    {
        var array = new JArray(fileAndPersonTags.Select(fpt => fpt.ToJson()));
        return array.ToString(Formatting.None);
    }

    public static IEnumerable<FileAndPersonTag> FromJsonStringArray(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return Array.Empty<FileAndPersonTag>();
        }

        JArray array = JArray.Parse(jsonString);
        return array.Select(v => FromJson((JObject)v));
    }

    public string FileName { get; }
    public PersonTag PersonTag { get; }

#endregion

#region Internal

    internal static FileAndPersonTag FromJson(JObject jsonObject)
    {
        return new(
            (string)jsonObject["file_name"],
            PersonTag.FromJson((JObject)jsonObject["person_tag"])
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

    private bool EqualsImp(FileAndPersonTag other)
    {
        return string.Equals(FileName, other.FileName, StringComparison.InvariantCulture) &&
               Equals(PersonTag, other.PersonTag);
    }

#endregion
}