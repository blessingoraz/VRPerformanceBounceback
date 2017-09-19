//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Helper to display various hmd stats via GUIText
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class FPSViewer : MonoBehaviour
{
    public Text text;

    public Color fadeColor = Color.black;
    public float fadeDuration = 1.0f;
    private CVRCompositor compositor;

    void Awake()
    {
        compositor = OpenVR.Compositor;

        if (text == null)
        {
            text = GetComponent<Text>();
            text.enabled = false;
        }

        if (fadeDuration > 0)
        {
            SteamVR_Fade.Start(fadeColor, 0);
            SteamVR_Fade.Start(Color.clear, fadeDuration);
        }
    }

    double lastUpdate = 0.0f;

    void Update()
    {
        if (text != null)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                text.enabled = !text.enabled;
            }

            if (text.enabled)
            {
                if (compositor != null)
                {
                    SetTiming();
                }
            }
        }
    }

    void SetTiming()
    {

        var timing = new Compositor_FrameTiming();
        timing.m_nSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Compositor_FrameTiming));
        compositor.GetFrameTiming(ref timing, 0);

        var update = timing.m_flSystemTimeInSeconds;
        if (update > lastUpdate)
        {
            var framerate = (lastUpdate > 0.0f) ? 1.0f / (update - lastUpdate) : 0.0f;
            int numFramesDropped = (int)timing.m_nNumDroppedFrames;
            lastUpdate = update;
            text.text = string.Format("FPS: {0:N0}\nDropped: {1}", framerate, numFramesDropped);
            ColorizeText(framerate);
        }
        else
        {
            lastUpdate = update;
        }
    }

    void ColorizeText(double framerate)
    {
        text.color = Color.black;
        if (framerate < 70)
        {
            text.color = Color.yellow;
        }
        if (framerate < 50)
        {
            text.color = Color.red;
        }
    }
}

