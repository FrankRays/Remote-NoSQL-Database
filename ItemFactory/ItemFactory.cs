////////////////////////////////////////////////////////////////////
// ItemFactory.cs - Item Factory provide methods to populate      //
//                  DBElement items                               //
// Ver 1.0                                                        //
// Application: ItemFactory for CSE681-SMA, Project#4             //
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
 * This package begins the demonstration of meeting requirements.
 * Much is left to students to finish.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   ItemFactory.cs,  DBElement.cs, DBEngine, Display, 
 *   DBExtensions.cs, UtilityExtensions.cs
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
    public class ItemFactory<Key>
    {
        //<----------method to create a DBElement of <Key,string> type------------->
        public DBElement<Key, string> createIntStringElement(string name, string desc, DateTime timestamp, List<Key> children, string payload)
        {
            DBElement<Key, string> elem = new DBElement<Key, string>();
            elem.name = name;
            elem.descr = desc;
            elem.timeStamp = timestamp;
            elem.children.AddRange(children);
            elem.payload = payload;
            return elem;
        }

        //<----------method to create a DBElement of <string, List<string>> type------------->
        public DBElement<string, List<string>> createStringListOfStringElement(string name, string desc, DateTime timestamp, List<string> children, List<string> payload)
        {
            DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
            elem.name = name;
            elem.descr = desc;
            elem.timeStamp = timestamp;
            elem.children.AddRange(children);
            elem.payload = payload;
            return elem;
        }

        //<----------method to create a DBElement of <int,string> type for CATEGORY DB------------->
        public DBElement<int, string> createIntStringElemForCategory(string name, string desc, DateTime timestamp, List<int> children, string payload, string category)
        {
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = name;
            elem.descr = desc;
            elem.timeStamp = timestamp;
            elem.children.AddRange(children);
            elem.payload = payload;
            elem.category = category;
            return elem;
        }
     }

#if(TEST_ITEMFACTORY)
    public class TestItemFactory
    {
        static void Main(string[] args)
        {
            "Testing ItemFactory Package".title();
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBEngine<int, DBElement<int, string>> categorydb = new DBEngine<int, DBElement<int, string>>();
            CategoryDBEngine<int> categorydbEngine = new CategoryDBEngine<int>();
            DBEngine<string, DBElement<string, List<string>>> db1 = new DBEngine<string, DBElement<string, List<string>>>();
            ItemFactory<int> itemFactoryObj = new ItemFactory<int>();

            DBElement<int, string> elem = itemFactoryObj.createIntStringElement
                ("element", "test element", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload");
            db.insert(1, elem);
            //db.showDB();
            DBElement<string, List<string>> elem2 = itemFactoryObj.createStringListOfStringElement
                ("Vaibhav", "Joshi", DateTime.Now, new List<string> { "4", "5", "6" }, new List<string> { "a", "b" });
            db1.insert("ER ar", elem2);
            //db1.showEnumerableDB();
            WriteLine();

            DBElement<int, string> elem3 = itemFactoryObj.createIntStringElemForCategory
                ("element10", "test element10", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload10", "Category1");
            categorydb.insert(101, elem3);
            categorydbEngine.insertCategoryDb("Category1", new List<int> { 101 });

            DBElement<int, string> elem4 = itemFactoryObj.createIntStringElemForCategory
                ("element20", "test element20", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload20", "Category2");
            categorydb.insert(201, elem4);
            categorydbEngine.insertCategoryDb("Category2", new List<int> { 201 });

            DBElement<int, string> elem5 = itemFactoryObj.createIntStringElemForCategory
                            ("element30", "test element30", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload30", "Category1");
            categorydb.insert(301, elem5);
            categorydbEngine.insertCategoryDb("Category1", new List<int> { 301 });
            categorydbEngine.showCategoryDB(categorydb);

            WriteLine();
        }
    }
#endif
}
