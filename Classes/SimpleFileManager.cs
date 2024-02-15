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
        string curFile = "";
        public SimpleFileManager(MainForm form)
        {
            this.form = form;
        }

        public void CreateFile()
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
        }

        public void Exit()
        {
            Application.Exit();
        }

        public void OpenFile()
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
        }

        public void SaveFile()
        {
            using (StreamWriter sw = new StreamWriter(curFile))
            {
                sw.Write(form.InputTextBox.Text);
            }
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
            }
        }
    }
}
