using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeamViewer
{
    class FrameManager
    {
        private long start, end;
        private int excute, frameDalay;
        public FrameManager(int hopeFPS)
        {
            start = end = 0;
            frameDalay = hopeFPS / 60;
        }

        public void Start()
        {
            start = DateTime.Now.Ticks;
        }

        public void StopAndDelay()
        {
            end = DateTime.Now.Ticks;
            excute = Convert.ToInt32((end - start) / 10000000);
            if(excute < frameDalay)
                Thread.Sleep(frameDalay - excute);
        }
    }
}
