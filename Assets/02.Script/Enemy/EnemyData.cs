
[System.Serializable]
public class EnemyData
{
    public enum EnemyType
    {
        normal,
        boss
    }

    public EnemyData(EnemyType enemyType, int enemyId, string enemyName, int itemNo, float dropChance, int maxAmount, int exp, int level, float maxHealth, float damage)
    {
        this.enemyType = enemyType;
        this.enemyId = enemyId;
        this.enemyName = enemyName;
        this.itemNo = itemNo;
        this.dropChance = dropChance;
        this.maxAmount = maxAmount;
        this.exp = exp;

        this.level = level;
        this.maxHealth = maxHealth;
        this.damage = damage;
    }

    public EnemyType enemyType;
    public int enemyId;
    public string enemyName;
    public int itemNo;
    public float dropChance;
    public int maxAmount;
    public int exp;

    public int level;
    public float maxHealth;
    public float damage;
}
