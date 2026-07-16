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

#endregion

namespace TCSystem.MetaData;

/// <summary>Represents a normalized image rectangle using 16.16 fixed-point coordinates.</summary>
/// <param name="_x">The left coordinate.</param>
/// <param name="_y">The top coordinate.</param>
/// <param name="_w">The width.</param>
/// <param name="_h">The height.</param>
public readonly struct Rectangle(FixedPoint32 _x, FixedPoint32 _y, FixedPoint32 _w, FixedPoint32 _h) : IEquatable<Rectangle>
{
#region Public

    /// <summary>Determines whether this rectangle fully contains another rectangle.</summary>
    public bool Contains(Rectangle other)
    {
        return Left <= other.Left &&
               Top <= other.Top &&
               Right >= other.Right &&
               Bottom >= other.Bottom;
    }

    /// <summary>Creates a rectangle from floating-point coordinates.</summary>
    public static Rectangle FromFloat(float x, float y, float w, float h)
    {
        return new(new(x),
            new(y),
            new(w),
            new(h)
        );
    }

    /// <summary>Creates a rectangle from raw 16.16 fixed-point coordinates.</summary>
    public static Rectangle FromRawValues(int x, int y, int w, int h)
    {
        return new(new(x),
            new(y),
            new(w),
            new(h)
        );
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Rectangle rect && Equals(rect);
    }

    /// <inheritdoc />
    public bool Equals(Rectangle other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y) &&
               W.Equals(other.W) &&
               H.Equals(other.H);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            hashCode = (hashCode * 397) ^ W.GetHashCode();
            hashCode = (hashCode * 397) ^ H.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>Serializes the rectangle to compact JSON.</summary>
    /// <returns>The serialized rectangle.</returns>
    public string ToJsonString()
    {
        return ToJson().ToString(Formatting.None);
    }

    /// <summary>Formats the rectangle as its X, Y, width, and height values.</summary>
    public override string ToString()
    {
        return $"{X}, {Y}, {W}, {H}";
    }

    /// <summary>Deserializes a rectangle from JSON text.</summary>
    public static Rectangle FromJsonString(string jsonString)
    {
        return FromJson(JObject.Parse(jsonString));
    }

    /// <summary>Determines whether two rectangles are equal.</summary>
    public static bool operator ==(Rectangle lhs, Rectangle rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two rectangles are not equal.</summary>
    public static bool operator !=(Rectangle lhs, Rectangle rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>Creates a rectangle from a JSON object containing raw fixed-point values.</summary>
    public static Rectangle FromJson(JObject jsonObject)
    {
        return new(
            FixedPoint32.FromJson(jsonObject["x"]),
            FixedPoint32.FromJson(jsonObject["y"]),
            FixedPoint32.FromJson(jsonObject["w"]),
            FixedPoint32.FromJson(jsonObject["h"])
        );
    }

    /// <summary>Converts the rectangle to a JSON object containing raw fixed-point values.</summary>
    public JObject ToJson()
    {
        var obj = new JObject
        {
            ["x"] = X.ToJson(),
            ["y"] = Y.ToJson(),
            ["w"] = W.ToJson(),
            ["h"] = H.ToJson()
        };

        return obj;
    }

    /// <summary>Gets the left coordinate.</summary>
    public FixedPoint32 X => _x;

    /// <summary>Gets the top coordinate.</summary>
    public FixedPoint32 Y => _y;

    /// <summary>Gets the width.</summary>
    public FixedPoint32 W => _w;

    /// <summary>Gets the height.</summary>
    public FixedPoint32 H => _h;

    /// <summary>Gets the left edge.</summary>
    public FixedPoint32 Left => X;

    /// <summary>Gets the top edge.</summary>
    public FixedPoint32 Top => Y;

    /// <summary>Gets the right edge.</summary>
    public FixedPoint32 Right => new(X.RawValue + W.RawValue);

    /// <summary>Gets the bottom edge.</summary>
    public FixedPoint32 Bottom => new(Y.RawValue + H.RawValue);

    /// <summary>Gets the diagonal length of the rectangle.</summary>
    public FixedPoint32 Diameter => new((int)Math.Sqrt((long)W.RawValue * (long)W.RawValue + (long)H.RawValue * (long)H.RawValue));

    /// <summary>Gets the center point.</summary>
    public (FixedPoint32 x, FixedPoint32 y) Center =>
        (new((Left.RawValue + Right.RawValue) / 2), new((Top.RawValue + Bottom.RawValue) / 2));

#endregion
}