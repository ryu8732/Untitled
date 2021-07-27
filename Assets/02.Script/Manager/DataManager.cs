using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // �̱��� ���ٿ� ������Ƽ
    public static DataManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<DataManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static DataManager m_instance; // �̱����� �Ҵ�� static ����

    [Serializable]
    public class Serialization<TKey, TValue>
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> GetDictionary() { return target; }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }
    }

    string jsonPath;

    public PlayerData playerData;
    //public QuestDataList questDataList;
    public TalkDataList talkDataList;

    public Dictionary<int, Item> itemDict;
    public Dictionary<int, PlayerStat> playerStatDict;
    public Dictionary<int, EnemyData> enemyDict;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }

        jsonPath = Application.persistentDataPath;

        playerData = new PlayerData();
        //questDataList = new QuestDataList();
        talkDataList = new TalkDataList();

        itemDict = new Dictionary<int, Item>();
        enemyDict = new Dictionary<int, EnemyData>();
        playerStatDict = new Dictionary<int, PlayerStat>();
    }

    private void Start()
    {
        // Json ������ �о� ��� ����Ʈ, ��ȭ ����Ʈ�� �ε��Ѵ�.
        LoadItemListFromJson();
        LoadQuestListFromJson();
        LoadTalkListFromJson();
        LoadExpListFromJson();
        LoadEnemyListFromJson();

        LoadPlayerDataFromJson();
    }

    // ���� �÷��̾� �����͸� Json ���Ͽ� �����Ѵ�.
    public void SavePlayerDataToJson(bool isExist = true)
    {
        if (isExist)
        {
            playerData.level = GameManager.instance.playerStatement.level;
            playerData.health = GameManager.instance.playerStatement.health;
            playerData.baseMaxHealth = GameManager.instance.playerStatement.baseMaxHealth;
            playerData.mana = GameManager.instance.playerStatement.mana;
            playerData.baseMaxMana = GameManager.instance.playerStatement.baseMaxMana;
            playerData.baseManaRegeneration = GameManager.instance.playerStatement.baseManaRegeneration;
            playerData.baseDamage = GameManager.instance.playerStatement.baseDamage;
            playerData.baseCriticalChance = GameManager.instance.playerStatement.baseCriticalChance;

            playerData.exp = GameManager.instance.playerStatement.exp;
            playerData.gold = GameManager.instance.playerStatement.inventory.gold;

            playerData.hasItemList = GameManager.instance.playerStatement.inventory.GetItemList();
            playerData.equipItemList = GameManager.instance.playerStatement.equipItemList.Values.ToList();

            playerData.questContainer = QuestManager.instance.questContainer;
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = GameManager.instance.currentSceneName;
            playerData.currentPosition = GameManager.instance.player.transform.position;
            playerData.currentRotation = GameManager.instance.player.transform.rotation;
        }

        else
        {
            GameManager.instance.isRespawn = true;
            playerData.level = 1;
            playerData.baseMaxHealth = playerStatDict[1].health;
            playerData.health = playerData.baseMaxHealth;
            playerData.baseMaxMana = playerStatDict[1].mana;
            playerData.mana = playerData.baseMaxMana;
            playerData.baseManaRegeneration = playerStatDict[1].manaRegeneration;
            playerData.baseDamage = playerStatDict[1].damage;
            playerData.baseCriticalChance = 0f;
            playerData.questContainer.activeQuests[10] = QuestManager.instance.allQuests[10];
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = "Town";
        }

        string fileName = "playerData.json";
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(jsonPath + "/" + fileName, jsonData);
    }

    // Json ������ �÷��̾� �����ͷκ��� ���ӻ��� �÷��̾� �����Ϳ� �ҷ��´�.
    public void LoadPlayerDataFromJson()
    {
        string fileName = "playerData.json";

        if (!File.Exists(jsonPath + "/" + fileName))
        {
            SavePlayerDataToJson(false);
        }

        string jsonData = File.ReadAllText(jsonPath + "/" + fileName);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);

        GameManager.instance.playerStatement.level = playerData.level;
        GameManager.instance.playerStatement.health = playerData.health;
        GameManager.instance.playerStatement.mana = playerData.mana;

        GameManager.instance.playerStatement.baseMaxHealth = playerData.baseMaxHealth;
        GameManager.instance.playerStatement.baseMaxMana = playerData.baseMaxMana;
        GameManager.instance.playerStatement.baseManaRegeneration = playerData.baseManaRegeneration;
        GameManager.instance.playerStatement.baseDamage = playerData.baseDamage;
        GameManager.instance.playerStatement.baseCriticalChance = playerData.baseCriticalChance;

        GameManager.instance.playerStatement.inventory.itemList = playerData.hasItemList;

        QuestManager.instance.questContainer = playerData.questContainer;
        QuestManager.instance.questContainer.ListToDictionary();

        for (int i = 0; i < playerData.equipItemList.Count; i++)
        {
            GameManager.instance.playerStatement.EquipItem(playerData.equipItemList[i]);
        }

        GameManager.instance.playerStatement.ApplyItems();

        GameManager.instance.playerStatement.healthSlider.value = GameManager.instance.playerStatement.health;
        GameManager.instance.playerStatement.manaSlider.value = GameManager.instance.playerStatement.mana;

        GameManager.instance.playerStatement.exp = playerData.exp;
        GameManager.instance.playerStatement.inventory.gold = playerData.gold;

        GameManager.instance.currentSceneName = playerData.currentSceneName;
        GameManager.instance.currentPosition = playerData.currentPosition;
        GameManager.instance.currentRotation = playerData.currentRotation;
    }

    // Json ������ ������ ����Ʈ���� ��� ������ �����͸� �ҷ��� allItemList�� �����Ѵ�. �̹� ������ ��ü�� itemNo�� ����������, �˻��� ���̼��� ���� itemNo�� Ű������ �ϴ� ��ųʸ��� �����Ͽ���.
    public void LoadItemListFromJson()
    {
        string fileName = "ItemList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        ItemList temp = JsonUtility.FromJson<ItemList>(jsonData.ToString());


        for (int i = 0; i < temp.itemList.Count; i++)
        {
            itemDict[temp.itemList[i].itemNo] = temp.itemList[i];
        }
    }

    // Json ������ ����Ʈ ����Ʈ���� ��� ����Ʈ �����͸� �ҷ��� �����Ѵ�.
    public void LoadQuestListFromJson()
    {
        string fileName = "QuestList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        QuestList questList = JsonUtility.FromJson<QuestList>(jsonData.ToString());

        foreach (Quest quest in questList.quests)
        {
            QuestManager.instance.allQuests[quest.questId] = quest;
        }
    }

    // Json ������ ��ȭ ����Ʈ���� ��� ��ȭ �����͸� �ҷ��� talkDataList�� �����Ѵ�. talkManager���� �ش� ��ȭ�� ��ųʸ� ���·� Ȱ���ϱ� ������ talkManager�� talkData���� ����Ʈ �����͸� ��ųʸ� ���·� �������ش�.
    public void LoadTalkListFromJson()
    {
        string fileName = "TalkList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        talkDataList = JsonUtility.FromJson<TalkDataList>(jsonData.ToString());

        foreach (TalkData talkData in talkDataList.talkDatas)
        {
            TalkManager.instance.talkDataList.Add(talkData.id, talkData.scripts.ToArray());
        }
    }

    public void LoadExpListFromJson()
    {
        string fileName = "PlayerStatTable";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        PlayerStatTable temp = JsonUtility.FromJson<PlayerStatTable>(jsonData.ToString());

        for (int i = 0; i < temp.playerStatTable.Count; i++)
        {
            playerStatDict[i + 1] = temp.playerStatTable[i];
        }
    }

    public void LoadEnemyListFromJson()
    {
        string fileName = "EnemyList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        EnemyDataList temp = JsonUtility.FromJson<EnemyDataList>(jsonData.ToString());

        for (int i = 0; i < temp.enemyList.Count; i++)
        {
            enemyDict[temp.enemyList[i].enemyId] = temp.enemyList[i];
        }
    }
}
