using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeEventLogger
{
    class EventLogger
    {
        
        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter newStream = System.IO.File.AppendText("eventNotificationLogger.txt");
            try
            {
                newStream.WriteLine("----------Logging started----------");
                newStream.WriteLine(msg);
                string logEntry = "Previous Event logged at: "+ DateTime.Now.ToString("dd\\/MM\\/yyyy h\\:mm tt");
                newStream.WriteLine(logEntry);
            }
            finally
            {
                newStream.Close();
            }
        }
        static void Main(string[] args)
        {
            EventLogger newLog = new EventLogger();
            newLog.LogMessageToFile("It's a logged messaged. Event invoked by Rohit.");
            Console.WriteLine("Events logged. Close window.");
        }
    }
}
