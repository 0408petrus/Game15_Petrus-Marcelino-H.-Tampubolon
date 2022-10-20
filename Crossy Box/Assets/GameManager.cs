using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Player player;
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent = 5;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int backDistance = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);

    TMP_Text gameOverText;   
    private void Start()
    {
        //setup gameover panel
        gameOverPanel.SetActive(false);
        gameOverText = gameOverPanel.GetComponentInChildren<TMP_Text>();
        
        //belakang
        for (int z = backDistance; z <= 0; z++)
        {

            CreateTerrain(grass, z);
        }

        //depan
        for (int z = 1; z <= frontDistance; z++)
        {

            var prefab = GetNextRandomTerrainPrefab(z);


            //instantiate
            CreateTerrain(prefab, z);
        }

        player.SetUp(backDistance, extent);
    }

    private int playerLastMaxTravel;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            player.SetUp(backDistance,extent);
        }
        // check player
        if (player.IsDie && gameOverPanel.activeInHierarchy == false)
            StartCoroutine(ShowGameOverPanel());

        //Infinite Terrain System
        if (player.MaxTravel == playerLastMaxTravel)
            return;

        playerLastMaxTravel = player.MaxTravel;
        //bikin ke depan
        var randTB = GetNextRandomTerrainPrefab(player.MaxTravel + frontDistance);
        CreateTerrain(randTB, player.MaxTravel + frontDistance);

        //hapus di belakang
        var lastTB = map[player.MaxTravel - 1 + backDistance];

        map.Remove(player.MaxTravel - 1 + backDistance);
        Destroy(lastTB.gameObject);

        //player.SetUp(player.MaxTravel + backDistance, extent);
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Game Over");
        gameOverText.text = "YOUR SCORE : " + player.MaxTravel;
        gameOverPanel.SetActive(true);
    }

    private void CreateTerrain(GameObject prefab, int zPos)
    {
        var go = Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        var tb = go.GetComponent<TerrainBlock>();
        tb.Build(extent);


        map.Add(zPos, tb);
    }

    private GameObject GetNextRandomTerrainPrefab(int nextPos)
    {

        bool isUniform = true;
        var tbRef = map[nextPos - 1];
        for (int distance = 2; distance <= maxSameTerrainRepeat; distance++)
        {
            if (map[nextPos - distance].GetType() != tbRef.GetType())
            {
                isUniform = false;
                break;
            }
        }

        if (isUniform)
        {
            if (tbRef is Grass)
                return road;
            else
                return grass;
        }


        //create block with 50% probabilty
        return Random.value > 0.5f ? road : grass;
    }
    
    
}
