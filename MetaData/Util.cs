﻿// *******************************************************************************
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
using System.Globalization;
using System.IO;
using System.Linq;

#endregion

namespace TCSystem.MetaData;

internal sealed class EmptyLastComparer : IComparer<PersonTag>
{
#region Public

    public int Compare(PersonTag x, PersonTag y)
    {
        string nameX = x?.Person.Name;
        string nameY = y?.Person.Name;
        if (string.IsNullOrWhiteSpace(nameX) &&
            !string.IsNullOrWhiteSpace(nameY))
        {
            return 1;
        }

        if (string.IsNullOrWhiteSpace(nameX))
        {
            return 0;
        }

        if (string.IsNullOrWhiteSpace(nameY))
        {
            return -1;
        }

        return string.Compare(nameX, nameY, StringComparison.Ordinal);
    }

#endregion
}

public static class Util
{
#region Public

    public static bool IsSupportedFileType(string fileName)
    {
        string ext = Path.GetExtension(fileName)?.ToLower(CultureInfo.InvariantCulture);
#if NET6_0_OR_GREATER
        return !fileName.Contains("$RECYCLE.BIN", StringComparison.InvariantCultureIgnoreCase) &&
#else
        return fileName.IndexOf("$RECYCLE.BIN", StringComparison.InvariantCultureIgnoreCase) == -1 &&
#endif
               _sExtensions.FirstOrDefault(x => x.Equals(ext, StringComparison.InvariantCulture)) != default;
    }

    public static IEnumerable<PersonTag> SortPersonTags(IEnumerable<PersonTag> personTags)
    {
        List<PersonTag> sortedPersonTags = personTags.ToList();
        sortedPersonTags.Sort(new EmptyLastComparer());
        return sortedPersonTags;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    public static DateTimeOffset Trim(this DateTimeOffset date, long ticks)
    {
        return new DateTime(date.Ticks - date.Ticks % ticks, DateTimeKind.Local);
    }

    public static IEnumerable<string> SupportedFileTypes => _sExtensions;

#endregion

#region Internal

    internal static bool HasMinimalDifference(double value1, double value2, int units = 16)
    {
        long lValue1 = BitConverter.DoubleToInt64Bits(value1);
        long lValue2 = BitConverter.DoubleToInt64Bits(value2);

        // If the signs are different, return false except for +0 and -0.
        if (lValue1 >> 63 != lValue2 >> 63)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value1 == value2)
            {
                return true;
            }

            return false;
        }

        long diff = Math.Abs(lValue1 - lValue2);
        return diff <= units;
    }

#endregion

#region Private

    private static readonly string[] _sExtensions = [".jpg", ".jpeg"];

#endregion
}