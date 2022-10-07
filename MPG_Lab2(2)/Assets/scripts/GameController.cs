using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject pickups;
    public GameObject player;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI distanceText;
    public DebugMode mode;
    private readonly Color blue = new Color(0, 0, 255);
    private readonly Color white = Color.white;
    private readonly Color green = Color.green;
    private LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        distanceText.text = "";

        //initializing the debug mode to normal
        mode = DebugMode.Normal;

        //setting all the texts to be initially invisible
        distanceText.gameObject.SetActive(false);
        positionText.gameObject.SetActive(false);
        velocityText.gameObject.SetActive(false);
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (mode == DebugMode.Distance) {

            List<GameObject> pickupChildren = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy && trans != pickups.transform)
                .OrderBy(trans => Vector3.Distance(trans.position, player.transform.position))
                .Select(transform => transform.gameObject)
                .ToList();

            //pickupChildren.FirstOrDefault().GetComponent<Renderer>().material.color 

            if (pickupChildren.FirstOrDefault())
            {
                //reset all the existing blue cubes back to white
                List<GameObject> blues = pickupChildren.Where(child => child.GetComponent<Renderer>().material.color.g != 255)
                    .ToList();
                blues
                    .ToList()
                    .ForEach(child => child.GetComponent<Renderer>().material.color = white);

                //pick the nearest cube and paint it blue
                pickupChildren.FirstOrDefault().GetComponent<Renderer>().material.color = blue;

                //draw the line between the player and the closest cube
                lineRenderer.SetPosition(0, player.transform.position);
                lineRenderer.SetPosition(1, pickupChildren.FirstOrDefault().transform.position);

                lineRenderer.startWidth = 0.06f;
                lineRenderer.endWidth = 0.06f;

                distanceText.text = $"closest pickup: {Math.Round(Vector3.Distance(pickupChildren.FirstOrDefault().transform.position, player.transform.position), 2)}";
            }
        }
        else if (mode == DebugMode.Vision)
        {
            //creating the line from player to its next position
            lineRenderer.SetPosition(0, player.transform.position);
            lineRenderer.SetPosition(1, player.transform.position + player.GetComponent<Rigidbody>().velocity);
            lineRenderer.startWidth = 0.06f;
            lineRenderer.endWidth = 0.06f;

            /*
             * CALCULATING WHICH PRODUCT WE'RE APPROACHING MOST DIRECTLY
             * - looking at two vectors - the player's velocity and the vector from the collectible to the player
             * - after normalizing, calculating their dot product gives us similarity
             * - smallest similarity = the objects are 'facing' one another
             */
            //list of vectors from the active pickups to the player (normalized)
            List<Transform> pickupChildren = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy && trans != pickups.transform)
                .OrderByDescending(trans => Vector3.Dot(player.GetComponent<Rigidbody>().velocity.normalized, (trans.position - player.transform.position).normalized))
                .ToList();

            if (pickupChildren.FirstOrDefault())
            {
                //reset all the existing blue cubes back to white
                List<GameObject> greens = pickupChildren
                    .Select(child => child.gameObject)
                    .Where(child => child.GetComponent<Renderer>().material.color.r != 255)
                    .ToList();
                greens
                    .ToList()
                    .ForEach(child => child.GetComponent<Renderer>().material.color = white);

                //pick the most direct cube and paint it green
                pickupChildren.FirstOrDefault().GetComponent<Renderer>().material.color = green;
                pickupChildren.FirstOrDefault().transform.LookAt(player.transform);
               
            }
        }
    

        if (Input.GetKeyUp(KeyCode.Space))
        {
            switch(mode)
            {
                case DebugMode.Normal:
                    mode = DebugMode.Distance;
                    changeToDistance();
                    break;
                case DebugMode.Distance:
                    mode = DebugMode.Vision;
                    changeToVision();
                    break;
                case DebugMode.Vision:
                    mode = DebugMode.Normal;
                    changeToNormal();
                    break;
            }
        }


    }

    public void changeToVision()
    {
        //DISABLING THE PREVIOUS MODE - DISTANCE
        //change all pickup children to white
        List<GameObject> blues = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy && trans.gameObject.GetComponent<Renderer>().material.color.g != 255)
                .OrderBy(trans => Vector3.Distance(trans.position, player.transform.position))
                .Select(transform => transform.gameObject)
                .ToList();

        blues.ForEach(child => child.GetComponent<Renderer>().material.color = white);

        //set the text fields and line renderer to not active
        distanceText.gameObject.SetActive(false);
        positionText.gameObject.SetActive(false);
        velocityText.gameObject.SetActive(false);
    }

    public void changeToDistance()
    {
        //set the text fields and line renderer to be active 
        distanceText.gameObject.SetActive(true);
        positionText.gameObject.SetActive(true);
        velocityText.gameObject.SetActive(true);
        lineRenderer.enabled = true;
    }

    public void changeToNormal()
    {
        distanceText.gameObject.SetActive(false);
        positionText.gameObject.SetActive(false);
        lineRenderer.enabled = false;

        List<GameObject> greens = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy && trans.gameObject.GetComponent<Renderer>().material.color.r != 255)
                .OrderBy(trans => Vector3.Distance(trans.position, player.transform.position))
                .Select(transform => transform.gameObject)
                .ToList();

        greens.ForEach(child => child.GetComponent<Renderer>().material.color = white);
    }
}
