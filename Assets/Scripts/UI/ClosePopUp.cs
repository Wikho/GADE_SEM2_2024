using UnityEngine;

public class ClosePopUp : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    public void Close()
    {
        popup.SetActive(false);
    }
}
