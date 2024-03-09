using UnityEngine;
using AK.Wwise;

public class Z_DynamicDialogueTest : MonoBehaviour
{
    public AKStaticDialogue akstaticdialogue;

    public Switch ChangeEncounter; 
    public Switch ChangeEncounter2; 
    public State ChangeSences; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TriggerZ();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerE();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TriggerF();
        }
    }

    public void TriggerZ()
    {
        akstaticdialogue.SetSwitchOrState(2, ChangeEncounter);
        akstaticdialogue.PlayDialogue();
    }

    public void TriggerE()
    {
        akstaticdialogue.SetSwitchOrState(0, null, ChangeSences);
        akstaticdialogue.PlayDialogue();
    }

    public void TriggerF()
    {
        akstaticdialogue.SetSwitchOrState(2, ChangeEncounter2);
        akstaticdialogue.PlayDialogue();
    }

}
