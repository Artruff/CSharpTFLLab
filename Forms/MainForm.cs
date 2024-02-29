using CSharpTFLLab.Interfaces;
using CSharpTFLLab.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CSharpTFLLab
{
    public partial class MainForm : Form
    {
        IShell _shell;
        internal IShell shell { get => _shell; }

        IFileManager _fileManager;
        internal IFileManager fileManager { get=> _fileManager; }

        IHelpManager _helpManager;
        internal IHelpManager helpManager { get => _helpManager; }
        IScaner _scaner;
        internal IScaner scaner { get => _scaner; }

        ICorrectionManager _correctionManager;
        internal ICorrectionManager correctionManager { get => _correctionManager; }
        public MainForm()
        {
            InitializeComponent();
            _shell = new SimpleShell();
            _fileManager = new SimpleFileManager(this);
            _helpManager = new SimpleHelpManager();
            _correctionManager = new SimpleCorrectionManager(this);

        }

        private void NewFileButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(fileManager.CreateFile));
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(fileManager.OpenFile));
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(fileManager.SaveFile));
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(fileManager.SaveFileAs));
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(fileManager.Exit));
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Undo));
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Redo));
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Copy));
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Cut));
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Paste));
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.Delete));
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.SelectAll));
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(helpManager.Help));
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(helpManager.About));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            shell.Execute(new Action(fileManager.Exit));
        }

        private void IncreaseFontButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.IncreaseFontIOText));
        }

        private void DecreaseFontButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(correctionManager.DecreaseFontIOText));
        }

        private void InsertButton_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void InsertButton_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (fileManager.CheckClose())
            {
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".c")
                    {
                        string content = File.ReadAllText(file);
                        InputTextBox.Text = content;
                        fileManager.curFile = file;
                    }
                }
            }
        }

        public bool changeLocker = false;
        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {

            string[] keywords = { "struct", "int", "bool", "float", "char" };
            int start = InputTextBox.SelectionStart;
            foreach (string keyword in keywords)
            {
                int index = 0;
                while (index < InputTextBox.Text.Length)
                {
                    int startIndex = InputTextBox.Find(keyword, index, RichTextBoxFinds.WholeWord);
                    if (startIndex == -1)
                    {
                        break;
                    }
                    InputTextBox.SelectionStart = startIndex;
                    InputTextBox.SelectionLength = keyword.Length;
                    InputTextBox.SelectionColor = Color.Blue;
                    InputTextBox.SelectionFont = new Font(InputTextBox.Font, FontStyle.Bold);
                    index = startIndex + keyword.Length;
                }
            }
            InputTextBox.SelectionStart = start;
            //if (!changeLocker)
            //{
            //    changeLocker = true;
            //    string[] array = new string[InputTextBox.Lines.Length];
            //    InputTextBox.Lines.CopyTo(array, 0);
            //    int startSelect = InputTextBox.SelectionStart;
            //    InputTextBox.Clear();
            //    for (int i = 0; i < array.Length; i++)
            //    {
            //        string[] txtNum = array[i].Split(':');
            //        string line = "";
            //        if (txtNum.Length > 1)
            //        {
            //            line = txtNum[1];
            //            startSelect -= txtNum[0].Length;
            //        }
            //        else
            //        {
            //            line = txtNum[0];
            //        }

            //        string num = "";
            //        if (i == 0)
            //            num = (i + 1).ToString() + ":";
            //        else
            //            num = "\n" + (i + 1).ToString() + ":";
            //        startSelect += num.Length;
            //        line = num + line;
            //        InputTextBox.AppendText(line);
            //    }
            //    InputTextBox.SelectionStart = startSelect;
            //    changeLocker = false;
            //}
        }

        private void RefactoringButton_Click(object sender, EventArgs e)
        {
            shell.Execute(new Action(_scaner.Check));
        }
    }
}
