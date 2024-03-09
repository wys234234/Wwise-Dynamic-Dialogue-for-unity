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

    // ��ǰ�¼���ID
    private uint eStatic_Dialogue = AkSoundEngine.GetIDFromString("Static_Dialogue");
    private uint sequenceID; // ʹ sequenceID ��Ϊ��ĳ�Ա�������Ա����������з���

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

        // �������¼�����ȡ�ڵ�ID
        uint nodeID = AkSoundEngine.ResolveDialogueEvent(eStatic_Dialogue, aStatic_Dialogue, 5);
            // ��ӡ�ڵ�ID
            UnityEngine.Debug.Log($"Resolved nodeID for this event: {nodeID}");

            //���nodeID��Ч������ӵ������б�
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
                // ���nodeID��0��ʾ��Ч������ӵ������б�������
                UnityEngine.Debug.LogWarning("Dialogue event was rejected, will not be played.");
                AkSoundEngine.DynamicSequenceClose(sequenceID);

            }


    }

    public void StopDialogue()
    {

        if (sequenceID != 0)
        {
            // ֹͣ��̬����
            //UnityEngine.Debug.Log("����Debugֹͣ");
            AkSoundEngine.DynamicSequenceStop(sequenceID);
        }
        //UnityEngine.Debug.Log("����Debugֹͣ");
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
