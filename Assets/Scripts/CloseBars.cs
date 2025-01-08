using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBars : MonoBehaviour
{
    public GameObject bars_group;
    public GameObject wall_hitbox;
    public GameObject cover;

    private GameObject level_generator;
    private LevelGenerator script;
    
    void Start() {
        level_generator = GameObject.Find("LevelGenerator");
        script = level_generator.GetComponent<LevelGenerator>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.CompareTag("Player")){
            wall_hitbox.SetActive(true);
            cover.SetActive(true);
            bars_group.SetActive(true);

            transform.parent.gameObject.SetActive(false);
            
            script.IncrementRoom();
        }
    }
}
