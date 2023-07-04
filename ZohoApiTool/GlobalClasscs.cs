using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZohoApiTool
{
    public class GlobalClasscs
    {
        public struct RestMessage
        {
            public int Ischeck;   //是否执行‘临盘操作’ (0:是 1:否)
        }

        public static RestMessage RmMessage;
    }
}
