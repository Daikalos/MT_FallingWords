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
        public Label TimeCounter { get => TimeLabel; set => TimeLabel = value; }
        public Label Score { get => ScoreLabel; set => ScoreLabel = value; }

        public MainForm()
        {
            InitializeComponent();

            Form = this;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (myGame == null)
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

                            StartButton.Enabled = false;
                            StopButton.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                myGame.IsPaused = false;

                StartButton.Enabled = false;
                StopButton.Enabled = true;
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            myGame.IsPaused = true;

            StartButton.Enabled = true;
            StopButton.Enabled = false;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void TypeWordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !myGame.IsPaused)
            {
                myGame.SignalCheckWord(TypeWordBox.Text);
                TypeWordBox.Text = string.Empty;
            }
        }

        public void Reset()
        {
            myGame.IsRunning = false;
            myGame = null;

            StartButton.Enabled = true;
            StopButton.Enabled = false;
        }
    }
}
