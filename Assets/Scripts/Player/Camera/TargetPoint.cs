using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    private float height;
    Vector3 playerPos;

    private void Start()
    {
        height = transform.position.y;
    }

    private void LateUpdate()
    {
        playerPos = PlayerController.INSTANCE.playerModel.transform.position;
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerPos.x, playerPos.y + height, playerPos.z), 10 * Time.deltaTime);
    }
}
