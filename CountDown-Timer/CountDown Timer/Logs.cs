using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CountDown_Timer
{
    class Logs
    {
        private string dateTimeFormat = "dd/MM/yy (H:mm:ss)";

        public Logs()
        {

        }

        private void WriteToFile(string msg)
        {
            StreamWriter sw = new StreamWriter("errors.txt", true);
            sw.WriteLine(msg);
            sw.Close();
        }

        public bool Message(string message)
        {
            try
            {
                WriteToFile("MSG " + DateTime.Now.ToString(dateTimeFormat) + " : [" + message + "]" + Environment.NewLine);
                return true;
            } catch (Exception ex)
            {
                return false;
            }

        }

    }
}
