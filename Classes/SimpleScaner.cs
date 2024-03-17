using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTFLLab.Enums;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters;
using System.Collections;

namespace CSharpTFLLab.Classes
{
    
    internal class SimpleScaner : IScaner
    {
        protected List<Word> _words = new List<Word>();
        internal int _iCurWord;
        public int iCurWord { set => _iCurWord = value >= _words.Count+2 ? _words.Count + 1 : value < 0 ? 0 : value; get => _iCurWord; }
        public Word curWord { get => _iCurWord == 0 || _iCurWord == _words.Count + 1 ? new Word(StructScanEnum.NONE, "", 0, 0) : _words[_iCurWord-1]; set { if (_iCurWord == 0 || _iCurWord == _words.Count + 1) _words[_iCurWord - 1] = value; } }
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
            _words = new List<Word>();
            _iCurWord = -1;
        }
        string _buffer;
        int _curLine = 0;
        int _curColumn = 0;
        int _curChr = 0;
        MainForm _form;
        public string buffer { get=>_buffer;}

        public int Count => _words.Count+2;

        public bool IsReadOnly => false;

        public Word this[int index] {
            get {
                if(index <=0 || index >= _words.Count+1) return new Word(StructScanEnum.NONE, "", 0, 0);
                index = index < 1 ? 0 : index >= _words.Count + 1 ? _words.Count - 1 : index - 1;
                return _words[index];
            }
            set {
                index = index < 1 ? 0 : index >= _words.Count + 1 ? _words.Count - 1 : index - 1;
                _words[index] = value;
            }
        }

        public void Check()
        {
            Clear();
            Scan();
            Checking();
        }
        public bool ScanNextWord()
        {
            int start = _curChr;
            while (_curChr < _buffer.Length && Char.IsLetterOrDigit(_buffer[_curChr]))
                _curChr++;
            if(start == _curChr)
            {
                if (_curChr == _buffer.Length)
                {
                    _words.Add(new Word(StructScanEnum.NONE, "", start, _curChr));
                    iCurWord++;
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
                    _words.Add(new Word(StructScanEnum.Space, subString, start, end));
                    iCurWord++;
                    return true;
                }
                else
                {
                    _words.Add(new Word(StructScanEnum.ERROR, subString, start, end));
                    iCurWord++;
                    return false;
                }
            }
            for (int i = 1; i < _structScanDescription.GetLength(0); i++)
            {
                if (i == 6) i++;
                if (subString == _structScanDescription[i, 1])
                {
                    _words.Add(new Word((StructScanEnum)i, subString, start, end));
                    iCurWord++;
                    return true;
                }
            }
            _words.Add(new Word(StructScanEnum.Identificatory, subString, start, end));
            iCurWord++;
            return true;
        }
        public bool NextWord() => iCurWord != ++iCurWord;
        public bool PrevWord() => iCurWord != --iCurWord;
        public bool NextWordAndSkipSpace()
        {
            do
            {
                NextWord();
                if (curWord.StructScanEnum == StructScanEnum.NONE || curWord.StructScanEnum == StructScanEnum.ERROR)
                    return false;
                else if(curWord.StructScanEnum != StructScanEnum.Space)
                    return true;
            } while (true);
        }

        public bool PrevWordAndSkipSpace()
        {
            do
            {
                PrevWord();
                if (curWord.StructScanEnum == StructScanEnum.NONE || curWord.StructScanEnum == StructScanEnum.ERROR)
                    return false;
                else if (curWord.StructScanEnum != StructScanEnum.Space)
                    return true;
            } while (true);
        }

        public bool Checking()
        {
            do
            {
                NextWord();   
                OutputCurWord();
            }while(curWord.StructScanEnum != StructScanEnum.NONE && curWord.StructScanEnum != StructScanEnum.ERROR);
            if (curWord.StructScanEnum == StructScanEnum.ERROR)
            {
                OutputError("Ошибка ликсемы - " + curWord.str);
                return false;
            }
            return true;
        }

        public void OutputCurWord()
        {
            if (curWord.StructScanEnum == StructScanEnum.ERROR || curWord.StructScanEnum == StructScanEnum.NONE)
                return;
            if (_form.OutputTextBox.Text.Length > 0)
                _form.OutputTextBox.AppendText("\n");
            Word word = curWord;
            if (curWord.str == " ")
                word.str = "(пробел)";
            else if (word.str == "\n")
                word.str = "(конец строки)";
            else if (word.str == "\t")
                word.str = "(табуляция)";
            curWord = word;
            _form.OutputTextBox.AppendText($"{(int)curWord.StructScanEnum} - { _structScanDescription[(int)curWord.StructScanEnum, 0]} - {curWord.str} - c {curWord.start} по {curWord.end} символ");
        }

        public void OutputError(string massage)
        {
            var row = _form.LogDataGrid.Rows[_form.LogDataGrid.Rows.Add()];
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
            _words.Clear();
            iCurWord = 0;
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
            while (ScanNextWord()) ;
            _curChr = 0;
            _curLine = 0;
            _curColumn = 0;
            iCurWord = 0;
        }
        public void Rebuild()
        {
            _form.InputTextBox.Text = "";
            foreach (Word word in _words)
                _form.InputTextBox.AppendText(word.str);
        }

        public int IndexOf(Word item)
        {
            return _words.IndexOf(item);
        }

        public void Insert(int index, Word item)
        {
            index = index < 1 ? 0 : index>= _words.Count+1 ? _words.Count - 1 : index - 1;
            _words.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            iCurWord = iCurWord > index ? iCurWord-1 : iCurWord;
            index = index < 1 ? 0 : index >= _words.Count + 1 ? _words.Count - 1 : index - 1;
            _words.RemoveAt(index);
        }

        public void Add(Word item)
        {
            _words.Add(item);
        }

        public bool Contains(Word item)
        {
            return item.StructScanEnum == StructScanEnum.NONE || _words.Contains(item);
        }

        public void CopyTo(Word[] array, int arrayIndex)
        {
            _words.CopyTo(array, arrayIndex);
        }

        public bool Remove(Word item)
        {
            return _words.Remove(item);
        }

        public IEnumerator<Word> GetEnumerator()
        {
            return _words.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
