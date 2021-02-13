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
    public readonly struct FixedPoint64 : IEquatable<FixedPoint64>
    {
#region Public

        public FixedPoint64(long val)
        {
            RawValue = val;
        }

        public static FixedPoint64 FromDouble(double val)
        {
            return new FixedPoint64((long) (val * (1L << 32)));
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

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public double Value => RawValue / (double) (1L << 32);
        public long RawValue { get; }

#endregion

#region Internal

        internal static FixedPoint64 FromJson(JToken jsonToken)
        {
            return new FixedPoint64((long) jsonToken);
        }

        internal JToken ToJson()
        {
            return RawValue;
        }

#endregion
    }
}