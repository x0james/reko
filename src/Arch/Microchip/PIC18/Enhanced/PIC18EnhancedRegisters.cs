﻿#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    public class PIC18EnhancedRegisters : PIC18Registers
    {
        private static HashSet<PICRegisterStorage> invalidMovfflDests
            = new HashSet<PICRegisterStorage>();

        private PIC18EnhancedRegisters()
        {
        }

        /// <summary>
        /// Creates the Enhanced PIC18 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(PIC pic)
        {
            PICRegisters.LoadRegisters(pic ?? throw new ArgumentNullException(nameof(pic)));
            new PIC18EnhancedRegisters().SetCoreRegisters();
        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        public override void SetCoreRegisters()
        {
            base.SetCoreRegisters();

            // Some registers are invalid memory destination for some instructions.

            invalidMovfflDests.Clear();
            invalidMovfflDests.Add(PCL);
            invalidMovfflDests.Add(TOSL);
            invalidMovfflDests.Add(TOSH);
            invalidMovfflDests.Add(TOSU);

        }

        /// <summary>
        /// Query if data memory absolute address corresponds to one of the MOVFFL forbidden destinations (per PIC data sheet)..
        /// </summary>
        /// <param name="dstaddr">The data memory absolute address.</param>
        /// <returns>
        /// True if forbidden, false allowed.
        /// </returns>
        public static bool NotAllowedMovlDest(ushort dstaddr)
        {
            if (PICRegisters.TryGetRegister(dstaddr, 8, out var fsr))
                return invalidMovfflDests.Contains(fsr);
            return false;
        }

    }

}
