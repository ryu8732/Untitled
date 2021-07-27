using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<ObjectPoolingManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static ObjectPoolingManager m_instance; // �̱����� �Ҵ�� static ����

    [System.Serializable]
    public class ObjectInfo {
        public GameObject prefab;
        public Queue<GameObject> queue = new Queue<GameObject>();
        public string key;
        public int amount;
    }

    public ObjectInfo[] objectInfo;
    public Dictionary<string, Queue<GameObject>> objPoolDictionary = new Dictionary<string, Queue<GameObject>>();

    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < objectInfo.Length; i++)
        {
            Init(objectInfo[i]);
        }
    }

    private void Init(ObjectInfo objectInfo)
    {
        objPoolDictionary.Add(objectInfo.key, objectInfo.queue);

        for (int i = 0; i < objectInfo.amount; i++)
        {
            GameObject clone = Instantiate(objectInfo.prefab, gameObject.transform);
            objectInfo.queue.Enqueue(clone);
            clone.SetActive(false);
        }
    }

    // �Լ��� ȣ���� �� Ű ���� �з����ͷ� ȣ���Ͽ� ��� ť���� �Լ��� ������ �ʿ� ���� Ű���� ���� �����ϵ��� �Ѵ�. (��ųʸ��� Ȱ��)
    public void InsertQueue(GameObject queueObj, string key)
    {
        objPoolDictionary[key].Enqueue(queueObj);
        queueObj.transform.SetParent(gameObject.transform);
        queueObj.SetActive(false);
    }

    // �Լ��� ȣ���� �� Ű ���� �з����ͷ� ȣ���Ͽ� ��� ť���� �Լ��� ������ �ʿ� ���� Ű���� ���� �����ϵ��� �Ѵ�. (��ųʸ��� Ȱ��)
    public GameObject GetQueue(string key, bool isActive = true)
    {
        GameObject queueObj = objPoolDictionary[key].Dequeue();
        queueObj.SetActive(isActive);
        return queueObj;
    }
}
