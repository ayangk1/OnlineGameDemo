using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform target;
    public Vector3 offest;
    void Update()
    {
        if (Camera.main != null)
        {
            var targetPos = Camera.main.WorldToScreenPoint(target.position + offest);
            transform.position = targetPos ;
        }
    }
}
