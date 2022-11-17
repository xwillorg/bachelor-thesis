using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BalanceAgent : Agent {
    Rigidbody rBody;
    void Start () {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Pole;
    public Transform Ground;
    public override void OnEpisodeBegin() {
        float startingNoise = Random.Range(170.0f, 190.0f);
        transform.localPosition = new Vector3(0, 0, 0);
        Pole.localPosition = new Vector3(0, 3.8f, -0.7f);
        Pole.localEulerAngles = new Vector3(0, 0, startingNoise);
        Pole.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Pole.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

      public override void CollectObservations(VectorSensor sensor) {
        // Position and velocity of the Agent
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);

        //Pole rotation
        sensor.AddObservation(Pole.localRotation);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        float poleRotation =  Pole.localEulerAngles.z;
        float xPostition = this.transform.localPosition.x;

        MoveAgent(actionBuffers.ContinuousActions[0]);

        if (poleRotation <= 100.0f || poleRotation >= 260.0f) {
            SetReward(-3f);
            EndEpisode();
        } else if (poleRotation <= 179.9f || poleRotation >= 180.1f) {
            SetReward(1f);
        } else {
            SetReward(-1f);
        }

        if (xPostition >= 7.0f) {
            this.transform.localPosition = new Vector3(6.99f, 0, 0);
            SetReward(-0.2f);
        } else if (xPostition <= -7.0f) {
            this.transform.localPosition = new Vector3(-6.99f, 0, 0);
            SetReward(-0.2f);
        }
    }

    public void MoveAgent(float moveX) {
        float moveSpeed = 20f;
        transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }
}

/*
        public float forceMultiplier = 10;

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        rBody.AddForce(controlSignal * forceMultiplier);

        float difference = Mathf.Abs(poleRotation - 180);
        float reward = Mathf.Abs((difference - 180) / -100) -1;
        Debug.Log("rewards = " + reward);

        // brother doesnt know the pole, just the rewards
 */