/////////////////////////////////////////////////////////////////////////
// MainWindows.xaml.cs - CommService GUI Client                        //
// ver 2.1                                                             //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis,       //
//           Project #4                                                //
// Author:      Rohit Sharma, SUID-242093353, Syracuse University      //
//              (315) 935-1323, rshar102@syr.edu                       //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# WPF Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, MakeMessage, Utilities
 * - Added using Project4Starter
 *
 * Note:
 * - This client receives and sends messages.
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
 * ver 2.1 : 23 Nov 2015o
 * - added method to show request and response of reader on GUI.
 * - added method to show performance time on GUI for reader and writer both.
 * ver 2.0 : 29 Oct 2015
 * - changed Xaml to achieve more fluid design
 *   by embedding controls in grid columns as well as rows
 * - added derived sender, overridding notification methods
 *   to put notifications in status textbox
 * - added use of MessageMaker in send_click
 * ver 1.0 : 25 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Project4Starter;
using System.Xml.Linq;

namespace WpfApplication1
{
    using HRTimer;
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        static wpfSender sndr = null;
        string localAddress = "localhost";
        string localPort = "8089";
        string remoteAddress = "localhost";
        string remotePort = "8080";
        private static string readClientXmlName = "Project4_SampleReaderXML.xml";

        /////////////////////////////////////////////////////////////////////
        // nested class wpfSender used to override Sender message handling
        // - routes messages to status textbox
        public class wpfSender : Sender
        {
            TextBox lStat_ = null;  // reference to UIs local status textbox
            RichTextBox req_text = null;
            RichTextBox resp_text = null;
            RichTextBox performance_text = null;
            System.Windows.Threading.Dispatcher dispatcher_ = null;

            public wpfSender(RichTextBox richTextBox, RichTextBox richTextBox1, RichTextBox richTextBox2, TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread
                lStat_ = lStat;
                req_text = richTextBox;
                resp_text = richTextBox1;
                performance_text = richTextBox2;
            }
            public override void sendMsgNotify(string msg)
            {
                Action act = () => { lStat_.Text = msg; };
                dispatcher_.Invoke(act);
            }
            public override void sendQueryResponseNotify(string msg)
            {
                Action act = () =>
                {
                    Thread.Sleep(100);
                    resp_text.AppendText("\n");
                    resp_text.AppendText("============Response Message===========");
                    resp_text.AppendText("\n");
                    if (msg == null || "".Equals(msg))
                    {
                        msg = "No results found in the database";
                    }
                    if (msg.Contains("#"))
                    {
                        int l = msg.IndexOf("#");
                        if (l == 0)
                        {
                            msg = "No results found in the database";
                        }
                        else if (l > 0)
                        {
                            msg = msg.Split('#')[0];
                        }
                    }
                    resp_text.AppendText(msg);
                };
                dispatcher_.Invoke(act);
            }

            //----<method to show performance time for Reader>-----
            public void showMsgPerformanceTimeForReader(string msg)
            {
                if (msg != null && !"".Equals(msg) && msg.Contains("#"))
                {
                    string timeInString = msg.Split('#')[1];
                    ulong elapsedTime = Convert.ToUInt64(timeInString);
                    performance_text.AppendText("\nTime taken by server to process READER message: " + elapsedTime + " microseconds = " + (Double.Parse((elapsedTime / 1000000).ToString())) + " seconds");
                }
            }

            //----<method to show performance time for writer>-----
            public void showMsgPerformTimeWriter(string msg)
            {
                ulong elapsedTime = UInt64.Parse(msg.Split(';')[1]);
                int totalMsgs = Int32.Parse(msg.Split(';')[2]);
                performance_text.AppendText("\nTime taken by server to process WRITER message: " + elapsedTime + " microseconds = " + (Double.Parse((elapsedTime / 1000000).ToString())) + " seconds");
            }

            //----< method to show overall performance time>-----
            public void showOverallPerformanceTime(string callerType, int totalMsgNum, ulong elapsedTime, string clientNum)
            {
                if (callerType == "Writer")
                {
                    performance_text.AppendText("\n===============Latency Time - Writer " + clientNum + " ================");
                    performance_text.AppendText("\n" + callerType + " Client: Overall Elapsed time for "
                        + totalMsgNum + " messages is: " + elapsedTime + " microseconds = " + ((double)elapsedTime / 1000000) + " seconds");
                    performance_text.AppendText("\nAverage Latency Time (Total Elapsed Time/Num of Msgs) is:" + ((double)elapsedTime / totalMsgNum)
                        + " microseconds = " + (((double)elapsedTime / totalMsgNum) / 1000000) + " seconds\n");
                }
                else if (callerType == "Reader")
                {
                    performance_text.AppendText("\n===============Latency Time - Reader "+ clientNum  +" ================");
                    performance_text.AppendText("\n" + callerType + " Client: Overall Elapsed time for "
                        + totalMsgNum + " messages is: " + elapsedTime + " microseconds = " + ((double)elapsedTime / 1000000) + " seconds");
                    performance_text.AppendText("\nAverage Latency Time (Total Elapsed Time/Num of Msgs) is:" + ((double)elapsedTime / totalMsgNum)
                        + " microseconds = " + (((double)elapsedTime / totalMsgNum) / 1000000) + " seconds\n");
                }
            }

            //----< method to send query request notification>-----
            public override void sendQueryRequestNotify(string msg)
            {
                Action act = () =>
                {
                    if (msg != null && !"".Equals(msg) && msg.Contains(";"))
                    {
                        msg = msg.Split(';')[0];
                    }
                    req_text.AppendText("\n");
                    req_text.AppendText("============Request Message===========");
                    req_text.AppendText("\n");
                    req_text.AppendText(msg);
                    req_text.AppendText("\n");
                };
                dispatcher_.Invoke(act);
            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { lStat_.Text = ex.Message; };
                dispatcher_.Invoke(act);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }

#if(TEST_WPFCLIENT)
        public MainWindow()
        {
            InitializeComponent();
            lAddr.Text = localAddress;
            lPort.Text = localPort;
            rAddr.Text = remoteAddress;
            rPort.Text = remotePort;
            Title = "WPF Reader Client/Performance Evaluation";
            send.IsEnabled = false;
            Loaded += ReaderWindow_Loaded;
        }
#endif
        //----< On load method for reader GUI window -- connection setup is done on load>------------------
        private void ReaderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            localPort = lPort.Text;
            localAddress = lAddr.Text;
            remoteAddress = rAddr.Text;
            remotePort = rPort.Text;

            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            rStat.Text = "connect setup";
            send.IsEnabled = true;
            connect.IsEnabled = false;
            lPort.IsEnabled = false;
            lAddr.IsEnabled = false;
        }

        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }
        //----< indirectly used by child receive thread to post results >----

        public void postRcvMsg(string content)
        {
            if (content != null && !"".Equals(content))
            {
                if (content.Contains("Overall"))
                {
                    ulong elapsedTime = UInt64.Parse(content.Split(';')[1]);
                    int totalMsgs = Int32.Parse(content.Split(';')[2]);
                    string clientNum = content.Split(';')[3];
                    sndr.showOverallPerformanceTime("Writer", totalMsgs, elapsedTime, clientNum);
                }
                else if (content.Contains("Composite"))
                {
                    ulong elapsedTime = UInt64.Parse(content.Split(';')[1]);
                    int totalMsgs = Int32.Parse(content.Split(';')[2]);
                    string clientNum = content.Split(';')[3];
                    sndr.showOverallPerformanceTime("Reader", totalMsgs, elapsedTime, clientNum);
                }
            }
            Thread.Sleep(100);
            sndr.sendQueryResponseNotify(content);
        }
        //----< used by main thread >----------------------------------------

        public void postSndMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            sndmsgs.Items.Insert(0, item);

        }
        //----< get Receiver and Sender running >----------------------------

        void setupChannel()
        {
            rcvr = new Receiver(localPort, localAddress);
            Action serviceAction = () =>
            {
                try
                {
                    Message rmsg = null;
                    while (true)
                    {
                        rmsg = rcvr.getMessage();
                        Action act = () => { postRcvMsg(rmsg.content); };
                        Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
                catch (Exception ex)
                {
                    Action act = () => { lStat.Text = ex.Message; };
                    Dispatcher.Invoke(act);
                }
            };
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction);
            }

            sndr = new wpfSender(richTextBox, richTextBox1, richTextBox2, lStat, this.Dispatcher);
        }
        //----< set up channel after entering ports and addresses >----------

        private void start_Click(object sender, RoutedEventArgs e)
        {
            localPort = lPort.Text;
            localAddress = lAddr.Text;
            remoteAddress = rAddr.Text;
            remotePort = rPort.Text;

            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            rStat.Text = "connect setup";
            send.IsEnabled = true;
            connect.IsEnabled = false;
            lPort.IsEnabled = false;
            lAddr.IsEnabled = false;
        }
        //----< send a demonstraton message >--------------------------------

        private void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region
                /////////////////////////////////////////////////////
                // This commented code was put here to allow
                // user to change local port and address after
                // the channel was started.  
                //
                // It does what is intended, but would throw 
                // if the new port is assigned a slot that
                // is in use or has been used since the
                // TCP tables were last updated.
                //
                // if (!localPort.Equals(lPort.Text))
                // {
                //   localAddress = rcvr.address = lAddr.Text;
                //   localPort = rcvr.port = lPort.Text;
                //   rcvr.shutDown();
                //   setupChannel();
                // }
                #endregion
                if (!remoteAddress.Equals(rAddr.Text) || !remotePort.Equals(rPort.Text))
                {
                    remoteAddress = rAddr.Text;
                    remotePort = rPort.Text;
                }
                // - Make a demo message to send
                // - You will need to change MessageMaker.makeMessage
                //   to make messages appropriate for your application design
                // - You might include a message maker tab on the UI
                //   to do this.

                MessageMaker maker = new MessageMaker();
                Message msg = maker.makeMessage(Utilities.makeUrl(lAddr.Text, lPort.Text),Utilities.makeUrl(rAddr.Text, rPort.Text));
                lStat.Text = "sending to" + msg.toUrl; sndr.localUrl = msg.fromUrl; sndr.remoteUrl = msg.toUrl;
                lStat.Text = "attempting to connect";
                if (sndr.sendMessage(msg))
                    lStat.Text = "connected";
                else
                    lStat.Text = "connect failed";
                postSndMsg(msg.content);
            }
            catch (Exception ex)
            {
                lStat.Text = ex.Message;
            }
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            XDocument xdoc = sndr.readClientXmlReader(readClientXmlName);
            try
            {
                if (!remoteAddress.Equals(rAddr.Text) || !remotePort.Equals(rPort.Text))
                {
                    remoteAddress = rAddr.Text;
                    remotePort = rPort.Text;
                }
                MessageMaker maker = new MessageMaker();
                Message msg = maker.makeMessage(
                  Utilities.makeUrl(lAddr.Text, lPort.Text),
                  Utilities.makeUrl(rAddr.Text, rPort.Text)
                );
                lStat.Text = "sending to" + msg.toUrl;
                sndr.localUrl = msg.fromUrl;
                sndr.remoteUrl = msg.toUrl;
                lStat.Text = "attempting to connect";
                List<string> list = new List<string>();
                int numOfRequest = 0;
                list = sndr.readerXmlMsgGenerator(xdoc, ref numOfRequest);
                int qType = list.Count;
                int msgPerType = numOfRequest / qType;
                HRTimer.HiResTimer timer = new HiResTimer();
                timer.Start();
                foreach (string lst in list)
                {
                    msg.content = lst;
                    for (int i = 0; i < msgPerType; i++)
                    {
                        Action act = () => { sndr.sendQueryRequestNotify(msg.content); };
                        Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                        if (sndr.sendMessage(msg))
                            lStat.Text = "connected";
                        else
                            lStat.Text = "connect failed";
                    }
                }
                timer.Stop();
                ulong elapsedTime = timer.ElapsedMicroseconds;
                Action act1 = () => { sndr.showOverallPerformanceTime("Reader", numOfRequest, elapsedTime, "Main"); };
                Dispatcher.Invoke(act1, System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                richTextBox.AppendText("\n" + ex.Message);
            }
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.Selection.Text = "";
        }

        private void richTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.SelectAll();
            richTextBox.Selection.Text = "";
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            richTextBox2.SelectAll();
            richTextBox2.Selection.Text = "";
        }
    }
}
