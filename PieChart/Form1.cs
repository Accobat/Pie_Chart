using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

namespace PieChart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            ArrayList data = new ArrayList();
            data.Add(new PieChartElement("East", (float)50.75));
            data.Add(new PieChartElement("West", (float)22));
            data.Add(new PieChartElement("North", (float)72.32));
            data.Add(new PieChartElement("South", (float)12));
            data.Add(new PieChartElement("Central", (float)44));

            chart.Image = drawPieChart(data, new Size(chart.Width, chart.Height));
        }

        private Image drawPieChart(ArrayList elements, Size s)
        {
            Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green,
                Color.Blue, Color.Indigo, Color.Violet, Color.DarkRed,
                Color.DarkOrange, Color.DarkSalmon, Color.DarkGreen,
                Color.DarkBlue, Color.Lavender, Color.LightBlue, Color.Coral };

            if (elements.Count > colors.Length)
            {
                throw new ArgumentException("Pie chart must have " + colors.Length + " or fewer elements");
            }

            Bitmap bm = new Bitmap(s.Width, s.Height);
            Graphics g = Graphics.FromImage(bm);
            g.SmoothingMode = SmoothingMode.HighQuality;
            
            float total = 0;

            foreach (PieChartElement e in elements)
            {
                if (e.value < 0)
                {
                    throw new ArgumentException("All elements must have positive values");
                }
                total += e.value;
            }

            if (!(total > 0))
            {
                throw new ArgumentException("Must provide at least one PieChartElement with a positive value");
            }

            // Define the rectangle that the pie chart will use
            Rectangle rect = new Rectangle(1, 1, s.Width - 2, s.Height - 2);

            Pen p = new Pen(Color.Black, 1);

            // Draw the first section at 0 degrees
            float startAngle = 0;

            // Draw each of the pie shapes
            foreach (PieChartElement e in elements)
            {
                // Calculate the degrees that this section will consume,
                // based on the percentage of the total
                float sweepAngle = (e.value / total) * 360;

                // Draw the pie shape
                g.DrawPie(p, rect, startAngle, sweepAngle);

                // Calculate the angle for the next pie shape by adding
                // the current shape's degrees to the previous total.
                startAngle += sweepAngle;
            }
            Point lRectCorner = new Point((s.Width / 2) + 2, 1);
            Size lRectSize = new Size(s.Width - (s.Width / 2) - 4, s.Height - 2);
            Rectangle lRect = new Rectangle(lRectCorner, lRectSize);

            // Draw a black box with a white background for the legend.
            Brush lb = new SolidBrush(Color.White);
            Pen lp = new Pen(Color.Black, 1);
            g.FillRectangle(lb, lRect);
            g.DrawRectangle(lp, lRect);

            // Determine the number of vertical pixels for each legend item
            int vert = (lRect.Height - 10) / elements.Count;

            // Calculate the width of the legend box as 20% of the total legend width
            int legendWidth = lRect.Width / 5;

            // Calculate the height of the legend box as 75% of the legend item height
            int legendHeight = (int)(vert * 0.75);

            // Calculate a buffer space between elements
            int buffer = (int)(vert - legendHeight) / 2;

            // Calculate the left border of the legend text
            int textX = lRectCorner.X + legendWidth + buffer * 2;

            // Calculate the width of the legend text
            int textWidth = lRect.Width - (lRect.Width / 5) - (buffer * 2);

            // Start the legend five pixels from the top of the rectangle
            int currentVert = 5;
            int legendColor = 0;

            foreach (PieChartElement e in elements)
            {
                // Create a brush with a nice gradient
                Rectangle thisRect = new Rectangle(lRectCorner.X + buffer, currentVert + buffer, legendWidth, legendHeight);
                Brush b = new LinearGradientBrush(thisRect, colors[legendColor++], Color.White, (float)45);

                // Draw the legend box fill and border
                g.FillRectangle(b, thisRect);
                g.DrawRectangle(lp, thisRect);

                // Define the rectangle for the text
                RectangleF textRect = new Rectangle(textX, currentVert + buffer, textWidth, legendHeight);

                // Define the font for the text
                Font tf = new Font("Arial", 12);

                // Create the foreground text brush
                Brush tb = new SolidBrush(Color.Black);

                // Define the vertical and horizontal alignment for the text
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                // Draw the text
                g.DrawString(e.name + ": " + e.value.ToString(), tf, tb, textRect, sf);

                // Increment the current vertical location
                currentVert += vert;
            }
            return bm;

        }
    }
}