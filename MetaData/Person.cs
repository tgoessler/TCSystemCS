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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCSystem.Util;

#endregion

namespace TCSystem.MetaData;

public sealed class Person(long _id, string _name, string _emailDigest, string _liveId,
                           string _sourceId) : IEquatable<Person>
{
#region Public

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Person, EqualsImp);
    }

    public bool Equals(Person other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

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

    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    public static Person FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public long Id => _id;
    public string Name => _name ?? string.Empty;
    public string EmailDigest => _emailDigest ?? string.Empty;
    public string LiveId => _liveId ?? string.Empty;
    public string SourceId => _sourceId ?? string.Empty;

    public bool IsValid => Name.Length != 0;
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