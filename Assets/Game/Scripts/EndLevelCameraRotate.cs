using UnityEngine;
using Cinemachine;

public class EndLevelCameraRotate : MonoBehaviour
{
    public CinemachineVirtualCamera endCamera;
    public float rotateSpeed = 30f; // degrees per second

    CinemachineOrbitalTransposer orbital;

    void Start()
    {
        orbital = endCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    public void StartRotation()
    {
        StartCoroutine(RotateCamera());
    }

    System.Collections.IEnumerator RotateCamera()
    {
        while (true)
        {
            orbital.m_Heading.m_Bias += rotateSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
