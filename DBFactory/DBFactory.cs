////////////////////////////////////////////////////////////////////
// DBFactory.cs - DBFactory created to demonstrate creation of    //
//               Immutable Database -                             //
// Ver 1.0                                                        //
// Application: DBFactory for CSE681-SMA, Project#4               //
// Language:    C#, ver 6.0, Visual Studio 2015                   //
// Platform:    Dell XPSL501, Core-i5, Windows 10                 //
// Source:      Jim Fawcett, CST 4-187, Syracuse University       //
//              (315) 443-3948, jfawcett@twcny.rr.com             //
// Author:      Rohit Sharma, SUID-242093353, Syracuse University //
//              (315) 935-1323, rshar102@syr.edu                  //
////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * DBFactory is created to support the requirement of an immutable database.
 * It contains a DataDictionary and does not have any Update or Insert methods
 * which makes it immutable.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBFactory.cs, DBEngine.cs, DBElement.cs. 
 *   Display.cs and UtilityExtensions.cs only if you activate test stub main method.
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
using static System.Console;

namespace Project2Starter
{
    public class DBFactory<Key, Value>
    {
        private Dictionary<Key, Value> immutableDB;//Dictionary for storing immutable database content

        //<----------Constructor for DBFactory------------->
        public DBFactory(DBEngine<Key,Value> dbe)
        {
            immutableDB = dbe.Dictionary;
        }

        //<----------Getter method for getting items from Immutable DB------------->
        public bool getValue(Key key, out Value val)
        {
            if (immutableDB.Keys.Contains(key))
            {
                val = immutableDB[key];
                return true;
            }
            val = default(Value);
            return false;
        }

        //<----------returns all keys present in Immutable DB------------->
        public IEnumerable<Key> Keys()
        {
            return immutableDB.Keys;
        }
    }

#if(TEST_DBFACTORY)
    public class TestDBFactory
    {    
        static void Main(string[] args)
        {
            WriteLine("===========Testing DBFactory package========");
            WriteLine("Demonstrating req 8 through MAIN- creation of immutable database");
            WriteLine();
            List<string> keyCollection = new List<string>() { "1", "2", "3"};
            //Changes to populate values into an immutable database object;
            DBElement<string, List<string>> dbElForDbf = null;
            DBEngine<string, DBElement<string, List<string>>> dbeForDbf = new DBEngine<string, DBElement<string, List<string>>>();
            foreach (string key in keyCollection)
            {
                dbElForDbf = new DBElement<string, List<string>>();
                dbElForDbf.name = "Name - " + key;
                dbElForDbf.descr = "Descr - " + key;
                dbElForDbf.timeStamp = DateTime.Now;
                dbElForDbf.children = new List<string>() { "child 1 - " + key, "child 2 - " + key };
                dbElForDbf.payload = new List<string>() { "payload 1 - " + key, "payload 2 - " + key };
                dbeForDbf.insert(key, dbElForDbf);
            }
            //dbeForDbf.showEnumerableDB();
            DBFactory<string, DBElement<string, List<string>>> dbf = new DBFactory<string, DBElement<string, List<string>>>(dbeForDbf);
            DBElement<string, List<string>> elemForDbf = new DBElement<string, List<string>>();
            foreach (string key in dbf.Keys())
            {
                dbf.getValue(key, out elemForDbf);
                WriteLine("---------------Key = "+key+" --------------");
                WriteLine("Name: " + elemForDbf.name);
                WriteLine("Desc: " + elemForDbf.descr);
                foreach(string child in elemForDbf.children)
                {
                    WriteLine("----Children: " + child);
                }
                foreach (string item in elemForDbf.payload)
                {
                    WriteLine("--------Payload: " + item);
                }
                WriteLine("Timestamp: " + elemForDbf.timeStamp);
            }
            WriteLine();
        }
    }
#endif
}
