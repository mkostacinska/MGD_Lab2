using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveValue;
    public float speed;
    private int count;
    public int numPickups = 42;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI distanceText;
    //public Vector3 lastPosition;

    private void Start()
    {
        count = 0;
        winText.text = "";
        positionText.text = "";
        velocityText.text = "";
        distanceText.text = "";
        //lastPosition = transform.position;
        SetCountText();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void FixedUpdate() //in fixedupdate as requires physics
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    private void SetCountText()
    {
        scoreText.text = "score: " + count.ToString();
        if(count >= numPickups)
        {
            winText.text = "you win!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        positionText.text = "player coordinates: " + transform.position.x.ToString() + ", " + transform.position.z.ToString();

        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        double scalarVelocity = Math.Round(Math.Sqrt(Math.Pow(velocity.x, 2) + Math.Pow(velocity.y, 2)), 2);

        velocityText.text = "player velocity: " + scalarVelocity.ToString();

        /* THE VECTOR WAY TO CALCULATE VELOCITY - USING RIGIDBODY AS THIS GIVES ME 0 MOST OF THE TIME :((
         * Vector3 velocity = (lastPosition - transform.position)/Time.deltaTime; 
         * 
         * Float scalarVelocity = Math.Sqrt(Math.Pow(velocity.x, 2) + Math.Pow(velocity.y,2));
         * 
         * lastPosition = transform.position;
         */
    }
}
