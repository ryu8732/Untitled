using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowCam : MonoBehaviour
{
    public Transform targetTr;

    public float rotateSpeed = 1.0f;

    public JoystickController rotateJoyStick;
    public Transform anchor;

    private Quaternion temp;

    private float xAxisLimit = 30f;
    private float clampX;

    Vector3 originPos;

    private void Start()
    {
        originPos = transform.localPosition;
    }
    void LateUpdate()
    {
        if (targetTr != null)
        {
            anchor.position = targetTr.position;

            if (rotateJoyStick.isMove)
            {
                anchor.transform.Rotate(0f, rotateJoyStick.JoyVec.x * rotateJoyStick.joyDisRatio * rotateSpeed, 0f);
                anchor.transform.Rotate(-rotateJoyStick.JoyVec.y * rotateJoyStick.joyDisRatio * rotateSpeed, 0f, 0f);

                clampX = anchor.transform.rotation.eulerAngles.x;
                clampX = clampX <= 180 ? clampX : (360 - clampX) * (-1);
                clampX = Mathf.Clamp(clampX, -xAxisLimit, xAxisLimit);

                anchor.transform.rotation = Quaternion.Euler(clampX, anchor.transform.rotation.eulerAngles.y, 0f);
            }
        }
        else
        {
            if(GameManager.instance.player != null)
            {
                targetTr = GameManager.instance.player.transform;

                SetPosToPlayerPos();
            }
        }
    }

    public void SetPosToPlayerPos()
    {
        anchor.transform.position = targetTr.position;
        anchor.transform.rotation = targetTr.rotation;
    }

    public void ShakeCamera(float time, float amount)
    {
        StartCoroutine(ShakeCameraCoroutine(time, amount));
    }

    private IEnumerator ShakeCameraCoroutine(float time, float amount)
    {
        float timer = 0;

        while(timer <= time)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originPos;
    }
}
