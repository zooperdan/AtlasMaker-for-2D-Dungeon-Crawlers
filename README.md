# AtlasMaker-for-2D-DungeonCrawlers
 A package for Unity which will allow you to render images atlases to be used in developing first person grid dungeon crawlers.
 
 There is only one required package and it can be downloaded from the built-in package manager.
- Package name: Editor Coroutines (com.unity.editorcoroutines)

No time to write a how-to (yet) but it's pretty straightforward to use. If you run into any problems or have some questions or suggestions then find me on the dungeoncrawler.org Discord server or Twitter (@zooperdan).

![This is an image](Media/screenshot-01.png)

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
