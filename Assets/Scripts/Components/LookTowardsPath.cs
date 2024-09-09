using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsPath : MonoBehaviour
{
    private void Start()
    {
        LookTowardPath();
    }

    public void LookTowardPath()
    {
        try
        {
            //Get the closest path tile from the TerrainGenerator
            Tile closestPathTile = TerrainGenerator.Instance.GetClosestPathTile(transform.position);

            if (closestPathTile != null)
            {
                Vector3 directionToPath = closestPathTile.transform.position - transform.position;
                directionToPath.y = 0;

                //Calculating the rotation needed to look at the path tile
                Quaternion lookRotation = Quaternion.LookRotation(directionToPath);

                //rotate towards the target point 
                transform.rotation = lookRotation;
            }
        }
        catch { }
    }
}
