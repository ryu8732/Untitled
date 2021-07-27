using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
{
    public string moveVerticalName = "Vertical";
    public string moveHorizontalName = "Horizontal";

    public float moveVertical { get; private set; } 
    public float moveHorizontal { get; private set; }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.playerStatement.dead)
        {
            moveVertical = 0;
            moveHorizontal = 0;
            return;
        }

        moveVertical = Input.GetAxis(moveVerticalName);
        moveVertical = Input.GetAxis(moveHorizontalName);

    }
}