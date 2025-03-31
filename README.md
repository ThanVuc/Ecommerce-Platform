# Ecommerce-Platform
## How to config and run project:
 ### 1. Install MongoDb, Redis, Sql Server Database, Asp.Net 8.0.3, Angular CLI: 18.2.10, Node: 22.13.1, Package Manager: npm 11.1.0, OS: win32 x64
 ### 2. In API, let's paste file appsettings.json, appsettings.Development.json and folder Properties contain launchSettings.json in main folder + cmd: dotnet restore
 ### 3. In UI: let's paste folder environment contain environment.development.ts and environment.ts into /src + cmd: npm install
 ### 4. Cmd: "dotnet watch run" in API and "ng s" in UI
## How to generate the random data:
 ### 1. In SeedData (Swagger), seed data for order: roles -> users -> categories -> warehouse -> shops -> VietNam location
 ### 2. Admin account: sinhhahaha1@gmail.com - string
 
#  Actor
 ## Admin, Customer, Shop's Owner, Drop-in Customer
 
#  Main Functional
 ## Authentication(JWT) - Authorization(JWT, Identity Framework)
 ## Shop Owners Management
   ### Products Management
   ### Orders Management
   ### Order Notifications (SignalR)
 ## Customer
   ### Home
   ### Search + filter by categories (auto completed)
   ### Carts + Order Product
   ### Purchases Product
 ## Drop-in Customer
   ### Home
   ### Search
# Conclusion
 ###  It's the first app that i do it myself from scratch, so that through each part the code style, UI maybe change a little. And, i do this app with the crucial target learns the programming so that i have tried quite a lot technique and technologies, therefore my code maybe is quite complicated and hard to read becauase the begin and middle phases i almost don't use the command
# Thanks for reading :))
### PS: My english is not good, sorry for that :))
