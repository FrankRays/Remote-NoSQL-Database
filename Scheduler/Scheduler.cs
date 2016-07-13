////////////////////////////////////////////////////////////////////
// Scheduler.cs - Scheduler for Project #4                        //
// Ver 1.0                                                        //
// Application: Scheduler for CSE681-SMA, Project#4               //
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
 * Scheduler runs after specified interval of time to save DB state in XML
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, PersistEngine.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * Self Implemented in Proj#2 - Reused in Proj#4 - 23 Nov 2015
 * ver 1.0 : 09 Oct 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static System.Console;

namespace Project2Starter
{
    public class Scheduler<Key, Value>
    {
        private PersistEngine<int, DBElement<int, string>> persistEngine = null;
        public Timer scheduler { get; set; } = new Timer();

        //<----------method to persist XML on DB at a fixed interval of time------------->
        public Scheduler(DBEngine<Key, Value> db)
        {
            persistEngine = new PersistEngine<int, DBElement<int, string>>();
            scheduler.Interval = 1000;
            scheduler.AutoReset = true;
            scheduler.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                Write("\n Persisting database into XML -----\n\n");
                persistEngine.createStringXML(db as DBEngine<int, DBElement<int, string>>, true);
                Write("\n  an event occurred at {0}", e.SignalTime);
            };
            scheduler.Enabled = true;
            ReadKey();
        }
    }

#if (TEST_SCHEDULER)
    public class TestScheduler
    {
        private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
        static void Main(string[] args)
        {
            TestScheduler ts = new TestScheduler();
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "Test Scheduler Name";
            elem.descr = "Test Scheduler Desc";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "Test Scheduler payload";
            ts.db.insert(9999, elem);
            "Demonstrating Requirement #6".title();
            WriteLine();
            "Demonstrating scheduled persistence of database into XML".title();
            Scheduler<int, DBElement<int, string>> scheduler = new Scheduler<int, DBElement<int, string>>(ts.db);
        }
    }
#endif
}
