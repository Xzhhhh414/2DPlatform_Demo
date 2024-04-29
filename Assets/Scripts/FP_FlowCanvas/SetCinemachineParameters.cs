using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowCanvas.Nodes;
using FlowCanvas;
using ParadoxNotion.Design;
using ParadoxNotion;

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

        AddFlowInput("In", (f) => {  Call(virtualCamera.value, followTarget.value, lookAtTarget.value); });
        AddFlowOutput("Out");

    }

    //void DoSet()
    //{ 
    
    //}



    private void Call(Cinemachine.CinemachineVirtualCamera virtualCamera, Transform followTarget, Transform lookAtTarget)
    {

        if (virtualCamera != null)
        {
            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = lookAtTarget;
        }
        else
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned!");
        }
    }
}
