using UnityEngine;

public class LineController : MonoBehaviour
{

    private LineRenderer _lineRenderer;

    [SerializeField] private Transform[] _transforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        _lineRenderer.positionCount = _transforms.Length;
        for (int i = 0; i < _transforms.Length; i++)
        {
            _lineRenderer.SetPosition(i, _transforms[i].position);
        }
    }
}
