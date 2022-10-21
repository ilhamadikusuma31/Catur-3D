using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapanCatur : MonoBehaviour
{

    //catatan:
    //public - muncul di inspector bisa diakses script lain
    //[SerialiseField] private - muncul di inspector gabisa diakses script lain

    [Header("Bagian Art")] //ngasih nama aja di inspector
    [SerializeField] private Material bahanTile;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("prefabs dan materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    //logic
    private GameObject[,] kotaks;
    private ChessPiece[,] bidaks;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 batas;




    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;  //Camera.main ==> langsung ngambil camera default
            return;
        }

        RaycastHit ingfo;
        Ray r = currentCamera.ScreenPointToRay(Input.mousePosition);


        //kalo ada tile yang kena
        if (Physics.Raycast(r,out ingfo,100,LayerMask.GetMask("tile","hover")))
        {
            //cari idx yang sesuai
            Vector2Int koordinat = cariKoordinat(ingfo.transform.gameObject);

            //ketika current hover gaada
            if (currentHover == -Vector2Int.one)   //koordinat (-1,-1) ga masuk akal
            {
                //hover ke tile yang udah kena
                currentHover = koordinat;

                //tile yang kena maka akan diset nama layer hover
                kotaks[koordinat.x, koordinat.y].layer = LayerMask.NameToLayer("hover");
            }

            //ketika salah satu di hover, tile yang sebelumnya diset nama layernya hover kembali menjadi tile
            if (currentHover != koordinat)
            {
                kotaks[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("tile");

                //hover ke tile yang udah kena
                currentHover = koordinat;
                //tile yang kena maka akan diset nama layer hover
                kotaks[koordinat.x, koordinat.y].layer = LayerMask.NameToLayer("hover");
            }
        }
        else //kalo mouse ngarah keluar chessboard
        {
            if (currentHover != -Vector2Int.one)
            {
                kotaks[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("tile");
                currentHover = new Vector2Int(-1,-1);
            }
        }
    }

    private Vector2Int cariKoordinat(GameObject g) 
    {

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (kotaks[i, j] == g)
                {
                    return new Vector2Int(i, j);
                }

        //ga mungkin bisa ngakses ini
        return -Vector2Int.one;
    }



    private void Awake()
    {
        MembuatSemuaKotak(tileSize, 8, 8);
        SpawnAllPieces();
    }

   

    //membuat kotak2 catur
    private void MembuatSemuaKotak(float ukuranTile, int ukuranTileX, int ukuranTileY)
    {
        //seting model chessboard agar mengikuti kotakan yang udah dibuat
        yOffset += transform.position.y;
        batas = new Vector3((ukuranTileX / 2) * tileSize, 0, (ukuranTileY / 2) * tileSize)+boardCenter;



        //bikin object baru
        kotaks = new GameObject[ukuranTileX, ukuranTileY];


        for (int i = 0; i < ukuranTileX; i++)
            for (int j = 0; j < ukuranTileY; j++)
                kotaks[i, j] = GenerateSatuKotak(ukuranTile, i, j);
        
    }

    private GameObject GenerateSatuKotak(float ukuranTile, int x, int y)
    {
        GameObject satukotak = new GameObject(string.Format("X:{0}, Y:{1}",x,y));
        satukotak.transform.parent = transform;


        //kalo ngerender mesh dari unity nya tanpa import model
        //butuh 2 komponen yaitu MeshFilter dan MeshRenderer
        Mesh jala = new Mesh();
        satukotak.AddComponent<MeshFilter>().mesh = jala;
        satukotak.AddComponent<MeshRenderer>().material = bahanTile;


        //kerangka
        Vector3[] sisi = new Vector3[4];
        sisi[0] = new Vector3(x * ukuranTile, yOffset, y * ukuranTile)-batas;
        sisi[1] = new Vector3(x * ukuranTile, yOffset, (y + 1) * ukuranTile)-batas;
        sisi[2] = new Vector3((x + 1) * ukuranTile, yOffset, y * ukuranTile)-batas;
        sisi[3] = new Vector3((x + 1) * ukuranTile, yOffset, (y + 1) * ukuranTile)-batas;
        jala.vertices = sisi;


        //ngerender
        int[] segitiga = new int[] { 0, 1, 2, 1, 3, 2 };
        jala.triangles = segitiga;


        //biar kalo kena cahaya itu normal
        jala.RecalculateNormals();


        //set satu kotak ini jadi layer tile
        satukotak.layer = LayerMask.NameToLayer("tile");

        satukotak.AddComponent<BoxCollider>();

        return satukotak;
    }


    //generate bidak 

    //semua
    private void SpawnAllPieces()
    {
        int whiteTeam = 0;
        int blackTeam = 1;
        bidaks = new ChessPiece[8, 8];
        //{
        //  {rook   w  ,pawn w, "", "", "", "", pawn b, rook   b} 
        //  {kngiht w  ,pawn w, "", "", "", "", pawn b, knight b} 
        //  {bishop w  ,pawn w, "", "", "", "", pawn b, bishop b} 
        //  {queen  w  ,pawn w, "", "", "", "", pawn b, queen  b} 
        //  {king   w  ,pawn w, "", "", "", "", pawn b, king   b} 
        //  {bishop w  ,pawn w, "", "", "", "", pawn b, bishop b} 
        //  {knight w  ,pawn w, "", "", "", "", pawn b, knight b} 
        //  {rook   w  ,pawn w, "", "", "", "", pawn b, rook   b} 
        //}
        bidaks[0, 0] = SpawnSinglePiece(TipeBuahCatur.Rook, whiteTeam);
        bidaks[1, 0] = SpawnSinglePiece(TipeBuahCatur.Knight, whiteTeam);
        bidaks[2, 0] = SpawnSinglePiece(TipeBuahCatur.Bishop, whiteTeam);
        bidaks[3, 0] = SpawnSinglePiece(TipeBuahCatur.Queen, whiteTeam);
        bidaks[4, 0] = SpawnSinglePiece(TipeBuahCatur.King, whiteTeam);
        bidaks[5, 0] = SpawnSinglePiece(TipeBuahCatur.Bishop, whiteTeam);
        bidaks[6, 0] = SpawnSinglePiece(TipeBuahCatur.Knight, whiteTeam);
        bidaks[7, 0] = SpawnSinglePiece(TipeBuahCatur.Rook, whiteTeam);
        for (int i = 0; i < 8; i++)
        {
            bidaks[i, 1] = SpawnSinglePiece(TipeBuahCatur.Pawn, whiteTeam);
        }

        bidaks[0, 7] = SpawnSinglePiece(TipeBuahCatur.Rook, blackTeam);
        bidaks[1, 7] = SpawnSinglePiece(TipeBuahCatur.Knight,blackTeam);
        bidaks[2, 7] = SpawnSinglePiece(TipeBuahCatur.Bishop, blackTeam);
        bidaks[3, 7] = SpawnSinglePiece(TipeBuahCatur.Queen, blackTeam);
        bidaks[4, 7] = SpawnSinglePiece(TipeBuahCatur.King, blackTeam);
        bidaks[5, 7] = SpawnSinglePiece(TipeBuahCatur.Bishop, blackTeam);
        bidaks[6, 7] = SpawnSinglePiece(TipeBuahCatur.Knight, blackTeam);
        bidaks[7, 7] = SpawnSinglePiece(TipeBuahCatur.Rook, blackTeam);
        for (int i = 0; i < 8; i++)
        {
            bidaks[i, 1] = SpawnSinglePiece(TipeBuahCatur.Pawn, whiteTeam);
        }
    }

    private ChessPiece SpawnSinglePiece(TipeBuahCatur c, int tim)
    {
        //mengenereate sesuai indek ke sekian dari array prefabs dan ditaruh di transform parent(PapanCatur.cs)
        //lalu mengambil skripnya juga agar bisa kita kasih nama
        ChessPiece cp = Instantiate(prefabs[(int)c-1], transform).GetComponent<ChessPiece>();
        cp.team = tim;
        cp.tipe = c;

        return cp;
    }
}


