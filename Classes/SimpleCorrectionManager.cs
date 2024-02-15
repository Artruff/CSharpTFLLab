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
        List<string> history;
        MainForm form;
        String buffer;
        public SimpleCorrectionManager(MainForm form)
        {
            this.form = form;
            buffer = "";
            history = new List<string>();
        }

        public void Copy()
        {
            buffer = form.InputTextBox.SelectedText;
        }

        public void Cut()
        {
            Copy();
            Delete();
        }

        public void Delete()
        {
            form.InputTextBox.SelectedText.Remove(form.InputTextBox.SelectionStart, form.InputTextBox.SelectionLength);
        }

        public void Paste()
        {
            Delete();
            form.InputTextBox.Text = form.InputTextBox.Text.Insert(form.InputTextBox.SelectionStart, buffer);
        }

        public void Redo()
        {

        }

        public void SelectAll()
        {
            form.InputTextBox.SelectAll();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
