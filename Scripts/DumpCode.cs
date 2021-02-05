//// Decarotor code to flip the level upside-down
///*int levelCell_json = (level.height - y - 1) * level.width + x;
//level.tileType[levelCell] = level.tileType[levelCell_json];
//level.tileCorrect[levelCell] = level.tileCorrect[levelCell_json];
//level.tileSelected[levelCell] = level.tileSelected[levelCell_json];*/

//string tileSel = "Select  ";
//foreach (var l in level.tileSelected)
//{
//  tileSel += l + " ";
//}
//Debug.Log(tileSel);

//string tileCor = "Correct ";
//foreach (var l in level.tileCorrect)
//{
//  tileCor += l + " ";
//}
//Debug.Log(tileCor);

//string tileSel = "Select  ";
//foreach (var l in level.tileSelected)
//{
//  tileSel += l + " ";
//}
//Debug.Log(tileSel);

//string tileCor = "Correct ";
//foreach (var l in level.tileCorrect)
//{
//  tileCor += l + " ";
//}
//Debug.Log(tileCor);

// Load map directory and get all map files
//mapDirectory = new DirectoryInfo(Application.persistentDataPath);

// Test patterns
//int[] tilePattern = new int[4] { 0, 1, 1, 1};
//int[] tilePattern = new int[9] { 0, 1, 1, 1, 1, 0, 1, 0, 0};
//tilePattern = new int[16] { 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1 }; 
//Debug.Log(tilePattern.Length);
//Debug.Log(Convert.ToString(UnityEngine.Random.Range(0, 16), 2));