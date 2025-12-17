====================================================================================================
---------------/ General Information


	YumeKōdo is a full-stack web application that 
	 serves as a video game distribution platform.
	 Users can browse, download,
	 and interact with indie games through an intuitive interface,
	 enhanced by an AI assistant named "Yume" that provides intelligent
	 game recommendations and direct download links.


	This project was developed as an academic exam project,
	 showcasing modern web development practices and full-stack integration.


====================================================================================================
--------------------/ Key Features


	----------/ Core Functionality:


		. Game Library - Browse and download video games created by the developer.
		. User Authentication - Secure registration and login system with email verification.
		. Comment System - Leave reviews and comments on games.
		. User Profiles - Customizable profiles with avatar support.


	----------/ AI Assistant - "Yume":


		. Rule-Based AI Chat - Intelligent conversational assistant.
		. Direct Download Links - Ask Yume for game downloads and
		  receive instant download buttons.


	----------/ Security Features:


		. JWT Authentication - Secure token-based authentication.
		. Password Hashing - BCrypt password encryption.
		. Email Verification - SMTP-based email confirmation system.
		. Password Reset - Secure password recovery with verification codes.
		. Role-Based Access Control - User, Creator and Admin roles.


====================================================================================================
--------------------/ User Roles


	| The Application Supports Three User Roles:

		- User
		- Admin
		- Creator


====================================================================================================
--------------------/ Tech Stack


	----------/ Front-End:

		- Framework: Angular
		- Language: TypeScript
		- HTTP Client: Angular HttpClient
		- Routing: Angular Router


	----------/ Back-End:

		- Framework: ASP.NET Core Web API
		- Language: C#
		- ORM: Entity Framework Core
		- Authentication: JWT (JSON Web Tokens)
		- Password Hashing: BCrypt.Net
		- Email Service: System.Net.Mail (SMTP)


	----------/ Database:

		- Database: SQL Server
		- ORM: Entity Framework Core


====================================================================================================
--------------------/ Prerequisites

	| Before You Begin, Ensure You Have The Following Installed:


		- Node.js (v16 or higher)
		- Angular CLI
		- .NET SDK (6.0 or higher)
		- Visual Studio 2022
		- Visual Studio Code



====================================================================================================
--------------------/ Installation & Setup (Step-By-Step)


	----------/ Back-End Setup:


		- Step 1:

		Navigate to Backend Directory And Launch The "YumeKodo.sln"

		- Step 2:

		Launch The Project By Clicking On Green "Play" Button And Close It
		 After Project Loads Properly

		- Step 3:

		Navigate The "DataContext.cs" And Delete The "Connection String"

		- Step 4:

		In "Feature Search" Bar, Search For "SQL Server Object Explorer"
		 And Open It

		- Step 5:

		Drop Down The "SQL Server" Menu And Navigate The "Project Models"
		 And Drop Down The "Databases" Folder

		- Step 6:

		Find The "YumeKodo" Database And Drop Down The Menu Of It

		- Step 7:

		Right Click The "YumeKodo" Database And Open "Properties"

		- Step 8:

		Find The Property "Connection String" And Copy It

		- Step 9:

		Navigate Back To "DataContext.cs" And Paste The New "Connection String"

		- Step 10:

		Run The Project Again By Clicking Green "Play" Button And Close It After
		 Project Is Properly Loaded

		- Step 11:

		In "Feature Search" Bar Type "Terminal" And Open It

		- Step 12:

		Run Following Commands In Terminal:


			"Add-Migration Init" OR "dotnet ef migrations add init"

					   And

			"Update-Database" OR "dotnet ef database update"

		- Step 13:

		After Actions From Above, Launch The Project By Clicking The Green
		 "Play" Button Again One Last Time



	----------/ Front-End Setup:


		- Step 1:

		Open The Project With "Visual Studio Code"

		- Step 2:

		Find The "Terminal" On The Top-Left Part Of The Screen
		 And Open It

		- Step 3:

		Run The Command: "npm i" OR "npm install"

		- Step 4:

		After "Node Modules" Are Installed, Launch The Command "ng serve -o"


====================================================================================================
--------------------/ License

	| This Project Is An Academic Exam Project And Is Provided As-Is For Educational Purposes.


====================================================================================================
--------------------/ Author

	| Nezukari
	| GitHub: @Nezukari-Official






