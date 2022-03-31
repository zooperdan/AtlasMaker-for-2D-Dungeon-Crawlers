using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public struct AtlasLayout
{
	public int width;
	public int height;
	public List<Texture2D> textures;
	public List<Rect> rects;

	public AtlasLayout(int w, int h)
	{
		width = w;
		height = h;
		textures = new List<Texture2D>();
		rects = new List<Rect>();
	}
};

public class TextureAtlasser 
{

	public static Texture2D MakeAtlas(ref Texture2D[] textures, out Rect[] packedRects, int padding)
	{
		AtlasLayout packResults = TextureAtlasser.PackTextures(textures, 512,512);
		Texture2D outAtlas = new Texture2D(packResults.width, packResults.height);

		textures = packResults.textures.ToArray();
		packedRects = packResults.rects.ToArray();

		Texture2D[] readables = new Texture2D[textures.Length];

		for (int i = 0; i < packResults.textures.Count; i++)
		{
			Rect rect = packResults.rects[i];
			Texture2D readableTex = packResults.textures[i];

			//load the image uncompressed
			/*
			string fileURL = AssetDatabase.GetAssetPath(packResults.textures[i]);
			byte[] imgByes = File.ReadAllBytes(fileURL);
			readableTex = new Texture2D(1,1,TextureFormat.ARGB32,true);
			readableTex.LoadImage(imgByes);
			*/
			readableTex.wrapMode = TextureWrapMode.Clamp;
			readables[i] = readableTex;

			int localPadding = Mathf.Min(padding, readableTex.width /4);

			Rect innerRect = packResults.rects[i];
			innerRect.x += localPadding;
			innerRect.y += localPadding;
			innerRect.width -= localPadding*2;
			innerRect.height -= localPadding*2;

			for (int x = (int)rect.x; x < (int)rect.x + (int)rect.width; x++)
			{
				for (int y = (int)rect.y; y < (int)rect.y + (int)rect.height; y++)
				{
					int xSample = x - (int)innerRect.x; 
					int ySample = y - (int)innerRect.y;

					Color pixel = readableTex.GetPixelBilinear(xSample / innerRect.width, ySample / innerRect.height);
					outAtlas.SetPixel(x,y, pixel);
				}
			}
				
			packedRects[i] = innerRect;
		}


		for (int x = 0; x < outAtlas.width; ++x)
		{
			for (int y = 0; y < outAtlas.height; ++y)
			{
				float closestDist = float.MaxValue;
				Color c = Color.clear;

				for (int r = 0; r < packResults.rects.Count; ++r)
				{
					Rect curRect = packResults.rects[r];
					if (curRect.Contains(new Vector2(x,y)))
					{
						closestDist = -1;
						break;
					}

					float d = DistanceToRect(curRect, x,y);

					if (d < closestDist)
					{
						closestDist = d;
						float uvX = (x - curRect.x) / curRect.width;
						float uvY = (y - curRect.y) / curRect.height;
						c = readables[r].GetPixelBilinear(uvX, uvY);
					}
				}

				if (closestDist > -1)
				{
					outAtlas.SetPixel(x,y,c);
				}
			}
		}

		outAtlas.wrapMode = TextureWrapMode.Clamp;
		outAtlas.Apply();


		return outAtlas;
	}

	private static float DistanceToRect(Rect r, int x, int y)
	{	
		float xDist = float.MaxValue;
		float yDist = float.MaxValue;

		xDist = Mathf.Max(Mathf.Abs(x - r.center.x) - r.width / 2, 0);
		yDist = Mathf.Max(Mathf.Abs(y - r.center.y) - r.height / 2, 0);
		return xDist * xDist + yDist * yDist;
	}

	public static AtlasLayout PackTextures(Texture2D[] textures, int maxWidth, int maxHeight)
	{
		AtlasLayout results = new AtlasLayout(maxWidth, maxHeight);

		List<Rect> freeRects = new List<Rect>();
		List<Texture2D> texturesToPlace = new List<Texture2D>(textures);
		texturesToPlace = texturesToPlace.OrderBy( x => x.width * x.height).ToList();

		freeRects.Add(new Rect(0,0,maxWidth, maxHeight));

		//Walk all textures and find the one that fits the best given 
		//our current freeRect list. Then start again

		while (texturesToPlace.Count > 0)
		{
			int bestShortSideScore = int.MaxValue;
			int bestLongSideScore = int.MaxValue;
			Texture2D bestTex = texturesToPlace[0];
			Rect bestRect = new Rect();

			foreach(Texture2D curTex in texturesToPlace)
			{
				int shortSideScore = int.MaxValue;
				int longSideScore = int.MaxValue;

				Rect target = FindIdealRect(curTex.width, 
					curTex.height, 
					freeRects, 
					ref shortSideScore, 
					ref longSideScore);

				if (shortSideScore < bestShortSideScore 
					|| (shortSideScore == bestShortSideScore && longSideScore < bestLongSideScore))
				{
					bestShortSideScore = shortSideScore;
					bestLongSideScore = longSideScore;
					bestTex = curTex;
					bestRect = target;
				}
			}

			if (bestRect.width > 0 && bestRect.height > 0)
			{
				RemoveRectFromFreeList(bestRect, freeRects);
				results.textures.Add(bestTex);
				results.rects.Add(bestRect);
				texturesToPlace.Remove(bestTex);

			}
			else break; //no room left
		}

		return results;
	}

	private static Rect FindIdealRect(int width, int height, List<Rect> freeRects, ref int bestShortSideFit, ref int bestLongSideFit)
	{
		Rect bestNode = new Rect();

		for (int i = 0; i < freeRects.Count; ++i)
		{
			if (freeRects[i].width >= width && freeRects[i].height >= height)
			{
				int remainingX = (int)(freeRects[i].width - width);
				int remainingY = (int)(freeRects[i].height - height);

				int shortSideFit = Mathf.Min(remainingX, remainingY);
				int longSideFit = Mathf.Max(remainingX, remainingY);

				if (shortSideFit < bestShortSideFit || 
					(shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit))
				{
					bestNode = new Rect(freeRects[i].x,freeRects[i].y, width, height);
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
				}
			}
		}

		return bestNode;
	}

	//remove a rect area from the freeRect list
	private static void RemoveRectFromFreeList(Rect rectToRemove, List<Rect> freeRects)
	{
		for (int i = 0; i < freeRects.Count; ++i)
		{
			Rect freeRect = freeRects[i];

			if (freeRect.Overlaps(rectToRemove))
			{
				if (rectToRemove.x < freeRect.x + freeRect.width && rectToRemove.x + rectToRemove.width > freeRect.x) {
					// New node at the top side of the used node.
					if (rectToRemove.y > freeRect.y && rectToRemove.y < freeRect.y + freeRect.height) {
						Rect newNode = freeRect;
						newNode.height = rectToRemove.y - newNode.y;
						freeRects.Add(newNode);
					}

					// New node at the bottom side of the used node.
					if (rectToRemove.y + rectToRemove.height < freeRect.y + freeRect.height) {
						Rect newNode = freeRect;
						newNode.y = rectToRemove.y + rectToRemove.height;
						newNode.height = freeRect.y + freeRect.height - (rectToRemove.y + rectToRemove.height);
						freeRects.Add(newNode);
					}
				}

				if (rectToRemove.y < freeRect.y + freeRect.height && rectToRemove.y + rectToRemove.height > freeRect.y) {
					// New node at the left side of the used node.
					if (rectToRemove.x > freeRect.x && rectToRemove.x < freeRect.x + freeRect.width) {
						Rect newNode = freeRect;
						newNode.width = rectToRemove.x - newNode.x;
						freeRects.Add(newNode);
					}

					// New node at the right side of the used node.
					if (rectToRemove.x + rectToRemove.width < freeRect.x + freeRect.width) {
						Rect newNode = freeRect;
						newNode.x = rectToRemove.x + rectToRemove.width;
						newNode.width = freeRect.x + freeRect.width - (rectToRemove.x + rectToRemove.width);
						freeRects.Add(newNode);
					}
				}

				freeRects.RemoveAt(i--);
			}
		}

		//remove free rects that are wholly contained by others

		for(int i = 0; i < freeRects.Count; ++i)
		{
			for(int j = i+1; j < freeRects.Count; ++j) 
			{
				if (freeRects[i].IsContainedIn(freeRects[j]))
				{
					freeRects.RemoveAt(i);
					--i;
					break;
				}

				if (freeRects[j].IsContainedIn(freeRects[i]))
				{
					freeRects.RemoveAt(j);
					--j;
				}
			}
		}
	}

}

static class RectExtension
{
	public static bool IsContainedIn(this Rect a, Rect b) 
	{
		return a.x >= b.x && a.y >= b.y 
			&& a.x+a.width <= b.x+b.width 
			&& a.y+a.height <= b.y+b.height;
	}
}

