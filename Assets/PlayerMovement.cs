using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float move_speed;
    public float jump_strength;
    public float rotation_speed;
    public int player_id = 0;

    private Rigidbody2D rb;
    private bool can_jump = false;
    private Vector2 next_jump_vector;
    private Dictionary<GameObject, Vector2> colliding_objects = new Dictionary<GameObject, Vector2>();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D> ();

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Environment")
        {
            // Add the colliding gameObject and its normal (perpendicular) vector to our list of
            // colliding objects.
            colliding_objects[col.gameObject] = col.contacts[0].normal;
            
            Debug.Log("can_jump set to true");
            can_jump = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Environment")
        {
            colliding_objects.Remove(col.gameObject);

        }

        if (colliding_objects.Count == 0)
        {
            // Disable jump when leaving all collisions, so that we can't magically gain momentum in mid air.
            Debug.Log("can_jump set to false");
            can_jump = false;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Environment")
        {
            /* Make jump realistic: whenever the player collides with the environment, set the
             * jump direction perpendicular to the surface(s) the player is touching.
             * This is in contrast to making jump always point upwards, which is completely wrong
             * when colliding with slanted or floating platforms.
             * Since forces are expressed as a vector, we can sum all the collision normal
             * vectors if the player is touching multiple points at once (e.g. corner jump).
             */
            Vector2 normal_sum = Vector2.zero;

            foreach (Vector2 vector in colliding_objects.Values)
            {
                normal_sum += vector;
            }
            next_jump_vector = normal_sum;
        }
    }

    // Update is called once per frame
    void Update () {
        // Get horizontal and vertical (rotation) movement

        if (GameState.Instance.current_player == player_id)
        {
            float x_move = Input.GetAxis("Horizontal");
            float r_move = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && can_jump)
            {
                rb.AddForce(next_jump_vector * jump_strength);

                // Disable jump while we're in mid-air (this is reset in OnCollisionStay2D).
                can_jump = false;
                Debug.Log("can_jump set to false");
            }

            Vector2 vector_move = new Vector2(x_move, 0.0F);

            rb.AddForce(vector_move * move_speed);
            // Up rotates clockwise, down rotates counterclockwise
            rb.AddTorque(-r_move * rotation_speed);
        }

    }
}