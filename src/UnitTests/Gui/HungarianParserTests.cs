﻿#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core.Types;
using Decompiler.Gui;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class HungarianParserTests
    {
        private HungarianParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new HungarianParser();
        }

        [Test]
        public void ParseEmpty()
        {
            DataType dt = parser.Parse("");
            Assert.IsNull(dt);
        }

        [Test]
        public void ParseI32()
        {
            var dt = parser.Parse("i32");
            Assert.AreEqual("int32", dt.ToString());
        }

        [Test]
        public void ParseBool()
        {
            var dt = parser.Parse("f");     // 'f' for 'flag'.
            Assert.AreEqual("bool", dt.ToString());
        }

        [Test]
        public void ParseArrayPrefix()
        {
            var dt = parser.Parse("ab");        // 'array of bytes' of unspecified length.
            Assert.AreEqual("(arr byte)", dt.ToString());
        }

        [Test]
        public void IncompleteArray_Fail()
        {
            var dt = parser.Parse("a");        // incomplete array, not a valid type
            Assert.IsNull(dt);
        }

        [Test]
        public void ParseChar()
        {
            var dt = parser.Parse("ch");        // 8-bit character
            Assert.AreEqual("char", dt.ToString());
        }

        [Test]
        public void ParseWideChar()
        {
            var dt = parser.Parse("wch");       // 16-bit character
            Assert.AreEqual("wchar_t", dt.ToString());
        }

        [Test]
        public void Parse8bit_c_string()
        {
            var dt = parser.Parse("sz");       // zero-terminated string
            Assert.AreEqual("(arr char)", dt.ToString());
        }

        [Test]
        public void Parse16bit_c_string()
        {
            var dt = parser.Parse("sz");       // zero-terminated string
            Assert.AreEqual("(arr char)", dt.ToString());
        }

        [Test]
        public void Parse_Pointer_To_Anything()
        {
            var dt = parser.Parse("p");
            Assert.AreEqual("ptr32", dt.ToString());
        }

        [Test]
        public void Parse_Pointer_To_Integer()
        {
            var dt = parser.Parse("pi16");
            Assert.AreEqual("(ptr int16)", dt.ToString());
        }

        [Test]
        public void Array_Pointers_To_Functions()
        {
            var dt = parser.Parse("apfn");
            Assert.AreEqual("(arr pfn32)", dt.ToString());
        }
    }
}
