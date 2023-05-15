using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class TeleportPoint : MonoBehaviour,IInteractable
{
    public SceneLoadEventSO loadEventSo;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        Debug.Log("传送！");
        loadEventSo.RaiseLoadRequestEvent(sceneToGo,positionToGo,true);
    }
}
