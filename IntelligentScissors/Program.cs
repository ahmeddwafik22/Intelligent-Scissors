using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IntelligentScissors
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            p_q<int> queue = new p_q<int>();

            Random rnd = new Random();
            //enqueue
            for (int i = 0; i < 10; i++)
            {
                int x = rnd.Next(3);
                queue.Enqueue(x, x);
            }
            //dequeue
            while (queue.Count > 0)
            {
                Console.Write(queue.Dequeue() + " ");
            }
            Console.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            
        }
    }
}