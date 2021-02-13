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

using System;
using System.Globalization;
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

        public static FixedPoint32 FromFloat(float val)
        {
            return new FixedPoint32((int) (val * (1 << 16)));
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

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public float Value => RawValue / (float) (1 << 16);
        public int RawValue { get; }

#endregion

#region Internal

        internal static FixedPoint32 FromJson(JToken jsonToken)
        {
            return new FixedPoint32((int) jsonToken);
        }

        internal JToken ToJson()
        {
            return RawValue;
        }

#endregion
    }
}