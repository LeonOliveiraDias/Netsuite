using System;
using Netsuite.com.netsuite.webservices;


namespace Netsuite
{
    class SuiteTalkCourse
    {
        public NetSuiteService _service;
        private Preferences _preferences;
        private SearchPreferences _searchPreferences;

        public SuiteTalkCourse() { 
            _service = new NetSuiteService();

            _service.Url = "https://td2967825.suitetalk.api.netsuite.com";

            SetTokenPassport();
            SetApplicationInfo();
            SetPreferences();
            GetEmployee();
        }


        private void SetApplicationInfo() {
            _service.applicationInfo = new ApplicationInfo
            {
                applicationId = ""
            };
        }

        private void SetTokenPassport() {
            _service.tokenPassport = new TokenPassport
            {
                consumerKey = "",
                token = "",
                Role = "",
                account = "",
            };
        }

        private void SetPreferences() {
            _service.preferences = new Preferences { };
            _service.searchPreferences = new SearchPreferences { };
        }

        private void GetEmployee() {
            RecordRef employeeRef = new RecordRef
            {
                internalId = "312",
                type = RecordType.employee,
                typeSpecified = true,
            };

            Console.WriteLine(employeeRef);

            _service.get(employeeRef);

            Console.WriteLine("After customer get");
        }

        static void Main(string[] args)
        {
            SuiteTalkCourse suiteTalkCourse = new SuiteTalkCourse();

            suiteTalkCourse.GetEmployee();


        }
    }
}
