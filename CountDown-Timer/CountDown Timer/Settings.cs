using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CountDown_Timer
{
    [Serializable()]
    public class Settings
    {

        public double consoleTop; // 
        public double consoleLeft;

        public double mainTop;
        public double mainLeft;
        public double mainWidth;
        public double mainHeight;

        public bool mainResizeMode;

        public int Hour;
        public int Minute;
        public int Second;

        public bool Sound;
        public int BeepSeconds;

        public bool Transparent;

    }
}
