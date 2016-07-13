/////////////////////////////////////////////////////////////////////////
// Client2.cs - CommService Reader client sends and receives messages  //
// ver 2.2                                                             //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis        //
//          Project #4                                                 //
//  Author:      Rohit Sharma, SUID-242093353, Syracuse University     //
//              (315) 935-1323, rshar102@syr.edu                       //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * Ver 2.2 : 23rd Nov 2015 
 * - added reader client functionality.
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project4Starter
{
    using System.Xml.Linq;
    using Util = Utilities;
    using HRTimer;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted

    class Client
    {
        string localUrl { get; set; } = "http://localhost:8082/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";

        string wpfUrl { get; set; } = "http://localhost:8089/CommService";
        string readerClientNumber;

        private static string readClientXmlName = "Project4_SampleReaderXML.xml";
        //----< retrieve urls from the CommandLine if there are any >--------

        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            localUrl = args[4];
            remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
            readerClientNumber = args[5];
        }
        static void Main(string[] args)
        {
            Console.Write("\n  starting CommService client"); Console.Write("\n =============================\n");
            Client clnt = new Client(); clnt.processCommandLine(args);
            Console.Title = "Reader Client #"+ clnt.readerClientNumber;
            captureMessageLogging();//capture message logging
            Receiver rcvr = startReceiver(clnt);//get receiver object
            Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message
            Message msg = new Message();
            msg.fromUrl = clnt.localUrl; msg.toUrl = clnt.remoteUrl;
            Console.Write("\n  sender's url is {0}", msg.fromUrl); Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);
            if (!sndr.Connect(msg.toUrl))
            {
                shutdownSndrRcvr(rcvr, sndr); return;
            }
            XDocument constReadXml = sndr.readClientXmlReader(readClientXmlName);
            List<string> list = new List<string>();
            int numOfRequest = 0, counter = 0;
            list = sndr.readerXmlMsgGenerator(constReadXml, ref numOfRequest);
            int qType = list.Count;
            int msgPerType = numOfRequest / qType;
            HRTimer.HiResTimer timer = new HiResTimer();
            timer.Start();
            foreach (string lst in list)
            {
                msg.content = lst;
                for (int i = 0; i < msgPerType; i++)
                {
                    Console.WriteLine("\nMessage #" + (++counter).ToString());
                    if (Util.isLoggerEnabled)
                    {
                        Console.Write("\nSending: \n{0}", msg.content);
                    }
                    if (!sndr.sendMessage(msg))
                        return;
                }
            }
            timer.Stop();
            ulong elapsedTime = timer.ElapsedMicroseconds;
            msg.content = "done"; sndr.sendMessage(msg);
            Console.WriteLine("\nElapsed time: " + elapsedTime + " microseconds = " + (elapsedTime / 1000000) + " seconds");
            msg.content = "Composite;" + elapsedTime + ";" + numOfRequest + ";" + clnt.readerClientNumber; msg.toUrl = clnt.wpfUrl;
            if (!sndr.Connect(msg.toUrl))
            {
                shutdownSndrRcvr(rcvr, sndr); return;
            }
            sndr.sendMessage(msg); Util.waitForUser(); rcvr.shutDown(); sndr.shutdown(); Console.Write("\n\n");
        }

        //----< method to capture logging>--------
        private static void captureMessageLogging()
        {
            Console.WriteLine("Do you want Message Logging? Press Y or N:");
            string key = Console.ReadKey().Key.ToString();
            if (key.ToUpper() == "Y")
            {
                Console.WriteLine("Logging enabled by user.");
                Util.isLoggerEnabled = true;
            }
            else
            {
                Console.WriteLine("\nLogging not enabled by user.");
            }
        }

        //----< method to start receiver>--------
        private static Receiver startReceiver(Client clnt)
        {
            string localPort = Util.urlPort(clnt.localUrl);
            string localAddr = Util.urlAddress(clnt.localUrl);
            Receiver rcvr = new Receiver(localPort, localAddr);
            if (rcvr.StartService())
            {
                rcvr.doService(rcvr.defaultServiceAction());
            }

            return rcvr;
        }

        //----< method to shut down sender and receiver>--------
        private static void shutdownSndrRcvr(Receiver rcvr, Sender sndr)
        {
            Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
            sndr.shutdown();
            rcvr.shutDown();
        }
    }
}

