using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeManager : MonoBehaviour
{
    public Image[] screens; // Array of screens (Screen 1, Screen 2, Screen 3)
    public float swipeRange = 50f; // Minimum distance for a swipe to be considered
    public float indicatorMovementDuration = 0.5f;

    private int currentIndex = 0;
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool stopTouch = false;

    void Update()
    {
        if (IsTouchWithinImageBounds() && IsAnyScreenActive())
        {
            Swipe();
        }
    }

    private bool IsTouchWithinImageBounds()
    {
        Vector2 touchPosition = Vector2.zero;

        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            touchPosition = Input.mousePosition;
        }
        else
        {
            return false;
        }

        RectTransform rectTransform = screens[currentIndex].GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, null, out Vector2 localPoint);

        return rectTransform.rect.Contains(localPoint);
    }

    private bool IsAnyScreenActive()
    {
        foreach (Image screen in screens)
        {
            if (screen.gameObject.activeSelf) return true; // At least one screen is active
        }
        return false; // No active screen
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

                        if (distance.x < -swipeRange) // Right to Left Swipe
                        {
                            SwipeLeft();
                            stopTouch = true;
                        }
                        else if (distance.x > swipeRange) // Left to Right Swipe
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

    void SwipeRight()
    {
        if (currentIndex == 0)
        {
            // Disable Screen 1 on Left to Right swipe
            screens[currentIndex].gameObject.SetActive(false);
        }
        else if (currentIndex > 0)
        {
            currentIndex--;
            ShowCurrentScreen();
        }
    }

    void SwipeLeft()
    {
        if (currentIndex < screens.Length - 1)
        {
            currentIndex++;
            ShowCurrentScreen();
        }

        // If we reach last screen, prevent further swipes
        if (currentIndex == screens.Length - 1)
        {
            // Only allow swiping back to Screen 2 (prevent from going forward)
            currentIndex = screens.Length - 1;
        }
    }

    void ShowCurrentScreen()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].gameObject.SetActive(i == currentIndex);
        }
    }

    // ✅ Reset Function to restore everything to default
    public void ResetSwipeManager()
    {
        currentIndex = 0; // Reset index to the first screen
        stopTouch = false;

        // Enable the first screen and disable others
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].gameObject.SetActive(false);
        }
    }
}
