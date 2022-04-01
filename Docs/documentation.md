# Documentation

## Structure

The AtlasMaker structure is as follows:

> AtlasMaker
> - Atlas
>    - Atlas layer
>       - Prefab
>         - 3D model(s)
>         - Material(s)
>         - Texture(s)

AtlasMaker can generate multiple atlases at once, and a single atlas can have multiple atlas layers. 

## AtlasMaker

| Property  | Description |
| :------------- |  :------------- |
| Dungeon depth  | How many grid cells should be visible from eye point of view  |
| Dungeon width  | How many grid cells should be visible from left to right  |
| Filter mode  | Choose between sharp or interpolated rendering output.  |
| Screen size  | The dimension of the area/viewport in the game where the dungeon will be rendered. |
| Output folder  | The atlas files will be saved here. |

## Atlas

| Property  | Description |
| :------------- |  :------------- |
| Enabled  | If unchecked then this atlas and all its atlas layers will be omitted in the atlas generation process.  |
| ID  | A unique identifier of the atlas used for the output filename. |
| Layers  | A list of all the assigned atlas layers for this atlas. |

## Atlas layer

| Property  | Description |
| :------------- | :------------- |
| Enabled  | If unchecked then this layer will be omitted in the atlas generation process.  |
| Model  | Link to the prefab that will be rendered on screen and eventually onto the atlas.  |
| ID  | Unique ID which will be used to identify this layer later in the JSON file. |
| Type  | Tells the generator what type of atlas this is and then will be handled accordingly. |
| Render mode | Tells the generator which tiles in the grid to render. All means render all visible tiles in the grid. Middle is just the column straight in front of camera. Left is middle column and all tiles to the left. |


