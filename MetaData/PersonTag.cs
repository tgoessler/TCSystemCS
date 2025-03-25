﻿// *******************************************************************************
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

public sealed class PersonTag(Person person, Face face) : IEquatable<PersonTag>
{
#region Public

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as PersonTag, EqualsImp);
    }

    public bool Equals(PersonTag other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Person.GetHashCode();
            hashCode = (hashCode * 397) ^ Face.GetHashCode();
            return hashCode;
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

    public static PersonTag FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public Person Person { get; } = person;
    public Face Face { get; } = face;

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