using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCylinders : MonoBehaviour
{
    public GameObject Cylinder;
    public int TotalMeshes;
    public float AnimTime;

    [Range(1f, 25f)]
    public float TimeScale;

    private List<InstanceShaderController> _shaderControllers;
    private float _animTimer;
    private float _radiusScale;

    void Start()
    {
        _animTimer = AnimTime;
        _shaderControllers = new List<InstanceShaderController>();

        for (int i = 0; i < TotalMeshes; i++)
        {
            GameObject mesh = Instantiate(Cylinder, transform.position, Quaternion.identity);
            InstanceShaderController shaderController = mesh.GetComponent<InstanceShaderController>();
            shaderController.Init(i, TotalMeshes);
            _shaderControllers.Add(shaderController);
            mesh.transform.parent = this.transform;
        }
    }

    void Update()
    {
        _animTimer = Mathf.Clamp(_animTimer + Time.deltaTime, 0f, AnimTime);

        foreach (var s in _shaderControllers)
        {
            s.SetTimeScale(TimeScale);

            float k = Back.Out(_animTimer / AnimTime);
            float d = Map(k, 0f, 1f, Mathf.PI, Mathf.PI + 2 * Mathf.PI);
            _radiusScale = Map(1 + Mathf.Cos(d), 0f, 2f, 1f, 1.3f);
            s.SetRadiusScale(_radiusScale);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _animTimer = 0f;
        }


        var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if (!isOutside)
        {
            float y = Map(view.x, 0f, 1f, -45, 45);
            float x = Map(view.y, 0f, 1f, -45, 45);
            transform.rotation = Quaternion.Euler(x, y, 0f);
        }
    }

    float Map(float value, float initialMin, float initialMax, float destinationMin, float destinationMax)
    {
        var t = (value - initialMin) / (initialMax - initialMin);
        return Mathf.Lerp(destinationMin, destinationMax, t);
    }

}

public class Back
{
    static float s = 1.70158f;
    static float s2 = 2.5949095f;

    public static float In(float k)
    {
        return k * k * ((s + 1f) * k - s);
    }

    public static float Out(float k)
    {
        return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
    }

    public static float InOut(float k)
    {
        if ((k *= 2f) < 1f) return 0.5f * (k * k * ((s2 + 1f) * k - s2));
        return 0.5f * ((k -= 2f) * k * ((s2 + 1f) * k + s2) + 2f);
    }
};
