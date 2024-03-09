using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using System.Diagnostics;
using System;
using UnityEngine.Events;

public class AKStaticDialogue : MonoBehaviour
{

    [Header("Switch")]
    [SerializeField] AK.Wwise.Switch sCharacters;
    [SerializeField] AK.Wwise.Switch sStatic_Trigger_Times;
    [SerializeField] AK.Wwise.Switch sStatic_SwitchID;

    [Header("State")]
    [SerializeField] AK.Wwise.State sScenes;
    [SerializeField] AK.Wwise.State sTask;


    public UnityEvent OnExecute;

    // 当前事件的ID
    private uint eStatic_Dialogue = AkSoundEngine.GetIDFromString("Static_Dialogue");
    private uint sequenceID; // 使 sequenceID 成为类的成员变量，以便在整个类中访问

    public void PlayDialogue()
    {
         sequenceID = AkSoundEngine.DynamicSequenceOpen(gameObject);

            uint[] aStatic_Dialogue = new uint[5]
            {
                sScenes.Id,
                sTask.Id,
                sCharacters.Id,
                sStatic_Trigger_Times.Id,
                sStatic_SwitchID.Id
            };

        // 解析此事件并获取节点ID
        uint nodeID = AkSoundEngine.ResolveDialogueEvent(eStatic_Dialogue, aStatic_Dialogue, 5);
            // 打印节点ID
            UnityEngine.Debug.Log($"Resolved nodeID for this event: {nodeID}");

            //如果nodeID有效，则添加到播放列表
            if (nodeID != 0)
            {
                AkPlaylist playlist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
                playlist.Enqueue(nodeID);
                AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);

                AkSoundEngine.DynamicSequencePlay(sequenceID);

                OnExecuteList();

            }
            else
            {
                // 如果nodeID是0表示无效，不添加到播放列表，不播放
                UnityEngine.Debug.LogWarning("Dialogue event was rejected, will not be played.");
                AkSoundEngine.DynamicSequenceClose(sequenceID);

            }


    }

    public void StopDialogue()
    {

        if (sequenceID != 0)
        {
            // 停止动态序列
            //UnityEngine.Debug.Log("里面Debug停止");
            AkSoundEngine.DynamicSequenceStop(sequenceID);
        }
        //UnityEngine.Debug.Log("外面Debug停止");
    }


    public void SetTask(AK.Wwise.State newState)
    {
        sTask = newState;
        sTask.SetValue();
    }

    public void SetCharacters(AK.Wwise.Switch newSwitch)
    {
        sCharacters = newSwitch;
        sCharacters.SetValue(gameObject);
    }

    public void SetStatic_Trigger_Times(AK.Wwise.Switch newSwitch)
    {
        sStatic_Trigger_Times = newSwitch;
        sStatic_Trigger_Times.SetValue(gameObject);
    }

    public void SetStatic_SwitchID(AK.Wwise.Switch newSwitch)
    {
        sStatic_SwitchID = newSwitch;
        sStatic_SwitchID.SetValue(gameObject);
    }

    public void OnExecuteList()
    {
        if (OnExecute != null)
            OnExecute.Invoke();
    }
}
