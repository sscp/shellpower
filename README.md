Shellpower
==========

Simulates solar insolation on a mesh surface. 
Simulates instantaneous solar flux and average flux over a time period. 
Simple GUI to visualize shading.

***

I wrote this to help design the next [Stanford solar car](http://solarcar.stanford.edu) topshell.

Instanteous inputs
------------------
* A mesh surface. 3dxml format. Dimension should be in mm, the +Y axis points upward.
* A polygon in the XY plane, representing the solar array. Simply a list of 2D points. 
  Optional additional polygons to exclude. Shellpower intersects the resulting shape 
  with the top of the mesh surface. The entire mesh--not just the array--can cast shadows.
* Latitude, longitude, date and time
* Solar insolation (square-plate). Default: 1000 W/m^2

Instantaneous outputs
---------------------
* Total solar flux in W
* Computed array area in m^2
* Solar flux in W/m^2