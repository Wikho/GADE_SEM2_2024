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

    public bool HasObjectAbove { get => m_spawnableCurrentlyAbove != null; }
    public bool IsClear { get => m_spawnableCurrentlyAbove == null; }

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

    public void SpawnCrystalAbove()
    {
        // Guard clauses
        if (ClickToSpawnManager.instance.crystal.prefabLevel1 == null)
        {
            Debug.LogWarning("Crystal Prefab not found");
            return;
        }
        if (m_tileComponent.GetTileType() != Tile.TileType.Build)
            return;
        if (m_spawnableCurrentlyAbove != null)
            return;

        // Spawn Crystal
        m_spawnableCurrentlyAbove = Instantiate(
            ClickToSpawnManager.instance.crystal.prefabLevel1,
            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.crystal.verticalOffset, 0),
            ClickToSpawnManager.instance.crystal.prefabLevel1.transform.rotation,
            this.transform
        );
    }

    public void SpawnMissileAbove()
    {
        // Guard clauses
        if (ClickToSpawnManager.instance.missile.prefabLevel1 == null)
        {
            Debug.LogWarning("Missile Prefab not found");
            return;
        }
        if (m_tileComponent.GetTileType() != Tile.TileType.Build)
            return;
        if (m_spawnableCurrentlyAbove != null)
            return;

        // Spawn Missile
        m_spawnableCurrentlyAbove = Instantiate(
            ClickToSpawnManager.instance.missile.prefabLevel1,
            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.missile.verticalOffset, 0),
            ClickToSpawnManager.instance.missile.prefabLevel1.transform.rotation,
            this.transform
        );
    }

    public void SpawnSendGlassAbove()
    {
        // Guard clauses
        if (ClickToSpawnManager.instance.sendGlass.prefabLevel1 == null)
        {
            Debug.LogWarning("SendGlass Prefab not found");
            return;
        }
        if (m_tileComponent.GetTileType() != Tile.TileType.Build)
            return;
        if (m_spawnableCurrentlyAbove != null)
            return;

        // Spawn SendGlass
        m_spawnableCurrentlyAbove = Instantiate(
            ClickToSpawnManager.instance.sendGlass.prefabLevel1,
            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.sendGlass.verticalOffset, 0),
            ClickToSpawnManager.instance.sendGlass.prefabLevel1.transform.rotation,
            this.transform
        );
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

        //Crystal's
        if (m_spawnableCurrentlyAbove.name.Contains("GB_Crystal_1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.crystal.prefabLevel2,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.crystal.verticalOffset, 0),
                                            ClickToSpawnManager.instance.crystal.prefabLevel2.transform.rotation,
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("GB_Crystal_2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.crystal.prefabLevel3,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.crystal.verticalOffset, 0),
                                            ClickToSpawnManager.instance.crystal.prefabLevel3.transform.rotation,
                                            this.transform);
        }

        //Missel's
        if (m_spawnableCurrentlyAbove.name.Contains("GB_Pyro_1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.missile.prefabLevel2,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.missile.verticalOffset, 0),
                                            ClickToSpawnManager.instance.missile.prefabLevel2.transform.rotation,
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("GB_Pyro_2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.missile.prefabLevel3,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.missile.verticalOffset, 0),
                                            ClickToSpawnManager.instance.missile.prefabLevel3.transform.rotation,
                                            this.transform);
        }

        //SandGlass's
        if (m_spawnableCurrentlyAbove.name.Contains("GB_Sandglass_1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.sendGlass.prefabLevel2,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.sendGlass.verticalOffset, 0),
                                            ClickToSpawnManager.instance.sendGlass.prefabLevel2.transform.rotation,
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("GB_Sandglass_2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.sendGlass.prefabLevel3,
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.sendGlass.verticalOffset, 0),
                                            ClickToSpawnManager.instance.sendGlass.prefabLevel3.transform.rotation,
                                            this.transform);
        }

        // Resources
        if (m_spawnableCurrentlyAbove.name.Contains("ResourceTower_1"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.resource.prefabLevel2, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.resource.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.resource.prefabLevel2.transform.rotation, 
                                            this.transform);
        }
        else if (m_spawnableCurrentlyAbove.name.Contains("ResourceTower_2"))
        {
            DeleteCurrentTower();
            m_spawnableCurrentlyAbove = Instantiate(ClickToSpawnManager.instance.resource.prefabLevel3, 
                                            m_tileComponent.gameObject.transform.position + new Vector3(0, ClickToSpawnManager.instance.resource.verticalOffset, 0), 
                                            ClickToSpawnManager.instance.resource.prefabLevel3.transform.rotation, 
                                            this.transform);
        }
    }
}
