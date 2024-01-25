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

public sealed class Face : IEquatable<Face>
{
#region Public

    public Face(long faceId, Rectangle rectangle, FaceMode faceMode, bool visible, IEnumerable<FixedPoint64> faceDescriptor)
    {
        Id = faceId;
        Rectangle = rectangle;
        FaceMode = faceMode;
        Visible = visible;
        _faceDescriptor = faceDescriptor?.ToArray();
        // safety check for wrong face descriptors
        if (_faceDescriptor != null && _faceDescriptor.Length != FaceDescriptorLength)
        {
            _faceDescriptor = null;
        }
    }

    public override bool Equals(object obj)
    {
        return EqualsUtil.Equals(this, obj as Face, EqualsImp);
    }

    public bool Equals(Face other)
    {
        return EqualsUtil.Equals(this, other, EqualsImp);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Rectangle.GetHashCode();
            hashCode = (hashCode * 397) ^ FaceMode.GetHashCode();
            hashCode = (hashCode * 397) ^ Visible.GetHashCode();
            return _faceDescriptor != null ?
                _faceDescriptor.Aggregate(hashCode, (current, fixedPoint64) => (current * 397) ^ fixedPoint64.GetHashCode()) :
                hashCode;
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

    public static Face FromJsonString(string jsonString)
    {
        return string.IsNullOrEmpty(jsonString) ? null : FromJson(JObject.Parse(jsonString));
    }

    public long Id { get; }
    public Rectangle Rectangle { get; }
    public FaceMode FaceMode { get; }
    public bool IsFrontFace => FaceMode == FaceMode.DlibFront;
    public bool Visible { get; }
    public bool HasFaceDescriptor => _faceDescriptor != null;
    public IReadOnlyCollection<FixedPoint64> FaceDescriptor => _faceDescriptor;

#endregion

#region Internal

    internal static Face FromJson(JObject jsonObject)
    {
        var fdJson = (JArray)jsonObject["face_descriptor"];
        return new((long)jsonObject["id"],
            Rectangle.FromJson((JObject)jsonObject["rectangle"]),
            (FaceMode)(int)jsonObject["face_mode"],
            (int)jsonObject["visible"] == 1,
            fdJson is { Count: > 0 } ? fdJson.Select(v => new FixedPoint64((double)v)) : null);
    }

    internal JObject ToJson()
    {
        var obj = new JObject
        {
            ["id"] = Id,
            ["rectangle"] = Rectangle.ToJson(),
            ["face_mode"] = (int)FaceMode,
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
        if (Id == other.Id && Rectangle.Equals(other.Rectangle) && Visible == other.Visible)
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