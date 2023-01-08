using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;


namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] IMGG;

        Bitmap lastClick;
        Stopwatch stopwatch = new Stopwatch();
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                IMGG = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.vv= ImageOperations.OpenImage(OpenedFilePath);




                ImageOperations.DisplayImage(IMGG, pictureBox1);
                




            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

        }

        RGBPixel[,] crop;

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {


              //ImageOperations.dx_mi
            ImageOperations.dx_min -= 10;
            if (ImageOperations.dx_min < 0)
             ImageOperations.dx_min = 0;
            ImageOperations.dx_max += 10;
            if (ImageOperations.dx_max > IMGG.GetLength(0))
                ImageOperations.dx_max = IMGG.GetLength(0)-1;
            ImageOperations.dy_min -= 10;
            if (ImageOperations.dy_min < 0)
                ImageOperations.dy_min = 0;
            ImageOperations.dy_max += 10;
            if (ImageOperations.dy_max > IMGG.GetLength(1))
               ImageOperations.dy_max = IMGG.GetLength(1) - 1;

          
            

            crop = new RGBPixel[ImageOperations.dx_max - ImageOperations.dx_min+1, ImageOperations.dy_max - ImageOperations.dy_min+1];

            for (int i = ImageOperations.dx_min; i <= ImageOperations.dx_max; i++)
            {
                Vector2D point = new Vector2D();
                if (i == ImageOperations.dx_min || i == ImageOperations.dx_max)
                {
                    for (int j = ImageOperations.dy_min; j <= ImageOperations.dy_max; j++)
                    {


                        point.X = i;
                        point.Y = j;

                        if (!ImageOperations.vis.ContainsKey(point))
                            IMGG = ImageOperations.crop_area(point, IMGG);
                    }
                }
                else
                {

                    point.X = i;
                    point.Y = ImageOperations.dy_min;
                    if (!ImageOperations.vis.ContainsKey(point))
                        IMGG = ImageOperations.crop_area(point, IMGG);

                    point.X = i;
                    point.Y = ImageOperations.dy_max;
                    if (!ImageOperations.vis.ContainsKey(point))
                        IMGG = ImageOperations.crop_area(point, IMGG);
                }
            }


         
















            int x = ImageOperations.dx_min;
           

            for (int i=0;i<= ImageOperations.dx_max - ImageOperations.dx_min; i++)
            {
                int z = ImageOperations.dy_min;
                for (int j = 0; j <= ImageOperations.dy_max - ImageOperations.dy_min; j++)
                {
                    crop[i, j] = IMGG[x, z];
                    z++;
                }

                x++;
            }

            
            

            ImageOperations.DisplayImage(crop, pictureBox2);





            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;


          
        }


        public static int x1, y1, x2, y2, p = 3;


       

        int s = 0;
        int ii = 0;
        public static RGBPixel[,] IMG ;



        public static Vector2D first = new Vector2D();
       public static  Vector2D last = new Vector2D();


       

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag&&checkBox1.Checked)
            {
                
                start++;
                if (start == time)
                {
                    i++;
                    count_anchor++;
                    start = 0;
                    time = 10 + int.Parse(textBox4.Text);
                }

            }


            Vector2D prev_anchor = new Vector2D();

            if (d_click)
            {

                ImageOperations.points_x = new List<int>();
                ImageOperations.points_y = new List<int>();

                ImageOperations.points_x.Add((int)first.X);
                ImageOperations.points_x.Add((int)last.X);
                ImageOperations.points_y.Add((int)first.Y);
                ImageOperations.points_y.Add((int)last.Y);


                s = 0;
                ii = 0;


                IMGG = ImageOperations.start(IMGG, s);

                foreach (Vector2D s in ImageOperations.back)
                {
                   
                    lastClick.SetPixel((int)(s.Y), (int)(s.X), Color.Black);

                    Vector2D point = new Vector2D();
                    point.X = (int)s.X;
                    point.Y = (int)s.Y;

                    if (ImageOperations.dx_min > (int)s.X)
                    {
                        ImageOperations.dx_min = (int)s.X;
                    }

                    if (ImageOperations.dx_max < (int)s.X)
                        ImageOperations.dx_max = (int)s.X;

                    if (ImageOperations.dy_min > (int)s.Y)
                        ImageOperations.dy_min = (int)s.Y;

                    if (ImageOperations.dy_max < (int)s.Y)
                        ImageOperations.dy_max = (int)s.Y;




                    ImageOperations.my_points[point] = true;


                    
                }

                pictureBox1.Image = lastClick;
                pictureBox1.Refresh();

               
                d_click = false;
                
            }


            if (i != next_i)
            {
                next_i = i;
                s = 0;
                ii = 0;


                if (count_anchor > 0)
                {

               


                    prev_anchor.X = ImageOperations.points_x[0];
                    prev_anchor.Y = ImageOperations.points_y[0];


                    ImageOperations.points_x = new List<int>();
                    ImageOperations.points_y = new List<int>();

                    ImageOperations.points_x.Add((int)prev_anchor.X);
                    ImageOperations.points_y.Add((int)prev_anchor.Y);



                    ImageOperations.points_x.Add(e.Y);
                    ImageOperations.points_y.Add(e.X);
                    IMGG = ImageOperations.start(IMGG, s);

                    foreach (Vector2D s in ImageOperations.back)
                    {
                        
                        lastClick.SetPixel((int)(s.Y), (int)(s.X), Color.Black);

                        Vector2D point = new Vector2D();
                        point.X = (int)s.X;
                        point.Y = (int)s.Y;


                        if (ImageOperations.dx_min > (int)s.X)
                        {
                            ImageOperations.dx_min = (int)s.X;
                        }

                        if (ImageOperations.dx_max < (int)s.X)
                            ImageOperations.dx_max = (int)s.X;

                        if (ImageOperations.dy_min > (int)s.Y)
                            ImageOperations.dy_min = (int)s.Y;

                        if (ImageOperations.dy_max < (int)s.Y)
                            ImageOperations.dy_max = (int)s.Y;


                        ImageOperations.my_points[point] = true;


                        
                    }

                    pictureBox1.Image = lastClick;
                    pictureBox1.Refresh();
                }

                ImageOperations.points_x = new List<int>();
                ImageOperations.points_y = new List<int>();

            }
            if (flag)
            {
                ImageOperations.points_x.Add(e.Y);
                ImageOperations.points_y.Add(e.X);
                if (ii > 0)
                {
                    


                    IMGG = ImageOperations.start(IMGG, s);


                    Bitmap bb = new Bitmap(lastClick);
                    pictureBox1.Image = lastClick;
                    pictureBox1.Refresh();

                    foreach (Vector2D s in ImageOperations.back)
                    {
                       
                        bb.SetPixel((int)(s.Y), (int)(s.X), Color.Black);
                      
                    }
                    pictureBox1.Image = bb;
                    pictureBox1.Refresh();

      
                    s++;
                }
                ii++;
             

            }
            textBox1.Text = e.X.ToString();
            textBox2.Text = e.Y.ToString();

           
            
        }

        public Pen crpPen = new Pen(Color.Blue);

        private void panel1_Enter(object sender, EventArgs e)
        {
            flag = false;
        }



        bool d_click = false;

        private void pictureBox1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {

            int time = 10;
            int start = 0;
            bool stop = true;

            d_click = true;

            last.X = e.Y;
            last.Y = e.X;


            flag = false;
             i = -1;
            next_i = -1;
            count_anchor = -1;
            textBox3.Text = (ImageOperations.my_points.Count).ToString();
            

      }


        int time = 10;
        int start = 0;
        bool stop = true;

        bool flag = false;

    

        int i = -1;
        int next_i = -1;

        int count_anchor = -1;

      


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            if (stop && checkBox1.Checked)
            {
                time = time + int.Parse(textBox4.Text);
                stop = false;

            }
          

            count_anchor++;
            if (count_anchor == 0)
            {


                ImageOperations.dx_min = e.Y;



                ImageOperations.dx_max = e.Y;


                ImageOperations.dy_min = e.X;

                ImageOperations.dy_max = e.X;


            }


            if (i == -1)
            {
                first.X = e.Y;
                first.Y = e.X;
            }
            


          

            i++;           
                          

            lastClick = (Bitmap)(pictureBox1.Image.Clone());
            flag = true;
           



        }

        
    }
}