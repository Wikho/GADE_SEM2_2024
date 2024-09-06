using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct SpawnableObject
{
    // This struct is really just a collection of info about each spawnable object, this could be scriptable objects, but thats overkill for the use case, its just to remove variable redundancy/repetition.
    public GameObject prefabLevel1, prefabLevel2, prefabLevel3;
    public float verticalOffset;
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

    // These are defined here since this singleton is accessible by every instance of a buildable tile
    public SpawnableObject ballista;
    public SpawnableObject resource;


    NewInputSystem inputSystem;
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private ClickMode m_currentClickMode = ClickMode.None;

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
        Debug.Log("Casting a ray rn");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Is this technically mixing new and old input system? yes. Is it much much cleaner than getting the mouse pos with new input system, also yes.
        
        // TODO: Make it not fire when clicking UI elements.
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _raycastLayerMask, QueryTriggerInteraction.Ignore)) // Arbitrary distance thats far enough, Mathf.Infinity seems excessive for no reason.
        {
            Debug.Log("Hit somthing " + hit.transform.parent.gameObject.name);
            if (hit.transform.parent.gameObject.TryGetComponent(out Tile tile))
            {
                if (tile.GetTileType() == Tile.TileType.Build)
                {
                    switch (m_currentClickMode)
                    {
                        case ClickMode.Ballista:
                            hit.transform.parent.GetComponent<BuildableTile>().SpawnBallistaAbove(); //TODO: Check for cost with resources availble
                            break;
                        case ClickMode.Resource:
                            hit.transform.parent.GetComponent<BuildableTile>().SpawnResourceAbove(); //TODO: Check for cost with resources availble
                            break;
                        case ClickMode.Delete:
                            hit.transform.parent.GetComponent<BuildableTile>().DeleteCurrentTower();
                            break;
                        case ClickMode.Upgrade:
                            hit.transform.parent.GetComponent<BuildableTile>().UpgradeTower();
                            break;

                        case ClickMode.None:
                        default:
                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Did not get Tile Component");
            }
        }
    }

    private void OnDisable()
    {
        // Unsubscribe on object Disable/Destroy
        inputSystem.Gameplay.LeftClicking.performed -= LeftClickPerformed;
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
