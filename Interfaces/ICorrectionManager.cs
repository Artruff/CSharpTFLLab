using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpTFLLab.Interfaces
{
    internal interface ICorrectionManager
    {
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        void Delete();
        void SelectAll();
        void IncreaseFontIOText();
        void DecreaseFontIOText();
    }
}
