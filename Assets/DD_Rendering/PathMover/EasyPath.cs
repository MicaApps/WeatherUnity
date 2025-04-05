using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class EasyPath : MonoBehaviour
{
    public int resolution=128;
    private Transform start;
    private Transform end;
    private Transform mid;
    [HideInInspector]public Vector3[] path;
    private LineRenderer lineRenderer;
    private void Awake()
    {
        start = transform.Find("Start");
        end = transform.Find("End");
        mid = transform.Find("Mid");
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void OnValidate()
    {
        if (start!=null)
        {
            var startPoint = start.position;
            var endPoint = end.position;
            var bezierControlPoint = mid.position;

            path = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                var t = (i + 1) / (float)resolution;
                path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);
            }
            lineRenderer.positionCount = path.Length;
            lineRenderer.SetPositions(path);

        }
       
    }
    private void Update()
    {
        if (Application.isEditor) 
        {
            OnValidate();
        }
    }
    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }
}
