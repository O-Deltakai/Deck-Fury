using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIBoxScreenBoundsChecker : MonoBehaviour
{
    private RectTransform uiBoxRectTransform;
    [SerializeField] RectTransform canvasRectTransform;

    [SerializeField] bool boxWithinBounds = false;

    [SerializeField] float timer = 0.0f;
    private float interval = 0.06f; // 15 FPS


    Coroutine CR_AdjustmentCoroutine = null;

    void Awake()
    {

        uiBoxRectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }


    public void OnPositionChanged()
    {
        if(CR_AdjustmentCoroutine != null)
        {
            StopCoroutine(CR_AdjustmentCoroutine);
        }
        CR_AdjustmentCoroutine = StartCoroutine(AdjustPositionCoroutine());
    }

    void FixedUpdate()
    {
        if(!gameObject.activeInHierarchy) { return; }
        timer += Time.fixedDeltaTime;

        if (timer >= interval)
        {
            AdjustPositionWithinCanvas();
            timer = 0.0f;
        }        

    }


    IEnumerator AdjustPositionCoroutine()
    {

        while(!IsWithinCanvasBounds())
        {
            print("Box not within canvas - adjusting");

            Vector3[] uiBoxCorners = new Vector3[4];
            uiBoxRectTransform.GetWorldCorners(uiBoxCorners);

            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            Rect canvasRect = new Rect(canvasCorners[0], canvasCorners[2] - canvasCorners[0]);

            for (int i = 0; i < uiBoxCorners.Length; i++)
            {
                if (!canvasRect.Contains(uiBoxCorners[i]))
                {
                    Vector2 adjustedPosition = uiBoxRectTransform.anchoredPosition;

                    if (uiBoxCorners[i].x < canvasRect.xMin)
                        adjustedPosition.x += canvasRect.xMin - uiBoxCorners[i].x;
                    else if (uiBoxCorners[i].x > canvasRect.xMax)
                        adjustedPosition.x -= uiBoxCorners[i].x - canvasRect.xMax;

                    if (uiBoxCorners[i].y < canvasRect.yMin)
                        adjustedPosition.y += canvasRect.yMin - uiBoxCorners[i].y;
                    else if (uiBoxCorners[i].y > canvasRect.yMax)
                        adjustedPosition.y -= uiBoxCorners[i].y - canvasRect.yMax;

                    uiBoxRectTransform.anchoredPosition = adjustedPosition;
                    break; // Adjust once to avoid overcorrection
                }
            }


            yield return new WaitForSecondsRealtime(0.03f);
        }

        //CR_AdjustmentCoroutine = null;

    }

    void AdjustPositionWithinCanvas()
    {
            Vector3[] uiBoxCorners = new Vector3[4];
            uiBoxRectTransform.GetWorldCorners(uiBoxCorners);

            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            Rect canvasRect = new Rect(canvasCorners[0], canvasCorners[2] - canvasCorners[0]);

            for (int i = 0; i < uiBoxCorners.Length; i++)
            {
                if (!canvasRect.Contains(uiBoxCorners[i]))
                {
                    Vector2 adjustedPosition = uiBoxRectTransform.anchoredPosition;

                    if (uiBoxCorners[i].x < canvasRect.xMin)
                        adjustedPosition.x += canvasRect.xMin - uiBoxCorners[i].x;
                    else if (uiBoxCorners[i].x > canvasRect.xMax)
                        adjustedPosition.x -= uiBoxCorners[i].x - canvasRect.xMax;

                    if (uiBoxCorners[i].y < canvasRect.yMin)
                        adjustedPosition.y += canvasRect.yMin - uiBoxCorners[i].y;
                    else if (uiBoxCorners[i].y > canvasRect.yMax)
                        adjustedPosition.y -= uiBoxCorners[i].y - canvasRect.yMax;

                    uiBoxRectTransform.anchoredPosition = adjustedPosition;
                    break; // Adjust once to avoid overcorrection
                }
            }
    }


    private bool IsWithinCanvasBounds()
    {
        Vector3[] uiBoxCorners = new Vector3[4];
        uiBoxRectTransform.GetWorldCorners(uiBoxCorners);

        foreach (var uiCorner in uiBoxCorners)
        {
            print("Box corner: " + uiCorner);
        }

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        foreach (var canvasCorner in canvasCorners)
        {
            print("Canvas corner: " + canvasCorner);
        }


        // Convert canvas corners to a Rect for easier comparison
        Rect canvasRect = new Rect(canvasCorners[0], canvasCorners[2] - canvasCorners[0]);

        foreach (Vector3 corner in uiBoxCorners)
        {
            // Convert corner from world space to screen space
            Vector2 screenCorner = RectTransformUtility.WorldToScreenPoint(null, corner);
            print("Comparing coordinates: Screen Corner: " + screenCorner + ", UIBoxCorner: " + corner);
            if (!canvasRect.Contains(screenCorner))
            {
                return false;
            }
        }
        print("Box within canvas ");

        return true;
    }


}