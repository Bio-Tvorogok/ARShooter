using UnityEngine;

public static class LineRendererExtensions
{
    public static void SetPositions(this LineRenderer lineRenderer, Transform[] transforms)
    {
        Vector3[] positions = new Vector3[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
            positions[i] = transforms[i].position;

        lineRenderer.SetPositions(positions);
    }
}
