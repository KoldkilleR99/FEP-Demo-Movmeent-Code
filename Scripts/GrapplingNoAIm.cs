using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingNoAIm : MonoBehaviour
{
    [Header("References")]
    private MovementNoAim pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling Stuff")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    private bool grappling;
    void Start()
    {
        pm = GetComponent<MovementNoAim>();
    }

    void Update()
    {
        if (Input.GetKeyDown(grapplingKey))
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }
        grappling = true;
        pm.activeGrapple = true;


        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            pm.freeze = true;
            
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

        lr.positionCount = 2;
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;
        

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
        Invoke(nameof(pm.ResetRestrictions), 0.2f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;
        pm.activeGrapple = false;
        grappling = false;
        pm.doubleJumped = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
        lr.positionCount = 0;
    }
}
