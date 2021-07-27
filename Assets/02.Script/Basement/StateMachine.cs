using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//상태들의 최상위 인터페이스.
public interface IState
{
    public void OperateEnter();
    public void OperateUpdate();
    public void OperateExit();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }

    // defalut 상태를 생성시 설정
    public StateMachine(IState defaultState)
    {
        CurrentState = defaultState;
        CurrentState.OperateEnter();
    }

    public void SetState(IState state)
    {
        // 같은 상태에 대해서는 다시 할당할 수 없음
        if (CurrentState == state)
        {
            Debug.Log("현재 이미 해당 상태입니다.");
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
