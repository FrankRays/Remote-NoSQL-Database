////////////////////////////////////////////////////////////////////
// ProcessStarter.cs -Process Starter to start executing all other//
//                     programs. Starting point of the Application//   
//                  Main Test Executive                           //                 
// Ver 1.1                                                        //
// Application: Process Starter for CSE681-SMA, Project#4         //
// Language:    C#, ver 6.0, Visual Studio 2015                   //
// Platform:    Dell XPSL501, Core-i5, Windows 10                 //
// Source:      Jim Fawcett, CST 4-187, Syracuse University       //
//              (315) 443-3948, jfawcett@twcny.rr.com             //
// Author:      Rohit Sharma, SUID-242093353, Syracuse University //
//              (315) 935-1323, rshar102@syr.edu
////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides functionality to start processes programatically.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: None
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.1 : 23 Nov 2015
 * - Added method to control flow of startup of application.
 * - Added test executive methods to demonstrate requirements.
 * ver 1.0 : 09 Oct 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using static System.Console;

namespace ProcessStarter
{
    class ProcessStarter
    {
        private static int clientNumber = 1;
        private static int readerClientNumber = 2;
        private static int port = 8091;
        private static int readerPort = 8191;
        private static string serverProcName = "Server\\bin\\Debug\\Server.exe";
        private static string clientProcName = "Client\\bin\\Debug\\Client.exe";
        private static string readerClientProcName ="Client2\\bin\\Debug\\Client2.exe";
        private static string wpfClientProcName = "WpfClient\\bin\\Debug\\WpfApplication1.exe";

        //<----------method to start process based on input exe file name------------->
        public bool startProcess(string process)
        {
            process = Path.GetFullPath(process);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = process,
                Arguments = "8080 localhost" + " " + clientNumber + " " + "http://localhost:" + port + "/CommService"+" "+ "http://localhost:"+ readerPort + "/CommService"+" "+ readerClientNumber,
                UseShellExecute = true,
                CreateNoWindow = true,
            };
            try
            {
                Process p = Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Write("\n  {0}", ex.Message);
                return false;
            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Test Executive";
            Write("\n  current directory is: \"{0}\"", Directory.GetCurrentDirectory());
            Input:
            int numOfClients = 0;
            WriteLine("\nHow many clients you would like to launch?");
            if (!Int32.TryParse(Console.ReadLine(), out numOfClients)) //take user input for number of clients
            {
                Console.WriteLine("Duh! The input is not correct. Try again.");
                goto Input;
            }
            ProcessStarter ps = new ProcessStarter();
            ps.startProcess(serverProcName); //start Server Process
            while (numOfClients > 0)
            {
                ps.startProcess(clientProcName); //start Client Process
                ps.startProcess(readerClientProcName);
                numOfClients--;
                clientNumber++;
                readerClientNumber++;
                port++;
                readerPort++;
            }
            ps.startProcess(wpfClientProcName); //start WPFClient Process
            ps.demoRequirements();
            Write("\n  press key to exit: ");
            ReadKey();
        }

        //<----------method to demonstrate all requirements------------->
        private void demoRequirements()
        {
            WriteLine("\n ************** Demonstrating requirements **************");
            R2();
            R3();
            R4();
            R5();
            R6();
            R7();
            R8();
            R9();
            R10();
        }

        //<----------method to demonstrate requirement # 2------------->
        private void R2()
        {
            WriteLine("\n ============== Demonstrating requirement 2 ============");
            WriteLine("Using NoSQL Database which was self implemented in Project 2\n" +
                "Check DBEngine.cs to verify.");
        }

        //<----------method to demonstrate requirement # 3------------->
        private void R3()
        {
            WriteLine("\n ============== Demonstrating requirement 3 ============");
            WriteLine("Using WCF to communicate between clients and a server\n" +
                "Check ICommService.cs and CommService.cs to verify.");
        }

        //<----------method to demonstrate requirement # 4------------->
        private void R4()
        {
            WriteLine("\n ============== Demonstrating requirement 4 ============");
            WriteLine("Add/Delete/Edit operations are performed through Writer clients.\n" +
                "Provide a logging switch to start Writers. Requests/Responses will be shown on client.\n" +
                "Database state will be shown on Server.");
            WriteLine("Persist/Restore/Search queries are supported through GUI Reader Clients.\n" +
                "Click on 'Execute Query' to start executing first set of queries.");
        }

        //<----------method to demonstrate requirement # 5------------->
        private void R5()
        {
            WriteLine("\n ============== Demonstrating requirement 5 ============");
            WriteLine("Write clients are using Console Interface.\n" +
                "Request/Response will be shown on Console." +
                "XML file used to load query formats is: Project4_SampleWriterXML.xml");
        }

        //<----------method to demonstrate requirement # 6------------->
        private void R6()
        {
            WriteLine("\n ============== Demonstrating requirement 6 ============");
            WriteLine("Logging switch is provided at Write Clients startup.");
        }

        //<----------method to demonstrate requirement # 7------------->
        private void R7()
        {
            WriteLine("\n ============== Demonstrating requirement 7 ============");
            WriteLine("Read clients are using WPF GUI Interface.\n" +
                "Request/Response will be shown on GUI." +
                "XML file used to load query formats is: Project4_SampleReaderXML.xml");
        }

        //<----------method to demonstrate requirement # 8------------->
        private void R8()
        {
            WriteLine("\n ============== Demonstrating requirement 8 ============");
            WriteLine("Check WPF Read Clients to verify.");
        }

        //<----------method to demonstrate requirement # 9------------->
        private void R9()
        {
            WriteLine("\n ============== Demonstrating requirement 9 ============");
            WriteLine("Test Executive is being used to demonstrate requirements.");
        }

        //<----------method to demonstrate requirement # 10------------->
        private void R10()
        {
            WriteLine("\n ============== Demonstrating requirement 10 ============");
            WriteLine("Test executive asked and expected number of clients to start at the beginning.");
        }
    }
}
