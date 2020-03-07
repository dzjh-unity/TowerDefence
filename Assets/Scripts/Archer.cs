using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Defender
{
    // 初始化数值
    protected override void Init() {
        // 实际项目中，数值通常会从数据库或配置中读取
        m_attackArea = 5.0f;
        m_power = 1;
        m_attackInterval = 1.0f;
        // 创建模型，这里资源时写死的，实际项目中通常会从配置中读取
        CreateModel("archer");
        StartCoroutine(Attack()); // 执行攻击逻辑
    }

    protected override IEnumerator Attack() {
        while(m_targetEnemy == null || !m_isFaceEnemy) { // 如果没有目标则一直等待
            yield return 0;
        }
        m_anim.CrossFade("attack", 0.1f); // 播放攻击动画
        while(!m_anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) { // 等待进入攻击动画
            yield return 0;
        }
        float anim_length = m_anim.GetCurrentAnimatorStateInfo(0).length; // 获取攻击动画长度
        yield return new WaitForSeconds(anim_length * 0.5f); // 等待完成攻击动作
        if (m_targetEnemy != null) {
            // 查找攻击点位置
            Vector3 pos = this.m_model.transform.Find("atkpoint").position;
            // 创建弓箭
            Projectile.Create(m_targetEnemy.transform, pos, (Enemy enemy) => {
                enemy.SetDamage(m_power);
                m_targetEnemy = null;
            });
        }
        yield return new WaitForSeconds(anim_length * 0.5f); // 等待播放剩余的攻击动画
        m_anim.CrossFade("idle", 1.0f); // 播放待机动画
        yield return new WaitForSeconds(m_attackInterval); // 间隔一段时间
        StartCoroutine(Attack()); // 下一轮攻击
    }
}
