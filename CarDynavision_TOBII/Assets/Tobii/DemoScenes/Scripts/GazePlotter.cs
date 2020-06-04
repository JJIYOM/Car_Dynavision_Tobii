//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;

/// <summary>
/// Draws the gaze point positions as a point cloud, or, if the use filtering
/// toggle is on, with a single bubble sprite with smoother movements.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class GazePlotter : MonoBehaviour
{
    [Range(3.0f, 15.0f), Tooltip("Number of gaze points in point cloud.")]
    public int PointCloudSize = 10;
    [Tooltip("Sprite to use for gaze points in the point cloud.")]
    public Sprite PointSprite;
    [Range(0.0f, 1.0f), Tooltip("Scale to draw the point sprites in the point cloud.")]
    public float PointScale = 0.1f;
    [Tooltip("Distance from screen to visualization plane in the World.")]
    public float VisualizationDistance = 10f; //gaze distance
    [Range(0.1f, 1.0f), Tooltip("How heavy filtering to apply to gaze point bubble movements. 0.1f is most responsive, 1.0f is least responsive.")]
    public float FilterSmoothingFactor = 0.35f; //gaze smoothing (response time)

    private GazePoint _lastGazePoint = GazePoint.Invalid;

    // Members used for the gaze point cloud:
    private const float MaxVisibleDurationInSeconds = 0.5f;
    private GazePoint[] _gazePoints;
    private int _last;
    private GameObject[] _gazePointCloudSprites;

    // Members used for gaze bubble (filtered gaze visualization):
    private SpriteRenderer _gazeBubbleRenderer;      // the gaze bubble sprite is attached to the GazePlotter game object itself
    private bool _useFilter = true;
    private bool _hasHistoricPoint;
    private Vector3 _historicPoint;

    public GameObject car;

    public bool UseFilter
    {
        get { return _useFilter; }
        set { _useFilter = value; }
    }

    void Start()
    {
        InitializeGazePointBuffer();
        InitializeGazePointCloudSprites();

        _last = PointCloudSize - 1;

        _gazeBubbleRenderer = GetComponent<SpriteRenderer>();
        UpdateGazeBubbleVisibility();
    }

    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        if (gazePoint.IsRecent()
            && gazePoint.Timestamp > (_lastGazePoint.Timestamp + float.Epsilon))
        {
            if (UseFilter)
            {
                UpdateGazeBubblePosition(gazePoint);
            }
            else
            {
                UpdateGazePointCloud(gazePoint);
            }

            _lastGazePoint = gazePoint;
        }

        UpdateGazePointCloudVisibility();
        UpdateGazeBubbleVisibility();
    }

    private void InitializeGazePointBuffer()
    {
        _gazePoints = new GazePoint[PointCloudSize];
        for (int i = 0; i < PointCloudSize; i++)
        {
            _gazePoints[i] = GazePoint.Invalid;
        }
    }

    private void InitializeGazePointCloudSprites()
    {
        _gazePointCloudSprites = new GameObject[PointCloudSize];
        for (int i = 0; i < PointCloudSize; i++)
        {
            var pointCloudSprite = new GameObject("PointCloudSprite" + i);
            pointCloudSprite.layer = gameObject.layer;

            var spriteRenderer = pointCloudSprite.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PointSprite;

            var cloudPointVisualizer = pointCloudSprite.AddComponent<CloudPointVisualizer>();
            cloudPointVisualizer.Scale = PointScale;

            pointCloudSprite.SetActive(false);
            _gazePointCloudSprites[i] = pointCloudSprite;
        }
    }

    private void UpdateGazePointCloudVisibility()
    {
        bool isPointCloudVisible = !UseFilter;

        for (int i = 0; i < PointCloudSize; i++)
        {
            if (IsNotTooOld(_gazePoints[i]))
            {
                _gazePointCloudSprites[i].SetActive(isPointCloudVisible);
            }
            else
            {
                _gazePointCloudSprites[i].SetActive(false);
            }
        }
    }

    private bool IsNotTooOld(GazePoint gazePoint)
    {
        return (Time.unscaledTime - gazePoint.Timestamp) < MaxVisibleDurationInSeconds;
    }

    private void UpdateGazeBubblePosition(GazePoint gazePoint) //position, rotation
    {
        Vector3 gazePointInWorld = ProjectToPlaneInWorld(gazePoint);
        transform.position = Smoothify(gazePointInWorld);

        //transform.position = new Vector3(Smoothify(gazePointInWorld).x, Smoothify(gazePointInWorld).y, transform.localPosition.z + 1.5f);
        //transform.localPosition = new Vector3(Smoothify(gazePointInWorld).x, Smoothify(gazePointInWorld).y, Smoothify(gazePointInWorld).z/*8.559983f*/);

        //카메라 포지션 가져와서 gaze를 카메라보다 앞으로 조정 camera 포지션이 중요
        //그냥 달릴때도 position이 조금 가까워지는 느낌

        //if (car.transform.eulerAngles.y == 0f)
        //    VisualizationDistance = 10f;
        //else if (car.transform.eulerAngles.y > 0f && car.transform.eulerAngles.y <= 90f)
        //    VisualizationDistance = car.transform.eulerAngles.y % 10f;
        //else if (car.transform.eulerAngles.y > 90f && car.transform.eulerAngles.y <= 180f)
        //    VisualizationDistance = car.transform.eulerAngles.y % -10;


        transform.eulerAngles = car.transform.eulerAngles;
    }

    private void UpdateGazePointCloud(GazePoint gazePoint)
    {
        _last = Next();
        _gazePoints[_last] = gazePoint;
        var cloudPointVisualizer = _gazePointCloudSprites[_last].GetComponent<CloudPointVisualizer>();
        Vector3 gazePointInWorld = ProjectToPlaneInWorld(gazePoint);
        cloudPointVisualizer.NewPosition(gazePoint.Timestamp, gazePointInWorld);
    }

    private void UpdateGazeBubbleVisibility()
    {
        _gazeBubbleRenderer.enabled = UseFilter;
    }

    private int Next()
    {
        return ((_last + 1) % PointCloudSize);
    }

    private Vector3 ProjectToPlaneInWorld(GazePoint gazePoint)
    {
        Vector3 gazeOnScreen = gazePoint.Screen;
        gazeOnScreen += (this.transform.InverseTransformDirection(transform.forward) /*transform.forward /*유동적인값*/ * VisualizationDistance /*고정값*/); //메인카메라부터 게이즈까지 거리 = 10f

        return Camera.main.ScreenToWorldPoint(gazeOnScreen);
        //transform.forward z 값 보정 필요
    }

    private Vector3 Smoothify(Vector3 point)
    {
        if (!_hasHistoricPoint)
        {
            _historicPoint = point;
            _hasHistoricPoint = true;
        }

        var smoothedPoint = new Vector3(
            point.x * (1.0f - FilterSmoothingFactor) + _historicPoint.x * FilterSmoothingFactor,
            point.y * (1.0f - FilterSmoothingFactor) + _historicPoint.y * FilterSmoothingFactor, /*transform.loposition.z);*/
            point.z * (1.0f - FilterSmoothingFactor) + _historicPoint.z * FilterSmoothingFactor); //여기서 보정?

        _historicPoint = smoothedPoint;

        return smoothedPoint;
    }
}
