using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<SpriteManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static SpriteManager m_instance; // �̱����� �Ҵ�� static ����

    public SpriteAtlas itemAtlas;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    public Sprite LoadItemImage(int itemNo)
    {
        return itemAtlas.GetSprite("img_" + itemNo);
    }
}
