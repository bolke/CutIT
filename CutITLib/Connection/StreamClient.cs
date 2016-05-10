using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CutIT.Connection
{
    public class StreamClient
    {
        private int readTimeout = 1000;
        private int writeTimeout = 100;
        
        private bool isRunning = false;
        
        private ConcurrentQueue<char[]> txData = new ConcurrentQueue<char[]>();
        private ConcurrentQueue<char[]> rxData = new ConcurrentQueue<char[]>();        
        
        private Thread readThread = null;
        private Thread writeThread = null;
        
        private Stream sourceStream = null;
        private StreamWriter writeStream = null;
        private StreamReader readStream = null;

        private bool readLines = false;

        public bool ReadLines
        {
            get { return readLines; }
            set { readLines = value; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public ConcurrentQueue<char[]> TxData
        {
            get { return txData; }
        }

        public ConcurrentQueue<char[]> RxData
        {
            get { return rxData; }
        }
        
        public int ReadTimeout
        {
            get { return readTimeout; }
            set
            {
                readTimeout = value;
                if (sourceStream != null)
                    sourceStream.ReadTimeout = readTimeout;
            }
        }

        public int WriteTimeout
        {
            get { return writeTimeout; }
            set {
                writeTimeout = value;
                if (sourceStream != null)
                    sourceStream.WriteTimeout = writeTimeout;
            }
        }

        public StreamClient()
        {            
        }

        public bool Start(Stream sourceStream)
        {
            if (!isRunning && sourceStream != null)
            {
                this.sourceStream = sourceStream;

                if (sourceStream.CanRead)
                {
                    sourceStream.ReadTimeout = readTimeout;                                        
                    readStream = new StreamReader(sourceStream);
                    readThread = new Thread(new ThreadStart(ReadMain));
                }

                if (sourceStream.CanWrite)
                {
                    sourceStream.WriteTimeout = writeTimeout;
                    writeStream = new StreamWriter(sourceStream);
                    writeStream.AutoFlush = true;
                    writeThread = new Thread(new ThreadStart(WriteMain));                    
                }

                isRunning = true;

                if (readThread != null) 
                    readThread.Start();

                if (writeThread != null)
                    writeThread.Start();

                isRunning = readThread != null || writeThread != null;
                return isRunning;
            }

            return false;
        }

        protected void ReadMain()
        {
            char[] buffer = new char[1024];
            int read = 0;
            while (isRunning)
            {
                try
                {
                    if (ReadLines)
                    {
                        rxData.Enqueue(readStream.ReadLine().ToCharArray());
                    }
                    else
                    {
                        read = readStream.Read(buffer, 0, 1024);
                        if (read > 0)
                        {
                            Array.Resize(ref buffer, read);
                            rxData.Enqueue(buffer);
                            buffer = new char[1024];
                        }
                    }
                }
                catch (IOException exception)
                {
                    //read throws this exception when a read time out occurs
                }
            }
            readStream.Close();
            readStream = null;
        }

        protected void WriteMain()
        {
            char[] buffer = null;
            while (isRunning)
            {
                if (txData.Count > 0)
                {
                    if (txData.TryDequeue(out buffer))
                    {
                        writeStream.Write(buffer, 0, buffer.Count());
                    }
                }else
                    Thread.Sleep(1);
            }
            writeStream.Close();
            writeStream = null;
        }

        public virtual bool Stop()
        {
            if (IsRunning)
            {
                isRunning = false;
                //TODO dangerous loops, create an escape clause
                while (writeStream != null || readStream != null) ;
                return true;
            }
            return false;
        }        
    }
}
