/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour {

    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private RectTransform dashGroup;
    private List<GameObject> gameObjectList;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.parent.Find("templates").Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.parent.Find("templates").Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.parent.Find("templates").Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.parent.Find("templates").Find("dashTemplateY").GetComponent<RectTransform>();
        dashGroup = graphContainer.Find("dashes").GetComponent<RectTransform>();

        gameObjectList = new List<GameObject>();
    }

    public void ShowGraph(List<float> valueList, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
        if (getAxisLabelX == null) {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null) {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        if (maxVisibleValueAmount <= 0) {
            maxVisibleValueAmount = valueList.Count;
        }

        ResetGraph();
        
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];
        
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float value = valueList[i];
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0) {
            yDifference = 5f;
        }
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        yMinimum = 0f; // Start the graph at zero

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;




            if (valueList.Count > 100)
            {
                if (i % 10 == 0)
                {
                    RectTransform labelX = Instantiate(labelTemplateX);
                    labelX.SetParent(graphContainer, false);
                    labelX.gameObject.SetActive(true);
                    labelX.anchoredPosition = new Vector2(xPosition, -7f);
                    labelX.GetComponent<Text>().text = getAxisLabelX(i);
                    gameObjectList.Add(labelX.gameObject);
                }
            }
            else if (valueList.Count > 50)
            {
                if (i % 2 == 0)
                {
                    RectTransform labelX = Instantiate(labelTemplateX);
                    labelX.SetParent(graphContainer, false);
                    labelX.gameObject.SetActive(true);
                    labelX.anchoredPosition = new Vector2(xPosition, -7f);
                    labelX.GetComponent<Text>().text = getAxisLabelX(i);
                    gameObjectList.Add(labelX.gameObject);
                }
            }
            else
            {
                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPosition, -7f);
                labelX.GetComponent<Text>().text = getAxisLabelX(i);
                gameObjectList.Add(labelX.gameObject);
            }

            if (valueList.Count > 70)
            {
                if (i % 10 == 0)
                {
                    RectTransform dashX = Instantiate(dashTemplateX);
                    dashX.SetParent(dashGroup, false);
                    dashX.gameObject.SetActive(true);
                    dashX.anchoredPosition = new Vector2(xPosition, -3f);
                    gameObjectList.Add(dashX.gameObject);
                }
            }
            else if (valueList.Count > 30)
            {
                if (i % 2 == 0)
                {
                    RectTransform dashX = Instantiate(dashTemplateX);
                    dashX.SetParent(dashGroup, false);
                    dashX.gameObject.SetActive(true);
                    dashX.anchoredPosition = new Vector2(xPosition, -3f);
                    gameObjectList.Add(dashX.gameObject);
                }
            }
            else
            {
                RectTransform dashX = Instantiate(dashTemplateX);
                dashX.SetParent(dashGroup, false);
                dashX.gameObject.SetActive(true);
                dashX.anchoredPosition = new Vector2(xPosition, -3f);
                gameObjectList.Add(dashX.gameObject);
            }

            GameObject barGameObject = CreateBar(new Vector2(xPosition, yPosition), xSize * .9f);
            gameObjectList.Add(barGameObject);

            xIndex++;
        }

        int separatorCount = 20;
        for (int i = 1; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            if (i < separatorCount)
            {
                RectTransform dashY = Instantiate(dashTemplateY);
                dashY.SetParent(dashGroup, false);
                dashY.gameObject.SetActive(true);
                dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
                gameObjectList.Add(dashY.gameObject);
            }
        }
    }

    public void ResetGraph()
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
    }

    private GameObject CreateBar(Vector2 graphPosition, float barWidth) {
        GameObject gameObject = new GameObject("bar", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
        rectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(.5f, 0f);
        return gameObject;
    }

}
