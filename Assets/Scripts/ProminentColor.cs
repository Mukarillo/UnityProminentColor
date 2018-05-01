/* 
 * FileName: ProminentColor.cs
 * Author:   Murillo Pugliesi
 * GitHub:   https://github.com/Mukarillo
 */

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ColorAmount
{
    public Color32 color;
    public int amount;

    public ColorAmount(Color32 color, int amount)
    {
        this.color = color;
        this.amount = amount;
    }
}

public struct Vector2Int{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public static class ProminentColor 
{
    private static readonly List<Color32> colorList = new List<Color32>();
    private static List<ColorAmount> pixelColorAmount = new List<ColorAmount>();

    /// <summary>
    /// Removes the border.
    /// </summary>
    /// <param name="texture">Texture to remove borders.</param>
    /// <param name="compareColor">Color to be removed.</param>
    /// <param name="tolerance">Tolerance to compare the color to be removed.</param>
    public static Texture2D RemoveBorder(Texture2D texture, Color32 compareColor, float tolerance)
    {
        if (texture == null)
            throw new Exception("Texture null");
        
        texture = FloodFill(texture, compareColor, 0, 0, tolerance);
        //texture = FloodFill(texture, compareColor, 0, texture.height - 1, tolerance);
        //texture = FloodFill(texture, compareColor, texture.width - 1, 0, tolerance);
        //texture = FloodFill(texture, compareColor, texture.width - 1, texture.height - 1, tolerance);

        return texture;
    }

    /// <summary>
    /// Gets a list of the main colors from image.
    /// </summary>
    /// <returns>Returns a list of Color32 elements, with size lower or equal maxColorAmount.</returns>
    /// <param name="texture">Texture to get colors.</param>
    /// <param name="maxColorAmount">Max color amount.</param>
    /// <param name="colorLimiterPercentage">Value used to compare color amounts. If color amount percentage is less than this value, it will not be marked as valid color.</param>
    /// <param name="toleranceUniteColors">Value used to compare colors and unite them if is a match, useful to discard gradients.</param>
    /// <param name="minimiumColorPercentage">Value used to remove colors that have lower amount percentage compared with full image.</param>
    public static List<Color32> GetColors32FromImage(Texture2D texture, int maxColorAmount, float colorLimiterPercentage, int toleranceUniteColors, float minimiumColorPercentage)
    {
        if (texture == null)
            throw new Exception("Texture null");
        if (maxColorAmount <= 0)
            throw new Exception("maxColorAmount must be higher than 0");
        
        colorList.Clear();
        pixelColorAmount.Clear();

        var pixels = texture.GetPixels32();

        for (int i = 0; i < pixels.Length; i += 1)
        {
            var px = pixels[i];
            if (px.a < 255) continue;
            var c = pixelColorAmount.Find(x => x.color.Equals(px));
            if (c == null)
                pixelColorAmount.Add(new ColorAmount(px, 1));
            else
                c.amount++;
        }

        if (pixelColorAmount.Count <= 0)
            return null;

        pixelColorAmount = UniteSimilarColors(pixelColorAmount, toleranceUniteColors);

        pixelColorAmount = pixelColorAmount.OrderByDescending(x => x.amount).ToList();

        var totalAmount = pixelColorAmount.Sum(x => x.amount);

        var lastAmount = pixelColorAmount[0].amount;
        colorList.Add(pixelColorAmount[0].color);
        pixelColorAmount.RemoveAt(0);
        
        for (int i = 0; i < pixelColorAmount.Count; i++)
        {
            if (pixelColorAmount.Count <= i || colorList.Count >= maxColorAmount || pixelColorAmount[i].amount < (float)totalAmount / minimiumColorPercentage) continue;

            if (((float)pixelColorAmount[i].amount / (float)lastAmount) * 100f > (i == 0 ? 5f : colorLimiterPercentage))
            {
                colorList.Add(pixelColorAmount[i].color);
                lastAmount = pixelColorAmount[i].amount;
            }
        }

        return colorList;
    }

    /// <summary>
    /// Gets a list of the main colors from image.
    /// </summary>
    /// <returns>Returns a list of strings (Hexadecimal colors) elements, with size lower or equal maxColorAmount.</returns>
    /// <param name="texture">Texture to get colors.</param>
    /// <param name="maxColorAmount">Max color amount.</param>
    /// <param name="colorLimiterPercentage">Value used to compare color amounts. If color amount percentage is less than this value, it will not be marked as valid color.</param>
    /// <param name="toleranceUniteColors">Value used to compare colors and unite them if is a match, useful to discard gradients.</param>
    /// <param name="minimiumColorPercentage">Value used to remove colors that have lower amount percentage compared with full image.</param>
    public static List<string> GetHexColorsFromImage(Texture2D texture, int maxColorAmount, float colorLimiterPercentage, int toleranceUniteColors, float minimiumColorPercentage)
    {
        var color32List = GetColors32FromImage(texture, maxColorAmount, colorLimiterPercentage, toleranceUniteColors, minimiumColorPercentage);
        if (color32List == null) return null;

        List<string> hexColors = new List<string>();
        color32List.ForEach(x => hexColors.Add(ColorToHex(x)));

        return hexColors;
    }

    private static Texture2D FloodFill(Texture2D texture, Color32 compareColor, int x, int y, float tolerance)
    {
        var textureColors = texture.GetPixels32();
        Queue<Vector2Int> nodes = new Queue<Vector2Int>();
        nodes.Enqueue(new Vector2Int(x, y));

        var textureWidth = texture.width;
        var textureHeight = texture.height;
        var alphaColor = new Color32(0, 0, 0, 0);

        while (nodes.Count > 0)
        {
            var current = nodes.Dequeue();

            Color32 color;

            for (int i = current.x; i < textureWidth; i++)
            {
                color = textureColors[i + current.y * textureWidth];
                if (!ColorTest(compareColor, color, tolerance) || color.Equals(alphaColor))
                    break;
                textureColors[i + current.y * textureWidth] = alphaColor;
                if (current.y + 1 < textureHeight)
                {
                    color = textureColors[i + current.y * textureWidth + textureWidth];
                    if (ColorTest(compareColor, color, tolerance) && !color.Equals(alphaColor))
                        nodes.Enqueue(new Vector2Int(i, current.y + 1));
                }
                if (current.y - 1 >= 0)
                {
                    color = textureColors[i + current.y * textureWidth - textureWidth];
                    if (ColorTest(compareColor, color, tolerance) && !color.Equals(alphaColor))
                        nodes.Enqueue(new Vector2Int(i, current.y - 1));
                }
            }

            for (int i = current.x - 1; i >= 0; i--)
            {
                color = textureColors[i + current.y * textureWidth];
                if (!ColorTest(compareColor, color, tolerance) || !color.Equals(alphaColor))
                    break;
                textureColors[i + current.y * textureWidth] = alphaColor;
                if (current.y + 1 < textureHeight)
                {
                    color = textureColors[i + current.y * textureWidth + textureWidth];
                    if (ColorTest(compareColor, color, tolerance) && !color.Equals(alphaColor))
                        nodes.Enqueue(new Vector2Int(i, current.y + 1));
                }
                if (current.y - 1 >= 0)
                {
                    color = textureColors[i + current.y * textureWidth - textureWidth];
                    if (ColorTest(compareColor, color, tolerance) && !color.Equals(alphaColor))
                        nodes.Enqueue(new Vector2Int(i, current.y - 1));
                }
            }
        }

        texture.SetPixels32(textureColors);
        texture.Apply();

        return texture;
    }

    private static List<ColorAmount> UniteSimilarColors(List<ColorAmount> colorAmounts, int tolerance = 30, bool replaceSimilarColors = false)
    {
        List<ColorAmount> toReturn = new List<ColorAmount>();

        bool found = false;
        for (int i = 0; i < colorAmounts.Count; i++)
        {
            found = false;
            for (int j = 0; j < toReturn.Count; j++)
            {
                if (ColorTest(colorAmounts[i].color, toReturn[j].color, tolerance))
                {
                    if (replaceSimilarColors)
                    {
                        if (GetColorSaturation(toReturn[j].color) < GetColorSaturation(colorAmounts[i].color))
                            toReturn[j].color = colorAmounts[i].color;
                    }

                    toReturn[j].amount += colorAmounts[i].amount;
                    found = true;
                }
            }

            if (!found)
                toReturn.Add(colorAmounts[i]);
        }

        return toReturn;
    }

    private static bool ColorTest(Color32 c1, Color32 c2, float tol)
    {
        float diffRed = Mathf.Abs(c1.r - c2.r);
        float diffGreen = Mathf.Abs(c1.g - c2.g);
        float diffBlue = Mathf.Abs(c1.b - c2.b);

        float diffPercentage = ((diffRed / 255f) + (diffGreen / 255f) + (diffBlue / 255f)) / 3 * 100;

        return diffPercentage < tol;
    }

    private static string ColorToHex(Color32 color)
    {
        return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    private static double GetColorSaturation(Color32 color)
    {
        int max = Math.Max(color.r, Math.Max(color.g, color.b));
        int min = Math.Min(color.r, Math.Min(color.g, color.b));
        return (max == 0) ? 0 : 1d - (1d * min / max);
    }
}
