using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Multithreading_02
{
    class Game : ThreadObject
    {
        private List<Word> myWords;
        private Panel myPanel;
        private float
            mySpawnWordTimer,
            mySpawnWordDelay;
        private string[] myListOfWords;

        public int Score { get; private set; }

        public Game(Panel panel, string[] listOfWords)
        {
            this.myPanel = panel;
            this.myListOfWords = listOfWords;

            myWords = new List<Word>();

            mySpawnWordTimer = 0.0f;
            mySpawnWordDelay = 3500.0f;
            Score = 0;

            StartThread();
        }

        public override void Update()
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (IsRunning)
            {
                mySpawnWordTimer = (float)timer.Elapsed.TotalMilliseconds;

                if (mySpawnWordTimer >= mySpawnWordDelay)
                {
                    myWords.Add(new Word(myPanel, myListOfWords[StaticRandom.RandomNumber(0, myListOfWords.Length - 1)]));
                    timer.Restart();
                }
            }
        }

        public void CheckWord(string word)
        {
            for (int i = myWords.Count - 1; i >= 0; i--)
            {
                myWords[i].CheckWord(word);
                if (!myWords[i].IsRunning)
                {
                    myWords[i].Dispose();
                    myWords.RemoveAt(i);
                }
            }
        }
    }
}
