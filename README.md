SH-TestFlightAutoDeploy
=======================

A simple and easy to use Unity editor script to auto deploy your iOS game to TestFlight (https://testflightapp.com)

Requirements
-------
1. Unity iOS free or pro
2. Xcode*
3. TestFlight account


*You should have this tools already installed in your Mac, but is good to check:
/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild and 
/Applications/Xcode.app/Contents/Developer/usr/bin/xcrun*


Installation
-------
Just copy the SHTestFlightAutoDeployWindow.cs script to Editor folder inside your iOS Unity project.

Usage
-------
Click on menu item "Skahal / TestFlight Auto Deploy"

In the editor window "TestFlight Auto Deploy" type the informations below:

* App file name:
	* The name of your app file. 
	* eg: buildronRC

* Ouptput path:
	* The path you want to use to put build player files.
	* eg: /Users/giacomelli/Tech/Skahal/Temp/BuildronRC_IOS_TestFlight

* SDK version:
	* The iOS SDK version to build the app.
	* eg: iphoneos6.0

* Auto increment version:
	* The script can auto increment your minor version number from "Build version" (player settings). 
	* You just need the follow the convention #.# or #.#.# your build version.

* API token
	* Your TestFlight API token.
	* See your API token here: https://testflightapp.com/account/#api

* Team token
	* Your TestFlight Team token.
	* See your Team Token here: https://testflightapp.com/dashboard/team/edit/

* Notes
	* The notes you want to show in build deployed at TestFlight.

* Notify
	* If you want notify team about the deploy.

* Distribution lists
	* A comma separated text of what TestFlight list should be notfied about the deploy.
	* You can manage your lists here: https://testflightapp.com/dashboard/team/list/


**You can click "Send to TestFlight" button and wait for the process end (this take some minutes).**


Improvements
------
Create a fork of <a href="fork_select">SH-TestFlightAutoDeploy</a>. Did you change it? <a href="pull/new/master">Submit a pull request</a>.

License
------
Licensed under the Apache License, Version 2.0 (the "License").

Change Log
------
* 0.1 Builds and send to TestFlight. Save and load configurations by project.
* 0.2 Added help box for API token and Team token fields.
