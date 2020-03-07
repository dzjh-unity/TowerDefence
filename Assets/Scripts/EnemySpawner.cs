using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/EnemySpawner")]

public class EnemySpawner : MonoBehaviour
{
    public PathNode m_startNode; // 起始节点
    private int m_liveEnemy = 0; // 存活的敌人数量
    public List<WaveData> waves; // 战斗波数配置数组
    int enemyIndex = 0; // 生成敌人数组的下标
    int waveIndex = 0; // 战斗波数组的下标

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies()); // 开始生成敌人
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemies() {
        yield return new WaitForEndOfFrame(); // 保证在Start函数后执行
        GameManager.Instance.SetWave(waveIndex + 1); // 设置UI上的波数显示
        WaveData wave = waves[waveIndex]; // 获得当前波的配置
        yield return new WaitForSeconds(wave.interval); // 生成敌人时间间隔
        while (enemyIndex < wave.enemyPrefab.Count) {
            Vector3 dir = m_startNode.transform.position - this.transform.position; // 初始方向
            GameObject enemyObj = (GameObject)Instantiate(wave.enemyPrefab[enemyIndex], transform.position, Quaternion.LookRotation(dir)); // 创建敌人
            Enemy enemy = enemyObj.GetComponent<Enemy>(); // 获得敌人的脚本
            enemy.m_curNode = m_startNode; // 设置敌人的第一个路点

            // 设置敌人数值，数值配置适合放到一个专用的数据库（SQLite数据库或Json、XML格式的配置）中
            enemy.m_life = wave.level *3;
            enemy.m_maxlife = enemy.m_life;

            m_liveEnemy++; // 增加敌人数量
            enemy.onDeath = new System.Action<Enemy>((Enemy e) => {m_liveEnemy--;}); // 当敌人死掉时减少敌人数量
            enemyIndex++; // 更新敌人数组下标
            yield return new WaitForSeconds(wave.interval); // 生成敌人时间间隔
        }
        // 创建完全部敌人，等待敌人全部被消灭
        while (m_liveEnemy > 0) {
            yield return 0;
        }
        enemyIndex = 0; // 重置敌人数组下标
        waveIndex++; // 更新敌人波数
        if (waveIndex < waves.Count) {
            StartCoroutine(SpawnEnemies()); // 继续生成后面的敌人
        } else {
            // 通知胜利
        }
    }

    // 在编辑器中显示一个图标
    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "spawner.tif");
    }
}
