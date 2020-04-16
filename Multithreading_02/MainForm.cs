using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Multithreading_02
{
    public partial class MainForm : Form
    {
        private Game myGame;

        public static MainForm Form { get; private set; }

        public MainForm()
        {
            InitializeComponent();

            Form = this;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openTextFile = new OpenFileDialog())
            {
                openTextFile.Title = "Open Text File";
                openTextFile.Filter = "TXT Files|*.txt";

                if (openTextFile.ShowDialog() == DialogResult.OK)
                {
                    if (openTextFile.CheckFileExists)
                    {
                        string[] listOfWords = File.ReadAllText(openTextFile.FileName).Split(' ');
                        myGame = new Game(GamePanel, listOfWords);
                    }  
                }
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {

        }

        private void TypeWordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                myGame.CheckWord(TypeWordBox.Text);

                ScoreLabel.Text = "Score: " + myGame.Score;
                TypeWordBox.Text = string.Empty;
            }
        }
    }
}
