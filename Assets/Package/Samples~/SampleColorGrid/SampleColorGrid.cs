using System.Collections;
using System.Collections.Generic;
using com.mukarillo.prominentcolor;
using UnityEngine;

public class SampleColorGrid : MonoBehaviour {
    public List<string> urls = new List<string>(); 
    public Transform elemenTransformParent;
    public GameObject elementPrefab;
    public int maxColors                    = 3;
    public float colorLimiterPercentage     = 85f;
    public int uniteColorsTolerance         = 5;
    public float minimiumColorPercentage    = 10f;

    private Texture2D mTexture;

	private void Start()
	{
        mTexture = new Texture2D(2, 2);

        StartCoroutine(ProminentColorsInUrl(urls));
	}

    private IEnumerator ProminentColorsInUrl(List<string> imgUrls)
    {
        for (int i = 0; i < imgUrls.Count; i++)
        {
            string imgUrl = imgUrls[i];
            WWW www = new WWW(imgUrl);

            yield return www;
            www.LoadImageIntoTexture(mTexture);
            if (mTexture == null || (mTexture.width == 8 && mTexture.height == 8)) continue;
 
            Instantiate(elementPrefab, elemenTransformParent)
                .GetComponent<Element>()
                .SetupElement(mTexture, 
                    ProminentColor.GetColors32FromImage(mTexture,
                        maxColors,
                        colorLimiterPercentage,
                        uniteColorsTolerance,
                        minimiumColorPercentage));
        }
    }
}
