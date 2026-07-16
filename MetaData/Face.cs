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

/// <summary>Represents a detected face and its normalized image rectangle, classification, and optional descriptor.</summary>
public sealed class Face : IEquatable<Face>
{
#region Public

    /// <summary>Initializes a face.</summary>
    /// <param name="faceId">The persistent face identifier.</param>
    /// <param name="rectangle">The normalized face rectangle.</param>
    /// <param name="faceMode">The detector that found the face.</param>
    /// <param name="faceQuality">The face quality classification.</param>
    /// <param name="visible">Whether the face is visible to consumers.</param>
    /// <param name="faceDescriptor">The optional 128-value face descriptor; descriptors of other lengths are discarded.</param>
    public Face(long faceId, Rectangle rectangle, FaceMode faceMode, FaceQuality faceQuality, bool visible, IReadOnlyList<FixedPoint64> faceDescriptor)
    {
        Id = faceId;
        Rectangle = rectangle;
        FaceMode = faceMode;
        FaceQuality = faceQuality;
        Visible = visible;
        _faceDescriptor = faceDescriptor?.ToArray();
        // safety check for wrong face descriptors
        if (_faceDescriptor != null && _faceDescriptor.Length != FaceDescriptorLength)
        {
            _faceDescriptor = null;
        }
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Face, EqualsImp);
    }

    /// <inheritdoc />
    public bool Equals(Face other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Rectangle.GetHashCode();
            hashCode = (hashCode * 397) ^ FaceMode.GetHashCode();
            hashCode = (hashCode * 397) ^ FaceQuality.GetHashCode();
            hashCode = (hashCode * 397) ^ Visible.GetHashCode();
            return _faceDescriptor != null ?
                _faceDescriptor.Aggregate(hashCode, (current, fixedPoint64) => (current * 397) ^ fixedPoint64.GetHashCode()) :
                hashCode;
        }
    }

    /// <summary>Serializes the face to compact JSON.</summary>
    /// <returns>The serialized face.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Serializes the face to indented JSON.</summary>
    /// <returns>The formatted face JSON.</returns>
    public override string ToString()
    {
        return ToJson().ToString(Formatting.Indented);
    }

    /// <summary>Deserializes a face from JSON.</summary>
    /// <param name="jsonString">The JSON to deserialize.</param>
    /// <returns>The face, or <see langword="null" /> for null or empty input.</returns>
    public static Face FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Gets the persistent face identifier.</summary>
    public long Id { get; }

    /// <summary>Gets the normalized face rectangle.</summary>
    public Rectangle Rectangle { get; }

    /// <summary>Gets the detector that found the face.</summary>
    public FaceMode FaceMode { get; }

    /// <summary>Gets the face quality classification.</summary>
    public FaceQuality FaceQuality { get; }

    /// <summary>Gets whether the face was found by the frontal-face detector.</summary>
    public bool IsFrontFace => FaceMode == FaceMode.DlibFront;

    /// <summary>Gets whether the face is visible to consumers.</summary>
    public bool Visible { get; }

    /// <summary>Gets whether a valid 128-value face descriptor is available.</summary>
    public bool HasFaceDescriptor => _faceDescriptor != null;

    /// <summary>Gets the face descriptor, or <see langword="null" /> when no valid descriptor is available.</summary>
    public IReadOnlyList<FixedPoint64> FaceDescriptor => _faceDescriptor;

#endregion

#region Internal

    internal static Face FromJson(JObject jsonObject)
    {
        var fdJson = (JArray)jsonObject["face_descriptor"];
        return new((long)jsonObject["id"],
            Rectangle.FromJson((JObject)jsonObject["rectangle"]),
            (FaceMode)(int)jsonObject["face_mode"],
            (FaceQuality)(int)jsonObject["face_quality"],
            (int)jsonObject["visible"] == 1,
            fdJson is { Count: > 0 } ? fdJson.Select(v => new FixedPoint64((double)v)).ToArray() : null);
    }

    internal JObject ToJson()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["rectangle"] = Rectangle.ToJson(),
            ["face_mode"] = (int)FaceMode,
            ["face_quality"] = (int)FaceQuality,
            ["visible"] = Visible ? 1 : 0,
            ["face_descriptor"] = HasFaceDescriptor ? new(FaceDescriptor.Select(v => v.Value)) : new JArray()
        };

        return obj;
    }

#endregion

#region Private

    private const int FaceDescriptorLength = 128;

    private bool EqualsImp(Face other)
    {
        var equal = false;
        if (Id == other.Id &&
            Rectangle.Equals(other.Rectangle) &&
            FaceMode == other.FaceMode &&
            FaceQuality == other.FaceQuality &&
            Visible == other.Visible)
        {
            if (_faceDescriptor == null && other._faceDescriptor == null)
            {
                equal = true;
            }
            else if (_faceDescriptor != null)
            {
                equal = _faceDescriptor.SequenceEqual(other._faceDescriptor);
            }
        }

        return equal;
    }

    private readonly FixedPoint64[] _faceDescriptor;

#endregion
}