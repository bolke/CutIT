using CutIT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutIT.Messages;
using System.Net.Sockets;
using CutIT.Connection;
using System.Threading;
using CutIT.GRBL;

namespace CutITConsole
{
    class Program
    {
        TcpGrblClient gc;
        GrblSettings gs;
        bool running = true;

        public void ReadMain()
        {
            while (running)
            {
                if (gc.RequestsToDo.Count < 10)
                {                    
                    gc.Add("g0 x10 y10 z10 f600");
                    gc.Add("g1 x0 y0 z0 f600");                    
                }else
                {
                    gc.Add("?");
                    Thread.Sleep(1000);
                }
                Thread.Sleep(100);
                //gc.Add(Console.ReadLine());
            }
        }

        public Program()
        {
            gc = new TcpGrblClient();
            gs = new GrblSettings();
            if (gc.Start("192.168.1.15", 8887))
            {
                running = true;
                new Thread(new ThreadStart(ReadMain)).Start();
                while (gc.IsRunning)
                {
                    if (gc.IsPaused)
                    {
                        Console.WriteLine("PAUSED");
                    }
                    if (gc.Responses.Count > 0)
                    {
                        GrblResponse r = gc.Responses.Pop();
                        gs.Parse(r);
                        Console.WriteLine(r.Content);
                    }
                    Thread.Sleep(10);
                }
            }

        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
