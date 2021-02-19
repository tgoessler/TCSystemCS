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
//  Copyright (C) 2003 - 2021 Thomas Goessler. All Rights Reserved.
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
    public readonly struct FixedPoint32 : IEquatable<FixedPoint32>
    {
#region Public

        public FixedPoint32(int val)
        {
            RawValue = val;
        }

        public FixedPoint32(float val)
        {
            RawValue = (int)(val * (1 << 16));
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPoint32 fixedPoint && Equals(fixedPoint);
        }

        public bool Equals(FixedPoint32 other)
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

        public static FixedPoint32 FromJsonString(string jsonString)
        {
            return FromJson(JToken.Parse(jsonString));
        }

        public float Value => RawValue / (float) (1 << 16);
        public int RawValue { get; }

#endregion

#region Internal

        internal static FixedPoint32 FromJson(JToken jsonToken)
        {
            return new((int) jsonToken);
        }

        internal JToken ToJson()
        {
            return RawValue;
        }

#endregion
    }
}