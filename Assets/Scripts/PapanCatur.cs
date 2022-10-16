using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapanCatur : MonoBehaviour
{

    private GameObject[,] kotaks;
    private void Awake()
    {
        MembuatSemuaKotak(1, 8, 8);
    }

   
    private void MembuatSemuaKotak(float ukuranTile, int ukuranTileX, int ukuranTileY)
    {
        //bikin object baru
        kotaks = new GameObject[ukuranTileX, ukuranTileY];


        for (int i = 0; i < ukuranTileX; i++)
        {
            for (int j = 0; j < ukuranTileY; j++)
            {
                kotaks[i, j] = GenerateSatuKotak(ukuranTile, ukuranTileX, ukuranTileY);
            }
        }

    }

    private GameObject GenerateSatuKotak(float ukuranTile, int x, int y)
    {

    }
}
