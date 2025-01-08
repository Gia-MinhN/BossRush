using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindWall : MonoBehaviour
{
    public GameObject sprite_object;

    private int default_order = 4;
    private int hidden_order = 0;
    private int wall_overlap_count = 0;

    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = sprite_object.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        string tag = col.tag;
        switch(tag) {
            case "Wall":
                wall_overlap_count++;
                sprite.sortingOrder = hidden_order;
                break;
            case "CloseRoom":
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Wall")){
            sprite.sortingOrder = hidden_order;
        }
       
    }
    private void OnTriggerExit2D(Collider2D col){
        if (col.CompareTag("Wall")){
            wall_overlap_count--;
            if(wall_overlap_count <= 0) {
                wall_overlap_count = 0;
                sprite.sortingOrder = default_order;
            }
        }
    }
}
