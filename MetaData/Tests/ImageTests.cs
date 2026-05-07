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
public class ImageTests
{
    [Test]
    public void AddPersonTagTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.PersonTags.Count, Is.EqualTo(1));

        // Adding a person with a different name should succeed
        Image updated = Image.AddPersonTag(image, TestData.PersonTag2);
        Assert.That(updated.PersonTags.Count, Is.EqualTo(2));
        Assert.That(updated.HasPersonTag(TestData.PersonTag2), Is.True);

        // Adding a person whose name already exists should be skipped
        Image updated2 = Image.AddPersonTag(updated, TestData.PersonTag1);
        Assert.That(updated2.PersonTags.Count, Is.EqualTo(2));

        // Adding an invalid person (empty name) is always allowed
        var invalidPersonTag = new PersonTag(new(Constants.InvalidId, "", "", "", ""), TestData.FaceZero);
        Image updated3 = Image.AddPersonTag(image, invalidPersonTag);
        Assert.That(updated3.PersonTags.Count, Is.EqualTo(2));
    }

    [Test]
    public void AddTagTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.NumTags, Is.EqualTo(1));

        // Adding a new tag should succeed
        Image updated = Image.AddTag(image, "NewTag");
        Assert.That(updated.NumTags, Is.EqualTo(2));
        Assert.That(updated.HasTag("NewTag"), Is.True);

        // Adding a duplicate tag should be a no-op
        Image updated2 = Image.AddTag(updated, "NewTag");
        Assert.That(updated2.NumTags, Is.EqualTo(2));

        // Adding an empty tag should be a no-op
        Image updated3 = Image.AddTag(image, "");
        Assert.That(updated3.NumTags, Is.EqualTo(1));
    }

    [Test]
    public void ChangeDateTakenTest()
    {
        Image image = CreateTestImage();
        
        var newDate = new DateTimeOffset(new(2022, 6, 15, 10, 30, 0, DateTimeKind.Local));
        Image updated = Image.ChangeDateTaken(image, newDate);
        Assert.That(updated.DateTaken.ToUnixTimeSeconds(), Is.EqualTo(newDate.ToUnixTimeSeconds()));
        Assert.That(updated.FileName, Is.EqualTo(image.FileName));
    }

    [Test]
    public void ChangeFileNameTest()
    {
        Image image = CreateTestImage();
        Image updated = Image.ChangeFileName(image, @"C:\New\Path\newfile.jpg");
        Assert.That(updated.FileName, Is.EqualTo(@"C:\New\Path\newfile.jpg"));
        Assert.That(updated.Id, Is.EqualTo(image.Id));
    }

    [Test]
    public void ChangeLocationTest()
    {
        Image image = CreateTestImage();
        Image updated = Image.ChangeLocation(image, TestData.Location2);
        Assert.That(updated.Location, Is.EqualTo(TestData.Location2));
        Assert.That(updated.FileName, Is.EqualTo(image.FileName));
    }

    [Test]
    public void ChangePersonTagsTest()
    {
        Image image = CreateTestImage();
        var newTags = new List<PersonTag> { TestData.PersonTag1, TestData.PersonTag2 };
        Image updated = Image.ChangePersonTags(image, newTags);
        Assert.That(updated.PersonTags.Count, Is.EqualTo(2));
        Assert.That(updated.PersonTags[0], Is.EqualTo(TestData.PersonTag1));
        Assert.That(updated.PersonTags[1], Is.EqualTo(TestData.PersonTag2));
    }

    [Test]
    public void ChangePersonTagVisibleTest()
    {
        Image image = CreateTestImage();
        PersonTag originalTag = image.PersonTags[0];

        Image updated = Image.ChangePersonTagVisible(image, originalTag, !originalTag.Face.Visible);
        PersonTag changedTag = updated.PersonTags.First(pt => pt.Person.Name == originalTag.Person.Name);
        Assert.That(changedTag.Face.Visible, Is.EqualTo(!originalTag.Face.Visible));

        // Changing visibility of a tag not in the image is a no-op
        Image unchanged = Image.ChangePersonTagVisible(image, TestData.PersonTag2, false);
        Assert.That(unchanged.PersonTags.Count, Is.EqualTo(image.PersonTags.Count));
    }

    [Test]
    public void ChangeProcessingInfoTest()
    {
        Image image = CreateTestImage();
        Image updated = Image.ChangeProcessingInfo(image, ProcessingInfos.DlibFrontalFaceDetection2000);
        Assert.That(updated.ProcessingInfos, Is.EqualTo(ProcessingInfos.DlibFrontalFaceDetection2000));
        Assert.That(updated.FileName, Is.EqualTo(image.FileName));
    }

    [Test]
    public void ChangeTagsTest()
    {
        Image image = CreateTestImage();
        var newTags = new List<string> { "Tag1", "Tag2", "Tag3" };
        Image updated = Image.ChangeTags(image, newTags);
        Assert.That(updated.NumTags, Is.EqualTo(3));
        Assert.That(updated.HasTag("Tag1"), Is.True);
        Assert.That(updated.HasTag("Tag2"), Is.True);
        Assert.That(updated.HasTag("Tag3"), Is.True);
    }

    [Test]
    public void EqualsTest()
    {
        Image image1 = CreateTestImage();
        Image image2 = CreateTestImage();

        Assert.That(image1.Equals(image1), Is.True);
        Assert.That(image1.Equals(image2), Is.True);
        Assert.That(image1.Equals(null), Is.False);
        Assert.That(image1, Is.Not.EqualTo(string.Empty));

        Image different = Image.ChangeFileName(image1, "other.jpg");
        Assert.That(image1.Equals(different), Is.False);
    }

    [Test]
    public void FromJsonStringTest()
    {
        Image image = CreateTestImage();

        string ToJson(Image d) => d.ToJsonString();
        Image FromJson(string s) => Image.FromJsonString(s);

        TestUtil.FromJsonStringTest(image, ToJson, FromJson);
    }

    [Test]
    public void GetHashCodeTest()
    {
        Image image1 = CreateTestImage();

        // Same instance always returns same hash
        Assert.That(image1.GetHashCode(), Is.EqualTo(image1.GetHashCode()));

        // Different images should have different hashes
        Image different = Image.ChangeFileName(image1, "other.jpg");
        Assert.That(image1.GetHashCode(), Is.Not.EqualTo(different.GetHashCode()));
    }

    [Test]
    public void GetPersonTagTest()
    {
        Image image = CreateTestImage();
        PersonTag found = image.GetPersonTag(TestData.Person1.Name);
        Assert.That(found, Is.EqualTo(TestData.PersonTag1));

        PersonTag notFound = image.GetPersonTag("NoSuchPerson");
        Assert.That(notFound, Is.Null);
    }

    [Test]
    public void HasPersonTagTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.HasPersonTag(TestData.PersonTag1), Is.True);
        Assert.That(image.HasPersonTag(TestData.PersonTag2), Is.False);
    }

    [Test]
    public void HasPersonTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.HasPerson(TestData.Person1.Name), Is.True);
        Assert.That(image.HasPerson("NoSuchPerson"), Is.False);
    }

    [Test]
    public void HasTagTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.HasTag("TestTag"), Is.True);
        Assert.That(image.HasTag("NonExistentTag"), Is.False);
    }

    [Test]
    public void ImageTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.Id, Is.EqualTo(42));
        Assert.That(image.FileName, Is.EqualTo(@"C:\photos\test.jpg"));
        Assert.That(image.ProcessingInfos, Is.EqualTo(ProcessingInfos.None));
        Assert.That(image.Width, Is.EqualTo(1920));
        Assert.That(image.Height, Is.EqualTo(1080));
        Assert.That(image.Orientation, Is.EqualTo(OrientationMode.Normal));
        Assert.That(image.Title, Is.EqualTo(""));
        Assert.That(image.Location, Is.EqualTo(TestData.Location1));
        Assert.That(image.PersonTags.Count, Is.EqualTo(1));
        Assert.That(image.NumTags, Is.EqualTo(1));
    }

    [Test]
    public void InvalidateIdsTest()
    {
        Image image = CreateTestImage();
        Image invalidated = image.InvalidateId();

        Assert.That(invalidated.Id, Is.EqualTo(Constants.InvalidId));
        Assert.That(invalidated.FileName, Is.EqualTo(image.FileName));
        foreach (PersonTag pt in invalidated.PersonTags)
        {
            Assert.That(pt.Person.Id, Is.EqualTo(Constants.InvalidId));
            Assert.That(pt.Face.Id, Is.EqualTo(Constants.InvalidId));
        }
    }

    [Test]
    public void RemovePersonTagTest()
    {
        Image image = CreateTestImage();
        Assert.That(image.PersonTags.Count, Is.EqualTo(1));

        Image updated = Image.RemovePersonTag(image, TestData.PersonTag1);
        Assert.That(updated.PersonTags.Count, Is.EqualTo(0));

        // Removing a non-existent tag is a no-op
        Image unchanged = Image.RemovePersonTag(image, TestData.PersonTag2);
        Assert.That(unchanged.PersonTags.Count, Is.EqualTo(1));
    }

    [Test]
    public void RemovePersonWithNameTest()
    {
        Image image = CreateTestImage();
        Image updated = Image.RemovePersonWithName(image, TestData.Person1.Name);
        Assert.That(updated.PersonTags.Count, Is.EqualTo(0));

        // Removing a non-existent name is a no-op
        Image unchanged = Image.RemovePersonWithName(image, "NoSuchPerson");
        Assert.That(unchanged.PersonTags.Count, Is.EqualTo(1));
    }

    [Test]
    public void RemoveTagTest()
    {
        Image image = CreateTestImage();
        Image updated = Image.RemoveTag(image, "TestTag");
        Assert.That(updated.NumTags, Is.EqualTo(0));
        Assert.That(updated.HasTag("TestTag"), Is.False);

        // Removing a non-existent tag is a no-op
        Image unchanged = Image.RemoveTag(image, "NonExistentTag");
        Assert.That(unchanged.NumTags, Is.EqualTo(1));
    }

    [Test]
    public void ToJsonStringTest()
    {
        Image image = CreateTestImage();
        string json = image.ToJsonString();
        Assert.That(json, Is.Not.Empty);
    }

    [Test]
    public void ToStringTest()
    {
        Image image = CreateTestImage();
        string str = image.ToString();
        Assert.That(str, Is.Not.Empty);
    }

    private static Image CreateTestImage()
    {
        return new(42, @"C:\photos\test.jpg", ProcessingInfos.None,
            1920, 1080, OrientationMode.Normal,
            new(2021, 5, 1, 12, 0, 0, TimeSpan.Zero),
            null,
            TestData.Location1,
            new[] { TestData.PersonTag1 },
            new[] { "TestTag" });
    }
}