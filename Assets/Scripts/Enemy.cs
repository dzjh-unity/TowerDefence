using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GameScripts/Enemy")]

public class Enemy : MonoBehaviour
{
    public PathNode m_curNode; // 敌人的当前路点
    public int m_life = 15; // 敌人的生命值
    public int m_maxlife = 15; // 敌人的最大生命值
    public float m_speed = 2; // 敌人的移动速度
    public System.Action<Enemy> onDeath; // 敌人的死亡事件

    Transform m_lifebarObj; // 敌人的UI生命条GameObject
    Slider m_lifebar;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.m_EnemyList.Add(this);

        // 读取生命条
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Canvas3D");
        // 创建生命条，将当前Transform设为父节点
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0); // 将生命条放置到角色头上
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifebarObj.GetComponentInChildren<Slider>();
        // 更新生命条位置和角度
        StartCoroutine(UpdateLifebar());
    }

    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
    }

    // 转向目标
    public void RotateTo() {
        var position = m_curNode.transform.position - transform.position;
        position.y = 0; // 保证只旋转y轴
        var targetRotation = Quaternion.LookRotation(position); // 获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0); // 进行旋转
    }

    // 向目标移动
    public void MoveTo() {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_curNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 1.0f) {
            if (m_curNode.m_next == null) {
                GameManager.Instance.SetDamage(1);
                DestroyMe();
            } else {
                m_curNode = m_curNode.m_next; // 更新到下一个路点
            }
        }
        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }

    public void DestroyMe() {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this); // 执行死亡回调
        Destroy(this.gameObject); // 注意在实际项目中一般不要直接调用Destroy
    }

    public void SetDamage(int damage) {
        m_life -= damage;
        if (m_life <= 0) {
            m_life = 0;
            // 每消灭一个敌人增加一些金币
            GameManager.Instance.SetCoin(5);
            DestroyMe();
        }
    }

    IEnumerator UpdateLifebar() {
        // 更新生命条值
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        // 更新角度，如始终面向摄像机
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0;
        StartCoroutine(UpdateLifebar());
    }
}
