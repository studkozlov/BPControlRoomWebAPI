# BPControlRoomWebAPI

## What Is It?
<p>Implementation of a **web API** for a web-based analogue of Blue Prism control room written in .NET Core 3.1 + lightweight HTML/JS **client**.</p>
<p>Ideal for monitoring purposes.</p>

## What Are Benefits?
<br>
* **Browser-based**: no need to install Blue Prism software on client machines. A good option for skilled business users who just want to monitor bot work progress.
* **Security**: 
	* Access to API is granted only to authorized Blue Prism users. 
	* API access control is based on JWT bearer authentication.
	* Access to BP database views is controlled by a specially created service account with limited 'read' rights.
	* Nothing is written or changed in BP database directly.
* **Easier deployment (vs BP official components)**: Run SQL scripts; deploy 2 components, API and client, to IIS; adjust config and settings.
* **Version-agnostic**: one client can connect to BP environments of different versions. No need to install several separate versions of BP to connect to different environments.
* **Old versions support**: is compatible with BP v7.0+, v6.0+ (that do not support official Blue Prism API). Older versions were not tested.
* **Familiar friendly interface**: html client mimics desktop control room well known to everyone working with BP
* **Self-sufficient client**: written in vanilla JS; well-portable incorporating HTML, CSS and JS in one file. Doesn't link to any external resources.
* **Extendable**: open for adding new functionality.


## What Are Disadvantages?
<br>
* **Limited functionality**: at the moment no scheduler management, some basic functions ("Force Retry", "Immediate Stop" etc) are absent, only "user/password" auth scheme is supported.
* **Not tested on high-volume environments.**

## What Can It Do?
<br>
* User authentication. Sign on/off between different environments.
* Monitoring of work queues and items, published processes, runtime resources, sessions.
* Start and request stopping sessions. Input parameters of business processes are supported (excl. TimeSpan, Image and Binary).
* Log Viewer.
* Client settings (API URL, refresh frequency).

## Installation steps
<br>
1. Prerequisites: 
	1. A VM the API will be deployed to should have connectivity to BP database(-s). Also, it should have BP installed locally (particularly, *automatec.exe* file).
	2. IIS. If not activated, look "Turn Windows features on and off"
	3. Install [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-3.1.32-windows-hosting-bundle-installer)
	4. Run script "*/Scripts/Prerequisites.sql*" across every BP database you want to connect to. The script will create 6 new views with a prefix "cstm_" and a service account with read access to only created views. Feel free to redefine username and password. Also, change DB name to a valid one before running it:

		11  /*Switch to a Blue Prism DB. IF YOUR BP DB IS NAMED DIFFERENTLY, CHANGE 'BluePrism' to that name*/
		12  USE [BluePrism]

2. Pre-build configuration:
	1. Feel free to redefine JWT bearer scheme parameters located in "*/Infra/Classes/AuthOptions.cs*".
	2. Modify file "*appsettings.json*" by redefining path to *automatec.exe* file, list of BP connection names and connection string for each of them. For User Id and Password, use those that were specified (redefined) in *Prerequisites.sql* script.
3. Deployment
	1. Create a web-site on IIS for API. Build solution and publish it (option for "Target" - "Folder") to the IIS site.
	2. <p>Create a web-site on IIS for API client. Copy all content of "*/Client*" folder to there.</p> <p>**OR**</p>
	<p>Distribute "*/Client*" folder separately and launch client by opening *index.html* file.</p>