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
        private Panel myPanel;
        private Label myWordLabel;
        private int myMoveDistance;
        private float 
            myUpdateLocationTimer,
            myUpdateLocationDelay;

        public Word(Panel panel, string word)
        {
            this.myPanel = panel;

            myUpdateLocationTimer = 0.0f;
            myUpdateLocationDelay = 100.0f;
            myMoveDistance = 2;

            myWordLabel = new Label();
            myWordLabel.Text = word.ToUpper();
            myWordLabel.ForeColor = Color.Black;
            myWordLabel.Font = new Font("Arial", 12.0f, FontStyle.Bold);
            myWordLabel.AutoSize = true;
            myWordLabel.Location = new Point(StaticRandom.RandomNumber(0, myPanel.Width - myWordLabel.Width), -30);

            myPanel.InvokeIfRequired(() =>
            {
                myPanel.Controls.Add(myWordLabel);
            });

            StartThread();
        }

        public override void Update()
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (IsRunning)
            {
                myUpdateLocationTimer = (float)timer.Elapsed.TotalMilliseconds;     

                if (myUpdateLocationTimer >= myUpdateLocationDelay)
                {
                    myPanel.InvokeIfRequired(() =>
                    {
                        myWordLabel.Location = new Point(myWordLabel.Location.X, myWordLabel.Location.Y + myMoveDistance);
                        timer.Restart();
                    });
                }
            }
        }

        public void CheckWord(string word)
        {
            if (myWordLabel.Text.ToLower() == word.ToLower())
            {
                IsRunning = false;
            }
        }

        public void Dispose()
        {
            myPanel.Controls.Remove(myWordLabel);
        }
    }
}
