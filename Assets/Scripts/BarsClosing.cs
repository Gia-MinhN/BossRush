using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsClosing : MonoBehaviour
{
    public float shut_time = 0.2f;
    public float bounce_time = 0.25f;
    public float bounce_distance = 0.5f;

    private float shut_time_passed;
    private float bounce_time_passed;
    private bool bounce_phase;

    private Vector3 initial_pos;
    private Vector3 target_pos;
    private Vector3 bounce_pos;

    // Start is called before the first frame update
    void Start()
    {
        shut_time_passed = 0f;
        bounce_time_passed = 0f;
        bounce_phase = false;
        initial_pos = transform.localPosition;
        target_pos = initial_pos + Vector3.up * 2f;
        bounce_pos = target_pos + Vector3.down * bounce_distance;
    }

    // Update is called once per frame
    void Update()
    {
        float progress;

        if(!bounce_phase) {
            shut_time_passed += Time.deltaTime;
            progress = shut_time_passed/shut_time;

            float curve = Mathf.Pow(progress, 2);
            transform.localPosition = Vector3.Lerp(initial_pos, target_pos, curve);

            if(progress >= 1f) {
                transform.localPosition = target_pos;
                bounce_phase = true;
            }
        } else {
            bounce_time_passed += Time.deltaTime;
            progress = bounce_time_passed/bounce_time;

            float curve = -Mathf.Pow((2f*progress-1), 2) + 1;
            transform.localPosition = Vector3.Lerp(target_pos, bounce_pos, curve);

            if(progress >= 1f) {
                transform.localPosition = target_pos;
                enabled = false;
            }
        }
        
    }
}
