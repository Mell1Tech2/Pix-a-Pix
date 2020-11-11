<<<<<<< HEAD
# Puzzle Game
 
=======
Methodology 1
Unity Hub
This is a tool used to develop the unity game. The most obvious change compared to other IDE is its focus on a scene and not any form of code. Most IDE have some drop and drag feature but it normally isnft the primary way you implement things in the IDE and only works for some languages.
In unity itfs in reverse the drop and drag UI if the primary feature and the scripts or code are the addition.
With this knowledge I will need to sift through the UI elements and turn them into code. The UI elements are inherently hard coded and not dynamic but if I can generate UI element with code, I can break this inbuilt cycle.
Working with Unity
The Scene and code division adds a very interesting problem especially for a puzzle game that needs dynamic generation. That problem is I donft need the scene at all, the only potential us is for something like backgrounds or UI.
The ability to create a dynamic puzzle requires the creation of the scenes assets that can be scaled. For future note a 2d game assets would instead be duplicated many times.
Another way to look at this is each scene asset is a separate view interface. They can instance once created and manipulated; this is how normal Unity development works but on a much smaller scale.
Pushed to the extreme one asset could serve the entire programfs needs, like letfs say like moving grass.
(Unity, 2019)


MoSCoW
A good idea of what I need to priorities in each prototype.
Must
1. Instanced grid asset
2. Instantiate one image into multiple grid tiles
3. UI for menu and end screen with reuse
Should
1. Update grid with changed on from a save variable
2. Saving variable parsed into a file
3. File can be edited and is parsed into the save variable
4. More than one level with some sort of selection menu
Could
1. Have image save file
2. Load save file into separate program to edit grid
3. Auto generated game logic based of save variable
4. Instantiate the UI assets.
Would
1. Add some navigation
2. Hints to help discover the image
>>>>>>> parent of 72bd128... Update and rename README.md to README.txt
=======
# Puzzle Game
 
>>>>>>> parent of 70d9278... Update Readme
