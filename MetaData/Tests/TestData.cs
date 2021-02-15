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

namespace TCSystem.MetaData.Tests
{
    public static class TestData
    {
        public static readonly GpsPosition TestPositionZero = new(0, 0, 0, 0, false);
        public static readonly GpsPosition TestPosition1 = new(47, 4, 15, 16, false);
        public static readonly GpsPosition TestPosition2 = new(47, 4, 15, 16, true);

        public static readonly GpsPoint PointZero = new(TestData.TestPositionZero, TestData.TestPositionZero, 0);
        public static readonly GpsPoint Point1 = new(TestData.TestPositionZero, TestData.TestPosition1, 365);
        public static readonly GpsPoint Point2 = new(TestData.TestPositionZero, TestData.TestPosition2, -365);

    }
}