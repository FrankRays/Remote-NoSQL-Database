////////////////////////////////////////////////////////////////////
// RestoreEngine.cs - Restore Engine for Project #4              //
// Ver 1.2                                                        //
// Application: RestoreEngine for CSE681-SMA, Project#4          //
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
 * This package provides methods to load/augment database with the 
 * XML files kept on file system
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
using System.Xml;
using System.Xml.Linq;
using static System.Console;
using System.IO;

namespace Project2Starter
{

    public class RestoreEngine<Key, Value>
    {
        private static string readXml = "intStringRead.xml"; //constant defined for the <int,string> DB ReadXML file name 
        private static string readStringXml = "stringListOfStringRead.xml"; //constant defined for the <string,List<string>> DB ReadXML file name 
        private static string descriptor = "packageDescription.xml"; ////constant defined for the package descriptor XML file name.
        private DBElement<Key, string> element = null;
        private DBElement<string, List<string>> stringElement = null;
        private ItemFactory<int> itemFactory = null;

        //<----------method to augment DB from string XML------------->
        public void augmentDB(DBEngine<Key, DBElement<Key, string>> db)
        {
            itemFactory = new ItemFactory<int>();
            XDocument newXml = new XDocument();
            try
            {
                newXml = XDocument.Load(readXml);
            }
            catch (ArgumentNullException ex)
            {
                WriteLine("Required argument is null" + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                WriteLine("File is not found at the provided location" + ex.Message);
            }
            catch (XmlException ex)
            {
                WriteLine("Malformed XML - could not parse the XML" + ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Error while reading the provided XML." + ex.Message);
            }
            Write("\n{0}", newXml.Declaration);
            Write("\n{0}", newXml.ToString());
            WriteLine();
            IEnumerable<XElement> nodes = newXml.Descendants("node");
            foreach (XElement node in nodes)
            {
                foreach (XElement elem in node.Elements("element"))
                {
                    string timestamp = elem.Element("timestamp").Value.ToString();
                    element = itemFactory.createIntStringElement(elem.Element("name").Value.ToString(), elem.Element("desc").Value.ToString(),
                    DateTime.Parse(timestamp), new List<int>(), elem.Element("payload").Value) as DBElement<Key, string>;
                }
                Key key = (Key)Convert.ChangeType(node.Element("key").Value, typeof(Key));
                db.insert(key, element as DBElement<Key, string>);
            }
        }

        //<----------method to augment DB from List<string> XML------------->
        public void augmentStringDB(DBEngine<string, DBElement<string, List<string>>> db)
        {
            itemFactory = new ItemFactory<int>();
            XDocument newXml = new XDocument();
            try
            {
                newXml = XDocument.Load(readStringXml);
            }
            catch (ArgumentNullException ex)
            {
                WriteLine("Required argument is null" + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                WriteLine("File is not found at the provided location" + ex.Message);
            }
            catch (XmlException ex)
            {
                WriteLine("Malformed XML - could not parse the XML" + ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Error while reading the provided XML." + ex.Message);
            }
            Write("\n{0}", newXml.Declaration);
            Write("\n{0}", newXml.ToString());
            WriteLine();
            IEnumerable<XElement> nodes = newXml.Descendants("node");
            foreach (XElement node in nodes)
            {
                foreach (XElement elem in node.Elements("element"))
                {
                    string timestamp = elem.Element("timestamp").Value.ToString();
                    List<string> data = new List<string>();
                    foreach (XElement item in elem.Elements("payload").Elements("item"))
                    {
                        data.Add(item.Value);
                    }
                    stringElement = itemFactory.createStringListOfStringElement(elem.Element("name").Value.ToString(), elem.Element("desc").Value.ToString(),
                    DateTime.Parse(timestamp), new List<string>(), data);
                }
                db.insert(node.Element("key").Value, stringElement);
            }
        }

        //<----------method to load Descriptor XML and show it on Console------------->
        public void loadDescriptorXml()
        {
            XDocument descriptorXml = new XDocument();
            try
            {
                descriptorXml = XDocument.Load(descriptor);
                StringBuilder builder = new StringBuilder();
                using (TextWriter writer = new StringWriter(builder))
                {
                    descriptorXml.Save(writer);
                }
                Console.WriteLine(builder.ToString());
            }
            catch (ArgumentNullException ex)
            {
                WriteLine("Required argument is null. " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                WriteLine("File is not found at the provided location. " + ex.Message);
            }
            catch (XmlException ex)
            {
                WriteLine("Malformed XML - could not parse the XML. " + ex.Message);
            }
            catch (Exception ex)
            {
                WriteLine("Error while reading the provided XML. " + ex.Message);
            }
            WriteLine("Package Description XML is as follows: \n" + descriptorXml.ToString());
        }
    }

#if(TEST_RESTOREENGINE)
    public class TestRestoreEngine
    {
        static void Main(string[] args)
        {
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBEngine<string, DBElement<string, List<string>>> db1 = new DBEngine<string, DBElement<string, List<string>>>();
            RestoreEngine<int, DBElement<int, string>> re = new RestoreEngine<int, DBElement<int, string>>();
            "Augmenting database with an XML of key type \"int\" and value type \"string\"".title();
            re.augmentDB(db);
            db.showDB();
            "Augmenting database with an XML of key type \"string\" and value type \"List<string>\"".title();
            re.augmentStringDB(db1);
            db1.showEnumerableDB();
        }
    }
#endif
}
