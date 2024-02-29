using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTFLLab.Enums;
using System.Windows.Forms;

namespace CSharpTFLLab.Classes
{
    internal class SimpleScaner : IScaner
    {
        private string[,] StructScanDescription = { { "ошибка", ""},
                                                    { "ключевое слово", "struct" },
                                                    { "ключевое слово", "int" },
                                                    { "ключевое слово", "bool" },
                                                    { "ключевое слово", "char" },
                                                    { "ключевое слово", "float" },
                                                    { "идентификатор", "" },
                                                    { "разделитель", " " },
                                                    { "открывающая скобка", "{" },
                                                    { "конец оператора", ";" },
                                                    { "закрывающая скобка", "}" },};
        internal SimpleScaner(MainForm form)
        {
            _form = form;
        }
        string _buffer;
        int _curLine = 0;
        int _curColumn = 0;
        int _curChr = 0;
        MainForm _form;
        public string buffer { get=>_buffer;}

        public void Check()
        {
            Clear();
            Scan();
            //Filter();
            CheckProcess();
        }
        private (StructScanEnum StructScanEnum, string str, int start, int end) GetNextWord()
        {

        }
        private void CheckProcess()
        {
            
        }

        private void OutputResult(int code, string description, string str, int start, int end)
        {
            if (_form.OutputTextBox.Text.Length > 0)
                _form.OutputTextBox.AppendText("\n");
            if (str == " ")
                str = "(пробел)";
            _form.OutputTextBox.AppendText($"{code} - {description} - {str} - c {start} по {end} символ");
        }

        private void OutputError(string massage)
        {
            _form.LogDataGrid.Rows.Add();
            var row = _form.LogDataGrid.Rows[_form.LogDataGrid.Rows.Count - 1];
            row.Cells[0].Value = _form.LogDataGrid.Rows.Count - 1;
            row.Cells[1].Value = _form.fileManager.curFile;
            row.Cells[2].Value = _curLine;
            row.Cells[3].Value = _curColumn;
            row.Cells[4].Value = massage;
        }

        public void Clear()
        {
            _buffer = "";
            int _curLine = 0;
            int _curColumn = 0;
            int _curChr = 0;
            _form.LogDataGrid.Rows.Clear();
            _form.OutputTextBox.Clear();
        }

        public void Filter()
        {
            _buffer.Trim('\n');
            _buffer.Trim('\t');
        }

        public void Scan()
        {
            _buffer = (string)_form.InputTextBox.Text.Clone();
        }
    }
}
