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
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<SpriteManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static SpriteManager m_instance; // 싱글톤이 할당될 static 변수

    public SpriteAtlas itemAtlas;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    public Sprite LoadItemImage(int itemNo)
    {
        return itemAtlas.GetSprite("img_" + itemNo);
    }
}
