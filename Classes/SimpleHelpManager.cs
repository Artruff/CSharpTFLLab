using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTFLLab.Forms;
using CSharpTFLLab.Interfaces;

namespace CSharpTFLLab.Classes
{
    internal class SimpleHelpManager : IHelpManager
    {
        public void About()
        {
            FormAbout form = new FormAbout();
            form.ShowDialog();
        }

        public void Help()
        {
            throw new NotImplementedException();
        }
    }
}
