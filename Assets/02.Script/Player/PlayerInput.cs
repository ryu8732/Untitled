using UnityEngine;

// �÷��̾� ĳ���͸� �����ϱ� ���� ����� �Է��� ����
// ������ �Է°��� �ٸ� ������Ʈ���� ����� �� �ֵ��� ����
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