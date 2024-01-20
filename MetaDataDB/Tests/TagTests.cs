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
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.MetaDataDB.Tests
{
    [TestFixture]
    public class TagTests : DBSetup
    {
        [Test]
        public void AddFileTag()
        {
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            var data = DB.GetMetaData(TestData.Image1.FileName);

            data = Image.AddTag(data, "Hello1");

            DB.AddMetaData(data, DateTimeOffset.Now);
            AsserImageDataNotEqual(DB.GetMetaData(data.FileName), data);

            data = Image.AddTag(data, "Hello2");
            data = Image.AddTag(data, "Hello3");

            DB.AddMetaData(data, DateTimeOffset.Now);
            AsserImageDataNotEqual(DB.GetMetaData(data.FileName), data);
        }

        [Test]
        public void RemoveFileTag()
        {
            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            var data = DB.GetMetaData(TestData.Image2.FileName);

            data = Image.RemoveTag(data, "test2");
            DB.AddMetaData(data, DateTimeOffset.Now);
            AsserImageDataNotEqual(DB.GetMetaData(data.FileName), data);

            data = Image.RemoveTag(data, "test3");
            DB.AddMetaData(data, DateTimeOffset.Now);
            AsserImageDataNotEqual(DB.GetMetaData(data.FileName), data);

            data = Image.RemoveTag(data, "test1");
            DB.AddMetaData(data, DateTimeOffset.Now);
            AsserImageDataNotEqual(DB.GetMetaData(data.FileName), data);
        }

        [Test]
        public void GetNumTags()
        {
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            
            Assert.That(DB.GetNumTags(), Is.EqualTo(2));
            Assert.That(DB.GetAllTagsLike().Count, Is.EqualTo(2));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            Assert.That(DB.GetNumTags(), Is.EqualTo(3));
            Assert.That(DB.GetAllTagsLike().Count, Is.EqualTo(3));
        }

        [Test]
        public void GetTags()
        {
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);

            var tags = DB.GetAllTagsLike().OrderBy(s => s).ToArray();
            Assert.That(tags[0], Is.EqualTo("test1"));
            Assert.That(tags[1], Is.EqualTo("test2"));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            tags = DB.GetAllTagsLike().OrderBy(s => s).ToArray();
            Assert.That(tags[0], Is.EqualTo("test1"));
            Assert.That(tags[1], Is.EqualTo("test2"));
            Assert.That(tags[2], Is.EqualTo("test3"));
        }

    }
}