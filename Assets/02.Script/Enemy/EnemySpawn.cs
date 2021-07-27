using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject playerDetectArea, traceArea;
    public int enemyId;
    public EnemyData enemyData;
    public Transform[] spawnPositions;

    public float respawnTime = 5f;

    private void Start()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Respawn(spawnPositions[i], 0);
        }
    }

    public void Respawn(Transform spawnPos, float time)
    {
        StartCoroutine(RespawnCoroutine(spawnPos, time));
    }

    IEnumerator RespawnCoroutine(Transform spawnPos, float time)
    {
        yield return new WaitForSeconds(time);

        enemyData = DataManager.instance.enemyDict[enemyId];
        GameObject enemyObj = ObjectPoolingManager.instance.GetQueue(enemyData.enemyName, false);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        SetEnemy(enemy, enemyData);

        enemy.enemySpawn = this;
        enemy.playerDetectArea = this.playerDetectArea;
        enemy.traceArea = this.traceArea;

        enemy.transform.SetParent(spawnPos);
        enemy.transform.position = spawnPos.position;
        enemy.transform.rotation = spawnPos.rotation;
        enemyObj.SetActive(true);
    }

    private void SetEnemy(Enemy enemy, EnemyData enemyData)
    {
        enemy.enemyType = enemyData.enemyType;
        enemy.enemyId = enemyData.enemyId;
        enemy.name = enemyData.enemyName;
        enemy.itemNo = enemyData.itemNo;
        enemy.dropChance = enemyData.dropChance;
        enemy.maxAmount = enemyData.maxAmount;
        enemy.exp = enemyData.exp;

        enemy.level = enemyData.level;
        enemy.baseMaxHealth = enemyData.maxHealth;
        enemy.health = enemy.baseMaxHealth;
        enemy.baseDamage = enemyData.damage;

        enemy.enemySlider.gameObject.SetActive(false);
        enemy.targetObj = null;

        enemy.dead = false;
    }
}
