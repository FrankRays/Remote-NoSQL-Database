////////////////////////////////////////////////////////////////////
// DBExtensions.cs - Define extension methods for Display         //  
// Ver 1.0                                                        //
// Application: Defines extension methods for CSE681-SMA,Project#4//
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
 * This package implements extensions methods to support 
 * displaying DBElements and DBEngine instances.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBExtensions.cs, DBEngine.cs, DBElement.cs, UtilityExtensions
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
    public static class DBElementExtensions
    {
        //----< write metadata to string >-------------------------------------

        public static string showMetaData<Key, Data>(this DBElement<Key, Data> elem)
        {   
            if (elem != null)
            {
                StringBuilder accum = new StringBuilder();
                accum.Append(String.Format("\n  name: {0}", elem.name));
                accum.Append(String.Format("\n  desc: {0}", elem.descr));
                accum.Append(String.Format("\n  time: {0}", elem.timeStamp));
                accum.Append(String.Format("\n  category: {0}", elem.category));
                if (elem.children.Count() > 0)
                {
                    accum.Append(String.Format("\n  Children: "));
                    bool first = true;
                    foreach (Key key in elem.children)
                    {
                        if (key != null)
                        {
                            if (first)
                            {
                                accum.Append(String.Format("{0}", key.ToString()));
                                first = false;
                            }
                            else
                                accum.Append(String.Format(", {0}", key.ToString()));
                        }
                    }
                }
                return accum.ToString();
            }
            return null;
        }
        //----< write details of element with simple Data to string >----------

        public static string showElement<Key, Data>(this DBElement<Key, Data> elem)
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(elem.showMetaData());
            if (elem!=null && elem.payload != null)
            {
                accum.Append(String.Format("\n  payload: {0}", elem.payload.ToString()));
            }
            return accum.ToString();
        }
        //----< write details of element with enumerable Data to string >------

        public static string showElement<Key, Data, T>(this DBElement<Key, Data> elem)
          where Data : IEnumerable<T>  // constraint clause
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(elem.showMetaData());
            if (elem.payload != null)
            {
                IEnumerable<object> d = elem.payload as IEnumerable<object>;
                if (d == null)
                    accum.Append(String.Format("\n  payload: {0}", elem.payload.ToString()));
                else
                {
                    bool first = true;
                    accum.Append(String.Format("\n  payload:  "));
                    foreach (var item in elem.payload)  // won't compile without constraint clause
                    {
                        if (first)
                        {
                            accum.Append(String.Format("{0}", item));
                            first = false;
                        }
                        else
                            accum.Append(String.Format(", {0}", item));
                    }
                }
            }
            return accum.ToString();
        }
    }
    public static class DBEngineExtensions
    {
        //----< write simple db elements out to Console >------------------

        public static void show<Key, Value, Data>(this DBEngine<Key, Value> db)
        {
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  -- key = {0} --", key);
                Write(elem.showElement());
            }
        }
        //----< write enumerable db elements out to Console >--------------
        public static void show<Key, Value, Data, T>(this DBEngine<Key, Value> db)
          where Data : IEnumerable<T>
        {
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  -- key = {0} --", key);
                Write(elem.showElement<Key, Data, T>());
            }
        }
        //----< write enumerable db factory elements out to Console >--------------
        public static void show<Key, Value, Data, T>(this DBFactory<Key, Value> db)
          where Data : IEnumerable<T>
        {
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  -- key = {0} --", key);
                Write(elem.showElement<Key, Data, T>());
            }
        }

        //----< Extension method for showing items in CategoryDB >--------------
        public static void showCategoryDBExt(this CategoryDBEngine<int> cdb, DBEngine<int, DBElement<int, string>> dbe)
        {
            if (cdb != null)
            { 
                foreach (string key in cdb.CategoryKeys())
                {
                    List<int> value = new List<int>();
                    try
                    {
                        cdb.getCategoryValue(key, out value);
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Exception while getting the values" + ex.Message);
                    }
                    WriteLine();
                    Write("  " + key + " : ");
                    foreach (int val in value)
                    {
                        Write(" " + val);
                    }
                }
            }
            else
            {
                WriteLine("Category DB object is null");
            }
        }
    }

#if (TEST_DBEXTENSIONS)

    class TestDBExtensions
    {
        static void Main(string[] args)
        {
            "Testing DBExtensions Package".title('=');
            WriteLine();

            Write("\n --- Test DBElement<int,string> ---");
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            Write(elem1.showElement<int, string>());

            DBEngine<int, DBElement<int, string>> dbs = new DBEngine<int, DBElement<int, string>>();
            dbs.insert(1, elem1);
            dbs.show<int, DBElement<int, string>, string>();
            WriteLine();

            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.children = new List<string> { "Key1", "Key2" };
            newelem1.payload = new List<string> { "one", "two", "three" };
            Write(newelem1.showElement<string, List<string>, string>());

            DBEngine<string, DBElement<string, List<string>>> dbe = new DBEngine<string, DBElement<string, List<string>>>();
            dbe.insert("key1", newelem1);
            dbe.show<string, DBElement<string, List<string>>, List<string>, string>();

            Write("\n\n");
        }
    }
#endif
}
