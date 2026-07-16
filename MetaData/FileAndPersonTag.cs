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

/// <summary>Associates an image file name with a person tag.</summary>
/// <param name="_fileName">The image file name.</param>
/// <param name="_personTag">The associated person tag.</param>
public sealed class FileAndPersonTag(string _fileName, PersonTag _personTag) : IEquatable<FileAndPersonTag>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as FileAndPersonTag, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(FileAndPersonTag other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = FileName.GetHashCode();
            hashCode = (hashCode * 397) ^ PersonTag.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the association to compact JSON.</summary>
    /// <returns>The serialized association.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the association to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes an association from JSON.</summary>
    /// <param name="jsonString">The JSON to deserialize.</param>
    /// <returns>The association, or <see langword="null" /> for null or empty input.</returns>
    public static FileAndPersonTag FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Serializes associations to a compact JSON array.</summary>
    /// <param name="fileAndPersonTags">The associations to serialize.</param>
    /// <returns>The serialized array.</returns>
    public static string ToJsonStringArray(IEnumerable<FileAndPersonTag> fileAndPersonTags)
    {
        var array = new JArray(fileAndPersonTags.Select(fpt => fpt.ToJson()));
        return array.ToString(Formatting.None);
    }

    /// <summary>Deserializes associations from a JSON array.</summary>
    /// <param name="jsonString">The JSON array to deserialize.</param>
    /// <returns>The associations, or an empty sequence for null or empty input.</returns>
    public static IEnumerable<FileAndPersonTag> FromJsonStringArray(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return Array.Empty<FileAndPersonTag>();
        }

        JArray array = JArray.Parse(jsonString);
        return array.Select(v => FromJson((JObject)v));
    }

    /// <summary>Gets the image file name, or an empty string when undefined.</summary>
    public string FileName => _fileName ?? string.Empty;

    /// <summary>Gets the associated person tag.</summary>
    public PersonTag PersonTag => _personTag;

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