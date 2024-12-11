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
            //suiteTalkCourse.GetEmployee();
            //suiteTalkCourse.createEmployee();
            //suiteTalkCourse.updateEmployee();
            //suiteTalkCourse.deleteEmployee()
            //suiteTalkCourse.GetFile();
            //suiteTalkCourse.getEmployees();
            //suiteTalkCourse.updateEmployees();
            //suiteTalkCourse.getCustomers();
            suiteTalkCourse.syncEmployee();
        }

        private void syncEmployee()
        {
            Employee employee = new Employee
            {
                externalId = "EMP001",
                firstName = "Baruch",
                lastName = "Simmons",
                email = "info@livingwaters.com",
                phone = "111-111",
                subsidiary = new RecordRef { internalId = "17" },
                supervisor = new RecordRef { internalId = "15" },
                customForm = new RecordRef { internalId = "-10" }
            };

            WriteResponse writeResponse = _service.upsert(employee);

            if (writeResponse.status.isSuccess)
            {
                Console.WriteLine("Sync Employee success");
            }
            else
            {

                Console.WriteLine("Sync Employee failed");
                displyError(writeResponse.status.statusDetail);
            }
        }

        private void getCustomers() {
            RecordRef[] customersRef = new RecordRef[] {
                new RecordRef { 
                    internalId = "1264",
                    type = RecordType.customer,
                    typeSpecified = true,
                },
                new RecordRef {
                    internalId = "325",
                    type = RecordType.customer,
                    typeSpecified = true,
                }
            };

            ReadResponseList readResponseList = _service.getList(customersRef);

            foreach (ReadResponse response in readResponseList.readResponse) {
                if (response.status.isSuccess)
                {
                    Console.WriteLine("\nGet customers success");
                    
                    Customer customer = (Customer)response.record;

                    if (customer.currency != null) {
                        RecordRef currency = customer.currency;

                        Console.WriteLine("Currency: {0}", currency.name);
                    }
                }
                else {
                    displyError(response.status.statusDetail);
                }
            }
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

            /*Console.WriteLine("timestamp String:");
            Console.WriteLine(timestamp);
            Console.WriteLine("nonce String:");
            Console.WriteLine(nonce);
            Console.WriteLine("Base String:");
            Console.WriteLine(baseString);
            Console.WriteLine("Key:");
            Console.WriteLine(key);
            Console.WriteLine("Signature:");
            Console.WriteLine(signature);*/

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

            if (response.status.isSuccess)
            {
                Console.WriteLine("Get Employee success");

                string department = null;

                Employee employee = (Employee) response.record;

                if (employee.department != null)
                {
                    department = employee.department.name;
                }
                else {
                    department = "N/A";
                }

                Console.WriteLine("First Name: {0}", employee.firstName);
                Console.WriteLine("Last Name: {0}", employee.lastName);
                Console.WriteLine("Phone: {0}", employee.phone ?? "N/A");
                Console.WriteLine("Email: {0}", employee.email);
                Console.WriteLine("Job title: {0}", employee.title);
                Console.WriteLine("Departement: {0}", department);
            }
            else {
                Console.WriteLine("Get Employee failed");
                displyError(response.status.statusDetail);
            }
        }

        private void getEmployees() {
            RecordRef[] employeeRefs = new RecordRef[] {
                new RecordRef { 
                    internalId = "187",
                    type = RecordType.employee, 
                    typeSpecified = true,
                },
                new RecordRef {
                    internalId = "1257",
                    type = RecordType.employee,
                    typeSpecified = true,
                },
                new RecordRef {
                    internalId = "9999999999999999999999",
                    type = RecordType.employee,
                    typeSpecified = true,
                }
            };

            ReadResponseList responseList = _service.getList(employeeRefs);

            foreach (ReadResponse readResponse in responseList.readResponse) {
                if (readResponse.status.isSuccess)
                {
                    Console.WriteLine("\nGet Employee success");

                    string department = null;

                    Employee employee = (Employee)readResponse.record;

                    if (employee.department != null)
                    {
                        department = employee.department.name;
                    }
                    else
                    {
                        department = "N/A";
                    }

                    Console.WriteLine("First Name: {0}", employee.firstName);
                    Console.WriteLine("Last Name: {0}", employee.lastName);
                    Console.WriteLine("Phone: {0}", employee.phone ?? "N/A");
                    Console.WriteLine("Email: {0}", employee.email);
                    Console.WriteLine("Job title: {0}", employee.title);
                    Console.WriteLine("Departement: {0}", department);
                }
                else
                {
                    Console.WriteLine("Get Employees failed");
                    displyError(readResponse.status.statusDetail);
                }
            }
        }

        private void createEmployee() {
            Employee employee = new Employee {
                firstName = "Raymond",
                lastName = "Comfort",
                email = "info@livingwaters.com",
                phone = "555-555",
                subsidiary = new RecordRef { internalId = "17" },
                supervisor = new RecordRef { internalId = "15" }
            };

            WriteResponse writeResponse = _service.add(employee);

            if (writeResponse.status.isSuccess)
            {
                Console.WriteLine("Add Employee success");
            }
            else {

                Console.WriteLine("Add Employee failed");
                displyError(writeResponse.status.statusDetail);
            }
        }

        private void updateEmployee()
        {
            Employee employee = new Employee {
                internalId = "1257",
                externalId = "EMP001",
                //firstName = "Ray",
                nullFieldList = new String[] { "supervisor", "email" },
                customForm = new RecordRef { internalId = "-10" }
            };


            WriteResponse writeResponse = _service.update(employee);

            if (writeResponse.status.isSuccess)
            {
                Console.WriteLine("Update Employee success");
                
                RecordRef employeeRef = (RecordRef)writeResponse.baseRef;

                Console.WriteLine("Internal ID: {0}", employeeRef.internalId);
            }
            else
            {

                Console.WriteLine("Update Employee failed");
                displyError(writeResponse.status.statusDetail);
            }
        }

        private void updateEmployees() {
            RecordRef customFormRef = new RecordRef { internalId = "-10" };
            Employee[] employees = new Employee[] { 
                new Employee() { 
                    internalId = "1257",
                    fax = "555-555",
                    customForm = customFormRef
                },
                new Employee() {
                    internalId = "187",
                    fax = "555-555",
                    customForm = customFormRef
                },
                new Employee() {
                    internalId = "999999999999999",
                    fax = "555-555",
                    customForm = customFormRef
                }
            };

            WriteResponseList writeResponseList = _service.updateList(employees);

            foreach (WriteResponse writeResponse in writeResponseList.writeResponse) {
                if (writeResponse.status.isSuccess)
                {
                    Console.WriteLine("Update Employee success");

                    RecordRef employeeRef = (RecordRef)writeResponse.baseRef;

                    Console.WriteLine("Internal ID: {0}", employeeRef.internalId);
                }
                else
                {

                    Console.WriteLine("Update Employee failed");
                    displyError(writeResponse.status.statusDetail);
                }
            }
        }

        private void deleteEmployee() {
            RecordRef employeeRef = new RecordRef { 
                internalId = "1662", 
                type = RecordType.employee, 
                typeSpecified = true 
            };

            WriteResponse response = _service.delete(employeeRef, null);

            if (response.status.isSuccess) {
                Console.WriteLine("Delete Employee Success");
            }
            else
            {
                displyError(response.status.statusDetail);
            }


        }

        private void GetFile() {
            RecordRef fileRef = new RecordRef
            {
                internalId = "2442",
                type = RecordType.file,
                typeSpecified = true,
            };

            Console.WriteLine(fileRef);

            ReadResponse response = _service.get(fileRef);

            if (response.status.isSuccess) { 
                File file = (File)response.record;
                Console.WriteLine("File Name: " + file.name + "\n" + "File Size: " + file.fileSize);
                Console.WriteLine("Get File success");
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

        private static void displyError(StatusDetail[] statusDetails) {
            foreach (StatusDetail statusDetail in statusDetails)
            {
                Console.WriteLine("Type: {0}", statusDetail.type);
                Console.WriteLine("Code: {0}", statusDetail.code);
                Console.WriteLine("Message: {0}", statusDetail.message);
            }
        }
    }
}
