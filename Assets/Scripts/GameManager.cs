using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("GameScripts/GameManager")]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LayerMask m_groundlayer; // 地面的碰撞Layer
    public int m_wave = 1; // 波数
    public int m_waveMax = 10; // 最大波数
    public int m_life = 10; // 生命值
    public int m_coin = 30; // 金币数量

    // UI文字控件
    Text m_textWave;
    Text m_textLife;
    Text m_textCoin;
    // UI重新游戏按钮控件
    Button m_tryBtn;

    // 当前是否选中的创建防守单位的按钮
    bool m_isSelectedBtn = false;

    public bool m_debug = true; // 显示路点的debug开关
    public List<PathNode> m_PathNodes; // 路点

    public List<Enemy> m_EnemyList = new List<Enemy>(); // 所有敌人列表

    void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 创建UnityAction，在OnButCreateDefenderDown函数中响应按钮按下事件
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        // 创建UnityAction，在OnButCreateDefenderUp函数中响应按钮抬起事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);
        // 按钮按下事件
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);
        
        // 按钮按下事件
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        // 查找所有子物体，根据名称获取UI控件
        foreach (Transform t in this.GetComponentsInChildren<Transform>()) {
            if (t.name.CompareTo("Wave") == 0) {
                m_textWave = t.GetComponent<Text>();
                SetWave(1); // 设置为第1波
            } else if (t.name.CompareTo("Life") == 0) {
                m_textLife = t.GetComponent<Text>();
                m_textLife.text = string.Format("生命: <color=yellow>{0}</color>", m_life);
            } else if (t.name.CompareTo("Coin") == 0) {
                m_textCoin = t.GetComponent<Text>();
                m_textCoin.text = string.Format("金币: <color=yellow>{0}</color>", m_coin);
            } else if (t.name.CompareTo("TryBtn") == 0) {
                m_tryBtn = t.GetComponent<Button>();
                // 重新开始按钮点击事件
                m_tryBtn.onClick.AddListener(delegate() {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
                // 默认隐藏重新游戏按钮
                m_tryBtn.gameObject.SetActive(false);
            } else if (t.name.Contains("PlayerBtn")) {
                // 给防守单位按照添加按钮事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 如果选中创建士兵的按钮，则取消摄像机操作
        if (m_isSelectedBtn) {
            return;
        }
        // 鼠标或触屏操作，注意不同平台的Input代码不同
    #if(UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        bool press = Input.touches.Length > 0 ? true : false; // 手指是否触屏
        float mx = 0;
        float my = 0;
        if (press) {
            if (Input.GetTouch(0).phase == TouchPhase.Moved) { // 获得手指移动距离
                mx = Input.GetTouch(0).deltaPosition.x * 0.01f;
                my = Input.GetTouch(0).deltaPosition.y * 0.01f;
            }
        }
    #else
        bool press = Input.GetMouseButton(0);
        // 获取鼠标移动距离
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
    #endif
        // 移动摄像机
        GameCamera.Instance.Control(press, mx, my);
    }

    public void SetWave(int wave) {
        m_wave = wave;
        m_textWave.text = string.Format("波数: <color=yellow>{0}/{1}</color>", m_wave, m_waveMax);
    }

    public void SetDamage(int damage) {
        m_life -= damage;
        if (m_life <= 0) {
            m_life = 0;
            m_tryBtn.gameObject.SetActive(true);
        }
        m_textLife.text = string.Format("生命: <color=yellow>{0}</color>", m_life);
    }

    public bool SetCoin(int coin) {
        if (m_coin + coin < 0) {
            return false;
        }
        m_coin += coin;
        m_textCoin.text = string.Format("金币: <color=yellow>{0}</color>", m_coin);
        return true;
    }

    void OnButCreateDefenderDown(BaseEventData data) {
        m_isSelectedBtn = true;
    }
    
    void OnButCreateDefenderUp(BaseEventData data) {
        // 创建射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        // 检测是否与地面相碰撞
        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer)) {
            // 如果选中一个可用格子
            if (TileObject.Instance.getDataFromPosition(hitinfo.point.x, hitinfo.point.z) == (int)Defender.TileStatus.GUARD) {
                // 获得碰撞点位置
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);
                // 获得GridObject坐标位置
                Vector3 gridPos = TileObject.Instance.transform.position;
                // 获得格子大小
                float tilesize = TileObject.Instance.tileSize;
                // 计算出所点击格子的中心位置
                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x)/tilesize) * tilesize + tilesize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z)/tilesize) * tilesize + tilesize * 0.5f;
                // 获得选择的按钮GameObject，并简单通过按钮名字判断选择了哪个按钮
                GameObject go = data.selectedObject;
                if (go.name.Contains("1")) {
                    if (SetCoin(-15)) {
                        Defender.Create<Defender>(hitpos, new Vector3(0, 180, 0));
                    }
                } else if (go.name.Contains("2")) {
                    if (SetCoin(-20)) {
                        Defender.Create<Archer>(hitpos, new Vector3(0, 180, 0));
                    }
                }
            }
        }
        m_isSelectedBtn = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath() {
        m_PathNodes = new List<PathNode>();
        // 通过路点的Tag查找所有路点
        GameObject[] objs = GameObject.FindGameObjectsWithTag("PathNode");
        for (int i = 0; i < objs.Length; i++) {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());
        }
    }

    void OnDrawGizmos() {
        if (!m_debug || m_PathNodes == null) {
            return;
        }
        Gizmos.color = Color.red; // 将路点连线的颜色设为蓝色
        foreach(PathNode node in m_PathNodes) {
            if (node.m_next != null) {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
