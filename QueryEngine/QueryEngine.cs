////////////////////////////////////////////////////////////////////
// QueryEngine.cs - QueryEngine provides methods to query         //
//                  DataDictionary                                //
// Ver 1.0                                                        //
// Application: Query Engine for CSE681-SMA, Project#4            //
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
 * This package provies methods to query data dictionary.
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
using System.Text.RegularExpressions;

namespace Project2Starter
{
    public class QueryEngine<Key>
    {
        //<----------method to get Values from the database for the provided Key------------->
        public List<string> getValueByKey(Key key, DBEngine<Key, DBElement<Key, List<string>>> db)
        {
            List<string> queryResult = new List<string>();
            foreach(Key k in db.Keys())
            {
                if (k.Equals(key))
                {
                    DBElement<Key, List<string>> s = new DBElement<Key, List<string>>();
                    db.getValue(k, out s);
                    foreach (string str in s.payload)
                    {
                        queryResult.Add(str);
                    }
                }
            }
            return queryResult;
        }

        //<----------method to get children from the database for the provided Key------------->
        public List<string> getChildrenByKey(Key key, DBEngine<Key, DBElement<Key, List<string>>> db)
        {
            List<string> queryResult = new List<string>();
            foreach (Key k in db.Keys())
            {
                if (k.Equals(key))
                {
                    DBElement<Key, List<string>> s = new DBElement<Key, List<string>>();
                    db.getValue(k, out s);
                    foreach(string str in s.children as List<string>)
                    {
                        queryResult.Add(str);
                    }
                }
            }
            return queryResult;
        }

        //<----------method to display query results------------->
        public void displayQueryResultForValues(Key key, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                foreach (string s in values)
                {
                    WriteLine("found \"{0}\" in database against key \"{1}\"", s, key);
                }
            }
            else
            {
                WriteLine("Nothing is found in database against key \"{0}\"", key);
            }
        }

        //<----------method to create a Query Predicate------------->
        public Func<string, bool> defineSearchQuery(Regex reg)
        {
            Func<string, bool> queryPredicate = (string keyInDb) =>
            {
                if (keyInDb == null || keyInDb.Length <= 0)
                {
                    return false;
                }
                if ("".Equals(reg.ToString()))
                {
                    return true;
                }
                if (reg.IsMatch(keyInDb))
                {
                    return true;
                }
                return false;
            };
            return queryPredicate;
        }

        //<----------method to process query based on Query Predicate------------->
        public bool processQuery(Func<string, bool> queryPredicate, out List<string> keyCollection, DBEngine<Key, DBElement<Key, List<string>>> db)
        {
            keyCollection = new List<string>();
            foreach (string k in db.Keys() as IEnumerable<string>)
            {
                if (queryPredicate(k))
                {
                    keyCollection.Add(k);
                }
            }
            if (keyCollection.Count() > 0)
                return true;
            return false;
        }

        //<----------method to display Keys from the database based on search pattern------------->
        public void displayQueryResultForKeys(Regex regex, List<string> keyCollection)
        {
            if (keyCollection != null && keyCollection.Count > 0)
            {
                if ("".Equals(regex.ToString()))
                {
                    WriteLine("No pattern is specified. Keys present in database are as below: ");
                }
                foreach (string s in keyCollection)
                {
                    if ("".Equals(regex.ToString()))
                    {
                        WriteLine("Key:  \"{0}\"", s);
                    }
                    else
                    {
                        WriteLine("Key found \"{0}\" in database matching the given pattern \"{1}\"", s, regex);
                    }
                }
            }
            else
            {
                WriteLine("Nothing is found in database matching the given pattern \"{0}\"", regex);
            }
        }
        
        //<----------method to get Keys from the database if pattern found in metadata------------->
        public List<string> getKeysByMetadataSearch(string searchPattern, DBEngine<Key, DBElement<Key, List<string>>> db)
        {
            List<string> queryResult = new List<string>();
            foreach (Key k in db.Keys())
            {
                DBElement<Key, List<string>> s = new DBElement<Key, List<string>>();
                db.getValue(k, out s);
                if (s.name!=null && s.name.Contains(searchPattern))
                {
                    queryResult.Add(k as string);
                }
                else if (s.descr!=null && s.descr.Contains(searchPattern))
                {
                    queryResult.Add(k as string);
                }
                else if (s.timeStamp!=null && s.timeStamp.ToString().Contains(searchPattern))
                {
                    queryResult.Add(k as string);
                }
            }
            return queryResult;
        }

        //<----------method to get Keys from the database for the provided timeinterval------------->
        public List<string> getKeysByTimeInterval(DateTime fromDate, DateTime toDate, DBEngine<Key, DBElement<Key, List<string>>> db)
        {
            List<string> queryResult = new List<string>();
            foreach (Key k in db.Keys())
            {
                DBElement<Key, List<string>> s = new DBElement<Key, List<string>>();
                db.getValue(k, out s);
                if (fromDate == default(DateTime))
                {
                    fromDate = DateTime.Now;
                }
                if (toDate == default(DateTime))
                {
                    toDate = DateTime.Now;
                }
                if (s.timeStamp >= fromDate && s.timeStamp <= toDate)
                {
                    queryResult.Add(k as string);
                }
            }
            return queryResult;
        }

        //<----------method to display result for the query based on timestamp------------->
        public void displayQueryResultForTimestamp(DateTime fromDate, DateTime toDate, List<string> keyCollection)
        {
            if (fromDate == default(DateTime))
            {
                fromDate = DateTime.Now;
            }
            if (toDate == default(DateTime))
            {
                toDate = DateTime.Now;
            }
            if (keyCollection != null && keyCollection.Count > 0)
            {
                foreach (string s in keyCollection)
                {
                    WriteLine("Key \"{0}\" was inserted in database between \"{1}\" and \"{2}\"", s, fromDate, toDate);
                }
            }
            else
            {
                WriteLine("Nothing is found in database between \"{0}\" and \"{1}\"", fromDate, toDate);
            }
        }

        //<----------method to get Values based on category------------->
        public List<string> getValueByCategory(string category, DBEngine<int, DBElement<int, string>> dbe, CategoryDBEngine<int> cdb)
        {
            DBEngine<int, DBElement<int, string>> dbeLocal = new DBEngine<int, DBElement<int, string>>();
            List<string> queryResult = new List<string>();
            foreach (string cat in cdb.CategoryKeys())
            {
                if (category.Equals(cat))
                {
                    List<int> list = new List<int>();
                    cdb.getCategoryValue(category, out list);

                    foreach (int key in list)
                    {
                            queryResult.Add(key.ToString());
                            DBElement<int, string> elem = new DBElement<int, string>();
                            dbe.getValue(key, out elem);
                            dbeLocal.insert(key, elem);
                    }
                }
            }
            Console.WriteLine("\nInside getValueByCat: - showing local db");
            dbeLocal.showDB();
            Console.WriteLine("\nInside getValueByCat: - showing query result"+queryResult);
            return queryResult;
        }
    }

#if (TEST_QUERYENGINE)
    class TestQueryEngine
    {
        static void Main(string[] args)
        {
            Regex regex = null;
            Regex regex1 = null;
            QueryEngine<string> qe = new QueryEngine<string>();
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBEngine<string, DBElement<string, List<string>>> db1 = new DBEngine<string, DBElement<string, List<string>>>();

            DBElement<string, List<string>> elem2 = new DBElement<string, List<string>>();
            elem2.name = "Vaibhav";
            elem2.descr = "Joshi";
            elem2.timeStamp = DateTime.Now;
            elem2.children.AddRange(new List<string> { "4", "5", "6" });
            elem2.payload = new List<string> { "a", "b" };
            db1.insert("ER ar", elem2);
            WriteLine();

            DBElement<string, List<string>> elem3 = new DBElement<string, List<string>>();
            elem3.name = "Afzal";
            elem3.descr = "Khan";
            elem3.timeStamp = DateTime.Now;
            elem3.children.AddRange(new List<string> { "Mahesh", "Ramesh", "Suresh" });
            elem3.payload = new List<string> { "Best", "Friend" };
            db1.insert("MeraYaar", elem3);
            WriteLine();

            DBElement<string, List<string>> elem4 = new DBElement<string, List<string>>();
            elem4.name = "Ramaayman";
            elem4.descr = "Mahabharat";
            elem4.timeStamp = DateTime.Now;
            elem4.children.AddRange(new List<string> { "Mahesh", "Ramesh", "Suresh" });
            elem4.payload = new List<string> { "Best", "Friend" };
            db1.insert("Ramayan ar", elem4);
            WriteLine();
            "Testing QueryEngine Package".title();
            WriteLine();

            "Demonstrating query - value of a specified key with string-List of Strings database".title();
            WriteLine();
            string searchKey = "MeraYaar";
            List<string> values = qe.getValueByKey(searchKey, db1);
            qe.displayQueryResultForValues(searchKey, values);
            WriteLine();

            "Demonstrating query - Children of a specified key with string-List of Strings database".title();
            WriteLine();
            List<string> children = qe.getChildrenByKey(searchKey, db1);
            qe.displayQueryResultForValues(searchKey, children);
            WriteLine();
            "---------------Current String DB state --------".title();
            WriteLine();
            db1.showEnumerableDB();
            WriteLine();

            "Demonstrating query - Keys matching a given pattern with string-List of Strings database".title();
            WriteLine();
            string searchPattern = "ay";
            regex = new Regex(searchPattern);
            Func<string, bool> query = qe.defineSearchQuery(regex);
            List<string> keyCollection;
            qe.processQuery(query, out keyCollection, db1);
            qe.displayQueryResultForKeys(regex, keyCollection);
            WriteLine();

            "Demonstrating query - Keys having the given pattern in their metadata with string-List of Strings database".title();
            WriteLine();
            string metadataSearchPattern = "Ed e";
            regex1 = new Regex(metadataSearchPattern);
            List<string> keyCollectionMetdataSearch;
            keyCollectionMetdataSearch = qe.getKeysByMetadataSearch(metadataSearchPattern, db1);
            qe.displayQueryResultForKeys(regex1, keyCollectionMetdataSearch);

            "Demonstrating query - Keys having values written within a specified time-date interval with string-List of Strings database".title();
            WriteLine();
            DateTime fromDate = new DateTime(2015, 10, 05);
            DateTime toDate = new DateTime(2015, 10, 07);
            qe.displayQueryResultForTimestamp(fromDate, toDate, qe.getKeysByTimeInterval(fromDate, toDate, db1));
            WriteLine();
        }
    }
#endif
}
