using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Multithreading_02
{
    class Word : ThreadObject
    {
        private Game myGame;
        private Panel myPanel;
        private float myUpdateWordPosDelay;
        private string myWord;
        private int myMoveDistance;

        public string WordToCheck { get => myWord; private set => myWord = value; }
        public bool WordFound { get; set; }

        public Word(Game game, Panel panel, string word, int moveDistance)
        {
            this.myGame = game;
            this.myPanel = panel;
            this.myWord = word;
            this.myMoveDistance = moveDistance;

            myUpdateWordPosDelay = 100.0f;
            WordFound = false;

            StartThread();
        }

        public override void Update()
        {
            Stopwatch timer = Stopwatch.StartNew();

            Label wordLabel = new Label();
            wordLabel.Text = myWord.ToUpper();
            wordLabel.ForeColor = Color.Black;
            wordLabel.Font = new Font("Arial", 12.0f, FontStyle.Bold);
            wordLabel.AutoSize = true;
            wordLabel.Location = new Point(StaticRandom.RandomNumber(0, myPanel.Width - wordLabel.Width), -wordLabel.Height);

            myPanel.InvokeIfRequired(() =>
            {
                myPanel.Controls.Add(wordLabel);
            });

            while (IsRunning)
            {
                if (!myGame.IsPaused)
                {
                    if ((float)timer.Elapsed.TotalMilliseconds >= myUpdateWordPosDelay)
                    {
                        myPanel.InvokeIfRequired(() =>
                        {
                            wordLabel.Location = new Point(wordLabel.Location.X, wordLabel.Location.Y + myMoveDistance);
                            if (wordLabel.Location.Y - myMoveDistance >= myPanel.Height)
                            {
                                MainForm.Form.Reset();
                            }
                        });
                        timer.Restart();
                    }
                }
            }

            myPanel.InvokeIfRequired(() =>
            {
                myPanel.Controls.Remove(wordLabel);
            });
        }
    }
}
