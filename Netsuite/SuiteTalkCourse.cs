using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Services.Description;
using Netsuite.com.netsuite.webservices;


namespace Netsuite
{
    class SuiteTalkCourse
    {
        public NetSuiteService _service;
        private Preferences _preferences;
        private SearchPreferences _searchPreferences;
        private static Random random = new Random();

        public SuiteTalkCourse() { 
            _service = new NetSuiteService();

            //_service.Url = "https://td2967825.suitetalk.api.netsuite.com/services/NetSuitePort_2024_1";

            SetTokenPassport();
            //SetApplicationInfo();
            SetPreferences();
            GetEmployee();
        }


        private void SetApplicationInfo() {
            _service.applicationInfo = new ApplicationInfo
            {
                applicationId = "4677B53C-A94F-4D39-B028-1B1BBCA84787"
            };
        }

        private void SetTokenPassport() {

            string nonce = GetNonce();
            Console.WriteLine("Nonce is " + nonce);

            //computing for timestamp
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string timestamp = unixTimestamp.ToString();
            Console.WriteLine("Timestamp is " + timestamp);

            long convertedtimestamp = Convert.ToInt64(timestamp);
            string compid = "TD2967825";
            string role = "158";
            string consumerkey = "6e822fd9cddb3837f325f822bbe2363ff63ee6bac3bb61fe7fea604cd678b975";
            string token = "2135d208a1d07332343802bb99fe6f79cfeefe911d0eb6aacc1732c51014b5e1";
            string consumersecret = "956cedc1c5e81249dcdc2415cdec669c9be9ef5270f37fcdfa46a2a1801d4f09";
            string tokensecret = "d4d9efd37e61aa6c3bff41e413559e8620d654614a4f637f97e94fe8b63223a5";

            string baseString = string.Concat(compid, "&", consumerkey, "&", token, "&", nonce, "&", timestamp);
            string key = string.Concat(consumersecret, "&", tokensecret);

            Console.WriteLine("Base String is " + baseString);
            Console.WriteLine("Signing Key is " + key);

            string signature = "";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(baseString);
            using (HMACSHA1 myhmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = myhmacsha1.ComputeHash(messageBytes);
                signature = Convert.ToBase64String(hashmessage);
            }
            Console.WriteLine("Computed Signature is " + signature);

            TokenPassportSignature sign = new TokenPassportSignature();
            sign.algorithm = "HMAC-SHA256";
            sign.Value = signature;

            _service.tokenPassport = new TokenPassport
            {
                consumerKey = consumerkey,
                token = token,
                Role = role,
                account = compid,
                timestamp = convertedtimestamp,
                nonce = nonce,
                signature = sign
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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string GetNonce() {
            Random res = new Random();

            // String of alphabets  
            String str = "abcdefghijklmnopqrstuvwxyz";
            int size = 10;

            // Initializing the empty string 
            String ran = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly 
                int x = res.Next(26);

                // Appending the character at the  
                // index to the random string. 
                ran = ran + str[x];
            }

            Console.WriteLine("Random String:" + ran);
            return ran;
        }
    }
}
