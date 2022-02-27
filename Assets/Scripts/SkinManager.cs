using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SkinManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> skinList = new List<Sprite>();
    private int selectedSkinIndex = 0;
    public GameObject playerShip;

    void Start()
    {
        if(skinList.Count == 0)
        {
            Debug.LogError("There is no available skin, it's bad.");
        }
    }


    public void NextOption()
    {
        selectedSkinIndex = (selectedSkinIndex + 1) % skinList.Count;
        ChangeSkin(skinList[selectedSkinIndex]);
    }

    public void BackOption()
    {
        selectedSkinIndex = (selectedSkinIndex - 1) < 0 ? skinList.Count - 1 : selectedSkinIndex - 1;
        ChangeSkin(skinList[selectedSkinIndex]);
    }

    public void Confirm()
    {
        // playerShip.transform.localScale = new Vector3(1.5f, 1.5f, 1); // CHANGE IF YOU DON'T LIKE IT, I KNOW IT'S MESSY

        // PrefabUtility.SaveAsPrefabAsset(playerShip, "Assets/Prefabs/PlayerShip.prefab");
        SceneManager.LoadScene("Space");
    }

    private void ChangeSkin(Sprite skin)
    {
        spriteRenderer.sprite = skin;
    }
}
