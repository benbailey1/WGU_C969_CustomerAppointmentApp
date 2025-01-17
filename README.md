# CustomerAppointmentApp

## Built with WinForms using .NET Framework 4.8 

### Core Application Requirements

- WinForms application in C# (.NET Framework 4.8)
- Must connect to MySQL database
- No external libraries/frameworks allowed except .NET Framework
- Test credentials: username="test", password="test"

### 1. Login Form Requirements

- Detect user's location
- Support English and one other language for messages
- Username/password verification
- Log login attempts (timestamp + username) to "Login_History.txt"

### 2. Customer Management

- CRUD operations (Add/Update/Delete/Read)
- Required fields: name, address, phone number
- Validation rules:

- All fields must be trimmed and non-empty
- Phone numbers only allow digits and dashes

- Must include exception handling for all operations

### 3. Appointment Management

- CRUD operations
- Must capture appointment type
- Must link to customer records
- Validation rules:

- Business hours: 9:00 AM - 5:00 PM EST, Monday-Friday
- No overlapping appointments


- Must include exception handling for all operations

### 4. Calendar Features

- Monthly calendar view
- Ability to view appointments for specific days
- Auto-adjust for user timezone and DST
- 15-minute appointment alert on login

### 5. Reports (using collections & lambda expressions)
- Must generate 3 reports:

- Number of appointment types by month
- Schedule for each user
- One additional custom report

### Technical Requirements

- Must use collection classes
- Must incorporate lambda expressions
- Must handle exceptions properly
- Must support localization
- Must manage time zones
- Must handle file I/O (for login history)

### Submission Requirements

- Must be exported in Visual Studio format
- Must be submitted as ZIP file with proper structure
- Must be original work (max 30% total similarity, max 10% to any single source)
