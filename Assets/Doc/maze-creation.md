Dungeon Creation in Build007
=========================

A dungeon in build 007 is created by the following steps:

![Dungeon generation](dungeon-generation.png.png?raw=true "Dungeon generation process")

* The __SubdivisionAlgorithm__ (Assets/Scripts/DungeonGeneration/SubdivisionAlgorithm.cs) takes a rectangle of a certain hidth and height and recursively subdivides it. Each step a rectangle is divided into two randomly sized pieces. 

To see how this works, open the TestScene (Assets/Scenes/TestScene). Select the "LevelGenerator", deselect the 'traverse path' option and click the 'build level' button. Depending on the other settings, the level may appear as follows: 

![Level sub division generation](level-generator-build-division.png?raw=true "Title")

* The __Traverse Path__ (Assets/Scripts/DungeonGeneration/SplitRectTraversalAlgorithm.cs) takes the rectangles created by the SubdivisionAlgorithm. From there it starts in a random rectangle and keeps traversing through the rectangles until a certain criterium has been met. This can be either a number of steps (rectangles) or a distance, measured by the enumeration of the center of each rectangle in the path. 

The LevelGenerator provides a means to see how this works out, simply make sure the 'traverse path' option is turned on an click the 'build level button'. Depending on the other settings, the generated dungeon may look like this: 

![Level sub division generation](level-generator-traverse-path.png?raw=true "Title")

* The aforementioned steps build a dungeon layout. This layout needs to be translated into level elements. This happens in the __LevelGrid__  (Assets/Scripts/GameScripts/LevelGrid.cs). The level grid take the layout, which has the form of a tree, and for each node in the tree creates a rectangle representing a 'room' in the dungeon. Each room gets create with two walls (a left and bottom). This is done in the LevelGrid.CreateLevelElement call

* After a room has been created, a doorway to either the parent and / or children will be created depending if the door is on one of the two walls in the room. Adding doorways is executed by the LevelGrid.DrawDoorway call.

* Finally all rooms will be revisited and if there are no walls on the top side or right side, walls are added by the LevelGrid.TraceRoomBorders call. 