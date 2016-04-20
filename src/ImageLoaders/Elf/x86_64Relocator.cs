﻿#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class x86_64Relocator : ElfRelocator
    {
        private ElfLoader64 loader;
        private Dictionary<Address, ImportReference> importReferences;

        public x86_64Relocator(ElfLoader64 loader)
        {
            this.loader = loader;
        }

        /// <remarks>
        /// According to the ELF x86_64 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public override void Relocate(Program program)
        {
            this.importReferences = program.ImportReferences;

            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            var plt = loader.GetSectionInfoByName(".plt");
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            for (ulong i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                ulong offset;
                if (!relaRdr.TryReadUInt64(out offset))
                    return;
                ulong info;
                if (!relaRdr.TryReadUInt64(out info))
                    return;
                long addend;
                if (!relaRdr.TryReadInt64(out addend))
                    return;

                ulong sym = info >> 32;
                string symStr = loader.GetSymbol64(rela_plt.LinkedSection, sym);

                var addr = plt.Address + (uint)(i + 1) * plt.EntrySize;
                importReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }
        }

        public override void RelocateEntry(List<ElfSymbol> symbols, ElfSection referringSection, Elf32_Rela rela)
        {
            throw new NotImplementedException();
        }
    }
}