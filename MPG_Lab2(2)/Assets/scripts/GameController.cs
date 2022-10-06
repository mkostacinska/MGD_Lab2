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
    private LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        distanceText.text = "";
        mode = DebugMode.Normal;
        distanceText.gameObject.SetActive(false);
        positionText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == DebugMode.Distance) {

            List<GameObject> pickupChildren = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy)
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

                lineRenderer.startWidth = 0.03f;
                lineRenderer.endWidth = 0.03f;

                distanceText.text = $"closest pickup: {Math.Round(Vector3.Distance(pickupChildren.FirstOrDefault().transform.position, player.transform.position), 2)}";
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
                    distanceText.gameObject.SetActive(false);
                    positionText.gameObject.SetActive(false);
                    break;
            }
        }


    }

    public void changeToVision()
    {
        //change all pickup children to white
        List<GameObject> blues = pickups.GetComponentsInChildren<Transform>()
                .Where(trans => trans.gameObject.activeInHierarchy && trans.gameObject.GetComponent<Renderer>().material.color.g != 255)
                .OrderBy(trans => Vector3.Distance(trans.position, player.transform.position))
                .Select(transform => transform.gameObject)
                .ToList();

        blues.ForEach(child => child.GetComponent<Renderer>().material.color = white);

        //set the text fields to invisible
        distanceText.gameObject.SetActive(false);
        positionText.gameObject.SetActive(false);
        velocityText.gameObject.SetActive(false);

        //get rid of line renderer
        lineRenderer.SetPositions(null);
    }

    public void changeToDistance()
    {
        //set the text fields to be active 
        distanceText.gameObject.SetActive(true);
        positionText.gameObject.SetActive(true);
        velocityText.gameObject.SetActive(true);
    }
}
