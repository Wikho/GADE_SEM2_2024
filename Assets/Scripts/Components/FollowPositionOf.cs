using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPositionOf : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;

    private void Update()
    {
        transform.position = transformToFollow.position;
    }
}
