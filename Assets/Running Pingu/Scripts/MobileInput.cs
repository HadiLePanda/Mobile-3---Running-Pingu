using UnityEngine;

public class MobileInput : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minSwipeTreshold = 100f;

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 swipeDelta, startTouch;

    public bool Tap => tap;
    public Vector2 SwipeDelta => swipeDelta;
    public bool SwipeLeft => swipeLeft;
    public bool SwipeRight => swipeRight;
    public bool SwipeUp => swipeUp;
    public bool SwipeDown => swipeDown;

    public static MobileInput Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // reset all the bools
        tap = swipeLeft = swipeRight = swipeDown = swipeUp = false;

        // check current input
        #region STANDALONE INPUT
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }
        #endregion

        #region MOBILE INPUT
        if (Input.touches.Length != 0)
        {
            // just tapped the screen
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }
        #endregion

        // get touch delta
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            // check for mobile
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            // check for standalone
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        // check for swipe
        if (swipeDelta.magnitude > minSwipeTreshold)
        {
            // we got a valid swipe
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // left or right
                if (x < 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            else
            {
                // up or down
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }

            // consider we ended up touch since we finished validating a swipe
            startTouch = swipeDelta = Vector2.zero;
        }
    }
}
