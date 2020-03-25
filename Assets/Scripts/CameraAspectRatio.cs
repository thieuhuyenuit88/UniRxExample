using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour {
    [Header("Custom Aspect")]
    public float targetAspectWidth = 750;
    public float targetAspectHeight = 1334;

    [SerializeField]
    private Vector2ReactiveProperty currentScreenSize;

    // Use this for initialization
    void Start() {
        Camera camera = GetComponent<Camera>();
        currentScreenSize = new Vector2ReactiveProperty(new Vector2((float)Screen.width, (float)Screen.height));
        float targetAspect = targetAspectWidth / targetAspectHeight;

        currentScreenSize.Subscribe(newSize => {
            // current aspect ratio
            float windowAspect = (float)Screen.width / (float)Screen.height;

            float scaleHeight = windowAspect / targetAspect;

            if (scaleHeight < 1.0f)
            {
                Rect rect = camera.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0f;
                rect.y = (1.0f - scaleHeight) * 0.5f;
                camera.rect = rect;
            }
            else
            {
                Rect rect = camera.rect;
                float scaleWidth = 1.0f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) * 0.5f;
                rect.y = 0f;
                camera.rect = rect;
            }
        }).AddTo(gameObject);
    }

    private void Update()
    {
        if (currentScreenSize.Value.x != Screen.width || currentScreenSize.Value.y != Screen.height)
        {
            currentScreenSize.Value = new Vector2((float)Screen.width, (float)Screen.height);
        }
    }
}
