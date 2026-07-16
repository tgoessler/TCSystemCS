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

/// <summary>Associates a person with a detected face in an image.</summary>
/// <param name="_person">The associated person.</param>
/// <param name="_face">The associated face.</param>
public sealed class PersonTag(Person _person, Face _face) : IEquatable<PersonTag>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as PersonTag, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(PersonTag other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Person.GetHashCode();
            hashCode = (hashCode * 397) ^ Face.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the person tag to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Serializes the person tag to compact JSON.</summary>
    /// <returns>The serialized person tag.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Deserializes a person tag from JSON.</summary>
    /// <returns>The person tag, or <see langword="null" /> for null or empty input.</returns>
    public static PersonTag FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the associated person.</summary>
    public Person Person => _person;

    /// <summary>Gets the associated face.</summary>
    public Face Face => _face;

#endregion

#region Internal

    internal static PersonTag FromJson(JObject jsonObject)
    {
        return new(Person.FromJson((JObject)jsonObject["person"]),
            Face.FromJson((JObject)jsonObject["face"])
        );
    }

    internal JObject ToJson()
    {
        var obj = new JObject
        {
            ["person"] = Person.ToJson(),
            ["face"] = Face.ToJson()
        };

        return obj;
    }

#endregion

#region Private

    private bool EqualsImp(PersonTag other)
    {
        return Equals(Person, other.Person) &&
               Equals(Face, other.Face);
    }

#endregion
}