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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public sealed class PersonTag
    {
#region Public

        public PersonTag(Person person, Face face)
        {
            Person = person;
            Face = face;
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

            return obj is PersonTag personTag && Equals(personTag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Person.GetHashCode();
                hashCode = (hashCode * 397) ^ Face.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ToJson().ToString(Formatting.Indented);
        }

        public static PersonTag FromJsonString(string jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
        }

        public static PersonTag InvalidateIds(PersonTag personTag)
        {
            return new PersonTag(Person.InvalidateId(personTag.Person),
                Face.InvalidateId(personTag.Face));
        }

        public Person Person { get; }
        public Face Face { get; }

#endregion

#region Internal

        internal static PersonTag FromJson(JObject jsonObject)
        {
            return new PersonTag(Person.FromJson((JObject) jsonObject["person"]),
                Face.FromJson((JObject) jsonObject["face"])
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

        private bool Equals(PersonTag other)
        {
            return Equals(Person, other.Person) &&
                   Equals(Face, other.Face);
        }

#endregion
    }
}