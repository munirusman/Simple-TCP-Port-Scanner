/* A simple multi-threaded Port Scanner
 * Author: Munir Usman http://munir.wordpress.com
 * Contact: munirus@gmail.com 
 */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string host;
            int portStart = 1, portStop = 65535, ctrThread = 200;

            try
            {
                host = args[0];
            }
            catch
            {
                printUsage();
                return;
            }
            if (args.Length > 2)
            {
                try
                {
                    portStart = int.Parse(args[1]);
                    portStop = int.Parse(args[2]);
                }
                catch
                {
                    printUsage();
                    return;
                }
            }
            if (args.Length > 3)
            {
                try
                {
                    ctrThread = int.Parse(args[3]);
                }
                catch
                {
                    printUsage();
                    return;
                }
            }
            PortScanner ps = new PortScanner(host, portStart, portStop);
            ps.start(ctrThread);
        }

        static void printUsage()
        {
            Console.WriteLine("Usage: PortScanner target-name [starting-port ending-port] [no-threads]\n");
            Console.WriteLine("Where\n\tstarting-port\tStarting port number. Default is 1");
            Console.WriteLine("\tending-port\tEnding port number. Default is 65535.");
            Console.WriteLine("\tno-threads\tNumber of threads. Default is 200");
            Console.WriteLine("\ttarget-name\tTarget host.\n");
            Console.WriteLine("Example 1: \"PortScanner 127.0.0.1 1 10000\" will scan for open TCP ports from port 0 to port 10000.\n");
            Console.WriteLine("Example 2: \"PortScanner 127.0.0.1\" will scan for open TCP ports from port 1 to port 65535.\n");
        }

    }

    public class PortScanner
    {
        private string host;
        private PortList portList;

        public PortScanner(string host, int portStart, int portStop)
        {
            this.host = host;
            this.portList = new PortList(portStart, portStop);
        }

        public PortScanner(string host)
            : this(host, 1, 65535)
        {
        }

        public PortScanner()
            : this("127.0.0.1")
        {
        }

        public void start(int threadCtr)
        {
            for (int i = 0; i < threadCtr; i++)
            {
                Thread th = new Thread(new ThreadStart(run));
                th.Start();
            }
        }
        public void run()
        {
            int port;
            TcpClient tcp = new TcpClient();
            while ((port = portList.getNext()) != -1)
            {
                try
                {
                    tcp = new TcpClient(host, port);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    try
                    {
                        tcp.Close();
                    }
                    catch { }
                }
                Console.WriteLine("TCP Port " + port + " is open");
            }
        }
    }
    public class PortList
    {
        private int start;
        private int stop;
        private int ptr;

        public PortList(int start, int stop)
        {
            this.start = start;
            this.stop = stop;
            this.ptr = start;
        }
        public PortList() : this(1, 65535)
        {
        }

        public bool hasMore()
        {
            return (stop - ptr) >= 0;
        }
        public int getNext()
        {
            if (hasMore())
                return ptr++;
            return -1;
        }
    }
}
