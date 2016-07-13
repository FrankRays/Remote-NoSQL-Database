////////////////////////////////////////////////////////////////////
// DBEngine.cs - This package defines the NoSQL Key-Value Database//  
// Ver 1.0                                                        //
// Application: DBElement tester for CSE681-SMA, Project#4        //
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
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
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
    public class DBEngine<Key, Value>
    {
        private Dictionary<Key, Value> dbStore;// DataDictionary for storing DB items

        public DBEngine()
        {
            dbStore = new Dictionary<Key, Value>();
        }

        //<----------Insert method for storing items in DB------------->
        public bool insert(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
                return false;
            dbStore[key] = val;
            return true;
        }

        //<----------Delete method for deleting items from DB------------->
        public bool delete(Key key)
        {
            if (dbStore.Keys.Contains(key))
            {
                dbStore.Remove(key);
                return true;
            }
            return false;
        }

        //<----------Update method to update items in DB------------->
        public bool update(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                dbStore.Remove(key);
                dbStore[key] = val;
                return true;
            }
            return false;
        }

        //<----------Getter method for getting items from DB------------->
        public bool getValue(Key key, out Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                val = dbStore[key];
                return true;
            }
            val = default(Value);
            return false;
        }
        //<----------Returns all keys from DB------------->
        public IEnumerable<Key> Keys()
        {
            return dbStore.Keys;
        }

        //<----------Returns an object of DB------------->
        public Dictionary<Key, Value> Dictionary
        {
            get { return dbStore; }
        }
    }

#if (TEST_DBENGINE)

    class TestDBEngine
    {
        static void Main(string[] args)
        {
            "Testing DBEngine Package".title('=');
            WriteLine();

            Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
            Write("\n  This allow use of DBExtensions package without circular dependencies.");

            Write("\n\n");
        }
    }
#endif
}

