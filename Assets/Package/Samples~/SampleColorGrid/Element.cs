using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    public GameObject colorPrefab;
    public Transform colorsParent;

    public Image imgTeamLogo;

    private Texture2D tex;

    public void SetupElement(Texture2D teamLogo, List<Color32> colors)
    {
        tex = new Texture2D(teamLogo.width, teamLogo.height);
        tex.SetPixels32(teamLogo.GetPixels32());
        tex.Apply();

        imgTeamLogo.sprite = Sprite.Create(tex, new Rect(0,0, tex.width, tex.height), Vector2.zero);

        for (int i = 0; i < colors.Count; i++)
            Instantiate(colorPrefab, colorsParent).GetComponent<Image>().color = colors[i];
    }
}
