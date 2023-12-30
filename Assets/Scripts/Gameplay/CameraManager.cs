using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Reference to your CinemachineFreeLook component.
    public CinemachineImpulseSource impulseSource; // Reference to CinemachineImpulseSource for camera shake.

    // Start is called before the first frame update
    void Start()
    {
        // Initialize references to the Cinemachine components.
        freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }


    // Call this method to trigger camera shake.
    void ShakeCamera(float shakeDuration, float shakeAmplitude, float shakeFrequency)
    {
        impulseSource.GenerateImpulse(Vector3.one); // Basic impulse, can be customized.
    }

    // Call this method to zoom the camera.
    void ZoomCamera(float zoomAmount)
    {
        freeLookCamera.m_Orbits[1].m_Radius += zoomAmount;
    }
}
