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
            //suiteTalkCourse.getEmployees();
            //suiteTalkCourse.syncEmployee();
            //suiteTalkCourse.createEmployee();
            //suiteTalkCourse.updateEmployee();
            //suiteTalkCourse.deleteEmployee()
            //suiteTalkCourse.updateEmployees();
            //suiteTalkCourse.GetFile();
            //suiteTalkCourse.getCustomers();
            //suiteTalkCourse.getExpenseReport();
            //suiteTalkCourse.updateExpenseReport();
            //suiteTalkCourse.getPerfomanceReview();
            //suiteTalkCourse.addPerfReview();
            //suiteTalkCourse.searchEmployees();
            //suiteTalkCourse.executeSavedSearch();
            suiteTalkCourse.searchEmployeesWithPages();

        }

        private void searchEmployeesWithPages() {
            EmployeeSearchBasic employeeSearchBasic = new EmployeeSearchBasic
            {
                email = new SearchStringField
                {
                    searchValue = "software.com",
                    @operator = SearchStringFieldOperator.hasKeywords,
                    operatorSpecified = true,
                }
            };

            SearchResult searchResult = _service.search(employeeSearchBasic);

            if (searchResult.status.isSuccess)
            {
                Console.WriteLine("Search Emplyee Success");
                Console.WriteLine("Total Records: {0}", searchResult.totalRecords);
                Console.WriteLine("Page: {0}", searchResult.pageIndex);

                int totalPages = searchResult.totalPages;
                int pageIndex = searchResult.pageIndex;
                string searchId = searchResult.searchId;

                foreach (Employee employee in searchResult.recordList)
                {
                    string department = null;
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

                for (int i = pageIndex;  i < totalPages;  i++)
                {
                    searchResult = _service.searchMoreWithId(searchId, i + 1);

                    Console.WriteLine("Page: {0}", searchResult.pageIndex);

                    if (searchResult.status.isSuccess)
                    {
                        foreach (Employee employee in searchResult.recordList)
                        {
                            string department = null;
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
                    }
                    else {
                        displyError(searchResult.status.statusDetail);
                    }
                }

            }
            else
            {
                Console.WriteLine("Search Emplyee Failed");
                displyError(searchResult.status.statusDetail);
            }
        }

        private void executeSavedSearch() {
            EmployeeSearchAdvanced employeeSearchAdvanced = new EmployeeSearchAdvanced { 
                savedSearchScriptId = "customsearch_sdr_emp_softwarecom_email",
                criteria = new EmployeeSearch { 
                    basic = new EmployeeSearchBasic { 
                        title = new SearchStringField { 
                            @operator = SearchStringFieldOperator.notEmpty,
                            operatorSpecified = true,
                        }
                    }
                },

                columns = new EmployeeSearchRow { 
                    basic = new EmployeeSearchRowBasic { 
                        entityId = new SearchColumnStringField[] { 
                            new SearchColumnStringField()
                        },
                        title = new SearchColumnStringField[] { 
                            new SearchColumnStringField()
                        }
                    }
                }
            };

            SearchResult searchResult = _service.search(employeeSearchAdvanced);

            if (searchResult.status.isSuccess)
            {
                Console.WriteLine("Advanced Saved Search Execution Success");
                Console.WriteLine("Total Records: {0}", searchResult.totalRecords);

                foreach (EmployeeSearchRow empSearchRow in searchResult.searchRowList)
                {
                    string name = empSearchRow.basic.entityId[0].searchValue;
                    string email = null;
                    string title = null;

                    if (empSearchRow.basic.email != null) {
                        email = empSearchRow.basic.email[0].searchValue.ToString();
                    } else {
                        email = "<none>";
                    }

                    if (empSearchRow.basic.title != null)
                    {
                        title = empSearchRow.basic.title[0].searchValue.ToString();
                    }
                    else
                    {
                        title = "<none>";
                    }

                    Console.WriteLine("Name: {0}", name);
                    Console.WriteLine("Job Title: {0}", title);
                    /* Console.WriteLine("Email: {0}", email);
                     Console.WriteLine("Supervisor Id: {0}", supervisorId);*/
                    Console.WriteLine("-------------------------");
                }
            }
            else {
                Console.WriteLine("Advanced Saved Search Execution Field");
                displyError(searchResult.status.statusDetail);
            }
        }

        private void searchEmployees() {
            EmployeeSearchBasic employeeSearchBasic = new EmployeeSearchBasic
            {
                email = new SearchStringField
                {
                    searchValue = "software.com",
                    @operator = SearchStringFieldOperator.hasKeywords,
                    operatorSpecified = true,
                }
            };

            TransactionSearchBasic transactionSearchBasic = new TransactionSearchBasic { 
                type = new SearchEnumMultiSelectField { 
                    searchValue = new string[] { "_expenseReport" },
                    @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                    operatorSpecified = true,
                }
            };

            EmployeeSearch employeeSearch = new EmployeeSearch
            {
                basic = employeeSearchBasic,
                transactionJoin = transactionSearchBasic,
            };

            SearchResult searchResult = _service.search(employeeSearch);

            if (searchResult.status.isSuccess)
            {
                Console.WriteLine("Search Emplyee Success");
                Console.WriteLine("Total Records: {0}", searchResult.totalRecords);

                foreach (Employee employee in searchResult.recordList)
                {
                    string department = null;
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

            }
            else {
                Console.WriteLine("Search Emplyee Failed");
                displyError(searchResult.status.statusDetail);
            }
        }

        private void addPerfReview() {
            CustomRecord perfReview = new CustomRecord
            {
                name = "Monthly Review",
                recType = new RecordRef { internalId = "106" },
                customFieldList = new CustomFieldRef[] {
                    new SelectCustomFieldRef() {
                        scriptId = "custrecord_sdr_perf_subordinate",
                        value = new ListOrRecordRef { internalId = "640"}
                    },
                    new DateCustomFieldRef() {
                        internalId = "custrecord_sdr_perf_date",
                        value = DateTime.Now
                    },
                    new SelectCustomFieldRef() {
                        scriptId = "custrecord_sdr_perf_review_type",
                        value = new ListOrRecordRef { internalId = "1"}
                    },
                    new BooleanCustomFieldRef() { 
                        internalId = "custrecord_sdr_perf_obj_met",
                        value = true
                    },
                    new StringCustomFieldRef() { 
                        internalId = "custrecord_sdr_perf_rating_code",
                        value = "A"
                    },
                    new DoubleCustomFieldRef() { 
                        internalId = "custrecord_sdr_perf_sal_incr_amt",
                        value = 300
                    },
                    new StringCustomFieldRef() { 
                        internalId = "custrecord_sdr_perf_supervisor_comments",
                        value = "Great Promotion"
                    }
                }
            };

            WriteResponse writeResponse = _service.add(perfReview);

            if (writeResponse.status.isSuccess)
            {
                Console.WriteLine("Add Perfomance Review success");
            }
            else
            {
                Console.WriteLine("Add Perfomance Review failed");
                displyError(writeResponse.status.statusDetail);
            }
        }
        private void getPerfomanceReview() { 
            CustomRecordRef prefRevRef = new CustomRecordRef { 
                internalId = "2",
                typeId = "106",
            };

            ReadResponse readResponse = _service.get(prefRevRef);

            if (readResponse.status.isSuccess)
            {
                Console.WriteLine("Get Perfomance Review Success");

                CustomRecord perReview = (CustomRecord)readResponse.record;
                string subordinate = "";
                string reviewType = "";
                double incrAmt = 0;

                foreach (CustomFieldRef customFieldRef in perReview.customFieldList)
                {
                    if (customFieldRef.scriptId == "custrecord_sdr_perf_subordinate") { 
                        SelectCustomFieldRef subordinateRef = (SelectCustomFieldRef)customFieldRef;
                        subordinate = subordinateRef.value.name;
                    }

                    if (customFieldRef.scriptId == "custrecord_sdr_perf_review_type")
                    {
                        SelectCustomFieldRef reviewTypeRef = (SelectCustomFieldRef)customFieldRef;
                        reviewType = reviewTypeRef.value.name;
                    }

                    if (customFieldRef.scriptId == "custrecord_sdr_perf_sal_incr_amt")
                    {
                        DoubleCustomFieldRef incrAmtRef = (DoubleCustomFieldRef)customFieldRef;
                        incrAmt = incrAmtRef.value; 
                    }
                }

                Console.WriteLine("Name: {0}", perReview.name);
                Console.WriteLine("Subordinate: {0}", subordinate);
                Console.WriteLine("Review Type: {0}", reviewType);
                Console.WriteLine("Increase Amount: {0}", incrAmt);
            }
            else {
                Console.WriteLine("Get Perfomance Review Failed");
                displyError(readResponse.status.statusDetail);
            }
        }

        private void updateExpenseReport() {
            ExpenseReport expenseReport = new ExpenseReport {
                internalId = "21839",
                expenseList = new ExpenseReportExpenseList {
                    replaceAll = false,
                    expense = new ExpenseReportExpense[] { 
                        new ExpenseReportExpense { 
                            line = 2,
                            lineSpecified = true,
                            foreignAmount = 500,
                            foreignAmountSpecified = true,
                            currency = new RecordRef {
                                internalId = "6",
                                type = RecordType.currency,
                                typeSpecified = true,
                            },
                            category = new RecordRef {
                                internalId = "8",
                            }

                        }
                    }
                }
            };

            WriteResponse writeResponse = _service.update(expenseReport);

            if (writeResponse.status.isSuccess)
            {
                Console.WriteLine("Expense Report Success");
            }
            else {
                Console.WriteLine("Expense Report Failed");
                displyError(writeResponse.status.statusDetail);
            }
        }

        private void getExpenseReport() {
            RecordRef expRepRef = new RecordRef { 
                internalId = "21839",
                type = RecordType.expenseReport,
                typeSpecified = true,
            };

            ReadResponse readResponse = _service.get(expRepRef);

            if (readResponse.status.isSuccess)
            {
                Console.WriteLine("Get Expense success");

                ExpenseReport expRep = (ExpenseReport)readResponse.record;
                Console.WriteLine("Expense ID: {0}", expRep.internalId);
                Console.WriteLine("Employee Name: {0}", expRep.entity.name);

                foreach (ExpenseReportExpense expLines in expRep.expenseList.expense) {

                    Console.WriteLine("\nLine: {0}", expLines.line);
                    Console.WriteLine("Expense Category: {0}", expLines.category.name);
                    Console.WriteLine("Expense Amount: {0}", expLines.amount);
                    Console.WriteLine("Expense Currency: {0}", expLines.currency.name);
                }
            }
            else {
                displyError(readResponse.status.statusDetail);
            }
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
            _service.searchPreferences = new SearchPreferences { 
                pageSize = 10,
                pageSizeSpecified = true,
            };
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

                foreach (CustomFieldRef customFieldRef in employee.customFieldList) {
                    if (customFieldRef.scriptId.Equals("custentity_sdr_it_proficiency")) {
                        SelectCustomFieldRef itProfFldRef = (SelectCustomFieldRef) customFieldRef;

                        Console.WriteLine("IT Proficiency : {0}", itProfFldRef.value.name);
                    }
                }
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
