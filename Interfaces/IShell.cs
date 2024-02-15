using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTFLLab.Interfaces
{
    internal interface IShell
    {
        void Execute(Action action);
    }
}
