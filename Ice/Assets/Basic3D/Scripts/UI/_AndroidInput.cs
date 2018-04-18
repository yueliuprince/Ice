using UnityEngine;
using UnityEngine.EventSystems;

public class _AndroidInput : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static _AndroidInput PUBLIC;
    public static bool wantFly = false;
    public static bool wantReset = false;
    //public static bool wantSkill0 = false;
    //public static bool wantSkill1 = false;

    [SerializeField] private float radius = 140f;
    private Vector2 startPos;
    private CanvasGroup canvasGroup;
    private float startAlpha;
    [Range(0, 1)] [SerializeField] private float invaildPercentage = 0.25f;
    private float sqrInvaildRadius;

    private void Awake()
    {
        PUBLIC = this;
        startPos = transform.position;
        canvasGroup = GetComponentInParent<CanvasGroup>();
        startAlpha = canvasGroup.alpha;

        sqrInvaildRadius = radius * invaildPercentage * radius * invaildPercentage;
    }

    private Vector2 currentVector;
    public Vector2 InputAxis {
        get {
            if (currentVector.sqrMagnitude < sqrInvaildRadius) return Vector2.zero;
            return currentVector;
        }
        private set {
            currentVector = value;
        }
    }

    public Vector2 GradientAxis { get { return InputAxis / radius; } }

    public Vector2 NormalAxis { get { return InputAxis.normalized; } }

    public Vector2Int AbsAxis {
        get {
            Vector2Int result = Vector2Int.zero;
            if (InputAxis.x > 0) result.x = 1;
            if (InputAxis.x < 0) result.x = -1;
            if (InputAxis.y > 0) result.y = 1;
            if (InputAxis.y < 0) result.y = -1;
            return result;
        }
    }

    public Vector2Int AbsAxisFor_4Dir {
        get {
            if (InputAxis == Vector2.zero) return Vector2Int.zero;
            Vector2Int result = Vector2Int.zero;
            if (Mathf.Abs(InputAxis.x) > Mathf.Abs(InputAxis.y))
            {
                if (InputAxis.x > 0) result.x = 1;
                if (InputAxis.x < 0) result.x = -1;
            }
            else
            {
                if (InputAxis.y > 0) result.y = 1;
                if (InputAxis.y < 0) result.y = -1;
            }
            return result;
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.alpha = startAlpha;
        transform.position = startPos;
        InputAxis = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        InputAxis = Vector2.ClampMagnitude(eventData.position - startPos, radius);
        transform.position = InputAxis + startPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
        InputAxis = Vector2.ClampMagnitude(eventData.position - startPos, radius);
        transform.position = InputAxis + startPos;
    }
}
