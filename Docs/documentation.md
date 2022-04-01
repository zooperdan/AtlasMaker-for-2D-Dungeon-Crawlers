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

## Atlas layer

| Property  | Type  | Description |
| :------------- | :------------- | :------------- |
| Enabled  | Boolean | If unchecked then this layer will be omitted in the atlas generation process.  |
| Model  | Prefab | Link to the prefab that will be rendered on screen and eventually onto the atlas.  |
| ID  | String | Unique ID which will be used to identify this layer later in the JSON file. |
| Type  | Enum (Wall, Ground, Ceiling, Decal, Object) | Tells the generator what type of atlas this is and then will be handled accordingly. |
| Render mode  | Enum (All, Left, Middle) | Tells the generator which tiles in the grid to render. All means render all visible tiles in the grid. Middle is just the column straight in front of camera. Left is middle column and all tiles to the left. |
