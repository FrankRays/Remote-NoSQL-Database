////////////////////////////////////////////////////////////////////
// ItemEditor.cs - This package is used to edit the items present //
//                  in database                                   //
// Ver 1.0                                                        //
// Application: Item Editor for CSE681-SMA, Project#4             //
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
 * This package provides methods to edit the items present in database
 *
 * Maintenance:
 * ------------
 * Required Files: 
 *   ItemEditor.cs,  DBElement.cs, DBEngine, UtilityExtensions.cs
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
    public class ItemEditor<Key, Value>
    {
        private DBElement<int, string> dbElement = new DBElement<int, string>();
        public DBEngine<Key, DBElement<Key, string>> DBEngine
        {
            get; set;
        }
        //<----------method to update DB instance------------->
        public bool updateInstance(Key key, Value value, DBEngine<Key,Value> newDbChecker) {
        Value existValue;
            try
            {
                foreach (Key k in DBEngine.Keys()) {
                    if (key.Equals(k)) {
                        existValue = value;
                        newDbChecker.update(key, existValue);
                        break;
                    }
                    else
                    {
                        "Key not found in database. Please provide valid key.".title();
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                "There was an error updating the instance in database.".title();
                WriteLine(e.Message);
                return false;
            }
      }
        //<----------method to update children of a key in DB------------->
        public bool updateChildren(Key key, List<Key> children) { 
            foreach (Key k in DBEngine.Keys()) {
                if (key.Equals(k))
                {
                    DBEngine.Dictionary[k].children = children;
                    break;
                }
            }
            return true;
        }

        //<----------method to update Name stored against a Key in DB------------->
        public bool updateName(Key key, string name)
        {
            foreach (Key k in DBEngine.Keys())
            {
                if (key.Equals(k))
                {
                    DBEngine.Dictionary[k].name = name;
                    break;
                }
            }
            return true;
        }

        //<----------method to update Desc stored against a Key in DB------------->
        public bool updateDesc(Key key, string desc)
        {
            foreach (Key k in DBEngine.Keys())
            {
                if (key.Equals(k))
                {
                    DBEngine.Dictionary[k].descr = desc;
                    break;
                }
            }
            return true;
        }

        //<----------method to update Timestamp stored against a Key in DB------------->
        public bool updateTimestamp(Key key, DateTime timestamp)
        {
            foreach (Key k in DBEngine.Keys())
            {
                if (key.Equals(k))
                {
                    DBEngine.Dictionary[k].timeStamp = timestamp;
                    break;
                }
            }
            return true;
        }

        //<----------method to update String Payload stored against a Key in DB------------->
        public bool updatePayloadString(Key key, string payload)
        {
            foreach (Key k in DBEngine.Keys())
            {
                if (key.Equals(k))
                {
                    DBEngine.Dictionary[k].payload = payload;
                    break;
                }
            }
            return true;
        }

        //<----------method to update List Payload stored against a Key in DB------------->
        public bool updateListPayload(Key key, List<string> listPayload)
        {
            foreach (Key k in DBEngine.Keys())
            {
                if (key.Equals(k))
                {
                    StringBuilder data = new StringBuilder();
                    foreach (string str in listPayload)
                    {
                        data.Append(str +" ");
                    }
                    DBEngine.Dictionary[k].payload = data.ToString();
                    break;
                }
            }
            return true;
        }
    }

#if (TEST_ITEMEDITOR)
    class TestItemEditor
    { 
        static void Main(string[] args)
        {
            "Testing Item Editor Package".title();
            DBEngine<int, DBElement<int, string>> dbChecker = new DBEngine<int, DBElement<int, string>>();
            ItemEditor<int, DBElement<int, string>> item = new ItemEditor<int, DBElement<int,string>> ();

            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "updated element 1";
            elem.descr = "updated test element 1";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 2, 3, 4});
            elem.payload = "updated elem's payload 1";
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.name = "updated element 2";
            elem1.descr = "updated test element 2";
            elem1.timeStamp = DateTime.Now;
            elem1.children.AddRange(new List<int> { 20, 30, 40 });
            elem1.payload = "updated elem's payload 2";
            DBElement<int, string> elem2 = new DBElement<int, string>();
            elem2.name = "updated element 3";
            elem2.descr = "updated test element 3";
            elem2.timeStamp = DateTime.Now;
            elem2.children.AddRange(new List<int> { 200, 300, 400 });
            elem2.payload = "updated elem's payload 3";
            //item.updateInstance(1, elem, dbChecker);
            dbChecker.insert(1, elem);
            dbChecker.insert(20, elem1);
            dbChecker.insert(300, elem2);
            Console.WriteLine("========= Current DB State ======");
            dbChecker.showDB();
            Console.WriteLine();
            item.DBEngine = dbChecker;
            item.updateName(1, "Rohit Mehra");
            item.updateDesc(20, "Rohit Mehra");
            item.updateTimestamp(20, new DateTime(2050, 01, 01));
            item.updateChildren(300, new List<int>() { 1000, 2000, 3000});
            item.updatePayloadString(300, "Rohit Mehra");
            Console.WriteLine();
            Console.WriteLine("========= New DB State ======");
            Console.WriteLine();
            dbChecker.showDB();
            Console.WriteLine();
        }
    }
#endif
}
