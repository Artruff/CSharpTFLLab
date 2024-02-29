using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public void IncreaseFontIOText()
        {
            ChangeFont(form.InputTextBox, 1.2f);
            ChangeFont(form.OutputTextBox, 1.2f);
        }
        public void DecreaseFontIOText()
        {
            ChangeFont(form.InputTextBox, 10f / 12f);
            ChangeFont(form.OutputTextBox, 10f / 12f);
        }
        void ChangeFont(RichTextBox richTextBox, float modify)
        {
            richTextBox.Select(0, richTextBox.Text.Length);

            Font currentFont = richTextBox.SelectionFont;
            float newSize = currentFont.Size * modify;
            richTextBox.SelectionFont = new Font(currentFont.FontFamily, newSize);
        }
    }
}
