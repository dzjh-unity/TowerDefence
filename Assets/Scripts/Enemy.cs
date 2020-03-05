using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/Enemy")]

public class Enemy : MonoBehaviour
{
    public PathNode m_curNode; // 敌人的当前路点
    public int m_life = 15; // 敌人的生命值
    public int m_maxlife = 15; // 敌人的最大生命值
    public float m_speed = 2; // 敌人的移动速度
    public System.Action<Enemy> onDeath; // 敌人的死亡事件

    // Start is called before the first frame update
    void Start()
    {
        
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
        Destroy(this.gameObject); // 注意在实际项目中一般不要直接调用Destroy
    }
}
