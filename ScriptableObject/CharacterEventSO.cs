using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//创建资产文件（没有继承Mono，所以需要资产文件给其他脚本调用）
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    //事件的订阅
    public UnityAction<Character> OnEventRaised;

    //事件的调用
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
