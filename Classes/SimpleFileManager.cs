using CSharpTFLLab.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpTFLLab.Classes
{
    internal class SimpleFileManager : IFileManager
    {
        MainForm form;
        string curFile;
        bool fileChanged;
        public SimpleFileManager(MainForm form)
        {
            this.form = form;
            curFile = "";
            fileChanged = false;
            this.form.InputTextBox.TextChanged += new EventHandler((sender, earg) => { fileChanged = true; });
        }
        internal bool CheckClose()
        {
            if (fileChanged)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в файле?", "Неприменённые изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    fileChanged = false;
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }
        public void CreateFile()
        {
            if (CheckClose())
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "C Files (*.c)|*.c";
                saveFileDialog.Title = "Создать файл";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                    sw.Close();
                    curFile = saveFileDialog.FileName;
                }
                fileChanged = false;
            }
        }

        public void Exit()
        {
            if (CheckClose())
            {
                Application.Exit();
            }
        }

        public void OpenFile()
        {
            if (CheckClose())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "C Files (*.c)|*.c";
                openFileDialog.Title = "Открыть файл";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        form.InputTextBox.Text = sr.ReadToEnd();
                    }
                    curFile = filePath;
                }
                fileChanged = false;
            }
        }

        public void SaveFile()
        {
            using (StreamWriter sw = new StreamWriter(curFile))
            {
                sw.Write(form.InputTextBox.Text);
            }
            fileChanged = false;
        }

        public void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "C Files (*.c)|*.c";
            saveFileDialog.Title = "Сохранить файл как";
            saveFileDialog.FileName = curFile.Split('\\').Last();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(form.InputTextBox.Text);
                }
                curFile = filePath;
                fileChanged = false;
            }
        }
    }
}
