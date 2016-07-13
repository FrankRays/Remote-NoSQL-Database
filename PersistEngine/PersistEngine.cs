////////////////////////////////////////////////////////////////////
// PersistEngine.cs - Persist Engine provides method to save XML  //
// Ver 1.0                                                        //
// Application: PersistEngine for CSE681-SMA, Project#4           //
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
 * This package provides methods to write XML on fileSystem \
 * to persist DB
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Console;
using static System.Exception;

namespace Project2Starter
{
    public class PersistEngine<Key, Value>
    {
        private static string xmlFilename = "intStringWrite.xml"; //constant defined for <int,string> XML file name.
        private static string listStringXmlFilename = "stringListOfStringWrite.xml";//constant defined for <string, List<string>> XML file name.
        private static string schedulerXmlFilename = "schedulerXml.xml";//constant defined for scheduled saved XML file name.
        
        //<----------method to write an XML for <int,string> database------------->
        public bool createStringXML(DBEngine<int, DBElement<int, string>> db, bool isScheduledCall)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment xmlComment = new XComment("XML document of type int-string");
            xml.Add(xmlComment);
            XElement root = new XElement("noSqlInput");
            xml.Add(root);
            XElement keyType = new XElement("keytype");
            keyType.SetValue("int");
            XElement payloadType = new XElement("payloadtype");
            payloadType.SetValue("string");
            root.Add(keyType);
            root.Add(payloadType);
            string absolutePath = Path.GetFullPath(xmlFilename);
            foreach (int key in db.Keys())
            {
                XElement node = new XElement("node");
                XElement keynode = new XElement("key", key.ToString());
                node.Add(keynode);
                XElement name = new XElement("name", db.Dictionary[key].name);
                XElement desc = new XElement("desc", db.Dictionary[key].descr);
                XElement timestamp = new XElement("timestamp", db.Dictionary[key].timeStamp.ToString());
                XElement payload = new XElement("payload", db.Dictionary[key].payload);
                XElement element = populateElementForXML(db, key, name, desc, timestamp, payload);
                node.Add(element);
                root.Add(node);
            }
            try
            {
                if (isScheduledCall)
                {
                    absolutePath = Path.GetFullPath(schedulerXmlFilename);
                    xml.Save(schedulerXmlFilename);
                }
                else
                    xml.Save(xmlFilename);
                    WriteLine("\nFile saved at: " + absolutePath);
                return true;
            }
            catch (DirectoryNotFoundException ex)
            {
                WriteLine("Directory not found. Provided path is incorrect" + ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Error occured while saving the XML on file system" + ex.Message);
            }
            return false;
        }

        //<----------method to populate XElement for <int,string> database------------->
        private static XElement populateElementForXML(DBEngine<int, DBElement<int, string>> db, int key, XElement name, XElement desc, XElement timestamp, XElement payload)
        {
            XElement element = new XElement("element");
            element.Add(name);
            element.Add(desc);
            element.Add(timestamp);
            element.Add(payload);
            if (db.Dictionary[key].children != null)
            {
                XElement children = new XElement("children");
                foreach (int child in db.Dictionary[key].children)
                {
                    XElement childKey = new XElement("key");
                    childKey.Add(child);
                    children.Add(childKey);
                }
                element.Add(children);
            }

            return element;
        }

        //<----------method to write an XML for <string,List<string>> database------------->
        public bool createListStringXML(DBEngine<string, DBElement<string, List<string>>> dbl)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment xmlComment = new XComment("XML document of type int-string");
            xml.Add(xmlComment);
            XElement root = new XElement("noSqlInput");
            xml.Add(root);
            XElement keyType = new XElement("keytype");
            keyType.SetValue("string");
            XElement payloadType = new XElement("payloadtype");
            payloadType.SetValue("list of strings");
            root.Add(keyType);
            root.Add(payloadType);
            string absolutePath = Path.GetFullPath(listStringXmlFilename);
            WriteLine("\nFile saved at: " + absolutePath);
            foreach (string key in dbl.Keys())
            {
                XElement node = new XElement("node");
                XElement keynode = new XElement("key", key.ToString());
                node.Add(keynode);
                XElement name = new XElement("name", dbl.Dictionary[key].name);
                XElement desc = new XElement("desc", dbl.Dictionary[key].descr);
                XElement timestamp = new XElement("timestamp", dbl.Dictionary[key].timeStamp.ToString());
                XElement payload = new XElement("payload");
                foreach (string load in dbl.Dictionary[key].payload)
                {
                    XElement item = new XElement("item", load);
                    payload.Add(item);
                }
                XElement element = populateElementForStringXml(dbl, key, name, desc, timestamp, payload);
                node.Add(element);
                root.Add(node);
            }
            try
            {
                xml.Save(listStringXmlFilename);
                return true;
            }
            catch (DirectoryNotFoundException ex)
            {
                WriteLine("Directory not found. Provide path is incorrect" + ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Error occured while saving the XML on file system" + ex.Message);
            }
            return false;
        }

        //<----------method to populate XElement for <string,List<string>> database------------->
        private static XElement populateElementForStringXml(DBEngine<string, DBElement<string, List<string>>> dbl, string key, XElement name, XElement desc, XElement timestamp, XElement payload)
        {
            XElement element = new XElement("element");
            element.Add(name);
            element.Add(desc);
            element.Add(timestamp);
            element.Add(payload);
            if (dbl.Dictionary[key].children != null)
            {
                XElement children = new XElement("children");
                foreach (string child in dbl.Dictionary[key].children)
                {
                    XElement childKey = new XElement("key");
                    childKey.Add(child);
                    children.Add(childKey);
                }
                element.Add(children);
            }

            return element;
        }
    }

#if (TEST_PERSISTENGINE)
    public class TestPersistEngine
    {
        private static PersistEngine<int, DBElement<int, string>> pe = new PersistEngine<int, DBElement<int, string>>();
        static void Main(string[] args)
        {
            Write("\n --- Test DBElement<int,string> ---");
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            WriteLine();

            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            WriteLine();

            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.children.AddRange(new List<int> { 1, 5, 23 });
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            WriteLine();

            Write("\n --- Test DBEngine<int,DBElement<int,string>> ---");

            int key = 0;
            Func<int> keyGen = () => { ++key; return key; };  // anonymous function to generate keys

            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(keyGen(), elem1);
            bool p2 = db.insert(keyGen(), elem2);
            bool p3 = db.insert(keyGen(), elem3);
            if (p1 && p2 && p3)
                Write("\n  all inserts succeeded");
            else
                Write("\n  at least one insert failed");
            db.showDB();
            WriteLine();

            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.payload = new List<string> { "one", "two", "three" };
            Write(newelem1.showElement<string, List<string>>());
            WriteLine();

            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "better formatting";
            newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
            newerelem1.payload.Add("delta");
            newerelem1.payload.Add("epsilon");
            Write(newerelem1.showElement<string, List<string>, string>());
            WriteLine();

            DBElement<string, List<string>> newerelem2 = new DBElement<string, List<string>>();
            newerelem2.name = "newerelem2";
            newerelem2.descr = "better formatting";
            newerelem1.children.AddRange(new[] { "first", "second" });
            newerelem2.payload = new List<string> { "a", "b", "c" };
            newerelem2.payload.Add("d");
            newerelem2.payload.Add("e");
            Write(newerelem2.showElement<string, List<string>, string>());
            WriteLine();

            Write("\n --- Test DBEngine<string,DBElement<string,List<string>>> ---");

            int seed = 0;
            string skey = seed.ToString();
            Func<string> skeyGen = () =>
            {
                ++seed;
                skey = "string" + seed.ToString();
                skey = skey.GetHashCode().ToString();
                return skey;
            };

            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            newdb.insert(skeyGen(), newerelem1);
            newdb.insert(skeyGen(), newerelem2);
            newdb.showEnumerableDB();
            WriteLine();
            WriteLine("Persisting int-string XML into Database");
            pe.createStringXML(db, false);
            WriteLine("Persisting string-List<string> XML into Database");
            pe.createListStringXML(newdb);
        }
    }
#endif
}
