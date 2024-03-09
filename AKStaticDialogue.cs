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


    // ��Ϊ��ĳ�Ա�������Ա����������з���
    private uint sequenceID;
    private uint eStatic_Dialogue;
    private uint[] aStatic_Dialogue = new uint[10];

    public UnityEvent EndDialogueEvent;

    private void Start()
    {
        //��ȡID
        eStatic_Dialogue = AkSoundEngine.GetIDFromString(sEventName);

        // �ϲ������� switchPaths �� statePaths
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

        // ��� aStatic_Dialogue ����
        for (int i = 0; i < allPaths.Count; i++)
        {
            if (i >= aStatic_Dialogue.Length) break; // ȷ�������������С

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
        sequenceID = AkSoundEngine.DynamicSequenceOpen(gameObject);

        // ��������з�������,Ҳ����ʵ��Ҫ�õ�����
        int validEntriesCount = Array.FindAll(aStatic_Dialogue, id => id != 0).Length;
        // �������¼�����ȡ�ڵ�ID
        uint nodeID = AkSoundEngine.ResolveDialogueEvent(eStatic_Dialogue, aStatic_Dialogue, (uint)validEntriesCount);
        // �ڵ�ID
        Debug.Log($"Resolved nodeID for this event: {nodeID}");

        //���nodeID��Ч������ӵ������б�
        if (nodeID != 0)
        {
            AkPlaylist playlist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
            playlist.Enqueue(nodeID);
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);

            AkSoundEngine.DynamicSequencePlay(sequenceID);
        }
        else
        {
            // ���nodeID��0��ʾ��Ч������ӵ������б�������
            Debug.LogWarning("nodeID = 0");
            AkSoundEngine.DynamicSequenceClose(sequenceID);
        }
    }

    public void StopDialogue()
    {
        // ֹͣ��̬���У�ֹͣ�˶Ի�
        if (sequenceID != 0)
        {
            AkSoundEngine.DynamicSequenceStop(sequenceID);
        }
    }

    private void DynamicSequenceCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfDynamicSequenceItem)
        {
            // �� AkCallbackInfo ǿ��ת��Ϊ AkDynamicSequenceItemCallbackInfo
            var dynamicSequenceInfo = (AkDynamicSequenceItemCallbackInfo)in_info;
            EndDialogueEvent.Invoke();
            //Debug.Log("�����Ի���");
        }
    }


    //  Switch �� State
    public void SetSwitchOrState(int pathID, Switch switchValue = null, State stateValue = null)
    {
        if (switchValue != null)
        {
            // �� switchPaths ������Ӧ�� PathID ������ Switch
            var switchPath = switchPaths.Find(sp => sp.PathID == pathID);
            if (switchPath != null)
            {
                switchPath.sSwitchPath = switchValue;
                aStatic_Dialogue[pathID] = switchPath.sSwitchPath.Id;
            }
        }
        else if (stateValue != null)
        {
            // �� statePaths ������Ӧ�� PathID ������ State
            var statePath = statePaths.Find(sp => sp.PathID == pathID);
            if (statePath != null)
            {
                statePath.sStatePath = stateValue;
                aStatic_Dialogue[pathID] = statePath.sStatePath.Id;
            }
        }
        //Debug.Log("��̬�Ի����޸���");
    }
}
