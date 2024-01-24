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

#endregion

namespace TCSystem.MetaData
{
    public readonly struct Rectangle : IEquatable<Rectangle>
    {
#region Public

        public Rectangle(FixedPoint32 x, FixedPoint32 y, FixedPoint32 w, FixedPoint32 h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public bool Contains(Rectangle other)
        {
            return Left <= other.Left &&
                   Top <= other.Top && 
                   Right >= other.Right &&
                   Bottom >= other.Bottom;
        }

        public static Rectangle FromFloat(float x, float y, float w, float h)
        {
            return new(new FixedPoint32(x),
                new FixedPoint32(y),
                new FixedPoint32(w),
                new FixedPoint32(h)
            );
        }

        public static Rectangle FromRawValues(int x, int y, int w, int h)
        {
            return new(new FixedPoint32(x),
                new FixedPoint32(y),
                new FixedPoint32(w),
                new FixedPoint32(h)
            );
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle rect && Equals(rect);
        }

        public bool Equals(Rectangle other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y) &&
                   W.Equals(other.W) &&
                   H.Equals(other.H);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                hashCode = (hashCode * 397) ^ H.GetHashCode();
                return hashCode;
            }
        }

        public string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {W}, {H}";
        }

        public static Rectangle FromJsonString(string jsonString)
        {
            return FromJson(JObject.Parse(jsonString));
        }

        public static bool operator ==(Rectangle lhs, Rectangle rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Rectangle lhs, Rectangle rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Rectangle FromJson(JObject jsonObject)
        {
            return new(
                FixedPoint32.FromJson(jsonObject["x"]),
                FixedPoint32.FromJson(jsonObject["y"]),
                FixedPoint32.FromJson(jsonObject["w"]),
                FixedPoint32.FromJson(jsonObject["h"])
            );
        }

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

        public FixedPoint32 X { get; }
        public FixedPoint32 Y { get; }
        public FixedPoint32 W { get; }
        public FixedPoint32 H { get; }

        public FixedPoint32 Left => X;
        public FixedPoint32 Top => Y;
        public FixedPoint32 Right => new(X.RawValue + W.RawValue);
        public FixedPoint32 Bottom => new(Y.RawValue + H.RawValue);
        public FixedPoint32 Diameter => new((int)Math.Sqrt(W.RawValue * W.RawValue + H.RawValue * H.RawValue));
        public (FixedPoint32 x, FixedPoint32 y) Center => 
            (new((Left.RawValue + Right.RawValue) / 2), new((Top.RawValue + Bottom.RawValue) / 2));
#endregion
    }
}