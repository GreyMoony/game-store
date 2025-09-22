# How to use
## 1. UI
1. (Optional/Required once) You need to install Node.js to use npm.
   1. Download and install Node.js from https://nodejs.org.
   2. Run command `npm install -g http-server`.
2. Inside comand line go to **gamestore-ui-app** folder.
3. Run `http-server -c-1 -p 8080`.
4. Open <http://localhost:8080> in your browser
## 1.1 New Angular UI
1. Inside comand line go to **gamestore-ui** folder.
2. Run `ng serve --port 8080` (Angular CLI should be installed).
   
## 2. Payment microservice
1. Go to [microservice\publish](microservice\publish) folder.
2. Launch WebApi.exe.
3. Configure Visa and IBox accounts in account.json file.
   - You can add an object with *Id* and *Money* properties.
   - Add diferent objects for Visa account, where *Id* is card number, or for IBox account, where *Id* is *CustomerId* used in your API

## 3. MongoDB
1. Navigate to [mongo](mongo) folder in command line with admin rights.
2. To install mongo:  
	Run setup.cmd.
3. To import northwind db:  
	Run dbInit.cmd.
4. Run commands to start/stop service:
   - `net start MongoShard1Daemon`
   - `net stop MongoShard1Daemon`

## 4. Authentication
Default user:
- Login: "admin".
- Password: "Admin@123"