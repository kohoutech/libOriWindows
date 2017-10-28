﻿/* ----------------------------------------------------------------------------
Origami Windows Library
Copyright (C) 1998-2017  George E Greaney

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Origami.Win32
{
    class Section
    {
        const uint IMAGE_SCN_CNT_CODE = 0x00000020;

        protected SourceFile source;

        public uint imageBase;
        public int secNum;
        public String sectionName;

        public uint memloc;                 //section addr in memory
        public uint memsize;                //section size in memory
        public uint fileloc;             //section addr in file
        public uint filesize;            //section size in file

        public uint pRelocations;
        public uint pLinenums;
        public int relocCount;
        public int linenumCount;

        public uint flags;

        public uint[] sourceBuf;

//using a factory method because we don't know what type of section it is until we read the section header
        public static Section getSection(SourceFile _source, int _secnum, uint _imageBase) 
        {
            SourceFile source = _source;
            String sectionName = source.getString(8);   
         
            uint memsize = source.getFour();
            uint memloc = source.getFour();
            uint filesize = source.getFour();
            uint fileloc = source.getFour();

            uint prelocations = source.getFour();
            uint plinenums = source.getFour();
            int reloccount = (int) source.getTwo();
            int linenumcount = (int) source.getTwo();
            uint flags = source.getFour();

            Section result;
            if ((flags & IMAGE_SCN_CNT_CODE) != 0)
            {
                result = new CodeSection(source, _secnum, sectionName, memsize, memloc,
                    filesize, fileloc, prelocations, plinenums, reloccount, linenumcount, flags, _imageBase);
            }
            else
            {
                result = new DataSection(source, _secnum, sectionName, memsize, memloc,
                    filesize, fileloc, prelocations, plinenums, reloccount, linenumcount, flags, _imageBase);
            }

            return result;
        }

        public Section(SourceFile _source, int _secnum, String _sectionName, uint _memsize, 
                uint _memloc, uint _filesize, uint _fileloc, uint _pRelocations, uint _pLinenums,
            int _relocCount, int _linenumCount, uint _flags, uint _imagebase)
        {
            source = _source;
            imageBase = _imagebase;
            secNum = _secnum;
            sectionName = _sectionName;
            memsize = _memsize;
            memloc = _memloc;
            filesize = _filesize;
            fileloc = _fileloc;
            pRelocations = _pRelocations;
            pLinenums = _pLinenums;
            relocCount = _relocCount;
            linenumCount = _linenumCount;
            flags = _flags;

            //Console.Out.WriteLine("[" + secNum + "] sectionName = " + sectionName);
            //Console.Out.WriteLine("[" + secNum + "] memsize = " + memsize.ToString("X"));
            //Console.Out.WriteLine("[" + secNum + "] memloc = " + memloc.ToString("X"));
            //Console.Out.WriteLine("[" + secNum + "] filesize = " + filesize.ToString("X"));
            //Console.Out.WriteLine("[" + secNum + "] fileloc = " + fileloc.ToString("X"));
            //Console.Out.WriteLine("[" + secNum + "] flags = " + flags.ToString("X"));

            sourceBuf = null;
        }

        public void loadSource() {

            sourceBuf = source.getRange(memloc, memsize);
        }
    }
}
