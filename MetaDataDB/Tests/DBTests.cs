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
        public void OpenNotExistingDB()
        {
            Assert.Throws<InvalidProgramException>(() => Factory.CreateRead("ABC"));
        }

        [Test]
        public void ModeChange()
        {
            DB.EnableUnsafeMode();

            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(0));
            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));

            DB.EnableDefaultMode();

            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
        }

        [Test]
        public void AddMetaData()
        {
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(0));

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));

            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(2));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(3));

            var data = DBReadOnly.GetMetaData(TestData.ImageZero.FileName);
            AssertImageDataNotEqual(TestData.ImageZero, data);

            data = DBReadOnly.GetMetaData(TestData.Image1.FileName);
            AssertImageDataNotEqual(TestData.Image1, data);

            data = DBReadOnly.GetMetaData(TestData.Image2.FileName);
            AssertImageDataNotEqual(TestData.Image2, data);
        }

        [Test]
        public void RemoveMetaData()
        {
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(0));

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

            DB.RemoveMetaData(TestData.ImageZero.FileName);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(2));

            var data = DBReadOnly.GetMetaData(TestData.ImageZero.FileName);
            Assert.That(data, Is.EqualTo(null));

            data = DBReadOnly.GetMetaData(TestData.Image1.FileName);
            AssertImageDataNotEqual(TestData.Image1, data);

            data = DBReadOnly.GetMetaData(TestData.Image2.FileName);
            AssertImageDataNotEqual(TestData.Image2, data);
        }

        [Test]
        public void AddSameMetaData()
        {
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(0));

            var imageZero = DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            var image1 = DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            var image2 = DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);

            DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(imageZero, DBReadOnly.GetMetaData(TestData.ImageZero.FileName));

            DB.AddMetaData(TestData.Image1, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(image1, DBReadOnly.GetMetaData(TestData.Image1.FileName));

            DB.AddMetaData(TestData.Image2, DateTimeOffset.Now);
            Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(3));
            AssertImageDataNotEqual(image2, DBReadOnly.GetMetaData(TestData.Image2.FileName));
        }

        [Test]
        public void TestManyTransactions()
        {
            for (int i = 0; i < 1000; i++)
            {
                DB.AddMetaData(TestData.ImageZero, DateTimeOffset.Now);
                Assert.That(DBReadOnly.GetNumFiles(), Is.EqualTo(1));
            }
        }

    }
}