using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTFLLab.Enums;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters;

namespace CSharpTFLLab.Classes
{
    internal struct Word
    {
        internal Word(StructScanEnum StructScanEnum, string str, int start, int end)
        {
            this.StructScanEnum = StructScanEnum;
            this.str = str;
            this.start = start;
            this.end = end;
        }
        internal StructScanEnum StructScanEnum;
        internal string str;
        internal int start;
        internal int end;
    }
    internal class SimpleScaner : IScaner
    {
        private string[,] _structScanDescription = {{ "", ""},
                                                    { "ключевое слово", "struct" },
                                                    { "ключевое слово", "int" },
                                                    { "ключевое слово", "bool" },
                                                    { "ключевое слово", "char" },
                                                    { "ключевое слово", "float" },
                                                    { "идентификатор", "" },
                                                    { "разделитель", " " },
                                                    { "открывающая скобка", "{" },
                                                    { "конец оператора", ";" },
                                                    { "закрывающая скобка", "}" }};
        char[] _spaceChars = { ' ', '\n', '\t' };
        char[] _specialChars = { '{', ';', '}' };
        internal SimpleScaner(MainForm form)
        {
            _form = form;
        }
        string _buffer;
        int _curLine = 0;
        int _curColumn = 0;
        int _curChr = 0;
        StructStepEnum _step = StructStepEnum.Search;
        MainForm _form;
        public string buffer { get=>_buffer;}

        public void Check()
        {
            Clear();
            Scan();
            //Filter();
            CheckProcess();
        }
        private Word GetNextWord()
        {
            int start = _curChr;
            while (_curChr < _buffer.Length && Char.IsLetterOrDigit(_buffer[_curChr]))
                _curChr++;
            if(start == _curChr)
            {
                if (_curChr == _buffer.Length)
                    return new Word(StructScanEnum.NONE, "", start, _curChr);
                else
                    _curChr += 1;
            }
            int end = _curChr;
            string subString = _buffer.Substring(start, end - start);
            _curColumn += subString.Length;
            if(subString.Length == 1 && !(Char.IsLetter(subString[0]) || _specialChars.Contains(subString[0])))
            {
                if (_spaceChars.Contains(subString[0]))
                {
                    if (subString[0] == '\n')
                    { 
                        _curLine++;
                        _curColumn = 0;
                    }
                    else
                        _curColumn++;
                    return new Word(StructScanEnum.Space, subString, start, end);
                }
                else
                    return new Word(StructScanEnum.ERROR, subString, start, end);
            }
            for (int i = 1; i < _structScanDescription.GetLength(0); i++)
            {
                if (i == 6) i++;
                if (subString == _structScanDescription[i, 1])
                    return new Word((StructScanEnum)i, subString, start, end);
            }
            return new Word(StructScanEnum.Identificatory, subString, start, end);
        }
        private void NextWordAndSkipSpace(ref Word word)
        {
            do
            {
                word = GetNextWord();
                OutputResult(word);
            } while (word.StructScanEnum == StructScanEnum.Space && (word.StructScanEnum != StructScanEnum.NONE || word.StructScanEnum == StructScanEnum.ERROR));
        }
        private void CheckProcess()
        {
            _step = StructStepEnum.Search;
            int levelDeep = 0;
            Word word;
            do
            {
                word = GetNextWord();   
                OutputResult(word);
            }while(word.StructScanEnum != StructScanEnum.NONE && word.StructScanEnum != StructScanEnum.ERROR);
            if (word.StructScanEnum == StructScanEnum.ERROR)
                OutputError("Ошибка ликсемы - " + word.str);
        }

        private void OutputResult(Word word)
        {
            if (word.StructScanEnum == StructScanEnum.ERROR || word.StructScanEnum == StructScanEnum.NONE)
                return;
            if (_form.OutputTextBox.Text.Length > 0)
                _form.OutputTextBox.AppendText("\n");
            if (word.str == " ")
                word.str = "(пробел)";
            else if (word.str == "\n")
                word.str = "(конец строки)";
            else if (word.str == "\t")
                word.str = "(табуляция)";
            _form.OutputTextBox.AppendText($"{(int)word.StructScanEnum} - { _structScanDescription[(int)word.StructScanEnum, 0]} - {word.str} - c {word.start} по {word.end} символ");
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
            throw new Exception("Ошибка проверки синтаксиса");
        }

        public void Clear()
        {
            _buffer = "";
            _curLine = 0;
            _curColumn = 0;
            _curChr = 0;
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
