Shellpower
==========

![image](https://user-images.githubusercontent.com/169280/126763521-c34eab37-8afd-4f18-9ffa-ee442ba14fe5.png)

Simulates insolation and shading on a curved solar array.
Simulates flux to each individual cell, does string calculations including bypass diodes, and computes power output.
Shows insolation, shading, and power output given a lat/lon/date/time, cell specification, and array specification.

I wrote this to help design the next [Stanford solar car](http://solarcar.stanford.edu). However, it's a generic solar array simulator, and I hope it will be useful for many projects.

Quick start
-----------
* Shellpower uses OpenGL for both input and simulation. You need a computer with decent graphics.
* Use Windows. Install Visual Studio. I strongly recommend VS C# 2010 Express Edition. 
* Clone the repo. I recommend installing Cygwin, and then installing git via the Cygwin installer. 
* Install OpenTK. It's in `dependencies/opentk.[...].exe`
* Open `src/ShellPower.sln`. You may have to delete and re-add the OpenTK dependencies. (Solution Explorer > References)
* Run Shellpower. It will load and display the example array. Click "Simulate" to run a simulation.


Instanteous inputs
------------------
* A 3D model of the array. Plain triangle mesh, 3dxml or STL format. Dimension should be in mm or m, the +Y axis points upward, and the +Z axis points forward.
* A texture of the of array. Top down view, orthographic projection, no shading, no antialiasing, just black pixels for each cell and white pixels everywhere else.
* Latitude, longitude, date and time.
* Solar insolation (square-plate). Default: 1000 W/m^2
* Solar cell parameters. Default: the parameters for a Sunpower C60

There is an example model--Luminos, the 2013 Stanford Solar Car-that illustrates how the program works and how the inputs should be formatted.

Instantaneous outputs
---------------------
* Total solar flux in W
* Computed array area in m^2 (useful for validating if your inputs are correct)
* Percent of array shaded
* Insolation at each cell
* Output power of each string (after stringing and bypass diode calculations)
* Output power of the array


Time-varying simulation
-----------------------
The time-varying simulation is a work in progress. It simply repeats the instantaneous simulation over the course of a day.



