using UnityEngine;

public class ChangeCustomMenu : MonoBehaviour
{
    public GameObject customMenuColors;
    public GameObject customMenuSkins;

    public void ActivarMenuColors()
    {
        customMenuColors.SetActive(true);
        customMenuSkins.SetActive(false);
    }

    public void ActivarMenuSkins()
    {
        customMenuColors.SetActive(false);
        customMenuSkins.SetActive(true);
    }

}
