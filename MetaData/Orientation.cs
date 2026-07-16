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
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************

#region Usings

using System;

#endregion

namespace TCSystem.MetaData;

/// <summary>Describes the EXIF orientation transform applied to an image.</summary>
public enum OrientationMode
{
    /// <summary>The orientation is unknown.</summary>
    Undefined = 0,

    /// <summary>No transform is required.</summary>
    Normal = 1,

    /// <summary>Mirror horizontally.</summary>
    MirrorHorizontal = 2,

    /// <summary>Rotate by 180 degrees.</summary>
    Rotate180 = 3,

    /// <summary>Mirror vertically.</summary>
    MirrorVertical = 4,

    /// <summary>Mirror horizontally and rotate clockwise by 270 degrees.</summary>
    MirrorHorizontalRotateCw270 = 5,

    /// <summary>Rotate clockwise by 90 degrees.</summary>
    RotateCw90 = 6,

    /// <summary>Mirror vertically and rotate clockwise by 90 degrees.</summary>
    MirrorVerticalRotateCw90 = 7,

    /// <summary>Rotate clockwise by 270 degrees.</summary>
    RotateCw270 = 8
}

/// <summary>Transforms normalized rectangles and faces according to image orientation.</summary>
public static class Orientation
{
#region Public

    /// <summary>Gets the rotation angle represented by an orientation mode.</summary>
    /// <returns>The rotation in degrees.</returns>
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

    /// <summary>Transforms a normalized rectangle into the displayed orientation.</summary>
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

    /// <summary>Returns a face whose rectangle is transformed into the displayed orientation.</summary>
    public static Face Orientate(Face face, OrientationMode mode)
    {
        return new(face.Id, Orientate(face.Rectangle, mode), face.FaceMode, face.FaceQuality, face.Visible, face.FaceDescriptor);
    }

    /// <summary>Transforms a normalized rectangle from the displayed orientation back to stored coordinates.</summary>
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

    /// <summary>Returns a face whose rectangle is transformed back to stored coordinates.</summary>
    public static Face OrientateBack(Face face, OrientationMode mode)
    {
        return new(face.Id, OrientateBack(face.Rectangle, mode), face.FaceMode, face.FaceQuality, face.Visible, face.FaceDescriptor);
    }

#endregion

#region Private

    private static Rectangle FlipVertical(Rectangle rectangle)
    {
        (float Value, float) min = (rectangle.Left.Value, 1.0f - rectangle.Bottom.Value);
        (float Value, float) max = (rectangle.Right.Value, 1.0f - rectangle.Top.Value);

        return Rectangle.FromFloat(min.Item1, min.Item2,
            max.Item1 - min.Item1, max.Item2 - min.Item2);
    }

    private static Rectangle FlipHorizontal(Rectangle rectangle)
    {
        (float, float Value) min = (1.0f - rectangle.Right.Value, rectangle.Top.Value);
        (float, float Value) max = (1.0f - rectangle.Left.Value, rectangle.Bottom.Value);

        return Rectangle.FromFloat(min.Item1, min.Item2,
            max.Item1 - min.Item1, max.Item2 - min.Item2);
    }

    private static Rectangle Rotate180(Rectangle rectangle)
    {
        (float, float) min = (1.0f - rectangle.Right.Value, 1.0f - rectangle.Bottom.Value);
        (float, float) max = (1.0f - rectangle.Left.Value, 1.0f - rectangle.Top.Value);

        return Rectangle.FromFloat(min.Item1, min.Item2,
            max.Item1 - min.Item1, max.Item2 - min.Item2);
    }

    private static Rectangle RotateCw90(Rectangle rectangle)
    {
        (float Value, float) min = (rectangle.Top.Value, 1.0f - rectangle.Right.Value);
        (float Value, float) max = (rectangle.Bottom.Value, 1.0f - rectangle.Left.Value);

        return Rectangle.FromFloat(min.Item1, min.Item2,
            max.Item1 - min.Item1, max.Item2 - min.Item2);
    }

    private static Rectangle RotateCw270(Rectangle rectangle)
    {
        (float, float Value) min = (1.0f - rectangle.Bottom.Value, rectangle.Left.Value);
        (float, float Value) max = (1.0f - rectangle.Top.Value, rectangle.Right.Value);

        return Rectangle.FromFloat(min.Item1, min.Item2,
            max.Item1 - min.Item1, max.Item2 - min.Item2);
    }

#endregion
}