# honours

There are many subfolders here each with a different purpose.

csvparse - The main CSV tool I use to marge the Kinect depth data and the data recieve from Chao into one CSV file. It also inclused some visualization to vire strands from longest to smallest length
dataclean - a toy program to help visualize data. Used early on to help debug, not necessary for final program.
kinectpic (1) - The Unity Project folder, the main program of the project
spline - an early attempt at data visualization
zcleaner - a program to parse the Kinect depth data, this functionality merged into csvparse
zvis - a program to vizualize to point cloud. Very very very slow, vizualization attempted in Unity instead, see kinectpic (1)

Programs required:
Processing
Unity

To open main program:
1. Open unity and open the project folder kinectpic (1)
2. Click the play button at the top of the screen
3. use W,A,S,D to rotate the camera, mousewheel to zoom
3. use the colour picker but selecting a colour with the sliders, clicking the area of the image to paint and click apply
4. cut hair by moving the cursor to the first point of the desired cutting plane, press '1' on the number row, move the cursor to the second paint and press '2' on the number row. Reset by clicking the play button to stop the program and restart
5. to see the attempt at vizualizing the point cloud, click the vizualize button. The again is verry verry slow. The program uses bout 5GB of ram and I wasn't successful in increasing the performance.
