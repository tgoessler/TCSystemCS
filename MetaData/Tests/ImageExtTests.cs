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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

#endregion

namespace TCSystem.MetaData.Tests;

[TestFixture]
public class ImageExtTests
{
    [Test]
    public void InvalidateId_Image_SetsInvalidId()
    {
        var image = new Image(1, "test.jpg", ProcessingInfos.None,
            100, 200, OrientationMode.Normal,
            DateTimeOffset.Now, "title", TestData.Location1,
            new[] { TestData.PersonTag1 }, new[] { "tag1" });

        Image invalidated = image.InvalidateId();

        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.FileName, Is.EqualTo("test.jpg"));
        Assert.That(invalidated.Width, Is.EqualTo(100));
        Assert.That(invalidated.Height, Is.EqualTo(200));
    }

    [Test]
    public void InvalidateId_Image_InvalidatesPersonTagIds()
    {
        var image = new Image(1, "test.jpg", ProcessingInfos.None,
            100, 200, OrientationMode.Normal,
            DateTimeOffset.Now, "title", TestData.Location1,
            new[] { TestData.PersonTag1 }, new[] { "tag1" });

        Image invalidated = image.InvalidateId();

        Assert.That(invalidated.PersonTags[0].Person.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.PersonTags[0].Face.Id, Is.EqualTo(Constants.InvalidId));
    }

    [Test]
    public void InvalidateId_Person_SetsInvalidId()
    {
        Person invalidated = TestData.Person1.InvalidateId();

        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Name, Is.EqualTo(TestData.Person1.Name));
        Assert.That(invalidated.EmailDigest, Is.EqualTo(TestData.Person1.EmailDigest));
        Assert.That(invalidated.LiveId, Is.EqualTo(TestData.Person1.LiveId));
        Assert.That(invalidated.SourceId, Is.EqualTo(TestData.Person1.SourceId));
    }

    [Test]
    public void InvalidateId_Face_SetsInvalidId()
    {
        Face invalidated = TestData.Face1.InvalidateId();

        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Rectangle, Is.EqualTo(TestData.Face1.Rectangle));
        Assert.That(invalidated.FaceMode, Is.EqualTo(TestData.Face1.FaceMode));
        Assert.That(invalidated.FaceQuality, Is.EqualTo(TestData.Face1.FaceQuality));
        Assert.That(invalidated.Visible, Is.EqualTo(TestData.Face1.Visible));
    }

    [Test]
    public void InvalidateId_PersonTag_InvalidatesBothIds()
    {
        PersonTag invalidated = TestData.PersonTag1.InvalidateId();

        Assert.That(invalidated.Person.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Face.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.Person.Name, Is.EqualTo(TestData.PersonTag1.Person.Name));
    }

    [Test]
    public void GetFace_ValidFaceId_ReturnsFace()
    {
        PersonTag[] personTags = [TestData.PersonTag1, TestData.PersonTag2];

        Face result = personTags.GetFace(TestData.Face1.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(TestData.Face1.Id));
    }

    [Test]
    public void GetFace_InvalidFaceId_ReturnsNull()
    {
        PersonTag[] personTags = [TestData.PersonTag1, TestData.PersonTag2];

        Face result = personTags.GetFace(Constants.InvalidId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetFace_NonExistentFaceId_ReturnsNull()
    {
        PersonTag[] personTags = [TestData.PersonTag1, TestData.PersonTag2];

        Face result = personTags.GetFace(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetFace_NullCollection_ReturnsNull()
    {
        Face result = ((IEnumerable<PersonTag>)null).GetFace(1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetFace_EmptyCollection_ReturnsNull()
    {
        Face result = Array.Empty<PersonTag>().GetFace(1);

        Assert.That(result, Is.Null);
    }
}
