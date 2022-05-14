using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevTools : MonoBehaviour {

    Canvas canvas;
    Text fpsCounter;

    void Start() {
        canvas = new GameObject("DebugCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fpsCounter = new GameObject("FPS Text").AddComponent<Text>();
        fpsCounter.transform.SetParent(canvas.transform);
        fpsCounter.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fpsCounter.fontSize = (int)Screen.dpi / 4;
        fpsCounter.rectTransform.localPosition = Vector2.zero;
        Font arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        fpsCounter.font = arial;
        fpsCounter.text = "Text";
    }

    //Declare these in your class
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.5f;


    void Update() {
        if (m_timeCounter < m_refreshTime) {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        } else {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
        fpsCounter.text = "FPS: " + m_lastFramerate;
    }
}
