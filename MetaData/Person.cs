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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData;

/// <summary>Represents a person associated with image metadata.</summary>
/// <param name="_id">The persistent person identifier.</param>
/// <param name="_name">The display name.</param>
/// <param name="_emailDigest">The email digest.</param>
/// <param name="_liveId">The external live identifier.</param>
/// <param name="_sourceId">The external source identifier.</param>
public sealed class Person(long _id, string _name, string _emailDigest, string _liveId,
                           string _sourceId) : IEquatable<Person>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Person, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(Person other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ EmailDigest.GetHashCode();
            hashCode = (hashCode * 397) ^ LiveId.GetHashCode();
            hashCode = (hashCode * 397) ^ SourceId.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the person to compact JSON.</summary>
    /// <returns>The serialized person.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the person to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes a person from JSON.</summary>
    /// <returns>The person, or <see langword="null" /> for null or empty input.</returns>
    public static Person FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the persistent person identifier.</summary>
    public long Id => _id;

    /// <summary>Gets the display name, or an empty string.</summary>
    public string Name => _name ?? string.Empty;

    /// <summary>Gets the email digest, or an empty string.</summary>
    public string EmailDigest => _emailDigest ?? string.Empty;

    /// <summary>Gets the external live identifier, or an empty string.</summary>
    public string LiveId => _liveId ?? string.Empty;

    /// <summary>Gets the external source identifier, or an empty string.</summary>
    public string SourceId => _sourceId ?? string.Empty;

    /// <summary>Gets whether the person has a non-empty name.</summary>
    public bool IsValid => Name.Length != 0;

    /// <summary>Gets whether all textual attributes are defined.</summary>
    public bool AllAttributesDefined => Name.Length != 0 && EmailDigest.Length != 0 && LiveId.Length != 0 && SourceId.Length != 0;

#endregion

#region Internal

    internal static Person FromJson(JObject jsonObject)
    {
        return new((long)jsonObject["id"],
            (string)jsonObject["name"],
            (string)jsonObject["email_digest"],
            (string)jsonObject["live_id"],
            (string)jsonObject["source_id"]
        );
    }

    internal JObject ToJson()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["name"] = Name,
            ["email_digest"] = EmailDigest,
            ["live_id"] = LiveId,
            ["source_id"] = SourceId
        };

        return obj;
    }

#endregion

#region Private

    private bool EqualsImp(Person other)
    {
        return Id == other.Id &&
               string.Equals(Name, other.Name, StringComparison.InvariantCulture) &&
               string.Equals(EmailDigest, other.EmailDigest, StringComparison.InvariantCulture) &&
               string.Equals(LiveId, other.LiveId, StringComparison.InvariantCulture) &&
               string.Equals(SourceId, other.SourceId, StringComparison.InvariantCulture);
    }

#endregion
}