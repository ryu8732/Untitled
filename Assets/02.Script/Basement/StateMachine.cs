using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���µ��� �ֻ��� �������̽�.
public interface IState
{
    public void OperateEnter();
    public void OperateUpdate();
    public void OperateExit();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }

    // defalut ���¸� ������ ����
    public StateMachine(IState defaultState)
    {
        CurrentState = defaultState;
        CurrentState.OperateEnter();
    }

    public void SetState(IState state)
    {
        // ���� ���¿� ���ؼ��� �ٽ� �Ҵ��� �� ����
        if (CurrentState == state)
        {
            Debug.Log("���� �̹� �ش� �����Դϴ�.");
            return;
        }

        CurrentState.OperateExit();

        CurrentState = state;
        CurrentState.OperateEnter();
    }

    public void DoOperateUpdate()
    {
        CurrentState.OperateUpdate();
    }
}
