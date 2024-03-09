using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using System;
using UnityEngine.SocialPlatforms;
using UnityEngine.Events;

[System.Serializable]
public class SwitchPath
{
    [Range(0,10)]
    public int PathID;
    public Switch sSwitchPath;
}

[System.Serializable]
public class StatePath
{
    [Range(0, 10)]
    public int PathID;
    public State sStatePath;
}

[System.Serializable]
public class PathItem
{
    public enum ItemType
    {
        Switch,
        State
    }
    public int PathID;
    public ItemType Type;
    public Switch SwitchValue;
    public State StateValue;
}

public class AKStaticDialogue : MonoBehaviour
{
    public string sEventName;

    public List<SwitchPath> switchPaths;
    public List<StatePath> statePaths;


    // 成为类的成员变量，以便在整个类中访问
    private uint sequenceID;
    private uint eStatic_Dialogue;
    private uint[] aStatic_Dialogue = new uint[10];

    public UnityEvent EndDialogueEvent;

    private void Start()
    {
        //获取ID
        eStatic_Dialogue = AkSoundEngine.GetIDFromString(sEventName);

        // 合并和排序 switchPaths 和 statePaths
        List<PathItem> allPaths = new List<PathItem>();
        foreach (var sp in switchPaths)
        {
            allPaths.Add(new PathItem { PathID = sp.PathID, Type = PathItem.ItemType.Switch, SwitchValue = sp.sSwitchPath });
        }
        foreach (var sp in statePaths)
        {
            allPaths.Add(new PathItem { PathID = sp.PathID, Type = PathItem.ItemType.State, StateValue = sp.sStatePath });
        }
        allPaths.Sort((x, y) => x.PathID.CompareTo(y.PathID));

        // 填充 aStatic_Dialogue 数组
        for (int i = 0; i < allPaths.Count; i++)
        {
            if (i >= aStatic_Dialogue.Length) break; // 确保不超过数组大小

            if (allPaths[i].Type == PathItem.ItemType.Switch)
            {
                aStatic_Dialogue[i] = allPaths[i].SwitchValue.Id;
            }
            else if (allPaths[i].Type == PathItem.ItemType.State)
            {
                aStatic_Dialogue[i] = allPaths[i].StateValue.Id;
            }
        }
    }

    public void PlayDialogue()
    {
        sequenceID = AkSoundEngine.DynamicSequenceOpen(gameObject, (uint)AkCallbackType.AK_EndOfDynamicSequenceItem, DynamicSequenceCallback, null);

        // 检查数组中非零数量,也就是实际要用的数量
        int validEntriesCount = Array.FindAll(aStatic_Dialogue, id => id != 0).Length;
        // 解析此事件并获取节点ID
        uint nodeID = AkSoundEngine.ResolveDialogueEvent(eStatic_Dialogue, aStatic_Dialogue, (uint)validEntriesCount);
        // 节点ID
        Debug.Log($"Resolved nodeID for this event: {nodeID}");

        //如果nodeID有效，则添加到播放列表
        if (nodeID != 0)
        {
            AkPlaylist playlist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
            playlist.Enqueue(nodeID);
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);

            AkSoundEngine.DynamicSequencePlay(sequenceID);
        }
        else
        {
            // 如果nodeID是0表示无效，不添加到播放列表，不播放
            Debug.LogWarning("nodeID = 0");
            AkSoundEngine.DynamicSequenceClose(sequenceID);
        }
    }

    public void StopDialogue()
    {
        // 停止动态序列，停止此对话
        if (sequenceID != 0)
        {
            AkSoundEngine.DynamicSequenceStop(sequenceID);
        }
    }

    private void DynamicSequenceCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfDynamicSequenceItem)
        {
            // 将 AkCallbackInfo 强制转换为 AkDynamicSequenceItemCallbackInfo
            var dynamicSequenceInfo = (AkDynamicSequenceItemCallbackInfo)in_info;
            EndDialogueEvent.Invoke();
            //Debug.Log("结束对话了");
        }
    }


    //  Switch 或 State
    public void SetSwitchOrState(int pathID, Switch switchValue = null, State stateValue = null)
    {
        if (switchValue != null)
        {
            // 在 switchPaths 查找相应的 PathID 并设置 Switch
            var switchPath = switchPaths.Find(sp => sp.PathID == pathID);
            if (switchPath != null)
            {
                switchPath.sSwitchPath = switchValue;
                aStatic_Dialogue[pathID] = switchPath.sSwitchPath.Id;
            }
        }
        else if (stateValue != null)
        {
            // 在 statePaths 查找相应的 PathID 并设置 State
            var statePath = statePaths.Find(sp => sp.PathID == pathID);
            if (statePath != null)
            {
                statePath.sStatePath = stateValue;
                aStatic_Dialogue[pathID] = statePath.sStatePath.Id;
            }
        }
        //Debug.Log("动态对话被修改了");
    }
}
