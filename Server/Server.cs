/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 2.4                                                             //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis,       //
//           Project #4                                                //
// Author:      Rohit Sharma, SUID-242093353, Syracuse University      //
//              (315) 935-1323, rshar102@syr.edu                       //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.4 : 23 Nov 2015
 * - added functionality to idetify DB Type.
 * - added functionality to idetify Query Type.
 * - added functionality to route Query Type and serve it.
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;
using System.IO;
using System.Xml;

namespace Project4Starter
{
    using Util = Utilities;
    using Project2Starter;
    using System.Text.RegularExpressions;
    using HRTimer;
    class Server
    {
        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";

        DBEngine<int, DBElement<int, string>> dbs = new DBEngine<int, DBElement<int, string>>();
        DBEngine<string, DBElement<string, List<string>>> dbstr = new DBEngine<string, DBElement<string, List<string>>>();
        private PersistEngine<int, DBElement<int, string>> pe = new PersistEngine<int, DBElement<int, string>>();
        private QueryEngine<string> qe = new QueryEngine<string>();
        private RestoreEngine<int, DBElement<int, string>> re = new RestoreEngine<int, DBElement<int, string>>();
        CategoryDBEngine<int> catDb = new CategoryDBEngine<int>();
        //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                port = args[0];
            }
            if (args.Length > 1)
            {
                address = args[1];
            }
        }

        //----< backbone method to process messages coming to server>-----
        public string serverProcessMessage(Message msg)
        {
            string localMsg = msg.content.Trim(), queryType = "", key = "";
            XDocument newXml = new XDocument();
            try
            {
                newXml = XDocument.Parse(msg.content.Trim());
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
            IEnumerable<XElement> elems = newXml.Descendants("query");
            DBElement<int, string> dbElem = new DBElement<int, string>();
            DBElement<string, List<string>> dbElmList = new DBElement<string, List<string>>();
            foreach (XElement elem in elems)
            {
                queryType = elem.Element("queryType").Value.ToString();
                string keyType = elem.Element("keytype").Value.ToString();
                string payloadType = elem.Element("payloadtype").Value.ToString();
                if ("Search".Equals(queryType) || "Persist".Equals(queryType) || "Restore".Equals(queryType))
                {
                    return routeAdvancedQueries(newXml, keyType, payloadType);
                }
                XElement node = elem.Element("node");
                key = node.TryGetElementValue("key");
                XElement e = node.Element("element");
                return identifyKeyType(queryType, key, dbElem, dbElmList, keyType, e);
            }
            return null;
        }

        //----< method to identify key type>-----
        private string identifyKeyType(string queryType, string key, DBElement<int, string> dbElem, DBElement<string, List<string>> dbElmList, string keyType, XElement e)
        {
            string message = null;
            if ("int".Equals(keyType))
            {
                extractIntKeyDBElem(dbElem, e);
                message = routeQuery(queryType, Int32.Parse(key), dbElem) ? "Msg Receive Succcess" : null;
            }
            else
            {
                extractStringKeyDBElem(dbElmList, e);
                message = routeListQueries(queryType, key, dbElmList) ? "Msg Receive Succcess" : null;
            }
            return message;
        }

        //----< method to extract db element from incoming message for the String-List<string> DB>-----
        private static void extractStringKeyDBElem(DBElement<string, List<string>> dbElmList, XElement e)
        {
            dbElmList.name = e.TryGetElementValue("name");
            dbElmList.descr = e.TryGetElementValue("desc");
            if (e.TryGetElementValue("timestamp") != null)
            {
                dbElmList.timeStamp = DateTime.Parse(e.TryGetElementValue("timestamp"));
            }
            else
            {
                dbElmList.timeStamp = DateTime.Now;
            }
            List<string> newListStr = new List<string> { e.TryGetElementValue("payload") };
            dbElmList.payload = newListStr;
            List<string> children = new List<string> { e.TryGetElementValue("children") };
            dbElmList.children = children;
            dbElmList.category = e.TryGetElementValue("category");
        }

        //----< method to extract db element from incoming message for the int-string DB>-----
        private static void extractIntKeyDBElem(DBElement<int, string> dbElem, XElement e)
        {
            dbElem.name = e.TryGetElementValue("name");
            dbElem.descr = e.TryGetElementValue("desc");
            if (e.TryGetElementValue("timestamp") != null)
            {
                dbElem.timeStamp = DateTime.Parse(e.TryGetElementValue("timestamp"));
            }
            else
            {
                dbElem.timeStamp = DateTime.Now;
            }
            dbElem.payload = e.TryGetElementValue("payload");
            XElement children = e.Element("children");
            if (children != null)
            {
                IEnumerable<XElement> keys = children.Descendants("key");
                foreach (XElement k in keys)
                {
                    int child = 0;
                    bool isConvSuccess = Int32.TryParse(k.Value, out child);
                    if (isConvSuccess)
                    {
                        List<int> childKeys = new List<int> { child };
                        dbElem.children = childKeys;
                    }
                }
            }
            dbElem.category = e.TryGetElementValue("category");
        }

        //----< method to route writer queries for int-string DB>-----
        public bool routeQuery(string queryType, int key, DBElement<int, string> dbElem)
        {
            switch (queryType)
            {
                case "Insert":
                    return insertIntStringDB(key, dbElem);
                case "Edit":
                    return editIntStringDB(key, dbElem);
                case "Delete":
                    return deleteIntStringDB(key);
                default:
                    WriteLine("\nNo queryType specified");
                    return false;
            }
        }

        //----< method to delete record from int-string DB>-----
        private bool deleteIntStringDB(int key)
        {
            if (dbs.delete(key))
            {
                WriteLine("\nData Deleted from DB");
                Console.WriteLine("\n------DB Start--------");
                dbs.showDB();
                Console.WriteLine("\n------DB End--------");
                return true;
            }
            else
            {
                return false;
            }
        }
        //----< method to edit record from int-string DB>-----
        private bool editIntStringDB(int key, DBElement<int, string> dbElem)
        {
            if (dbs.update(key, dbElem))
            {
                WriteLine("\nData Edited in DB");
                Console.WriteLine("\n------DB Start--------");
                dbs.showDB();
                Console.WriteLine("\n------DB End--------");
                return true;
            }
            else
            {
                return false;
            }
        }
        //----< method to insert record in int-string DB>-----
        private bool insertIntStringDB(int key, DBElement<int, string> dbElem)
        {
            if (dbs.insert(key, dbElem))
            {
                WriteLine("\nData Inserted in DB");
                Console.WriteLine("\n------DB Start--------");
                string cat = dbElem.category;
                List<int> val = new List<int>() { key };
                catDb.insertCategoryDb(cat, val);
                dbs.showDB();
                catDb.showCategoryDB(dbs);
                Console.WriteLine("\n------DB End--------");
                return true;
            }
            else
            {
                return false;
            }
        }

        //----< method to route writer queries for string-List<string> DB>-----
        public bool routeListQueries(string queryType, string key, DBElement<string, List<string>> dbElemList)
        {
            switch (queryType)
            {
                case "Insert":
                    if (dbstr.insert(key, dbElemList))
                    {
                        WriteLine("\nData Inserted in string-List<string> DB");
                        Console.WriteLine("\n------DB Start--------");
                        dbstr.showEnumerableDB();
                        Console.WriteLine("\n------DB End--------");
                        return true;
                    }
                    else
                        return false;
                case "Edit":
                    if (dbstr.update(key, dbElemList))
                    {
                        WriteLine("\nData Edited in string-List<string> DB");
                        Console.WriteLine("\n------DB Start--------");
                        dbstr.showEnumerableDB();
                        Console.WriteLine("\n------DB End--------");
                        return true;
                    }
                    else
                        return false;
                case "Delete":
                    if (dbstr.delete(key))
                    {
                        WriteLine("\nData Deleted from string-List<string> DB");
                        Console.WriteLine("\n------DB Start--------");
                        dbstr.showEnumerableDB();
                        Console.WriteLine("\n------DB End--------");
                        return true;
                    }
                    else
                        return false;
                default:
                    WriteLine("\nNo queryType specified");
                    return false;
            }
        }

        //----< method to route reader queries>-----
        public string routeAdvancedQueries(XDocument newXml, string keyType, string payloadType)
        {
            IEnumerable<XElement> elems = newXml.Descendants("query");
            DBElement<int, string> dbElem = new DBElement<int, string>();
            string queryType = "", querySubType = "";
            foreach (XElement elem in elems)
            {
                queryType = elem.Element("queryType").Value.ToString();
                if ("Search".Equals(queryType))
                {
                    querySubType = elem.Element("querySubType").Value.ToString();
                    XElement node = elem.Element("node");
                    string key = node.TryGetElementValue("key"), pattern = node.TryGetElementValue("pattern"), category = node.TryGetElementValue("category");
                    return checkQuerySubType(querySubType, key, pattern, category);
                }
                else if ("Persist".Equals(queryType))
                {
                    return persistXml(keyType, payloadType);
                }
                else
                {
                    return augmentDBFromXml(keyType, payloadType);
                }
            }
            return "No Elements in XML";
        }

        //----< method to check query subtype>-----
        private string checkQuerySubType(string querySubType, string key, string pattern, string category)
        {
            switch (querySubType)
            {
                case "Value by Key":
                    {
                        return searchValueByKey(key);
                    }
                case "Children by Key":
                    {
                        return searchChildrenByKey(key);
                    }
                case "Key by Metadata":
                    {
                        return searchKeyByMetadata(pattern);
                    }
                case "Key by Pattern":
                    {
                        return searchKeyByPattern(pattern);
                    }
                case "Key by Category":
                    {
                        return searchKeyByCategory(category);
                    }
                case "Key by Timestamp":
                    {
                        return searchKeyByTimestamp();
                    }
                default:
                    return "Unknown Search criteria.";
            }
        }

        //----< method to augment db from xml>-----
        private string augmentDBFromXml(string keyType, string payloadType)
        {
            try
            {
                if ("int".Equals(keyType))
                {
                    re.augmentDB(dbs);
                    dbs.showDB();
                    return "Restore Success";
                }
                else
                {
                    re.augmentStringDB(dbstr);
                    dbstr.showEnumerableDB();
                    return "Restore Success";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while restoring database from XML: " + ex.Message);
                return "Restore Failure";
            }
        }

        //----< method to persist db into xml>-----
        private string persistXml(string keyType, string payloadType)
        {
            bool result = false;
            if ("string".Equals(keyType))
            {
                result = pe.createListStringXML(dbstr);
            }
            else
            {
                result = pe.createStringXML(dbs, false);
            }
            if (result)
            {
                return "Persist Success";
            }
            else
                return "Persist Failure";
        }

        //----< method to search key by timestamp>-----
        private string searchKeyByTimestamp()
        {
            StringBuilder sb = new StringBuilder("Key by Timestamp Search - ");
            List<string> result = qe.getKeysByTimeInterval(new DateTime(DateTime.Now.Year, 09, 13), DateTime.Now, dbstr);
            if (result != null && result.Count > 0)
            {
                foreach (string rslt in result)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

        //----< method to search key by category>-----
        private string searchKeyByCategory(string category)
        {
            StringBuilder sb = new StringBuilder("Key by Category Search - " + category + " = ");
            List<string> result = qe.getValueByCategory(category, dbs, catDb);
            if (result != null && result.Count > 0)
            {
                foreach (string rslt in result)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

        //----< method to search key by pattern>-----
        private string searchKeyByPattern(string pattern)
        {
            StringBuilder sb = new StringBuilder("Key by Pattern Search - " + pattern + " = ");
            Regex regex = new Regex(pattern);
            Func<string, bool> query = qe.defineSearchQuery(regex);
            List<string> keyCollection = new List<string>();
            qe.processQuery(query, out keyCollection, dbstr);
            if (keyCollection != null && keyCollection.Count > 0)
            {
                foreach (string rslt in keyCollection)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

        //----< method to search key by metadata>-----
        private string searchKeyByMetadata(string pattern)
        {
            StringBuilder sb = new StringBuilder("Key by Metadata Search- " + pattern + " = ");
            List<string> result = qe.getKeysByMetadataSearch(pattern, dbstr);
            if (result != null && result.Count > 0)
            {
                foreach (string rslt in result)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

        //----< method to search children by key>-----
        private string searchChildrenByKey(string key)
        {
            StringBuilder sb = new StringBuilder("Children by Key - " + key + " = ");
            List<string> result = qe.getChildrenByKey(key, dbstr);
            if (result != null && result.Count > 0)
            {
                foreach (string rslt in result)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

        //----< method to search value by key>-----
        private string searchValueByKey(string key)
        {
            StringBuilder sb = new StringBuilder("Value by Key - " + key + " = ");
            List<string> result = qe.getValueByKey(key, dbstr);
            if (result != null && result.Count > 0)
            {
                foreach (string rslt in result)
                {
                    sb.Append(rslt + ";");
                }
            }
            else
            {
                sb.Append("No results found in the database");
            }
            return sb.ToString();
        }

#if (TEST_SERVER)
        static void Main(string[] args)
        {
            Util.verbose = false;
            Server srvr = new Server();
            srvr.ProcessCommandLine(args);
            Sender sndr; Receiver rcvr;
            createAndStartServer(srvr, out sndr, out rcvr);
            // - serviceAction defines what the server does with received messages
            // - This serviceAction just announces incoming messages and echos them
            //   back to the sender.  
            // - Note that demonstrates sender routing works if you run more than
            //   one client.
            Action serviceAction = serverOpServiceAction(srvr, sndr, rcvr);
            startReceiver(rcvr, serviceAction);
            Util.waitForUser();
        }

        private static Action serverOpServiceAction(Server srvr, Sender sndr, Receiver rcvr)
        {
            Action serviceAction = () =>
            {
                Message msg = null;
                while (true)
                {
                    msg = rcvr.getMessage();   // note use of non-service method to deQ messages
                    if (msg.content == "connection start message")
                    {
                        continue; // don't send back start message
                    }
                    if (msg.content == "done")
                    {
                        Console.Write("\n  client has finished\n");
                        continue;
                    }
                    if (msg.content == "closeServer")
                    {
                        Console.Write("received closeServer");
                        break;
                    }
                    HRTimer.HiResTimer timer = new HiResTimer(); timer.Start();
                    msg.content = srvr.serverProcessMessage(msg); timer.Stop();
                    printMessagseOnServer(msg); Util.swapUrls(ref msg); sndr.sendMessage(msg);
#if (TEST_WPFCLIENT)
          /////////////////////////////////////////////////
          // The statements below support testing the
          // WpfClient as it receives a stream of messages
          // - for each message received the Server
          //   sends back 1000 messages
          //
          int count = 0;
          for (int i = 0; i < 1000; ++i)
          {
            Message testMsg = new Message();
            testMsg.toUrl = msg.toUrl;
            testMsg.fromUrl = msg.fromUrl;
            testMsg.content = String.Format("test message #{0}", ++count);
            Console.Write("\n  sending testMsg: {0}", testMsg.content);
            sndr.sendMessage(testMsg);
          }
#else
                    /////////////////////////////////////////////////
                    // Use the statement below for normal 
#endif
                }
            };
            return serviceAction;
        }

        private static void startReceiver(Receiver rcvr, Action serviceAction)
        {
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction); // This serviceAction is asynchronous,
            }                                // so the call doesn't block.
        }

        private static void printMessagseOnServer(Message msg)
        {
            if (Util.isLoggerEnabled)
            {
                Console.Write("\n  Message received.");
                Console.Write("\n  sender is {0}", msg.fromUrl);
                Console.Write("\n  content is {0}\n", msg.content);
            }
        }

        private static void createAndStartServer(Server srvr, out Sender sndr, out Receiver rcvr)
        {
            Console.Title = "Server";
            Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
            Console.Write("\n ====================================================\n");
            sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
            rcvr = new Receiver(srvr.port, srvr.address);
        }
#endif
    }
}
