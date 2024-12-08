using System;
using System.Security.Cryptography;
using System.Text;
using Netsuite.com.netsuite.webservices;


namespace Netsuite
{
    class SuiteTalkCourse
    {
        public NetSuiteService _service;

        public SuiteTalkCourse() { 
            _service = new NetSuiteService();

            SetTokenPassport();
            SetPreferences();
        }
        static void Main(string[] args)
        {
            SuiteTalkCourse suiteTalkCourse = new SuiteTalkCourse();
            suiteTalkCourse.GetEmployee();
        }

        private void SetTokenPassport() {

            string compid = "TD2967825";
            string consumerkey = "35cabf17768c563ed787ccbe1f3986cdecf2cc5c612d94990ff30bccf3ab2053";
            string token = "9c591c4511d1e42f31c63f3b07c29f53bc9f06e3fe7e78ea5d9c36ccd599669c";
            string consumersecret = "c7733f772655d2ee6f578df73060599911d6c1e4d85864fa7d5b8dcb6fe355ed";
            string tokensecret = "219b85004de8fbdcbb5d6f2f2c9b14b0aa5ae311d498e18b407d725458f17276";

            string nonce = GenerateNonce();
            long timestamp = GetGmtMinusThreeHours();

            string baseString = GetBaseString(compid, consumerkey, token, nonce, timestamp);
            string key = GetKey(consumersecret, tokensecret);

            string signature = GetRfc2104CompliantSignature(baseString, key);

            Console.WriteLine("timestamp String:");
            Console.WriteLine(timestamp);
            Console.WriteLine("nonce String:");
            Console.WriteLine(nonce);
            Console.WriteLine("Base String:");
            Console.WriteLine(baseString);
            Console.WriteLine("Key:");
            Console.WriteLine(key);
            Console.WriteLine("Signature:");
            Console.WriteLine(signature);

            TokenPassportSignature sign = new TokenPassportSignature();
            sign.algorithm = "HMAC-SHA256";
            sign.Value = signature;

            _service.tokenPassport = new TokenPassport
            {
                consumerKey = consumerkey,
                token = token,
                account = compid,
                timestamp = timestamp,
                nonce = nonce,
                signature = sign,
                Role = "1058"
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

            ReadResponse response = _service.get(employeeRef);

            if (response.status.isSuccess) {
                Employee employee = (Employee)response.record;
                Console.WriteLine("Employee Name: " + employee.firstName + " " + employee.lastName);
                Console.WriteLine("Get success");
            } 
        }

        private static string GetBaseString(string account, string consumerKey, string tokenId, string nonce, long timestamp)
        {
            return string.Format("{0}&{1}&{2}&{3}&{4}", account, consumerKey, tokenId, nonce, timestamp);
        }

        private static string GetKey(string consumerSecret, string tokenSecret)
        {
            return string.Format("{0}&{1}", consumerSecret, tokenSecret);
        }

        private static string GetRfc2104CompliantSignature(string baseString, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] baseStringBytes = Encoding.UTF8.GetBytes(baseString);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var signatureBytes = hmacsha256.ComputeHash(baseStringBytes);
                return Convert.ToBase64String(signatureBytes);
            }
        }

        private static long GetGmtMinusThreeHours()
        {
            var now = DateTime.UtcNow;
            return ((DateTimeOffset)now).ToUnixTimeSeconds();
        }

        public static string GenerateNonce()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[32];
            rng.GetBytes(buffer);
            var nonce = Convert.ToBase64String(buffer).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return nonce;
        }
    }
}
