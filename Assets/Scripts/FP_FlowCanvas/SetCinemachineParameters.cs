using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowCanvas.Nodes;
using FlowCanvas;
using ParadoxNotion.Design;
using ParadoxNotion;
using UnityEngine.Experimental.AI;

public class SetCinemachine : FlowNode
{
 
    private ValueInput<Cinemachine.CinemachineVirtualCamera> virtualCamera;


  
    public override void OnGraphStarted()
    {
        base.OnGraphStarted();
  
    }

    protected override void RegisterPorts()
    {
        virtualCamera = AddValueInput<Cinemachine.CinemachineVirtualCamera>("virtualCamera");
        var followTarget = AddValueInput<Transform>("followTarget");
        var lookAtTarget = AddValueInput<Transform>("lookAtTarget");
        var backGround = AddValueInput<PolygonCollider2D>("backGround");

        AddFlowInput("In", (f) => {  Call(virtualCamera.value, followTarget.value, lookAtTarget.value, backGround.value); });
        AddFlowOutput("Out");

    }

    private void Call(Cinemachine.CinemachineVirtualCamera virtualCamera, Transform followTarget, Transform lookAtTarget, PolygonCollider2D backGround)
    {

        if (virtualCamera != null)
        {
            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = lookAtTarget;

            Cinemachine.CinemachineConfiner2D confiner = virtualCamera.GetComponent<Cinemachine.CinemachineConfiner2D>();
            //PolygonCollider2D boundingShape = backGround.GetComponent<PolygonCollider2D>();
            if (backGround != null)
            {
                confiner.m_BoundingShape2D = backGround;
            }
        }
        else
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned!");
        }
    }
}
