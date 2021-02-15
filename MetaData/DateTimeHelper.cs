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
using Newtonsoft.Json.Linq;

#endregion

namespace TCSystem.MetaData
{
    public static class DateTimeHelper
    {
#region Internal

        internal static DateTimeOffset FromJson(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                return new DateTime(
                    (int) jsonObject["year"],
                    (int) jsonObject["month"],
                    (int) jsonObject["day"],
                    (int) jsonObject["hours"],
                    (int) jsonObject["minutes"],
                    (int) jsonObject["seconds"],
                    (int) jsonObject["milli_seconds"],
                    DateTimeKind.Local);
            }

            return Image.InvalidDateTaken;
        }

        internal static JObject ToJson(DateTimeOffset dateTime)
        {
            var obj = new JObject
            {
                ["year"] = dateTime.Year,
                ["month"] = dateTime.Month,
                ["day"] = dateTime.Day,
                ["hours"] = dateTime.Hour,
                ["minutes"] = dateTime.Minute,
                ["seconds"] = dateTime.Second,
                ["milli_seconds"] = dateTime.Millisecond
            };

            return obj;
        }

#endregion
    }
}