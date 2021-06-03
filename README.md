![Build](https://github.com/noobquire/social-network/actions/workflows/dotnet.yml/badge.svg?branch=main)

This is a project for KPI subject "Theory of Internet Networks"

Requirements and stack:
* This is ASP.NET Core REST API written in C# for single-page application social network
* Multi-layered architecture with relational database (MS SQL Server, SQLite) and EF Core
* Authentication and authorization capabilities with IdentityServer, with JWT Bearer auth
* API documentation with Swagger and Swagger UI
* Main logic covered with unit and integration tests with NUnit, FluentAssertions, Moq

Features:
* Register/authenticate with login and password to get JWT token
* User and profile management
* Posts in user profiles
* Attachment of small images to posts and messages, or as profile avatar
* Send personal messages to specified user
* Create group chat with multiple users and send messages to it
* Pagination for "Get All" requests
* Basic user and chat administration
* CI/CD pipeline with GitHub Actions with deployment to remote server  
Deployed API can be accessed at http://oleksiilytvynov.tk:5000/swagger/index.html 

Future ideas:
* React SPA frontend
* Post comments and likes
* Friend requests
* Better profile management
* Public groups with posts
* SignalR event notifications for clients
* Logging
