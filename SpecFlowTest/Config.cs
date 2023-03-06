using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowTest
{
    public static class Config
    {
        public static int CircuitOpenTimeout => 4000;
        public static int CircuitClosedErrorLimit = 1000;
        public static int CircuitHalfOpenSuccessLimit = 1;
    }
}
