using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;


namespace IntelligentScissors
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }

    /// <summary>
    /// Holds the edge energy between 
    ///     1. a pixel and its right one (X)
    ///     2. a pixel and its bottom one (Y)
    /// </summary>
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }


    public struct Vector2DD
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double weight { get; set; }
    }





    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        Stopwatch stopwatch = new Stopwatch();

        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[0];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[2];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Calculate edge energy between
        ///     1. the given pixel and its right one (X)
        ///     2. the given pixel and its bottom one (Y)
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns>edge energy with the right pixel (X) and with the bottom pixel (Y)</returns>
        public static Vector2D CalculatePixelEnergies(int x, int y, RGBPixel[,] ImageMatrix)
        {
            if (ImageMatrix == null) throw new Exception("image is not set!");

            Vector2D gradient = CalculateGradientAtPixel(x, y, ImageMatrix);

            double gradientMagnitude = Math.Sqrt(gradient.X * gradient.X + gradient.Y * gradient.Y);
            double edgeAngle = Math.Atan2(gradient.Y, gradient.X);
            double rotatedEdgeAngle = edgeAngle + Math.PI / 2.0;

            Vector2D energy = new Vector2D();
            energy.X = Math.Abs(gradientMagnitude * Math.Cos(rotatedEdgeAngle));
            energy.Y = Math.Abs(gradientMagnitude * Math.Sin(rotatedEdgeAngle));

            return energy;
        }


        public static Dictionary<Vector2D, bool> my_points = new Dictionary<Vector2D, bool>();//  tO get the num of pixels in the final path


        public static Dictionary<Vector2D, LinkedList<Vector2DD>> graph = new Dictionary<Vector2D, LinkedList<Vector2DD>>(); // to construct graph

       

        
       

        public static LinkedList<Vector2D> back;

        // crop
        public static int dx_min = 0;
        public static int dx_max = 0;
        public static int dy_min = 0;
        public static int dy_max = 0;

        //crop
        public static Dictionary<Vector2D, bool> vis = new Dictionary<Vector2D, bool>();
    
        
        
        
        




        
        public static void backtrack(Dictionary<Vector2D, Vector2D> parent, Vector2D free)
        {


            
            while (free.X != -1)
            {
                back.AddLast(free);
                free = parent[free];
            }
            
            


        }

        
        
        public static int x_min = 0;
        public static int x_max = 0;
        public static int y_min = 0;
        public static int y_max = 0;

      public static  RGBPixel[,] vv;

        public static bool sample = false;




        //analysis
        public static RGBPixel[,] start(RGBPixel[,] image,int start)
        {

            RGBPixel[,] x = image;
            

           

            int column = ImageOperations.GetWidth(image);
            int row = ImageOperations.GetHeight(image);
            if (column <= 100 && row <= 100)
            {
                x_min = 0;
                x_max = row - 1;
                y_min = 0;
                y_max = column - 1;
                sample = true;
                construct(image);



            }



            Stopwatch stopwatch1 = new Stopwatch();


            

            
                for (int i = start; i < points_x.Count; i++)
                {


                if (!sample) { 
                
                
                    x_min = points_x[0];
                    x_max = points_x[0];
                    y_min = points_y[0];
                    y_max = points_y[0];


                   
                    
                    if (x_min > points_x[i])
                        x_min = points_x[i];

                    if (y_min > points_y[i])
                        y_min = points_y[i];

                    if (x_max < points_x[i])
                        x_max = points_x[i];

                    if (y_max < points_y[i])
                        y_max = points_y[i];


                    
                    x_min -= 25;
                        if (x_min < 0)
                            x_min = 0;
                        x_max += 25;
                        if (x_max > image.GetLength(0))
                            x_max = image.GetLength(0) - 1;
                        y_min -= 25;
                        if (y_min < 0)
                            y_min = 0;
                        y_max += 25;
                        if (y_max > image.GetLength(1))
                            y_max = image.GetLength(1) - 1;
                    
                    

                    stopwatch1.Start();
                    construct(image);
                    stopwatch1.Stop();

                    Console.WriteLine("Elapsed Time is {0} ms Construct:..", stopwatch1.ElapsedMilliseconds);
                }

                back = new LinkedList<Vector2D>();
                    Vector2D source = new Vector2D();
                    Vector2D free = new Vector2D();
                    source.X = points_x[0];
                source.Y = points_y[0];
                        
                    free.X = points_x[i];
                    free.Y = points_y[i];
                stopwatch1.Start();
                
                
                Dictionary<Vector2D, Vector2D> parent = dijkstra(source, graph);
                stopwatch1.Stop();

                Console.WriteLine("Elapsed Time is {0} ms Dijkstra:..", stopwatch1.ElapsedMilliseconds);
                stopwatch1.Start();
                backtrack(parent, free);
                stopwatch1.Stop();

                Console.WriteLine("Elapsed Time is {0} ms backtrack:..", stopwatch1.ElapsedMilliseconds);

             

                }
           

            return x;

        }




        
        public static Dictionary<Vector2D, Vector2D> dijkstra(Vector2D anchor, Dictionary<Vector2D, LinkedList<Vector2DD>> graph)
        {


            p_q<Vector2D> queue = new p_q<Vector2D>();
            /// key_pair
            Dictionary<Vector2D, double> distance = new Dictionary<Vector2D, double>();

            Dictionary<Vector2D, Vector2D> parent = new Dictionary<Vector2D, Vector2D>();

            Dictionary<Vector2D, bool> visit = new Dictionary<Vector2D, bool>();





            foreach (Vector2D x in graph.Keys)
            {
                
                distance.Add(x, double.PositiveInfinity);
                parent.Add(x, new Vector2D());
                visit.Add(x, false);

            } //O(v)

         

            distance[anchor] = 0;

            Vector2D d = new Vector2D();
            d.X = -1;
            d.Y = -1;
           

            parent[anchor] = d;



            queue.Enqueue(0, anchor); // o(logv)


            


            while (queue.Count != 0) // v
            {
                double w = queue.get_w();
                Vector2D outt = queue.Dequeue(); //o(logv)
                
               
                visit[outt] = true;
                if (distance[outt] < w)
                    continue;
                foreach (Vector2DD x in graph[outt]) //o(adjlogv)
                {

                
                    Vector2D s = new Vector2D() ;
                    s.X = x.X;
                    s.Y = x.Y;




                    if (visit[s] == true)
                        continue;

                    

                    if (x.weight == double.PositiveInfinity)
                    {
                        Vector2DD ss = x;

                        ss.weight = 10000000000000000;


                        if (distance[outt] + ss.weight < distance[s])
                        {
                            
                            distance[s] = distance[outt] + ss.weight;
                            queue.Enqueue(distance[s], s);//o(logv)
                            parent[s] = outt;

                        }
                    }
                    else
                    {
                        if (distance[outt] + x.weight < distance[s])
                        {

                            
                            distance[s] = distance[outt] + x.weight;
                            queue.Enqueue(distance[s], s);//o(logv)
                            parent[s] = outt;

                        }
                    }
                }

            }


            return parent;




        }



   

        
        








        public static List<Vector2DD> points = new List<Vector2DD>();
        public static List<int> points_x = new List<int>();
        public static List<int> points_y = new List<int>();



       public static Queue<Vector2D> q = new Queue<Vector2D>();






        public static RGBPixel[,] crop_area(Vector2D point, RGBPixel[,] img)
        {
            q.Enqueue(point);

            while (q.Count != 0)
            {
                Vector2D p = q.Dequeue();

                if (p.X < dx_min || p.X > dx_max  || p.Y < dy_min || p.Y > dy_max ||
                my_points.ContainsKey(p))
                    continue;
                else
                {

                    img[(int)p.X, (int)p.Y].blue = 255;
                    img[(int)p.X, (int)p.Y].green = 255;
                    img[(int)p.X, (int)p.Y].red = 255;


                    Vector2D p1 = new Vector2D();

                    p1.X = p.X - 1;
                    p1.Y = p.Y;

                    if (!vis.ContainsKey(p1))
                    {
                        vis[p1] = true;

                        q.Enqueue(p1);
                    }

                    Vector2D p2 = new Vector2D();

                    p2.X = p.X + 1;
                    p2.Y = p.Y;



                    if (!vis.ContainsKey(p2))
                    {
                        vis[p2] = true;

                        q.Enqueue(p2);
                    }



                    Vector2D p3 = new Vector2D();

                    p3.X = p.X;
                    p3.Y = p.Y + 1;

                    if (!vis.ContainsKey(p3))
                    {
                        vis[p3] = true;

                        q.Enqueue(p3);
                    }


                    Vector2D p4 = new Vector2D();

                    p4.X = p.X;
                    p4.Y = p.Y - 1;

                    if (!vis.ContainsKey(p4))
                    {
                        vis[p4] = true;

                        q.Enqueue(p4);
                    }


                }

            }

            return img;
        }










        public static void construct(RGBPixel[,] arr)//n power 2
        {
            graph = new Dictionary<Vector2D, LinkedList<Vector2DD>>();  
            int column = ImageOperations.GetWidth(arr);
            int row = ImageOperations.GetHeight(arr);
          StreamWriter str_w = new StreamWriter(@"graph.txt");
          str_w.WriteLine("The constructed graph............................");

            for (int i = x_min; i < x_max + 1; i++)
            {

                for (int j = y_min; j < y_max + 1; j++)
                {
                    int index = i * column + j;
                    str_w.WriteLine("The index node:" + index);
                    Vector2D x = new Vector2D();
                    x.X = i;
                    x.Y = j;
                    graph.Add(x, new LinkedList<Vector2DD>());
                    Vector2D v = new Vector2D();
                    Vector2DD e = new Vector2DD();
                    LinkedList<Vector2DD> adj = new LinkedList<Vector2DD>();
                    
                    str_w.WriteLine("Edges : ");
                    if (!(i - 1 < x_min))
                    {
                        Vector2D s = CalculatePixelEnergies(j, i - 1, arr);
                        v.X = i;
                        v.Y = j;
                        e.X = i - 1;
                        e.Y = j;
                        e.weight = 1 / s.Y;
                        str_w.WriteLine("edge.from " + index + " to " + ((i - 1) * column + (j)) + " with weights " + e.weight);
                        adj.AddLast(e);
                        graph[v] = adj;
                    }

                    if (!(j - 1 < y_min))
                    {

                        Vector2D s = CalculatePixelEnergies(j - 1, i, arr);
                        v.X = i;
                        v.Y = j;
                        e.X = i;
                        e.Y = j - 1;
                        e.weight = 1 / s.X;
                        str_w.WriteLine("edge.from " + index + " to " + ((i) * column + (j - 1)) + " with weights " + e.weight);
                        adj.AddLast(e);
                        graph[v] = adj;
                    }


                    if (!(j + 1 == y_max + 1))
                    {
                        Vector2D s = CalculatePixelEnergies(j, i, arr);
                        v.X = i;
                        v.Y = j;


                        e.X = i;
                        e.Y = j + 1;
                        e.weight = 1 / s.X;
                        str_w.WriteLine("edge.from " + index + " to " + ((i) * column + (j + 1)) + " with weights " + e.weight);
                        adj.AddLast(e);


                        graph[v] = adj;
                    }



                    
                    if (!(i + 1 == x_max + 1))
                    {
                        Vector2D s = CalculatePixelEnergies(j, i, arr);
                        v.X = i;
                        v.Y = j;


                        e.X = i + 1;
                        e.Y = j;
                        e.weight = 1 / s.Y;
                        str_w.WriteLine(" edge.from " + index + " to " + ((i + 1) * column + (j)) + " with weights " + e.weight);
                        adj.AddLast(e);


                        graph[v] = adj;
                    }



                }

            }

            str_w.Close();



        }


      





        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[2] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }





        #region Private Functions
        /// <summary>
        /// Calculate Gradient vector between the given pixel and its right and bottom ones
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns></returns>
        private static Vector2D CalculateGradientAtPixel(int x, int y, RGBPixel[,] ImageMatrix)
        {
            Vector2D gradient = new Vector2D();

            RGBPixel mainPixel = ImageMatrix[y, x];
            double pixelGrayVal = 0.21 * mainPixel.red + 0.72 * mainPixel.green + 0.07 * mainPixel.blue;

            if (y == GetHeight(ImageMatrix) - 1)
            {
                //boundary pixel.
                for (int i = 0; i < 3; i++)
                {
                    gradient.Y = 0;
                }
            }
            else
            {
                RGBPixel downPixel = ImageMatrix[y + 1, x];
                double downPixelGrayVal = 0.21 * downPixel.red + 0.72 * downPixel.green + 0.07 * downPixel.blue;

                gradient.Y = pixelGrayVal - downPixelGrayVal;
            }

            if (x == GetWidth(ImageMatrix) - 1)
            {
                //boundary pixel.
                gradient.X = 0;

            }
            else
            {
                RGBPixel rightPixel = ImageMatrix[y, x + 1];
                double rightPixelGrayVal = 0.21 * rightPixel.red + 0.72 * rightPixel.green + 0.07 * rightPixel.blue;

                gradient.X = pixelGrayVal - rightPixelGrayVal;
            }


            return gradient;
        }


        #endregion
    }
}

