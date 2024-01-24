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
using System.Linq;
using TCSystem.MetaData;
using static TCSystem.MetaData.Tests.TestData;

#endregion

namespace TCSystem.MetaDataDB.Tests
{
    public static class TestData
    {
#region Public

        public static readonly Face Face1 = new(Constants.InvalidId, Rectangle.FromFloat(10, 10, 100, 100), FaceMode.DlibFront, true, Enumerable.Repeat(new FixedPoint64(1), 128));
        public static readonly Face Face2 = new(Constants.InvalidId, Rectangle.FromFloat(15, 17, 90, 70), FaceMode.Undefined, false, Enumerable.Repeat(new FixedPoint64(2), 128));
        public static readonly Face Face3 = new(Constants.InvalidId, Rectangle.FromFloat(17, 15, 70, 90), FaceMode.DlibCnn, true, Enumerable.Repeat(new FixedPoint64(3), 128));

        public static readonly Person Person1 = new(Constants.InvalidId, "01 Thomas", "thomas@email.com", "123", "456");
        public static readonly Person Person2 = new(Constants.InvalidId, "02 Sabine", "sabine@email.com", "789", "012");
        public static readonly Person Person3 = new(Constants.InvalidId, "03 Laura", "Laura@email.com", "456", "234");

        public static readonly PersonTag PersonTag1 = new(Person1, Face1);
        public static readonly PersonTag PersonTag2 = new(Person2, Face2);

        public const string FileNameZero = "01 foto.jpeg";
        public const string FileName1 = "02 foto.jpeg";
        public const string FileName2 = "03 foto.jpeg";

        public const string Tag1 = "01 Urlaub";
        public const string Tag2 = "02 Geburtstag";
        public const string Tag3 = "03 Ostern";

        public static readonly Image ImageZero = new(Constants.InvalidId, FileNameZero, ProcessingInfos.None, 0, 0, OrientationMode.Normal,
            DateTimeOffset.Now, "", null, Array.Empty<PersonTag>(), Array.Empty<string>());

        public static readonly Image Image1 = new(Constants.InvalidId, FileName1, ProcessingInfos.DlibCnnFaceDetection1000, 100, 100, OrientationMode.Normal,
            DateTimeOffset.Now, "", Location1, new[] { PersonTag1 }, new[] { Tag1, Tag2 });

        public static readonly Image Image2 = new(Constants.InvalidId, FileName2, ProcessingInfos.DlibCnnFaceDetection2000, 200, 200, OrientationMode.MirrorHorizontal,
            DateTimeOffset.Now, "", Location2, new[] { PersonTag1, PersonTag2 }, new[] { Tag1, Tag2, Tag3 });

#endregion
    }
}