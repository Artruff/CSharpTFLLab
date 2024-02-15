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

namespace CSharpTFLLab
{
    public partial class MainForm : Form
    {

        IShell shell_;
        internal IShell shell { get => shell_; }

        IFileManager fileManager_;
        internal IFileManager fileManager { get=> fileManager_; }

        IHelpManager helpManager_;
        internal IHelpManager helpManager { get => helpManager_; }

        ICorrectionManager correctionManager_;
        internal ICorrectionManager correctionManager { get => correctionManager_; }

        public MainForm()
        {
            InitializeComponent();
            shell_ = new SimpleShell();
            fileManager_ = new SimpleFileManager(this);
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

        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
