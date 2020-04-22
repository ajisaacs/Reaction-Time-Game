using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace WindowsFormsApp1
{
    public class ReactionTimeControl : Control
    {
        private Stopwatch sw;
        private State state;
        private StringFormat stringFormat;
        private Timer timer;

        public event EventHandler<TestResults> TestComplete;

        public ReactionTimeControl()
        {
            sw = new Stopwatch();

            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;

            stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            state = State.React;
            sw.Restart();
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            const int BorderWidth = 1;

            using (var pen = new Pen(SystemColors.ActiveBorder, BorderWidth))
            using (var graphicsPath = new GraphicsPath())
            {
                var halfPenWidth = BorderWidth / 2.0f;
                var borderRect = new RectangleF(
                    halfPenWidth,
                    halfPenWidth,
                    Width - BorderWidth, 
                    Height - BorderWidth);

                graphicsPath.AddRectangle(borderRect);
                pevent.Graphics.DrawPath(pen, graphicsPath);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            var bounds = Bounds;
            bounds.X = 0;
            bounds.Y = 0;

            switch (state)
            {
                case State.Idle:
                    BackColor = Color.White;
                    e.Graphics.DrawString("Click here to start", Font, Brushes.Gray, bounds, stringFormat);
                    break;

                case State.Waiting:
                    BackColor = Color.LightBlue;
                    e.Graphics.DrawString(". . .", Font, Brushes.DodgerBlue, bounds, stringFormat);
                    break;

                case State.React:
                    BackColor = Color.LimeGreen;
                    e.Graphics.DrawString("React fast!!!", Font, Brushes.Green, bounds, stringFormat);
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            switch (state)
            {
                case State.Idle:
                    state = State.Waiting;
                    timer.Interval = new Random().Next(1000, 4000);
                    timer.Start();
                    Invalidate();
                    break;

                case State.Waiting:
                    timer.Stop();
                    sw.Stop();
                    TestComplete?.Invoke(this, new TestResults
                    {
                        Time = DateTime.Now,
                        Message = "You clicked too soon."
                    });
                    state = State.Idle;
                    Invalidate();
                    break;

                case State.React:
                    sw.Stop();
                    TestComplete?.Invoke(this, new TestResults
                    {
                        Time = DateTime.Now,
                        ReactionTimeMilliseconds = sw.ElapsedMilliseconds
                    });
                    state = State.Idle;
                    Invalidate();
                    break;
            }
        }
    }
}
