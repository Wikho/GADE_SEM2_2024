using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[System.Serializable]
public struct SpawnableObject
{
    // This struct is really just a collection of info about each spawnable object, this could be scriptable objects, but thats overkill for the use case, its just to remove variable redundancy/repetition.
    public GameObject prefabLevel1, prefabLevel2, prefabLevel3;
    public float verticalOffset;
    public int woodCost;
    public int stoneCost;
}

public class ClickToSpawnManager : MonoBehaviour
{
    public static ClickToSpawnManager instance;


    private enum ClickMode
    {
        None = 0,
        Ballista,
        Resource,
        Delete,
        Upgrade,
    }

    // Defined here to be accessed by the BuildableTile component, not crazy extensible, 
    public SpawnableObject ballista;
    public SpawnableObject resource;


    NewInputSystem inputSystem;
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private ClickMode m_currentClickMode = ClickMode.None;
    [SerializeField] private bool _printDebug = false;
    [SerializeField] private int upgradeWoodCost;
    [SerializeField] private int upgradeStoneCost;
    

    void Awake()
    {
        // Singleton Logic
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        // Input System Initialization
        inputSystem = new();
        inputSystem.Gameplay.Enable();
        inputSystem.Gameplay.LeftClicking.performed += LeftClickPerformed;
    }

    /// <summary>
    /// The method called when the <i>Left Mouse Button</i> is pressed, <b>should not be called directly in normal circumstances</b>.
    /// </summary>
    private void LeftClickPerformed(InputAction.CallbackContext context)
    {
        //Check if the mouse is over any UI elements - I added this Maclome
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (_printDebug) Debug.Log("Click on UI element, ignoring.");
            return; //Exit the method if the pointer is over a UI element
        }

        if (_printDebug) Debug.Log("Casting a ray rn");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Is this technically mixing new and old input system? yes. Is it much much cleaner than getting the mouse pos with new input system, also yes.
        
        // TODO: Make it not fire when clicking UI elements. (Maybe doesnt happen anymore??)
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _raycastLayerMask, QueryTriggerInteraction.Ignore)) // Arbitrary distance thats far enough, Mathf.Infinity seems excessive for no reason.
        {
            if (_printDebug) Debug.Log("Hit something " + hit.transform.parent.gameObject.name);
            if (hit.transform.parent.gameObject.TryGetComponent(out Tile tile))
            {
                if (tile.GetTileType() == Tile.TileType.Build)
                {

                    switch (m_currentClickMode)
                    {
                        case ClickMode.Ballista:
                            if (hit.transform.parent.GetComponent<BuildableTile>().HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(ballista.woodCost, ballista.stoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().SpawnBallistaAbove();
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources(); // Call the UI function to show "Not enough resources" message.
                            }
                            break;

                        case ClickMode.Resource:
                            if (hit.transform.parent.GetComponent<BuildableTile>().HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(resource.woodCost, resource.stoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().SpawnResourceAbove();
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources(); // Call the UI function to show "Not enough resources" message.
                            }
                            break;
                        case ClickMode.Delete:
                            hit.transform.parent.GetComponent<BuildableTile>().DeleteCurrentTower();
                            break;
                        case ClickMode.Upgrade:
                            if (hit.transform.parent.GetComponent<BuildableTile>().IsClear)
                                return;
                            if (ResourceManager.Instance.CanPurchase(upgradeWoodCost, upgradeStoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().UpgradeTower();
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources(); // Call the UI function to show "Not enough resources" message.
                            }
                            break;
                        case ClickMode.None:
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (_printDebug) Debug.LogWarning("Did not get Tile Component");
            }
        }
    }


    private void OnDisable()
    {
        // Unsubscribe on object Disable/Destroy
        if (inputSystem != null)
        {
            inputSystem.Gameplay.LeftClicking.performed -= LeftClickPerformed;
        }
    }

    #region OnButton events for changing LeftClickMode
    // These are called in the inspector by buttons. Can be called manually to change the current ClickMode
    public void OnBallistaButtonPress()
    {
        m_currentClickMode = ClickMode.Ballista;
    }

    public void OnResourceButtonPress()
    {
        m_currentClickMode = ClickMode.Resource;
    }

    public void OnDestroyButtonPress()
    {
        m_currentClickMode = ClickMode.Delete;
    }

    public void OnUpgradeButtonPress()
    {
        m_currentClickMode = ClickMode.Upgrade;
    }
    #endregion
}
