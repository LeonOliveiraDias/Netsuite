using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netsuite.NetSuiteService;

namespace Netsuite
{
    class SuiteTalkCourse
    {
        static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(args.Length);


            Netsuite.NetSuiteService.NetSuitePortTypeClient _client = new NetSuitePortTypeClient();
            Netsuite.NetSuiteService.TokenPassport tokenPassport = new TokenPassport();
        }
    }
}
