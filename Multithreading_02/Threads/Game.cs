using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Multithreading_02
{
    class Game : ThreadObject
    {
        private List<Word> myWords;
        private Panel myPanel;
        private float mySpawnWordDelay;
        private string[] myListOfWords;
        private object myLock = new object();

        public int Score { get; private set; }

        public bool IsPaused { get; set; }
        public bool IsGameOver { get; set; }

        public Game(Panel panel, string[] listOfWords)
        {
            this.myPanel = panel;
            this.myListOfWords = listOfWords;

            myWords = new List<Word>();

            mySpawnWordDelay = 3000.0f;

            Score = 0;

            StartThread();
        }

        public override void Update()
        {
            Stopwatch spawnWordTimer = Stopwatch.StartNew();
            Stopwatch totalTime = Stopwatch.StartNew();

            while (IsRunning)
            {
                if (!IsPaused)
                {
                    if ((float)spawnWordTimer.Elapsed.TotalMilliseconds >= mySpawnWordDelay)
                    {
                        myWords.Add(new Word(this, myPanel, myListOfWords[StaticRandom.RandomNumber(0, myListOfWords.Length)], 3));
                        spawnWordTimer.Restart();
                    }

                    myPanel.InvokeIfRequired(() =>
                    {
                        MainForm.Form.TimeCounter.Text = "Time: " + ((int)totalTime.Elapsed.TotalSeconds).ToString();
                        MainForm.Form.Score.Text = "Score: " + Score;
                    });
                }
            }

            myPanel.InvokeIfRequired(() =>
            {
                MainForm.Form.TimeCounter.Text = "Time: 0";
                MainForm.Form.Score.Text = "Score: 0";
            });

            myWords.ForEach(w => w.IsRunning = false);
            myWords.Clear();
        }

        public void CheckWord(Word word)
        {
            lock (myLock)
            {
                if (word.Text == word.WordToCheck)
                {
                    Score += word.Text.Length;

                    word.IsRunning = false;
                    myWords.Remove(word);
                }
            }
        }

        public void SignalCheckWord(string wordToCheck)
        {
            myWords.ForEach(w => w.WordToCheck = wordToCheck);
        }
    }
}
