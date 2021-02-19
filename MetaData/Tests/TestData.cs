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

using System.Linq;

namespace TCSystem.MetaData.Tests
{
    public static class TestData
    {
#region Public

        public static readonly Address AddressZero = new();
        public static readonly Address Address1 = new("Austria", "Steiermark", "Graz", "Steinberg 155");
        public static readonly Address Address2 = new("Kroatien", "Dalmatien", "Skradin", "Vadavuca 1");

        public static readonly GpsPosition PositionZero = new(0, 0, 0, 0, false);
        public static readonly GpsPosition Position1 = new(47, 4, 15, 16, false);
        public static readonly GpsPosition Position2 = new(47, 4, 15, 16, true);

        public static readonly GpsPoint PointZero = new();
        public static readonly GpsPoint Point1 = new(PositionZero, Position1, 365);
        public static readonly GpsPoint Point2 = new(PositionZero, Position2, -365);

        public static readonly Location LocationZero = new(AddressZero, PointZero);
        public static readonly Location Location1 = new(Address1, Point1);
        public static readonly Location Location2 = new(Address2, Point2);

        public static readonly FaceDistanceInfo FaceDistanceInfo1 = new(0, new FixedPoint32(10), 1);
        public static readonly FaceDistanceInfo FaceDistanceInfo2 = new(1, new FixedPoint32(5), 2);

        public static readonly FaceInfo FaceInfo1 = new(1, 2, 3, FaceMode.DlibCnn, new FixedPoint64[0]);
        public static readonly FaceInfo FaceInfo2 = new(2, 3, 4, FaceMode.DlibCnn, 
            Enumerable.Repeat(new FixedPoint64(1), 128));

        public static readonly Face FaceZero = new(0, Rectangle.FromFloat(0, 0, 0, 0), FaceMode.Undefined, null);
        public static readonly Face Face1 = new(1, Rectangle.FromFloat(10, 10, 100, 100), FaceMode.DlibFront,
            Enumerable.Repeat(new FixedPoint64(1), 128));
        public static readonly Face Face2 = new(2, Rectangle.FromFloat(15, 17, 90, 70), FaceMode.Undefined,
            Enumerable.Repeat(new FixedPoint64(2), 128));

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