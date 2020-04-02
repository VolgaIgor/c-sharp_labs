using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Bezier сurve with a De Casteljau algorithm
namespace BezierCurveWithCasteljau
{
    public partial class Form1 : Form
    {
        int indexButton = 0;
        List<string> listButtons = new List<string>();

        public Form1()
        {
            InitializeComponent();

            addButton(120, 100);
            addButton(200, 200);
            addButton(300, 110);

            drawBeizeWithCasteljau();
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

                drawBeizeWithCasteljau();

                return;
            }

            isDown = true;
        }

        private void buttonMouseMove( object sender, MouseEventArgs e )
        {
            Button b = (Button)sender;
            if ( isDown ) {
                b.Location = this.PointToClient( Control.MousePosition );

                drawBeizeWithCasteljau();
            }
        }

        private void buttonMouseUp( object sender, MouseEventArgs e )
        {
            isDown = false;
        }
        /* ------------ */

        private void drawBeizeWithCasteljau( bool visualisation = false )
        {
            Bitmap bitmap;
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g;
            g = Graphics.FromImage(bitmap);

            Pen pB = new Pen(Color.Black, 1);
            Pen pG = new Pen(Color.Gray, 1);

            List<PointF> listPoints = new List<PointF>();

            // Получает координаты кнопок по их названиям
            foreach ( string buttonName in listButtons ) {
                Button b = this.Controls.Find(buttonName, true).FirstOrDefault() as Button;
                listPoints.Add(
                    new PointF(
                        b.Location.X + 5,
                        b.Location.Y + 5
                    )
                );
            }

            PointF lastPoint = new PointF(
                listPoints[ 0 ].X,
                listPoints[ 0 ].Y
            );
            for ( double t = 0.01; t <= 1.0; t += 0.01 ) {

                PointF newPoint = getPointFromDeCasteljau(listPoints, t);

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

        // Рекурсивная функция, находит точку для параметра t
        private PointF getPointFromDeCasteljau( List<PointF> listPoints, double t )
        {
            if ( listPoints.Count == 1 ) {
                return listPoints[ 0 ];
            }

            // Создаёт новый список точек, состоящий из середин пар точек предыдущего списка точек
            List<PointF> newListPoints = new List<PointF>();
            for ( int i = 0; i < listPoints.Count - 1; i++ ) {
                double x = listPoints[ i ].X + ( ( listPoints[ i + 1 ].X - listPoints[ i ].X ) * t );
                double y = listPoints[ i ].Y + ( ( listPoints[ i + 1 ].Y - listPoints[ i ].Y ) * t );
                newListPoints.Add( new PointF( (float)x, (float)y ) );
            }

            return getPointFromDeCasteljau(newListPoints, t);
        }

        private void pictureBox1_Click( object sender, EventArgs e )
        {
            addButton( ( (MouseEventArgs)e ).X - 5, ( (MouseEventArgs)e ).Y - 5 );

            drawBeizeWithCasteljau();
        }

    }
}
