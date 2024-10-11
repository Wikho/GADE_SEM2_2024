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
        Crystal,    
        Missel,     
        SandGlass,
        Resource,
        Delete,
        Upgrade,
    }

    // Defined here to be accessed by the BuildableTile component, not crazy extensible, 
    public SpawnableObject ballista;
    public SpawnableObject crystal;
    public SpawnableObject missel;
    public SpawnableObject sendGlass;
    public SpawnableObject resource;


    NewInputSystem inputSystem;
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private ClickMode m_currentClickMode = ClickMode.None;
    [SerializeField] private bool _printDebug = false;
    [SerializeField] private int totalBuildingsBuild = 0;
    [SerializeField] private int totalBuildingsUpgraded = 0;
    [SerializeField] private int totalResourceBuildings = 0;

    [Header("Upgrade Cost")]
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
        

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _raycastLayerMask, QueryTriggerInteraction.Ignore)) // Arbitrary distance thats far enough, Mathf.Infinity seems excessive for no reason.
        {
            if (_printDebug) Debug.Log("Hit something " + hit.transform.parent.gameObject.name);
            if (hit.transform.parent.gameObject.TryGetComponent(out Tile tile))
            {
                if (tile.GetTileType() == Tile.TileType.Build)
                {
                    BuildableTile buildableTile = hit.transform.parent.GetComponent<BuildableTile>();

                    switch (m_currentClickMode)
                    {
                        case ClickMode.Ballista:
                            if (hit.transform.parent.GetComponent<BuildableTile>().HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(ballista.woodCost, ballista.stoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().SpawnBallistaAbove();
                                totalBuildingsBuild++;
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources(); // Call the UI function to show "Not enough resources" message.
                            }
                            break;

                        case ClickMode.Crystal:
                            if (buildableTile.HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(crystal.woodCost, crystal.stoneCost))
                            {
                                buildableTile.SpawnCrystalAbove();
                                totalBuildingsBuild++;
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources();
                            }
                            break;

                        case ClickMode.Missel:
                            if (buildableTile.HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(missel.woodCost, missel.stoneCost))
                            {
                                buildableTile.SpawnMisselAbove();
                                totalBuildingsBuild++;
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources();
                            }
                            break;

                        case ClickMode.SandGlass:
                            if (buildableTile.HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(sendGlass.woodCost, sendGlass.stoneCost))
                            {
                                buildableTile.SpawnSendGlassAbove();
                                totalBuildingsBuild++;
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources();
                            }
                            break;

                        case ClickMode.Resource:
                            if (hit.transform.parent.GetComponent<BuildableTile>().HasObjectAbove)
                                return;
                            if (ResourceManager.Instance.CanPurchase(resource.woodCost, resource.stoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().SpawnResourceAbove();
                                totalResourceBuildings++;
                            }
                            else
                            {
                                UiManager.Instance.NotEnoughResources(); // Call the UI function to show "Not enough resources" message.
                            }
                            break;

                        case ClickMode.Delete:
                            hit.transform.parent.GetComponent<BuildableTile>().DeleteCurrentTower();
                            totalBuildingsBuild--;
                            break;

                        case ClickMode.Upgrade:
                            if (hit.transform.parent.GetComponent<BuildableTile>().IsClear)
                                return;
                            if (ResourceManager.Instance.CanPurchase(upgradeWoodCost, upgradeStoneCost))
                            {
                                hit.transform.parent.GetComponent<BuildableTile>().UpgradeTower();
                                totalBuildingsUpgraded++;
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

    public void OnCrystalButtonPress()
    {
        m_currentClickMode = ClickMode.Crystal;
    }

    public void OnMisselButtonPress()
    {
        m_currentClickMode = ClickMode.Missel;
    }

    public void OnSandGlassButtonPress()
    {
        m_currentClickMode = ClickMode.SandGlass;
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
