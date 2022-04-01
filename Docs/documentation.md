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

General
| Property  | Description |
| :------------- |  :------------- |
| Dungeon depth  | How many grid cells should be visible from eye point of view  |
| Dungeon width  | How many grid cells should be visible from left to right  |
| Filter mode  | Choose between sharp or interpolated rendering output.  |
| Screen size  | The dimension of the area/viewport in the game where the dungeon will be rendered. |
| Output folder  | The atlas files will be saved here. |

Camera settings
| Property  | Description |
| :------------- |  :------------- |
| Field of view  | Sets the camera field of view  |
| Y offset  | Offset camera position in Y axis  |
| Z offset  | Offset camera position in Z axis  |
| Lens shift Y  | This one is a little bit difficult to explain. A negative value will tilt camera down while still keeping all vertical edges 100% vertical. It prevents perspective distortion when tilting a 3D camera down. This emulates old classics with manually drawn graphics. Quite often they show more of the floor (tilting head down) but all vertical edges are still vertically straight. |

Light settings
| Property | Description |
| :------------- |  :------------- |
| Light mode | Set the light mode to be used (None, Point light or Directional light).  |

Light settings (Point light)
| Property  | Description |
| :------------- |  :------------- |
| Offset | The position of the light relative to the camera position.  |
| Color | The color of the light.  |
| Range | The range of the light.  |
| Intensity  | Light intensity multiplier.  |

Light settings (Directional light)
| Property  | Description |
| :------------- |  :------------- |
| Rotation | The rotation of the light. Use this to control where the light (sun) comes from. |
| Intensity  | Light intensity multiplier.  |
| Color  | The color of the light.  |

Render settings
| Property  | Description |
| :------------- |  :------------- |
| Rotation | The rotation of the light. Use this to control where the light (sun) comes from. |
| Intensity  | Light intensity multiplier.  |
| Color  | The color of the light.  |

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


