# Blue Prism Control Room Web API

## What Is It?
<p>Implementation of a <b>web API</b> written in .NET Core 3.1 for a web-based analogue of Blue Prism control room  + lightweight HTML/JS <b>client</b>.</p>
<p>Ideal for monitoring purposes.</p>
<p>Link to a corresponding BP community thread: <a href="https://community.blueprism.com/discussion/personal-project-web-based-control-room#bm37cc54b8-b9e8-4aed-9d7e-018ab6ead2ae">Personal project - web-based Control Room</a>

## Short Demo
<p>Watch 8 min video</p>

[![IMAGE ALT TEXT](http://img.youtube.com/vi/VyGaMMfndpQ/0.jpg)](http://www.youtube.com/watch?v=VyGaMMfndpQ "Blue Prism - Web-based Control Room")

## What Are Benefits?
<ul>
<li><b>Browser-based</b>: no need to install Blue Prism software on client machines. A good option for skilled business users who just want to monitor bot work progress.</li>
<li><b>Security</b>: 
	<ul><li>Access to API is granted only to authorized Blue Prism users.</li>
	<li>API access control is based on JWT bearer authentication.</li>
	<li>Access to BP database views is controlled by a specially created service account with limited 'read' rights.</li>
	<li>Nothing is being written or changed in BP database directly.</li></ul></li>
<li><b>Authentication schemes</b>: Support of both, user/password and SSO authentication.</li>
<li><b>Easier deployment (vs BP official components)</b>: Run SQL scripts; deploy 2 components, API and client, to IIS; adjust config and settings.</li>
<li><b>Version-agnostic</b>: one client can connect to BP environments of different versions. No need to install several separate versions of BP to connect to different environments (valid only for SSO authentication with subsequent monitoring activities).</li>
<li><b>Old versions support</b>: is compatible with BP v7.0+ and v6.0+ (that are not compatible with official Blue Prism API). Older versions were not tested.</li>
<li><b>Familiar friendly interface</b>: html client mimics desktop control room well known to everyone working with BP.</li>
<li><b>Self-sufficient client</b>: written in vanilla JS; well-portable incorporating HTML, CSS and JS in one file. Doesn't link to any external resources (though SSO authentication option requires one external JS library).</li>
<li><b>Extendable</b>: open for adding new functionality.</li>
</ul>
<br>


## What Are Disadvantages?
<ul>
<li><b>Limited functionality</b>: at the moment no scheduler management, some basic functions ("Force Retry", <s>"Immediate Stop"</s> etc) are absent, <s>only "user/password" auth scheme is supported</s>.</li>
<li><b>Not tested on high-volume environments.</b></li>
</ul>
<br>

## What Can It Do?
<ul>
<li>User authentication using two methods, user-password or Azure AD SSO. Sign on/off between different environments.</li>
<li>Monitoring of work queues and items, published processes, runtime resources, sessions.</li>
<li>Start and request stopping sessions. Input parameters of business processes are supported (excl. TimeSpan, Image and Binary).</li>
<li>Basic sessions filtration.</li>
<li>Log Viewer.</li>
<li>Client settings (API URL, refresh frequency).</li>
</ul>
<br>

## Installation steps
<br>
<ol>
<li>Prerequisites: 
<ol>
<li>A VM the API will be deployed to should have connectivity to BP database(-s). Also, it should have BP installed locally (particularly, <i>automatec.exe</i> file).</li>
<li>IIS. If not activated, look at "Turn Windows features on and off"
<li>Install <a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-3.1.32-windows-hosting-bundle-installer">ASP.NET Core Hosting Bundle<a/></li>
<li>Run script "<i>/Scripts/Prerequisites.sql</i>" across every BP database you want to be able to connect to. The script will create 6 new views with a prefix "<i>cstm_</i>" and a service account with read access to only created views. Feel free to redefine username and password. Also, change DB name to a valid one before running it:

		11  /*Switch to a Blue Prism DB. IF YOUR BP DB IS NAMED DIFFERENTLY, CHANGE 'BluePrism' to that name*/
		12  USE [BluePrism]
</li>
</ol>
</li>
<li>Pre-build configuration:
<ol>
<li>Feel free to redefine JWT bearer scheme parameters located in "<i>/Infra/Classes/AuthOptions.cs</i>".</li>
<li>Modify file "<i>appsettings.json</i>" by redefining path to <i>automatec.exe</i> file, list of BP connection names and connection strings for each of them. For User Id and Password, use those that were specified (redefined) in <i>Prerequisites.sql</i> script.</li>
</ol>
</li>
<li>Deployment
<ol>
<li>Create a web-site on IIS for API. Build solution and publish it (option for "Target" - "Folder") to the IIS site.</li>
<li><p>Create a web-site on IIS for API client. Copy all content of "<i>/Client</i>" folder to there.</p> <p><b>OR</b></p>
	<p>Distribute "<i>/Client</i>" folder separately and launch client by opening <i>index.html</i> file.</p>
</li>
</ol>
</li>
</ol>
<br>

## Updates
<br>
<p><b>10/5/2023</b>: Authentication using Azure AD has been added</p>

[![IMAGE ALT TEXT](http://img.youtube.com/vi/9N35LKYlG04/0.jpg)](https://www.youtube.com/watch?v=9N35LKYlG04 "Blue Prism - SSO for web-based Control Room")

<br>
<p><b>12/8/2023</b>: "Immediate Stop" option has been added to sessions management</p>

<br>
<p><b>3/15/2024</b>: List of schedules and tasks has been added to the left "Control" Panel</p>

<br>
<p><b>3/16/2024</b>: "Run Now" option for schedules has been implemented</p>