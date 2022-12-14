using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody ballRb;
    public float speed = 15;

    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColour;

    private void Start()
    {
        solveColour = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColour;
    }

    private void FixedUpdate()
    {
        if (isTravelling)
        {
            ballRb.velocity = speed * travelDirection;
            Debug.Log("Distance interval betwixt sphere and cube: " + Vector3.Distance(transform.position, nextCollisionPosition));
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColoured)
            {
                ground.ChangeColour(solveColour);
            }
        }

        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTravelling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        // TODO: Find out why this doesn't stop this.gameObject from translating!
        if (isTravelling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                // Up/Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    // Go up/down
                    setDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                // Left/Right
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    // Go left/right
                    setDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }

            }

            swipePosLastFrame = swipePosCurrentFrame;

        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }

    }

    private void setDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTravelling = true;
    }

}
