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
//  Copyright (C) 2003 - 2023 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;
using System.Linq;

#endregion

namespace TCSystem.MetaData.Tests
{
    public static class TestData
    {
#region Public

        public static readonly FixedPoint32 FixedPoint32Zero = new(0);
        public static readonly FixedPoint32 FixedPoint321 = new(-123);
        public static readonly FixedPoint32 FixedPoint322 = new(1234);

        public static readonly FixedPoint64 FixedPoint64Zero = new(0);
        public static readonly FixedPoint64 FixedPoint641 = new(-123);
        public static readonly FixedPoint64 FixedPoint642 = new(1234);

        public static readonly Rectangle RectangleZero = new(FixedPoint32Zero, FixedPoint32Zero, FixedPoint32Zero, FixedPoint32Zero);
        public static readonly Rectangle Rectangle1 = new(FixedPoint321, FixedPoint321, FixedPoint322, FixedPoint321);
        public static readonly Rectangle Rectangle2 = new(FixedPoint322, FixedPoint321, FixedPoint321, FixedPoint322);

        public static readonly Address AddressZero = new();
        public static readonly Address Address1 = new("Austria", "Steiermark", "Graz", "Steinberg 155");
        public static readonly Address Address2 = new("Kroatien", "Dalmatien", "Skradin", "Vadavuca 1");

        public static readonly GpsPosition GpsPositionZero = new(0, 0, 0, 0, false);
        public static readonly GpsPosition GpsPosition1 = new(47, 4, 15, 16, false);
        public static readonly GpsPosition GpsPosition2 = new(47, 4, 15, 16, true);

        public static readonly GpsPoint GpsPointZero = new();
        public static readonly GpsPoint GpsPoint1 = new(GpsPosition2, GpsPosition1, 365);
        public static readonly GpsPoint GpsPoint2 = new(GpsPosition1, GpsPosition2, -365);

        public static readonly Location LocationZero = new(AddressZero, GpsPointZero);
        public static readonly Location Location1 = new(Address1, GpsPoint1);
        public static readonly Location Location2 = new(Address2, GpsPoint2);

        public static readonly FaceDistanceInfo FaceDistanceInfoZero = new(0, 0, 0);
        public static readonly FaceDistanceInfo FaceDistanceInfo1 = new(1, 1, 10);
        public static readonly FaceDistanceInfo FaceDistanceInfo2 = new(2, 2, 5);

        public static readonly FaceInfo FaceInfoZero = new(Constants.InvalidId, Constants.InvalidId, Constants.InvalidId, FaceMode.DlibCnn, null);
        public static readonly FaceInfo FaceInfo1 = new(1, 2, 3, FaceMode.DlibCnn, Array.Empty<FixedPoint64>());

        public static readonly FaceInfo FaceInfo2 = new(2, 3, 4, FaceMode.DlibCnn,
            Enumerable.Repeat(new FixedPoint64(1), 128));

        public static readonly Face FaceZero = new(0, Rectangle.FromFloat(0, 0, 0, 0), FaceMode.Undefined, false, null);
        public static readonly Face Face1 = new(1, Rectangle.FromFloat(10, 10, 100, 100), FaceMode.DlibFront, true, Enumerable.Repeat(new FixedPoint64(1), 128));
        public static readonly Face Face2 = new(2, Rectangle.FromFloat(15, 17, 90, 70), FaceMode.Undefined, true, Enumerable.Repeat(new FixedPoint64(2), 128));

        public static readonly Person PersonZero = new(0, null, null, null, null);
        public static readonly Person Person1 = new(1, "Thomas", "thomas@email.com", "123", "456");
        public static readonly Person Person2 = new(2, "Sabine", "sabine@email.com", "789", "012");

        public static readonly PersonTag PersonTagZero = new(PersonZero, FaceZero);
        public static readonly PersonTag PersonTag1 = new(Person1, Face1);
        public static readonly PersonTag PersonTag2 = new(Person2, Face2);

        public static readonly FileAndPersonTag FileAndPersonTagZero = new(null, PersonTagZero);
        public static readonly FileAndPersonTag FileAndPersonTag1 = new("file1", PersonTag1);
        public static readonly FileAndPersonTag FileAndPersonTag2 = new("file2", PersonTag2);

#endregion
    }
}