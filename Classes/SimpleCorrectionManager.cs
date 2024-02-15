using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTFLLab.Interfaces;

namespace CSharpTFLLab.Classes
{
    internal class SimpleCorrectionManager : ICorrectionManager
    {
        MainForm form;
        public SimpleCorrectionManager(MainForm form)
        {
            this.form = form;
        }

        public void Copy()
        {
           form.InputTextBox.Copy();
        }

        public void Cut()
        {
            form.InputTextBox.Cut();
        }

        public void Delete()
        {
            form.InputTextBox.Text = form.InputTextBox.Text.Remove(form.InputTextBox.SelectionStart, form.InputTextBox.SelectionLength);
        }

        public void Paste()
        {
            form.InputTextBox.Paste();
        }

        public void Redo()
        {
            form.InputTextBox.Redo();
        }

        public void SelectAll()
        {
            form.InputTextBox.SelectAll();
        }

        public void Undo()
        {
            form.InputTextBox.Undo();
        }
    }
}
