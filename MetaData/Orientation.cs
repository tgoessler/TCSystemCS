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

#endregion

namespace TCSystem.MetaData
{
    public enum OrientationMode
    {
        Undefined = 0,
        Normal = 1,
        MirrorHorizontal = 2,
        Rotate180 = 3,
        MirrorVertical = 4,
        MirrorHorizontalRotateCw270 = 5,
        RotateCw90 = 6,
        MirrorVerticalRotateCw90 = 7,
        RotateCw270 = 8
    }

    public static class Orientation
    {
#region Public

        public static double Orientation2Degree(OrientationMode mode)
        {
            switch (mode)
            {
                case OrientationMode.Undefined:
                case OrientationMode.Normal:
                case OrientationMode.MirrorHorizontal:
                    return 0.0;
                case OrientationMode.Rotate180:
                case OrientationMode.MirrorVertical:
                    return 180;
                case OrientationMode.MirrorHorizontalRotateCw270:
                case OrientationMode.RotateCw270:
                case OrientationMode.MirrorVerticalRotateCw90:
                    return 90;
                case OrientationMode.RotateCw90:
                    return 270;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static Rectangle Orientate(Rectangle rectangle, OrientationMode mode)
        {
            switch (mode)
            {
                case OrientationMode.Undefined:
                case OrientationMode.Normal:
                    return rectangle;
                case OrientationMode.MirrorHorizontal:
                    return FlipHorizontal(rectangle);
                case OrientationMode.Rotate180:
                    return Rotate180(rectangle);
                case OrientationMode.MirrorVertical:
                    return FlipVertical(rectangle);
                case OrientationMode.MirrorHorizontalRotateCw270:
                    return RotateCw270(FlipHorizontal(rectangle));
                case OrientationMode.RotateCw90:
                    return RotateCw90(rectangle);
                case OrientationMode.MirrorVerticalRotateCw90:
                    return RotateCw90(FlipVertical(rectangle));
                case OrientationMode.RotateCw270:
                    return RotateCw270(rectangle);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static Face Orientate(Face face, OrientationMode mode)
        {
            return new Face(face.Id, Orientate(face.Rectangle, mode), face.FaceMode, face.FaceDescriptor);
        }

        public static Rectangle OrientateBack(Rectangle rectangle, OrientationMode mode)
        {
            switch (mode)
            {
                case OrientationMode.Undefined:
                case OrientationMode.Normal:
                    return rectangle;
                case OrientationMode.MirrorHorizontal:
                    return FlipHorizontal(rectangle);
                case OrientationMode.Rotate180:
                    return Rotate180(rectangle);
                case OrientationMode.MirrorVertical:
                    return FlipVertical(rectangle);
                case OrientationMode.MirrorHorizontalRotateCw270:
                    return FlipHorizontal(RotateCw90(rectangle));
                case OrientationMode.RotateCw90:
                    return RotateCw270(rectangle);
                case OrientationMode.MirrorVerticalRotateCw90:
                    return FlipVertical(RotateCw270(rectangle));
                case OrientationMode.RotateCw270:
                    return RotateCw90(rectangle);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static Face OrientateBack(Face face, OrientationMode mode)
        {
            return new Face(face.Id, OrientateBack(face.Rectangle, mode), face.FaceMode, face.FaceDescriptor);
        }

#endregion

#region Private

        private static Rectangle FlipVertical(Rectangle rectangle)
        {
            var min = (rectangle.Left.Value, 1.0f - rectangle.Bottom.Value);
            var max = (rectangle.Right.Value, 1.0f - rectangle.Top.Value);

            return Rectangle.FromFloat(min.Item1, min.Item2,
                max.Item1 - min.Item1, max.Item2 - min.Item2);
        }

        private static Rectangle FlipHorizontal(Rectangle rectangle)
        {
            var min = (1.0f - rectangle.Right.Value, rectangle.Top.Value);
            var max = (1.0f - rectangle.Left.Value, rectangle.Bottom.Value);

            return Rectangle.FromFloat(min.Item1, min.Item2,
                max.Item1 - min.Item1, max.Item2 - min.Item2);
        }

        private static Rectangle Rotate180(Rectangle rectangle)
        {
            var min = (1.0f - rectangle.Right.Value, 1.0f - rectangle.Bottom.Value);
            var max = (1.0f - rectangle.Left.Value, 1.0f - rectangle.Top.Value);

            return Rectangle.FromFloat(min.Item1, min.Item2,
                max.Item1 - min.Item1, max.Item2 - min.Item2);
        }

        private static Rectangle RotateCw90(Rectangle rectangle)
        {
            var min = (rectangle.Top.Value, 1.0f - rectangle.Right.Value);
            var max = (rectangle.Bottom.Value, 1.0f - rectangle.Left.Value);

            return Rectangle.FromFloat(min.Item1, min.Item2,
                max.Item1 - min.Item1, max.Item2 - min.Item2);
        }

        private static Rectangle RotateCw270(Rectangle rectangle)
        {
            var min = (1.0f - rectangle.Bottom.Value, rectangle.Left.Value);
            var max = (1.0f - rectangle.Top.Value, rectangle.Right.Value);

            return Rectangle.FromFloat(min.Item1, min.Item2,
                max.Item1 - min.Item1, max.Item2 - min.Item2);
        }

#endregion
    }
}