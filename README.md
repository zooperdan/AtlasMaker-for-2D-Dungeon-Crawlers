# AtlasMaker for 2D Dungeon Crawlers
 A package for Unity which will allow you to render images atlases to be used in developing first person grid dungeon crawlers.
 
 ![This is an image](Media/screenshot.png)

## How to use
 
No time to write this yet but it's pretty straightforward to use. If you run into any problems or have some questions or suggestions then find me on the dungeoncrawler.org [Discord server](https://discord.gg/XerEseQ) or Twitter ([@zooperdan](https://twitter.com/zooperdan)).


## Installation

Download [AtlasMaker-for-2D-Dungeon-Crawlers.unitypackage](Package/AtlasMaker-for-2D-Dungeon-Crawlers.unitypackage) and import it into a Unity project.

There is only one required package and it can be downloaded from the built-in package manager.
> Package: Editor Coroutines (com.unity.editorcoroutines)

## TO-DO

>	- Support for rendering two different types of ground.
>		- Whole ground which can be drawn in-game as a background and flipped to simulate movement.
>		- Separate ground tiles
>	- Support for rendering two different types of ceilings.
>		- Whole ceiling which can be drawn in-game as a background and flipped to simulate movement.
>		- Separate ceiling tiles
>	- Implement two types of atlas packing.
>		- Optimized (This is the current one that is already implemented).
>		- Layed out. This method will arrange the atlas in a way so the side walls are connected to each other like they would be when rendered in a dungeon. This makes it easy to use this as a template for manual pixelling on top.
>	- Output the atlas data to other formats in addition to JSON (LUA, XML, CSV, BINARY etc.)
>	- Color quantization to indexed palettes presets.
>	- Option to write the image output to separate image files instead of a single atlas image.

## Change log

Version 0.5

>	- Rewrote and optimized the render/grab image process quite a bit.
>	- Added layer type property to AtlasLayer. It was previously located on the Atlas itself. Now it's removed from there.
>	- Added "Enabled" property to AtlasLayer. Uncheck this if you don't want this layer to be renderered to the atlas.
>	- Added "Render both sides" property to AtlasLayer. Check this to render all tiles, not just the ones left of center.
	
	Note: Rendering both sides allow you to have atlas layers with 3D objects that have depth.
	If unchecked the left side will have to be flipped in-game before rendering it on the right side.
	The drawback of rendering both sides is that the atlas filesize is considerably larger.

Version 0.4

>	- Added option to use either a Point Light or a Directional Light to light up the rendered output.
>	- Added ambient light color parameter.
>	- Fixed a bug with the rendering of Object type Atlases.
