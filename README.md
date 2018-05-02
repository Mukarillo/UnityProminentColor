# Unity Prominent Color
This is a very simple tool to gather main colors of an image using Unity.

## How to use
You will find a static class called ProminentColor. You can call **GetColors32FromImage** to get a `List<Color32>` or **GetHexColorsFromImage** to get a `List<string>` (of hex colors). Both of the methods will take these parameters:

|name  |type  |description  |
|--|--|--|
|`texture` |**Texture2D** |*Texture to get colors*  |
|`maxColorAmount` |**int** |*The max length of the list to be returned*  |
|`colorLimiterPercentage` |**float** |*Value used to compare color amounts. If color amount percentage is less than this value, it will not be marked as valid color*  |
|`toleranceUniteColors` |**int** |*Value used to compare colors and unite them if is a match, useful to discard gradients*  |
|`minimiumColorPercentage` |**float** |*Value used to remove colors that have lower amount percentage compared with full image*  |

![result image](https://github.com/Mukarillo/UnityProminentColor/blob/master/result.png?raw=true)
*simple result, without much fine tunning*

## Removing the white (or any color) border
If your image has borders, you will probably have this color in the list. If you wish to remove this border, there is a helper in ProminentColor class called **RemoveBorder**, it returns the `Texture2D` without the border. This method take these parameters:

|name  |type  |description  |
|--|--|--|
|`texture` |**Texture2D** |*Texture to remove the border*  |
|`compareColor` |**Color32** |*Color to be removed*  |
|`tolerance` |**float** |*Tolerance to compare the color to be removed*  |

![removing borders](https://github.com/Mukarillo/UnityProminentColor/blob/master/remove_border.png?raw=true)<br />
*as you can see, there are small glitches, but white is not prominent anymore.*




