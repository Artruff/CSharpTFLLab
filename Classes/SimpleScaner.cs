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
    
    internal class SimpleScaner : IScaner
    {
        internal Word _curWord;
        public Word curWord { get => _curWord; }
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
            Checking();
        }
        public bool GetNextWord()
        {
            int start = _curChr;
            while (_curChr < _buffer.Length && Char.IsLetterOrDigit(_buffer[_curChr]))
                _curChr++;
            if(start == _curChr)
            {
                if (_curChr == _buffer.Length)
                {
                    _curWord = new Word(StructScanEnum.NONE, "", start, _curChr);
                    return false;
                }
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
                    _curWord = new Word(StructScanEnum.Space, subString, start, end);
                    return true;
                }
                else
                {
                    _curWord = new Word(StructScanEnum.ERROR, subString, start, end);
                    return false;
                }
            }
            for (int i = 1; i < _structScanDescription.GetLength(0); i++)
            {
                if (i == 6) i++;
                if (subString == _structScanDescription[i, 1])
                {
                    _curWord = new Word((StructScanEnum)i, subString, start, end);
                    return true;
                }
            }
            _curWord = new Word(StructScanEnum.Identificatory, subString, start, end);
            return true;
        }
        public bool NextWordAndSkipSpace()
        {
            do
            {
                GetNextWord();
                if (_curWord.StructScanEnum == StructScanEnum.NONE || _curWord.StructScanEnum == StructScanEnum.ERROR)
                    return false;
                else if(_curWord.StructScanEnum != StructScanEnum.Space)
                    return true;
            } while (true);
        }
        public bool Checking()
        {
            _step = StructStepEnum.Search;
            do
            {
                GetNextWord();   
                OutputCurWord();
            }while(_curWord.StructScanEnum != StructScanEnum.NONE && _curWord.StructScanEnum != StructScanEnum.ERROR);
            if (_curWord.StructScanEnum == StructScanEnum.ERROR)
            {
                OutputError("Ошибка ликсемы - " + _curWord.str);
                return false;
            }
            return true;
        }

        public void OutputCurWord()
        {
            if (_curWord.StructScanEnum == StructScanEnum.ERROR || _curWord.StructScanEnum == StructScanEnum.NONE)
                return;
            if (_form.OutputTextBox.Text.Length > 0)
                _form.OutputTextBox.AppendText("\n");
            if (_curWord.str == " ")
                _curWord.str = "(пробел)";
            else if (_curWord.str == "\n")
                _curWord.str = "(конец строки)";
            else if (_curWord.str == "\t")
                _curWord.str = "(табуляция)";
            _form.OutputTextBox.AppendText($"{(int)_curWord.StructScanEnum} - { _structScanDescription[(int)_curWord.StructScanEnum, 0]} - {_curWord.str} - c {_curWord.start} по {_curWord.end} символ");
        }

        public void OutputError(string massage)
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
