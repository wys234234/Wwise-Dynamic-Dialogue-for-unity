using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using System.Diagnostics;

public class MatchStaticDialogueSample : MonoBehaviour
{
    public int sCharacterID;
    public AK.Wwise.Switch sCharacter;
    public AK.Wwise.Switch sStatic_DialogueID;

    public AKStaticDialogue staticDialogue;

    // ¸³Öµ sCharacter Switch
    public void CharacterSwitch()
    {
        if (staticDialogue != null && sCharacter != null)
        {
            staticDialogue.SetSwitchOrState(sCharacterID, sCharacter);
        }
    }

    public void Static_DialogueIDSwitch()
    {
        if (staticDialogue != null && sCharacter != null)
        {
            staticDialogue.SetSwitchOrState(0, sStatic_DialogueID);
        }
    }
}
