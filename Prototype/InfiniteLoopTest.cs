using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Prototype
{
    class InfiniteLoopTest
    {
        private static int timeoutVal = 100000; //timout value configuration

        static void Main(string[] args)
        {
            int msgCount = 0;
            Stopwatch newTimer = new Stopwatch();
            newTimer.Start();
            //Developer made the mistake and put a infinite while loop in code 
            while (true) {
                //Suppose code is doing some msg processing here.
                //Sending the message here.
                Console.WriteLine("Inside infinite loop - Message : {0}", msgCount); //And logging the message here.
                if (newTimer.ElapsedTicks > timeoutVal) //Test Executor will implement this //code and check if elapsed time is greater 
                {                                    //than a certain //timeinterval, if yes it will break the execution of test cases.
                    Console.WriteLine("Infinite loop terminated");
                    break;
                }
                msgCount++;
            }
            newTimer.Stop();
        }
    }
}
