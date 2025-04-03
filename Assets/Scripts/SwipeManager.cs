using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeManager : MonoBehaviour
{
    public Image[] screens; // Array of screens (Screen 1, Screen 2, Screen 3)
    public float swipeRange = 50f; // Minimum swipe distance

    private int currentIndex = -1; // Start with no screen active
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool stopTouch = false;


    private void Start()
    {
        ResetSwipeManager();
    }
    void Update()
    {
        Swipe();
    }

    void Swipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    stopTouch = false;
                    break;

                case TouchPhase.Moved:
                    currentTouchPosition = touch.position;
                    if (!stopTouch)
                    {
                        Vector2 distance = currentTouchPosition - startTouchPosition;

                        if (distance.x < -swipeRange) // Swipe left
                        {
                            SwipeLeft();
                            stopTouch = true;
                        }
                        else if (distance.x > swipeRange) // Swipe right
                        {
                            SwipeRight();
                            stopTouch = true;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    stopTouch = false;
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            stopTouch = false;
        }
        else if (Input.GetMouseButton(0))
        {
            currentTouchPosition = Input.mousePosition;
            if (!stopTouch)
            {
                Vector2 distance = (Vector2)currentTouchPosition - startTouchPosition;

                if (distance.x < -swipeRange)
                {
                    SwipeLeft();
                    stopTouch = true;
                }
                else if (distance.x > swipeRange)
                {
                    SwipeRight();
                    stopTouch = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            stopTouch = false;
        }
    }

    void SwipeLeft()
    {
        if (currentIndex < screens.Length - 1)
        {
            currentIndex++;
            screens[currentIndex].gameObject.SetActive(true);
        }
    }

    void SwipeRight()
    {
        if (currentIndex >= 0)
        {
            screens[currentIndex].gameObject.SetActive(false);
            currentIndex--;
        }
    }

    // Reset Function: Disable all screens
    public void ResetSwipeManager()
    {
        foreach (Image screen in screens)
        {
            screen.gameObject.SetActive(false);
        }
        currentIndex = -1;
        stopTouch = false;
    }
}
