////////////////////////////////////////////////////////////////////
// CategoryDBEngine.cs - DBEngine created for category DB.        //
// Ver 1.0                                                        //
// Application: CategoryDBEngine for CSE681-SMA, Project#4        //
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
 * This package is used to demonstrate requirement 12 for showing database
 * content based on category.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBEngine.cs, CategoryDBEngine.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * Self Implemented in Proj#2 - Reused in Proj#4 - 23 Nov 2015
 * - first release - 10/09/2015
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{

    public class CategoryDBEngine<Key>
    {
        private Dictionary<string, List<Key>> categoryDbStore; // DataDictionary for storing items by category
        public CategoryDBEngine()
        {
            categoryDbStore = new Dictionary<string, List<Key>>();
        }

        //<----------Insert method for storing items in CategoryDB------------->
        public bool insertCategoryDb(string key, List<Key> val)
        {
            if (categoryDbStore.Keys.Contains(key))
            {
                List<Key> newVal;
                getCategoryValue(key, out newVal);
                newVal.AddRange(val);
                categoryDbStore[key] = newVal;
                return true;
            }
            else
            {
                categoryDbStore[key] = val;
                return true;
            }
        }

        //<----------Getter method for getting items from CategoryDB------------->
        public bool getCategoryValue(string key, out List<Key> val)
        {
            val = new List<Key>();
            if (categoryDbStore.Keys.Contains(key))
            {
                val = categoryDbStore[key];
                return true;
            }
            val = default(List<Key>);
            return false;
        }

        //<----------Returns all Keys from CategoryDB------------->
        public IEnumerable<string> CategoryKeys()
        {
            return categoryDbStore.Keys as IEnumerable<string>;
        }

        //<----------Returns instance of CategoryDB------------->
        public Dictionary<string, List<Key>> Dictionary
        {
            get { return categoryDbStore; }
        }

    }

#if (TEST_CATEGORYDBENGINE)
    public class TestCategoryDBEngine
    {

        static void Main(string[] args)
        {
            DBEngine<int, DBElement<int, string>> categorydb = new DBEngine<int, DBElement<int, string>>();
            CategoryDBEngine<int> categorydbEngine = new CategoryDBEngine<int>();
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "element20";
            elem.descr = "test element20";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "elem's payload20";
            elem.category = "Category2";
            categorydb.insert(201, elem);
            categorydbEngine.insertCategoryDb("Category2", new List<int> { 201 });
        }
    }
#endif
}
