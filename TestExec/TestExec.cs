////////////////////////////////////////////////////////////////////
// TestExec.cs - Test Requirements for Project #4                 //
// Ver 1.2                                                        //
// Application: Test Executive for CSE681-SMA, Project#4           //
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
 *   TestExec.cs,  DBElement.cs, DBEngine, Display, 
 *   DBExtensions.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * Self Implemented in Proj#2 - Reused in Proj#4 - 23 Nov 2015
 * ver 1.2 : 09 Oct 15
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Project2Starter
{
    class TestExec
    {
        private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
        private DBEngine<string, DBElement<string, List<string>>> db1 = new DBEngine<string, DBElement<string, List<string>>>();
        private DBEngine<int, DBElement<int, string>> categorydb = new DBEngine<int, DBElement<int, string>>();
        private CategoryDBEngine<int> categorydbEngine = new CategoryDBEngine<int>();
        private ItemEditor<int, DBElement<int, string>> item = new ItemEditor<int, DBElement<int, string>>();
        private ItemEditor<string, DBElement<string, List<string>>> item1 = new ItemEditor<string, DBElement<string, List<string>>>();
        private PersistEngine<int, DBElement<int, string>> pe = new PersistEngine<int, DBElement<int, string>>();
        private RestoreEngine<int, DBElement<int, string>> re = new RestoreEngine<int, DBElement<int, string>>();
        private QueryEngine<string> qe = new QueryEngine<string>();
        private DBFactory<string, DBElement<string, List<string>>> dbf = null;
        private DBEngine<string, DBElement<string, List<string>>> dbEngineForDbfactory = new DBEngine<string, DBElement<string, List<string>>>();
        private ItemFactory<int> itemFactoryObj = new ItemFactory<int>();
        private Regex regex = null;
        private Regex regex1 = null;
        private List<string> keyCollection = new List<string>();
        private static string searchKey = "The Epic"; //constant defind to find values and children based on this key.
        private static string keySearchPattern = ".*ew.*";//constant define to find all the keys matching this pattern.
        private static string metadataSearchPattern = "ha";//constant defined to find all records matching this string in metadata.
        private static string catSearchString = "Category1";//constant defined to find all records matching this category.

        void TestR2()
        {
            "Demonstrating Requirement #2".title('=');
            WriteLine();
            "Demonstrating <int-string> database".title();
            DBElement<int, string> elem = itemFactoryObj.createIntStringElement
                ("Vishal", "Laad", DateTime.Now, new List<int> { 2, 3 }, "elem's payload");
            db.insert(1, elem);

            DBElement<int, string> elem1 = itemFactoryObj.createIntStringElement
                  ("Samuel", "Jackson", DateTime.Now, new List<int> { 1 }, "This is payload"); ;
            db.insert(2, elem1);

            DBElement<int, string> elem2 = itemFactoryObj.createIntStringElement
                  ("Rohit", "Sharma", DateTime.Now, new List<int> { 2 }, "This is payload"); ;
            db.insert(3, elem2);
            db.showDB();
            WriteLine();
            "Demonstrating <string-List<string>> database".title();
            DBElement<string, List<string>> elem3 = itemFactoryObj.createStringListOfStringElement
                ("Vaibhav", "Joshi", DateTime.Now, new List<string> { "Afzal"}, new List<string> { "a", "b" });
            db1.insert("Er Vaibav", elem3);

            DBElement<string, List<string>> elem4 = itemFactoryObj.createStringListOfStringElement
                ("Afzal", "Khan", DateTime.Now, new List<string> { "Mahesh", "Ramesh", "Suresh" }, new List<string> { "Best", "Friend" });
            db1.insert("My Friend", elem4);
            db1.showEnumerableDB();
            WriteLine();
            ReadKey();
        }
        void TestR3()
        {
            "Demonstrating Requirement #3".title('=');
            "Adding a new element  with Key 4 in <int-string> DB".title();
            DBElement<int, string> elem = itemFactoryObj.createIntStringElement
                ("Matt", "Demon", DateTime.Now, new List<int> { 2, 3 }, "The Martian");
            db.insert(4, elem);
            "Updated DB after addition of new element".title();
            db.showDB();
            WriteLine();
            "Adding a new element with Key \"The Epic\" in <string-List<string>> DB".title();
            DBElement<string, List<string>> elem1 = itemFactoryObj.createStringListOfStringElement
                ("Ramayana", "Mahabharata", DateTime.Now, new List<string> { "Ram", "Arjun", "Krishna" }, new List<string> { "Warriors", "Clan" });
            db1.insert("The Epic", elem1);
            "Updated DB after addition of new element".title();
            db1.showEnumerableDB();
            WriteLine();
            "Demonstrating deletion of keys".title();
            WriteLine();
            "Deleting Key 2 from <int-string> DB".title();
            bool response = db.delete(2);
            if (response)
                "Key 2 Deleted".title();
            else
                "Problem deleting key 2".title();
            "Updated DB after deletion of key".title();
            db.showDB();
            WriteLine();
            "Deleting Key \"Er Vaibav\" from <string-List<string>> DB".title();
            WriteLine();
            bool response1 = db1.delete("Er Vaibav");
            if (response1)
                "Key \"Er Vaibav\" Deleted".title();
            else
                "Problem deleting key \"Er Vaibav\"".title();
            "Updated DB after deletion of key".title();
            db1.showEnumerableDB();
            WriteLine();
            ReadKey();
        }
        void TestR4()
        {
            item.DBEngine = db;
            "Demonstrating Requirement #4".title('=');
            "Demonstrating editing of value".title();
            DBElement<int, string> elem = new DBElement<int, string>();
            item.updatePayloadString(1, "UPDATED");
            WriteLine();
            "Updated Key 1 payload. Showing UPDATED value in DB".title();
            db.showDB();
            WriteLine();
            "Demonstrating addition/deletion of children".title();
            item.updateChildren(1, new List<int> { 2, 3, 4 });
            item.updateChildren(4, new List<int> { 3 });
            "Added Child 4 to Key 1. Deleted Child 2 from Key 4. Showing updated children in DB".title();
            db.showDB();
            WriteLine();
            "Demonstrating metadata update. Updating description for Key 1".title();
            elem.descr = "UPDATED";
            item.updateDesc(1, elem.descr);
            "Showing updated metadata in DB".title();
            db.showDB();
            WriteLine();
            DBElement<int, string> elem4 = itemFactoryObj.createIntStringElement
                ("UPDATED element - complete object", "UPDATED desc - complete object", new DateTime(2050, 10, 13), new List<int> { 10, 22, 33 }, "UPDATED elem's payload - complete object");
            item.updateInstance(1, elem4, db);
            "Showing updated instance in DB".title();
            db.showDB();
            WriteLine();
            ReadKey();
        }
        void TestR5()
        {
            "Demonstrating Requirement #5".title('=');
            WriteLine();
            "Persisting int-string XML into Database".title();
            pe.createStringXML(db,false);
            "Persisting string-List<string> XML into Database".title();
            pe.createListStringXML(db1);
            "Augmenting database with a int-string XML".title();
            re.augmentDB(db);
            WriteLine();
            "Current <int,string> database state".title();
            db.showDB();
            WriteLine();
            "Augmenting database with a string-List<string> XML".title();
            re.augmentStringDB(db1);
            "Current <string,List<string>> database state".title();
            db1.showEnumerableDB();
            WriteLine();
            ReadKey();
        }
        void TestR6()
        {
            "Demonstrating Requirement #6".title('=');
            WriteLine();
            "Demonstrating scheduled persistence of database into XML".title();
            Scheduler<int, DBElement<int, string>> scheduler = new Scheduler<int, DBElement<int, string>>(db);
            scheduler.scheduler.Enabled = false;
            WriteLine();
            ReadKey();
        }
        void TestR7()
        {
            "Demonstrating Requirement #7".title('=');
            WriteLine();
            "Demonstrating query - value of a specified key with <string,List<string>> database".title();
            WriteLine();
            
            List<string> values = qe.getValueByKey(searchKey, db1);
            qe.displayQueryResultForValues(searchKey, values);
            WriteLine();

            "Demonstrating query - Children of a specified key with <string,List<string>> database".title();
            WriteLine();
            List<string> children = qe.getChildrenByKey(searchKey, db1);
            qe.displayQueryResultForValues(searchKey, children);
            WriteLine();
            "Current String DB state ".title('=');
            WriteLine();
            db1.showEnumerableDB();
            WriteLine();

            "Demonstrating query - Keys matching a given pattern with <string,List<string>> database".title();
            WriteLine();
            
            regex = new Regex(keySearchPattern);
            Func<string, bool> query = qe.defineSearchQuery(regex);
            
            qe.processQuery(query, out keyCollection, db1);
            qe.displayQueryResultForKeys(regex, keyCollection);
            WriteLine();

            "Demonstrating query - Keys having the given pattern in their metadata with <string,List<string>> database".title();
            WriteLine();
            
            regex1 = new Regex(metadataSearchPattern);
            qe.displayQueryResultForKeys(regex1, qe.getKeysByMetadataSearch(metadataSearchPattern, db1));

            "Demonstrating query - Keys having values written within a specified time-date interval with <string,List<string>> database".title();
            WriteLine();
            DateTime fromDate = new DateTime(2015, 10, 03);
            DateTime toDate = new DateTime(2015, 10, 08);
            qe.displayQueryResultForTimestamp(fromDate, toDate, qe.getKeysByTimeInterval(fromDate, toDate, db1));
            WriteLine();
            ReadKey();
        }
        void TestR8()
        {
            "Demonstrating Requirement #8 - creation of immutable database".title('=');
            "Immutable database contains the search result of pattern matching on keys".title();
            WriteLine();
            //Changes to populate values into an immutable database object;
            DBElement<string, List<string>> dbElemForDbfactory = new DBElement<string, List<string>>();
            foreach (string key in keyCollection)
            {
                db1.getValue(key, out dbElemForDbfactory);
                dbEngineForDbfactory.insert(key, dbElemForDbfactory);
            }
            dbf = new DBFactory<string, DBElement<string, List<string>>>(dbEngineForDbfactory);
            dbf.showEnumerableDBFactory();
            WriteLine();
            "Immutable database created -- go to DBFactory.cs in DBFactory package and verify that it doesn't have any updated or set method.".title();
            WriteLine();
            ReadKey();
        }

        void TestR9()
        {
            "Demonstrating requirement #9".title('=');
            WriteLine();
            "Showing Package Descriptor XML".title();
            WriteLine();
            re.loadDescriptorXml();
            WriteLine();
            ReadKey();
        }
        void TestR12()
        {
            "Demonstrating requirement 12".title('=');
            WriteLine();
            DBElement<int, string> elem1 = itemFactoryObj.createIntStringElemForCategory
                ("element10", "test element10", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload10", "Category1");
            categorydb.insert(101, elem1);
            categorydbEngine.insertCategoryDb("Category1", new List<int> { 101 });

            DBElement<int, string> elem2 = itemFactoryObj.createIntStringElemForCategory
                ("element20", "test element20", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload20", "Category2");
            categorydb.insert(201, elem2);
            categorydbEngine.insertCategoryDb("Category2", new List<int> { 201 });

            DBElement<int, string> elem3 = itemFactoryObj.createIntStringElemForCategory
                ("element30", "test element30", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload30", "Category1");
            categorydb.insert(301, elem3);
            categorydbEngine.insertCategoryDb("Category1", new List<int> { 301 });

            DBElement<int, string> elem4 = itemFactoryObj.createIntStringElemForCategory
                ("element40", "test element40", DateTime.Now, new List<int> { 1, 2, 3 }, "elem's payload40", "Category3");
            categorydb.insert(401, elem4);
            categorydbEngine.insertCategoryDb("Category3", new List<int> { 401 });

            "New DB instance created for demonstrating bonus requirement 12".title();
            categorydb.showDB();
            WriteLine();
            categorydbEngine.showCategoryDB(categorydb);
            ("Showing DB Elements for "+ catSearchString).title();
            qe.getValueByCategory(catSearchString, categorydb, categorydbEngine);
        }
        static void Main(string[] args)
        {
            TestExec exec = new TestExec();
            "Demonstrating Project#2 Requirements".title('=');
            WriteLine();
            exec.TestR2();
            exec.TestR3();
            exec.TestR4();
            exec.TestR5();
            exec.TestR6();
            exec.TestR7();
            exec.TestR8();
            exec.TestR9();
            exec.TestR12();
            Write("\n\n");
        }
    }
}
