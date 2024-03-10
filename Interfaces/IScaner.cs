using CSharpTFLLab.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTFLLab.Interfaces
{
    internal struct Word
    {
        internal Word(StructScanEnum StructScanEnum, string str, int start, int end)
        {
            this.StructScanEnum = StructScanEnum;
            this.str = str;
            this.start = start;
            this.end = end;
        }
        internal StructScanEnum StructScanEnum;
        internal string str;
        internal int start;
        internal int end;
    }
    internal interface IScaner
    {
        string buffer { get; }
        Word curWord { get; }
        bool GetNextWord();
        void OutputError(string massage);
        bool NextWordAndSkipSpace();
        bool Checking();
        void Scan();
        void Check();
        void Clear();
        void Filter();
    }
}
