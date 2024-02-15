using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTFLLab.Interfaces
{
    internal interface IFileManager
    {
        void CreateFile();
        void OpenFile();
        void SaveFile();
        void SaveFileAs();
        void Exit();
    }
}
