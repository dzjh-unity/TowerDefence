using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/TileObject")]

public class TileObject : MonoBehaviour
{
    public static TileObject Instance = null;

    // tile 碰撞层
    public LayerMask tileLayer;
    // tile 大小
    public float tileSize = 1;
    // x轴方向tile数量
    public int xTileCnt = 2;
    // z轴方向tile数量
    public int zTileCnt = 2;
    // 格子的数值，0表示锁定，无法摆放任何物体；1表示敌人通道；2表示可摆放防御单位。
    public int[] data;
    // 当前数据id
    [HideInInspector]
    public int dataID = 0;
    // 是否显示数据信息
    [HideInInspector]
    public bool debug = false;

    void Awake() {
        Instance = this;
    }

    // 初始化地图数据
    public void Reset() {
        data = new int[xTileCnt * zTileCnt];
    }

    // 获得相应tile的数值
    public int getDataFromPosition(float posx, float posz) {
        int index = (int)((posx - transform.position.x)/tileSize) * zTileCnt + (int)((posz - transform.position.z)/tileSize);
        if (index >= 0 && index < data.Length) {
            return data[index];
        }
        return 0;
    }

    // 设置相应tile的数值
    public void setDataFromPosition(float posx, float posz, int num) {
        int index = (int)((posx - transform.position.x)/tileSize) * zTileCnt + (int)((posz - transform.position.z)/tileSize);
        if (index >= 0 && index < data.Length) {
            data[index] = num;
        }
    }

    // 在编辑模式显示帮助信息
    void OnDrawGizmos() {
        if (!debug) {
            return;
        }
        if (data == null) {
            Debug.Log("Please reset data first");
            return;
        }
        Vector3 pos = transform.position;
        for (int i = 0; i < xTileCnt; i++) { // 画z方向轴辅助线
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0), transform.TransformPoint(tileSize *i, pos.y, tileSize * zTileCnt));
            for (int j = 0; j < zTileCnt; j++) {
                if ((i * zTileCnt + j) < data.Length && data[i * zTileCnt + j] == dataID) {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f, pos.y, pos.z + j * tileSize + tileSize * 0.5f), new Vector3(tileSize, 0.2f, tileSize));
                }
            }
        }
        for (int i = 0; i < zTileCnt; i++) { // 画x方向轴辅助线
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * i), transform.TransformPoint(tileSize * xTileCnt, pos.y, tileSize * i));
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
