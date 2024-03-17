using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpTFLLab.Enums;
using System.Runtime.ConstrainedExecution;

namespace CSharpTFLLab.Classes
{
    internal class SimpleParser : IParser
    {
        IScaner _scaner;
        Action<string> output;
        Action<string> error;
        Action clear;
        List<CStruct> structs;
        public SimpleParser(IScaner scaner, RichTextBox outputBox, DataGridView errorTable)
        {
            _scaner = scaner;
            output += outputBox.AppendText;
            error += scaner.OutputError;
            structs = new List<CStruct>();
            clear += () => { 
                outputBox.Clear();
                errorTable.Rows.Clear();
                structs.Clear();
            };
        }

        public void Output()
        {
            if (output != null)
                for(int i = 0; i < structs.Count; i++)
                {
                    output("Была создана структура "+structs[i].name+"\n" +
                        "Список полей структуры:\n");
                    var enumerator = structs[i].GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        output("\t"+enumerator.Current.Key + " - " + enumerator.Current.Value.Name + "\n");
                    }
                }
        }

        private bool NextWord(StructScanEnum type)
        {
            return _scaner.NextWordAndSkipSpace() && _scaner.curWord.StructScanEnum == type;
        }
        private bool NextWordWithError(StructScanEnum type)
        {
            while (_scaner.NextWordAndSkipSpace() && _scaner.curWord.StructScanEnum != type)
            {
                _scaner.RemoveAt(_scaner.iCurWord);
                _scaner.PrevWord();
            };
            return _scaner.curWord.StructScanEnum == type;
        }
        public void Parse()
        {
            if(clear != null)
                clear();

            _scaner.Clear();
            _scaner.Scan();
            bool result = true;
            int errCount = 0;
            bool errorCheck = false;
            if (_scaner.Checking())
            {
                _scaner.Clear();
                _scaner.Scan();

                while (_scaner.NextWordAndSkipSpace())
                {
                    result = _scaner.curWord.StructScanEnum == StructScanEnum.Struct;
                    if (result)
                    {
                        string name = null;
                        result = NewCStruct(ref name);
                        if (!result)
                        {
                            errCount++;
                            if(!errorCheck)
                            {
                                DialogResult res = MessageBox.Show("Обнаружены ошибки. Исправить?", "Исправить ошибки?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (res == DialogResult.Yes)
                                {
                                    ParseWithError();
                                    return;
                                }
                                else if (res == DialogResult.No)
                                {
                                    errorCheck = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        error("ожидается ключевое слово struct");
                        errCount++;
                    }
                }
                if(errCount!=0)
                    output($"Всего ошибок: {errCount}");
            }
            if(result)
                Output();
            else
                throw new Exception("ошибка парсинга");
        }
        private bool NewCStruct(ref string nameStruct)
        {
            if (NextWord(StructScanEnum.Identificatory))
            {
                if(nameStruct != null)
                    nameStruct = _scaner.curWord.str;
                CStruct cStruct = new CStruct(_scaner.curWord.str);
                structs.Add(cStruct);
                if (NextWord(StructScanEnum.OpenBracket))
                {
                    if (_scaner.NextWordAndSkipSpace())
                    {
                        while(_scaner.curWord.StructScanEnum != StructScanEnum.CloseBracket)
                        {
                            Type type;
                            string nameElement = "";
                            switch (_scaner.curWord.StructScanEnum)
                            {
                                case StructScanEnum.Struct:
                                    if(!NewCStruct(ref nameElement))
                                        return false;
                                    type = typeof(CStruct);
                                    break;
                                case StructScanEnum.Int:
                                    type = typeof(int);
                                    break;
                                case StructScanEnum.Float:
                                    type = typeof(float); 
                                    break;
                                case StructScanEnum.Char:
                                    type = typeof(char);
                                    break;
                                case StructScanEnum.Bool:
                                    type = typeof(bool);
                                    break;
                                default:
                                    error("Ожидается закрывающая скобка или объявление переменных структуры");
                                    return false;
                            }
                            if (type == typeof(CStruct))
                            {
                                cStruct.Add(nameElement, type);
                                _scaner.NextWordAndSkipSpace();
                            }
                            else if(NextWord(StructScanEnum.Identificatory))
                            {
                                nameElement = _scaner.curWord.str;
                                if (NextWord(StructScanEnum.EndOperator))
                                {
                                    cStruct.Add(nameElement, type);
                                    _scaner.NextWordAndSkipSpace();
                                }
                                else
                                {
                                    error("Ожидается завершающая точка с запятой");
                                    return false;
                                }
                            }
                            else
                            {
                                error("Ожидается имя свойства структуры");
                                return false;
                            }
                        }
                        if (NextWord(StructScanEnum.EndOperator))
                        {
                            return true;
                        }
                        else
                        {
                            error("Ожидается завершающая точка с запятой");
                            return false;
                        }
                    }
                    else
                    {
                        error("Ожидается закрывающая скобка или объявление переменных структуры");
                        return false;
                    }
                }
                else
                {
                    error("Ожидается открывающая скобка");
                    return false;
                }
            }
            else
            {
                error("Ожидается имя структуры");
                return false;
            }
        }

        public void ParseWithError()
        {
            if (clear != null)
                clear();

            _scaner.Clear();
            _scaner.Scan();
            bool result = true;
            if (_scaner.Checking())
            {
                _scaner.Clear();
                _scaner.Scan();

                while (NextWordWithError(StructScanEnum.Struct))
                {
                    string name = null;
                    result = NewCStructWithError(ref name);
                    if (!result)
                    {
                        output("Ошибка парсинга. Не хватает текста. Парсинг прерван");
                        break;
                    }
                }
            }
            if (result)
            {
                _scaner.Rebuild();
                Output();
            }
            else
                throw new Exception("ошибка парсинга");
        }
        private bool NewCStructWithError(ref string nameStruct)
        {
            if (NextWordWithError(StructScanEnum.Identificatory))
            {
                if (nameStruct != null)
                    nameStruct = _scaner.curWord.str;
                CStruct cStruct = new CStruct(_scaner.curWord.str);
                structs.Add(cStruct);
                if (NextWordWithError(StructScanEnum.OpenBracket))
                {
                    if (_scaner.NextWordAndSkipSpace())
                    {
                        while (_scaner.curWord.StructScanEnum != StructScanEnum.CloseBracket
                            && _scaner.curWord.StructScanEnum != StructScanEnum.NONE)
                        {
                            Type type = null;
                            string nameElement = "";
                            bool flag = true;
                            switch (_scaner.curWord.StructScanEnum)
                            {
                                case StructScanEnum.Struct:
                                    if (!NewCStructWithError(ref nameElement))
                                        return false;
                                    type = typeof(CStruct);
                                    break;
                                case StructScanEnum.Int:
                                    type = typeof(int);
                                    break;
                                case StructScanEnum.Float:
                                    type = typeof(float);
                                    break;
                                case StructScanEnum.Char:
                                    type = typeof(char);
                                    break;
                                case StructScanEnum.Bool:
                                    type = typeof(bool);
                                    break;
                                default:
                                    _scaner.RemoveAt(_scaner.iCurWord);
                                    flag = false;
                                    break;
                            }
                            if(!flag)
                                continue;
                            if (type == typeof(CStruct))
                            {
                                cStruct.Add(nameElement, type);
                                _scaner.NextWordAndSkipSpace();
                            }
                            else if (NextWordWithError(StructScanEnum.Identificatory))
                            {
                                nameElement = _scaner.curWord.str;
                                if (NextWordWithError(StructScanEnum.EndOperator))
                                {
                                    cStruct.Add(nameElement, type);
                                    _scaner.NextWordAndSkipSpace();
                                }
                                else
                                {
                                    error("Ожидается завершающая точка с запятой");
                                    return false;
                                }
                            }
                            else
                            {
                                error("Ожидается имя свойства структуры");
                                return false;
                            }
                        }
                        if (NextWordWithError(StructScanEnum.EndOperator))
                        {
                            return true;
                        }
                        else
                        {
                            error("Ожидается завершающая точка с запятой");
                            return false;
                        }
                    }
                    else
                    {
                        error("Ожидается закрывающая скобка или объявление переменных структуры");
                        return false;
                    }
                }
                else
                {
                    error("Ожидается открывающая скобка");
                    return false;
                }
            }
            else
            {
                error("Ожидается имя структуры");
                return false;
            }
        }
    }
}
