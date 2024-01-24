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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public readonly struct FixedPoint64 : IEquatable<FixedPoint64>
    {
#region Public

        public FixedPoint64(long val)
        {
            RawValue = val;
        }

        public FixedPoint64(double val)
        {
            RawValue = (long)(val * (1L << 32));
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPoint64 fixedPoint && Equals(fixedPoint);
        }

        public bool Equals(FixedPoint64 other)
        {
            return RawValue == other.RawValue;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }


        public string ToJsonString()
        {
            return ToJson().ToString(Formatting.None);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static FixedPoint64 FromJsonString(string jsonString)
        {
            return FromJson(JToken.Parse(jsonString));
        }

        public static bool operator ==(FixedPoint64 lhs, FixedPoint64 rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FixedPoint64 lhs, FixedPoint64 rhs)
        {
            return !lhs.Equals(rhs);
        }

        public double Value => RawValue / (double) (1L << 32);
        public long RawValue { get; }

#endregion

#region Internal

        internal static FixedPoint64 FromJson(JToken jsonToken)
        {
            return new((long) jsonToken);
        }

        internal JToken ToJson()
        {
            return RawValue;
        }

#endregion
    }
}