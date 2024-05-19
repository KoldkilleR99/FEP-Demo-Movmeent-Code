using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public MovementNoAim pm;

    [Header("Swinging")]
    public float maxSwingDistance;
    private Vector3 swingPoint;
    private SpringJoint joint;

    void Start()
    {
        pm = FindObjectOfType<MovementNoAim>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        pm.swinging = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 100f;
            joint.massScale = 10f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
            pm.rb.drag = 0;

            lr.enabled = true;
        }
    }
    private Vector3 currentGrapplePosition;
    private void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
        if (pm.swinging)
        {

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, swingPoint);
        }
    }

    private void StopSwing()
    {
        pm.swinging = false;
        lr.positionCount = 0;
        lr.enabled = false;
        Destroy(joint);
        pm.ResetRestrictions();
    }
}
