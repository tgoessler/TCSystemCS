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

#endregion

namespace TCSystem.MetaData;

/// <summary>Associates two face identifiers with their match percentage.</summary>
/// <param name="_faceId1">The first face identifier.</param>
/// <param name="_faceId2">The second face identifier.</param>
/// <param name="_distance">The match percentage.</param>
public readonly struct FaceDistanceInfo(long _faceId1, long _faceId2, int _distance) : IEquatable<FaceDistanceInfo>
{
#region Public

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is FaceDistanceInfo other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(FaceDistanceInfo other)
    {
        return FaceId1.Equals(other.FaceId1) &&
               Distance.Equals(other.Distance) &&
               FaceId2.Equals(other.FaceId2);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = FaceId1.GetHashCode();
            hashCode = (hashCode * 397) ^ Distance.GetHashCode();
            hashCode = (hashCode * 397) ^ FaceId2.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the face-distance information to compact JSON.</summary>
    /// <returns>The serialized value.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the face-distance information to indented JSON.</summary>
    /// <returns>The formatted JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes face-distance information from JSON.</summary>
    /// <param name="jsonString">The JSON object to deserialize.</param>
    /// <returns>The deserialized value.</returns>
    public static FaceDistanceInfo FromJsonString(string jsonString)
    {
        return FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Deserializes an array of face-distance values.</summary>
    /// <param name="jsonString">The JSON array to deserialize.</param>
    /// <returns>The values, or an empty sequence for null or empty input.</returns>
    public static IEnumerable<FaceDistanceInfo> FromJsonStringArray(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return Array.Empty<FaceDistanceInfo>();
        }

        JArray array = JArray.Parse(jsonString);
        return array.Select(v => FromJson((JObject)v));
    }

    /// <summary>Determines whether two values are equal.</summary>
    public static bool operator ==(FaceDistanceInfo lhs, FaceDistanceInfo rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two values are not equal.</summary>
    public static bool operator !=(FaceDistanceInfo lhs, FaceDistanceInfo rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>Gets the first face identifier.</summary>
    public long FaceId1 => _faceId1;

    /// <summary>Gets the second face identifier.</summary>
    public long FaceId2 => _faceId2;

    /// <summary>Gets how closely the first face matches the second, in percent.</summary>
    public int Distance => _distance;

#endregion

#region Private

    private static FaceDistanceInfo FromJson(JObject jsonObject)
    {
        return new(
            (long)jsonObject["face_id_1"],
            (long)jsonObject["face_id_2"],
            (int)jsonObject["distance"]
        );
    }

    private JObject ToJson()
    {
        var obj = new JObject
        {
            ["face_id_1"] = FaceId1,
            ["distance"] = Distance,
            ["face_id_2"] = FaceId2
        };

        return obj;
    }

#endregion
}