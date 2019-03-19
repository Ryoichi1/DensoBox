using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoBoxTester
{
    public class TestSpec
    {
        //検査規格のバージョン
        public string TestSpecVer { get; set; }

        //電源電圧規格値
        public double Ac24vMax { get; set; }
        public double Ac24vMin { get; set; }

        public double Ac24vOffMax { get; set; }

    
    }
}
