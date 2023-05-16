using TMPro;
using UnityEngine;

public class BuildNeedGoldShow : MonoBehaviour
{
    [SerializeField] private TMP_Text goldNeed;
    [SerializeField] private TMP_Text buildingName;
    [SerializeField] private Animator _Animator;

    public void ActiveBuildingNeeds()
    {
        _Animator.SetBool("Appear", true);
    }
    public void DeactiveBuildingNeeds()
    {
        _Animator.SetBool("Appear", false);
    }
    public void SetGoldText(string text)
    {
        goldNeed.text = text;
    }
    public void SetNameText(string name)
    {
        buildingName.text = name;
    }
}
