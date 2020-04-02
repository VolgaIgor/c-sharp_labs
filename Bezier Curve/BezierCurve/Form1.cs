using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Basic Bezier Curve
namespace BezierCurve
{
    public partial class Form1 : Form
    {
        int indexButton = 0;
        List<string> listButtons = new List<string>();

        private int Factorial( int n )
        {
            int r = 1;
            for ( int i = 2; i <= n; i++ ) {
                r *= i;
            }
            return r;
        }

        private int C( int n, int k )
        {
            return Factorial(n) / ( Factorial(k) * Factorial(n - k) );
        }

        public Form1()
        {
            InitializeComponent();

            addButton(120, 100);
            addButton(200, 200);
            addButton(300, 110);

            drawBeize();
        }

        private void addButton( int X, int Y )
        {
            Button button = new Button();
            button.Left = X;
            button.Top = Y;
            button.Size = new Size(10, 10);
            button.BackColor = Color.Black;

            string name = "btn" + indexButton++;
            listButtons.Add(name);

            button.Name = name;
            button.MouseDown += buttonMouseDown;
            button.MouseMove += buttonMouseMove;
            button.MouseUp += buttonMouseUp;

            pictureBox1.Controls.Add(button);
        }

        bool isDown = false;

        /* Перемещение кнопок */
        private void buttonMouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Right ) {
                Button b = (Button)sender;
                listButtons.Remove( b.Name );
                b.Dispose();

                drawBeize();

                return;
            }

            isDown = true;
        }

        private void buttonMouseMove( object sender, MouseEventArgs e )
        {
            Button b = (Button)sender;
            if ( isDown ) {
                b.Location = this.PointToClient( Control.MousePosition );

                drawBeize();
            }
        }

        private void buttonMouseUp( object sender, MouseEventArgs e )
        {
            isDown = false;
        }
        /* ------------ */

        private void drawBeize()
        {
            Bitmap bitmap;
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g;
            g = Graphics.FromImage(bitmap);

            Pen pB = new Pen(Color.Black, 1);
            Pen pG = new Pen(Color.Gray, 1);

            List<Point> listPoints = new List<Point>();

            // Получает координаты кнопок по их названиям
            foreach ( string buttonName in listButtons ) {
                Button b = this.Controls.Find(buttonName, true).FirstOrDefault() as Button;
                listPoints.Add(
                    new Point(
                        b.Location.X + 5,
                        b.Location.Y + 5
                    )
                );
            }

            Point lastPoint = new Point(
                listPoints[ 0 ].X,
                listPoints[ 0 ].Y
            );
            for ( double t = 0.01; t <= 1.0; t += 0.001 ) {
                double x = 0.0, y = 0.0;

                // Вычисляет многочлен Бернштейна и координаты точки для каждой новой t
                int n = listPoints.Count - 1;
                for ( int i = 0; i < listPoints.Count; i++ ) {
                    double bernstein = C(n, i) * Math.Pow(1.0 - t, n - i) * Math.Pow(t, i);
                    x += bernstein * listPoints[ i ].X;
                    y += bernstein * listPoints[ i ].Y;
                }

                Point newPoint = new Point( (int)x, (int)y );

                g.DrawLine(pB, lastPoint, newPoint);
                lastPoint = newPoint;
            }

            for ( int i = 0; i < listPoints.Count - 1; i++ ) {
                g.DrawLine(pG, listPoints[i], listPoints[i+1]);
            }

            pictureBox1.Image = bitmap;
            pictureBox1.Refresh();
            g.Dispose();
        }

        private void pictureBox1_Click( object sender, EventArgs e )
        {
            addButton( ( (MouseEventArgs)e ).X - 5, ( (MouseEventArgs)e ).Y - 5 );

            drawBeize();
        }

    }
}
