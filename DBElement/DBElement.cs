////////////////////////////////////////////////////////////////////
// DBElement.cs - Define Element for NoSQL Database.              //
// Ver 1.0                                                        //
// Application: DBElement for CSE681-SMA, Project#4               //
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
 * This package implements the DBElement<Key, Data> type, used by 
 * DBEngine<key, Value> where Value is DBElement<Key, Data>.
 *
 * The DBElement<Key, Data> state consists of metadata and an
 * instance of the Data type.
 *
 * I intend this DBElement type to be used by both:
 *
 *   ItemFactory - used to ensure that all db elements have the
 *                 same structure even if built by different
 *                 software parts.
 *   ItemEditor  - used to ensure that db elements are edited
 *                 correctly and maintain the intended structure.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs, UtilityExtensions.cs
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
    /////////////////////////////////////////////////////////////////////
    // DBElement<Key, Data> class
    // - Instances of this class are the "values" in our key/value 
    //   noSQL database.
    // - Key and Data are unspecified classes, to be supplied by the
    //   application that uses the noSQL database.
    //   See the teststub below for examples of use.

    
    public class DBElement<Key, Data>
  {
    public string name { get; set; }          // metadata    |
    public string descr { get; set; }         // metadata    |
    public DateTime timeStamp { get; set; }   // metadata   value
    public List<Key> children { get; set; }   // metadata    |
    public Data payload { get; set; }         // data        |
    public string category { get; set; }      // metadata    |
    public DBElement(string Name = "unnamed", string Descr = "undescribed", string Default= "default")
    {
      name = Name;
      descr = Descr;
      timeStamp = DateTime.Now;
      children = new List<Key>();
      category = Default;
    }
  }

#if (TEST_DBELEMENT)
  class TestDBElement
  {
    static void Main(string[] args)
    {
      "Testing DBElement Package".title('=');
      WriteLine();

      Write("\n  All testing of DBElement class moved to DBElementTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
