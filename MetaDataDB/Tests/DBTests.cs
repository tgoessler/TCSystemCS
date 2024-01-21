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
using NUnit.Framework;

#endregion

namespace TCSystem.MetaDataDB.Tests
{
    [TestFixture]
    public sealed class DBTests : DBSetup
    {
        [Test]
        public void AddMetaData()
        {
            Assert.That(DB.GetNumFiles(), Is.EqualTo(0));

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(1));

            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(2));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(3));

            var data = DB.GetMetaData(TestData.ImageZero.FileName);
            AssertImageDataNotEqual(TestData.ImageZero, data);

            data = DB.GetMetaData(TestData.Image1.FileName);
            AssertImageDataNotEqual(TestData.Image1, data);

            data = DB.GetMetaData(TestData.Image2.FileName);
            AssertImageDataNotEqual(TestData.Image2, data);
        }

        [Test]
        public void RemoveMetaData()
        {
            Assert.That(DB.GetNumFiles(), Is.EqualTo(0));

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

            DB.RemoveMetaData(TestData.ImageZero.FileName);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(2));

            var data = DB.GetMetaData(TestData.ImageZero.FileName);
            Assert.That(data, Is.EqualTo(null));

            data = DB.GetMetaData(TestData.Image1.FileName);
            AssertImageDataNotEqual(TestData.Image1, data);

            data = DB.GetMetaData(TestData.Image2.FileName);
            AssertImageDataNotEqual(TestData.Image2, data);
        }

        [Test]
        public void AddSameMetaData()
        {
            Assert.That(DB.GetNumFiles(), Is.EqualTo(0));

            var imageZero = DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            var image1 = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            var image2 = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(imageZero, DB.GetMetaData(TestData.ImageZero.FileName));

            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(image1, DB.GetMetaData(TestData.Image1.FileName));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            Assert.That(DB.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(image2, DB.GetMetaData(TestData.Image2.FileName));
        }

    }
}