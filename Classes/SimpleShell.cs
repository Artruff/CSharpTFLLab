using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpTFLLab.Classes
{
    internal class SimpleShell : IShell
    {
        public void Execute(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
