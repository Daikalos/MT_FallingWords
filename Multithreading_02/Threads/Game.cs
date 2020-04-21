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
        private float 
            mySpawnWordDelay,
            mySpawnWordDelayMax,
            myUpdateWordPosDelay,
            myUpdateWordPosDelayMax;
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
            mySpawnWordDelayMax = mySpawnWordDelay;

            myUpdateWordPosDelay = 100.0f;
            myUpdateWordPosDelayMax = myUpdateWordPosDelay;

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
                    if (!spawnWordTimer.IsRunning)
                    {
                        spawnWordTimer.Start();
                    }
                    if (!totalTime.IsRunning)
                    {
                        totalTime.Start();
                    }

                    if ((float)spawnWordTimer.Elapsed.TotalMilliseconds >= mySpawnWordDelay)
                    {
                        myUpdateWordPosDelay = myUpdateWordPosDelayMax - ((int)totalTime.Elapsed.TotalSeconds / 4);
                        mySpawnWordDelay = mySpawnWordDelayMax - ((int)totalTime.Elapsed.TotalSeconds * 10);

                        myWords.Add(new Word(this, myPanel, myListOfWords[StaticRandom.RandomNumber(0, myListOfWords.Length)], myUpdateWordPosDelay, 3));
                        spawnWordTimer.Restart();
                    }

                    myPanel.InvokeIfRequired(() =>
                    {
                        MainForm.Form.TimeCounter.Text = "Time: " + ((int)totalTime.Elapsed.TotalSeconds).ToString();
                        MainForm.Form.Score.Text = "Score: " + Score;
                    });
                }
                else
                {
                    totalTime.Stop();
                    spawnWordTimer.Stop();
                }
            }

            //When not running anymore, reset

            myPanel.InvokeIfRequired(() =>
            {
                MainForm.Form.TimeCounter.Text = "Time: 0";
                MainForm.Form.Score.Text = "Score: 0";
            });

            lock (myLock)
            {
                myWords.ForEach(w => w.IsRunning = false);
                myWords.Clear();
            }
        }

        /// <summary>
        /// Each word checks the current inputed word and performs appropriate action
        /// <para>Lock is used because myWords would change size at removal which could crash program when any active concurrent action is being performed on myWords at the same time a word is removed</para>
        /// <para>Lock ensures that there is always an up-to-date version of myWords being handled</para>
        /// </summary>
        public void CheckWord(Word word)
        {
            if (word.Text.ToLower() == word.WordToCheck.ToLower())
            {
                word.IsRunning = false;

                lock (myLock)
                {
                    Score += word.Text.Length;
                    myWords.Remove(word);
                }
            }     
        }

        /// <summary>
        /// Signal each to word to check the current inputed word
        /// </summary>
        public void SignalEachWord(string wordToCheck)
        {
            myWords.ForEach(w => w.WordToCheck = wordToCheck);
        }
    }
}
