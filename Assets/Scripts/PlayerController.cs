using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float move_speed = 10f;
    public float camera_follow_speed = 5f;

    public float dash_speed = 30f;
    public float dash_duration = 0.1f;
    public float dash_cooldown = 0.2f;

    private bool is_dashing = false;
    private bool can_dash = true;
    private Vector2 dash_direction;
    private bool space_down = false;

    public Transform player_camera;
    public GameObject sprite_object;
    public GameObject background;

    private Rigidbody2D rb;
    private Vector2 move_input; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        background.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)) {
            space_down = true;
        }
        if(Input.GetKeyUp(KeyCode.Space)) {
            space_down = false;
        }
        
        if(!is_dashing)
            move_input = new Vector2(moveX, moveY).normalized;

        if (space_down && move_input != Vector2.zero && can_dash) 
            StartDash();
    }

    void FixedUpdate()
    {
        Vector2 target_position;

        if(is_dashing) {
            target_position = rb.position + move_input * dash_speed * Time.fixedDeltaTime;
        } else {
            target_position = rb.position + move_input * move_speed * Time.fixedDeltaTime;
        }

        rb.MovePosition(target_position);

        // Camera drag
        Vector3 camera_target_pos = new Vector3(transform.position.x, transform.position.y, player_camera.position.z);
        player_camera.position = Vector3.Lerp(player_camera.position, camera_target_pos, camera_follow_speed * Time.fixedDeltaTime);
    }

    void StartDash() {
        is_dashing = true;
        can_dash = false;

        Invoke(nameof(StopDash), dash_duration);
        Invoke(nameof(AllowDash), dash_cooldown);
    }
    
    void StopDash() {
        is_dashing = false;
        rb.velocity = Vector2.zero;
    }

    void AllowDash() {
        can_dash = true;
    }
}