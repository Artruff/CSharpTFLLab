using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTFLLab.Interfaces
{
    internal interface IScaner
    {
        string buffer { get; }
        void Scan();
        void Check();
        void Clear();
        void Filter();
    }
}
