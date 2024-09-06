using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO: Decide whether we want to have an enum for each level, i.e. Ballista Lvl 1, Ballista Lvl 2, etc, or decide if we even want Ballista as a type as opposed to just "Weapon" or "Resource"



[RequireComponent(typeof(Tile))]
public class BuildableTile : MonoBehaviour
{
    private Tile m_tileComponent;
    private GameObject m_spawnableCurrentlyAbove;

    private void Awake()
    {
        m_tileComponent = GetComponent<Tile>();
    }

    /// <summary>
    /// Spawns the Ballista tower above the given <i>Tile</i> object, with the specified offset on the component declaration.
    /// </summary>
    public void SpawnBallistaAbove()
    {
        // Guard clauses
        if (ClickToSpawnManager.instance.ballista.prefabLevel1 == null)
        {
            Debug.LogWarning("Prefab not found");
            return;
        }
        if (m_tileComponent.GetTileType() != Tile.TileType.Build)
            return;
        if (m_spawnableCurrentlyAbove != null) // We don't want to Spawn a tower if one exists
            return;

        // Spawn relevant type
        m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.ballista.prefabLevel1, 
                                        m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.ballista.verticalOffset, 0), 
                                        ClickToSpawnManager.instance.ballista.prefabLevel1.transform.rotation, 
                                        this.transform);
    }

    /// <summary>
    /// Spawns the Resource tower above the given <i>Tile</i> object, with the specified offset on the component declaration.
    /// </summary>
    public void SpawnResourceAbove()
    {
        // Guard clauses
        if (ClickToSpawnManager.instance.resource.prefabLevel1 == null)
        {
            Debug.LogWarning("Prefab not found");
            return;
        }
        if (m_tileComponent.GetTileType() != Tile.TileType.Build)
            return;
        if (m_spawnableCurrentlyAbove != null) // We don't want to Spawn a tower if one exists
            return;

        // Spawn relevant type
        m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.resource.prefabLevel1, 
                                        m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.resource.verticalOffset, 0), 
                                        ClickToSpawnManager.instance.resource.prefabLevel1.transform.rotation, 
                                        this.transform);
    }

    public void DeleteCurrentTower()
    {
        if (m_spawnableCurrentlyAbove == null) // Can't delete what doesn't exist
            return;

        // Since C# variables are essentially all pointers, it might seem like m_spawnableCurrentlyAbove is a copy of the gameObject, but it isn't, its a reference to it, so the destroy call makes sense.
        Destroy(m_spawnableCurrentlyAbove);
    }

    public void UpgradeTower()
    {
        // NOTE: This is a jank ass solution, something better should be found later.
        if (m_spawnableCurrentlyAbove == null) // Can't delete what doesn't exist
            return;

        // Ballista's
        if (m_spawnableCurrentlyAbove.name.Contains("GB_ballista_1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.ballista.prefabLevel2, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.ballista.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.ballista.prefabLevel2.transform.rotation, 
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("GB_ballista_2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.ballista.prefabLevel3, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.ballista.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.ballista.prefabLevel3.transform.rotation, 
                                            this.transform);
        }
        // else do nothing since cant be upgraded, later when the stats to increase, we'd check if its top tier, and then just increase the stat values instead of instantiating a new tower.

        // Resources
        if (m_spawnableCurrentlyAbove.name.Contains("PlaceHolderResourceTower1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.resource.prefabLevel2, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.resource.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.resource.prefabLevel2.transform.rotation, 
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("PlaceHolderResourceTower2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.resource.prefabLevel3, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.resource.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.resource.prefabLevel3.transform.rotation, 
                                            this.transform);
        }
    }
}
