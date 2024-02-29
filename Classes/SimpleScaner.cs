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
        private string[,] _structScanDescription = { { "ошибка", ""},
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
        char [] _unicChr = { ' ', '{', '}', ';', '\n', '\t'};
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
        private (StructScanEnum StructScanEnum, string str, int start, int end) GetNextWord()
        {
            if(_curChr >= _buffer.Length)
                return (StructScanEnum.ERROR, "", _buffer.Length, _buffer.Length);
            int start = _curChr;
            int end =buffer.IndexOfAny(_unicChr, _curChr);
            if(end == -1)
                end = _buffer.Length;
            if (start == end)
                end++;
            _curChr = end;
            _curColumn += end - start;
            string subString = buffer.Substring(start, _curChr - start);
            //_curChr++;
            int newLine = subString.Count(x => x == '\n');
            if(newLine != 0)
            {
                _curLine += newLine;
                _curColumn = end - start;
            }
            subString = subString.Trim(new char[]{ '\n', '\t' });
            int code = 0;
            //Если это Уникальный символ
            if(subString.Length == 1)
            {
                code = 10;
                for (; code >= 7; code--)
                    if (_structScanDescription[code, 1] == subString)
                        break;
            }//Если это длинное слово
            else
            {
                code = 1;
                //Ищем ключевое слово
                for(; code <6; code++)
                    if (_structScanDescription[code, 1] == subString)
                        break;
                //Иначе - идентификатор
            }
            return ((StructScanEnum)code, subString, start, end);
        }
        private void SkipSpace(ref (StructScanEnum StructScanEnum, string str, int start, int end) word)
        {
            while (word.StructScanEnum == StructScanEnum.Space && _curChr < _buffer.Length)
            {
                OutputResult((int)word.StructScanEnum, _structScanDescription[(int)word.StructScanEnum, 0], word.str, word.start, word.end);
                word = GetNextWord();
            }
        }
        private void CheckProcess()
        {
            _step = StructStepEnum.Search;
            int levelDeep = 0;
            while(_curChr < _buffer.Length)
            {
                (StructScanEnum StructScanEnum, string str, int start, int end) word = GetNextWord();
                SkipSpace(ref word);
                switch (_step)
                {
                    case StructStepEnum.Search:
                        if (word.StructScanEnum == StructScanEnum.Struct)
                        {
                            _step = StructStepEnum.KeywordStructure;
                        }
                        else if (((int)word.StructScanEnum) >= 2 && ((int)word.StructScanEnum) <= 5)
                        {
                            _step = StructStepEnum.KeywordType;
                        }
                        else if(levelDeep -1 >= 0 && word.StructScanEnum == StructScanEnum.CloseBracket)
                        {
                            _step = StructStepEnum.CloseBracket;
                            levelDeep--;
                        }
                        else if (word.StructScanEnum == StructScanEnum.Identificatory)
                            OutputError("Работа с переменными пока не реализована");
                        else
                            OutputError("Ожидается ключевое слово"); 
                        break;
                    case StructStepEnum.KeywordStructure:
                        if (word.StructScanEnum != StructScanEnum.Identificatory)
                            OutputError("Ожидается идентификатор структуры");
                        else
                        {
                            OutputResult((int)word.StructScanEnum, _structScanDescription[(int)word.StructScanEnum, 0], word.str, word.start, word.end);
                            SkipSpace(ref word); 
                            word = GetNextWord();
                            if (word.StructScanEnum == StructScanEnum.OpenBracket)
                            {
                                _step = StructStepEnum.Search;
                                levelDeep++;
                            }
                            else
                                OutputError("Ожидается открывающая скобка");
                        }
                        break;
                    case StructStepEnum.KeywordType:
                        if (word.StructScanEnum != StructScanEnum.Identificatory)
                            OutputError("Ожидается идентификатор переменной");
                        else
                        {
                            OutputResult((int)word.StructScanEnum, _structScanDescription[(int)word.StructScanEnum, 0], word.str, word.start, word.end);
                            SkipSpace(ref word);
                            word = GetNextWord();
                            if (word.StructScanEnum == StructScanEnum.EndOperator)
                                _step = StructStepEnum.Search;
                        }
                        break;
                    case StructStepEnum.CloseBracket:
                        if (word.StructScanEnum != StructScanEnum.EndOperator)
                            OutputError("Ожидается завершающий оператор ;");
                        else
                        {
                            levelDeep--;
                            _step = StructStepEnum.Search;
                        }
                        break;
                    default:
                        OutputError("Недопустимая лексема");
                        break;
                }
                OutputResult((int)word.StructScanEnum, _structScanDescription[(int)word.StructScanEnum, 0], word.str, word.start, word.end);
            }
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
            row.Cells[3].Value = _curChr;
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
