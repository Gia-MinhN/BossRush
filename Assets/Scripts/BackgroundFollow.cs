using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void Update()
    {   
        Vector3 pos = player.transform.position;
        Vector3 clamped = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y),Mathf.Round(pos.z));

        transform.position = clamped;
    }
}
