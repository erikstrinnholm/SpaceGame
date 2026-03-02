using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MissileIndicator : BaseThreatIndicator {
    [Header("Visuals")]
    [SerializeField] private Image image;    
    [SerializeField] private Sprite edgeSprite;
    [SerializeField] private Sprite onScreenSprite;

    [Header("Behavior")]
    [SerializeField] private float edgePaddingPixels = 40f;
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private float onScreenAlpha = 0.6f;
    [SerializeField] private float offScreenAlpha = 1f;
    [SerializeField] private float hideCenterRadius = 0.05f; // viewport units

    private Transform missile;
    private RectTransform root;

    protected override void Awake() {
        base.Awake(); // ensures rect is assigned
    }

    public void Initialize(Camera camera, Transform missileTransform, RectTransform indicatorRoot) {
        base.Initialize(camera);

        missile = missileTransform;
        root = indicatorRoot;

        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        gameObject.SetActive(true);
    }

    private void Update() {
        if (missile == null) {
            Destroy(gameObject);
            return;
        }
        UpdateIndicator();
    }

    private void UpdateIndicator() {
        Vector3 vp = cam.WorldToViewportPoint(missile.position);
        bool onScreen = IndicatorMath.IsOnScreen(vp);

        Vector2 viewportPos;
        float angle = 0f;

        if (onScreen) {
            viewportPos = vp;
            image.sprite = onScreenSprite;
        } else {
            Vector3 dir = (missile.position - cam.transform.position).normalized;
            viewportPos = IndicatorMath.DirectionToEdgeViewport(
                cam,
                dir,
                out angle,
                root,
                edgePaddingPixels
            );
            image.sprite = edgeSprite;
        }

        // Hide near screen center
        bool hide =
            onScreen &&
            Vector2.Distance(viewportPos, new Vector2(0.5f, 0.5f)) < hideCenterRadius;

        image.enabled = !hide;

        // Fade smoothly
        float targetAlpha = onScreen ? onScreenAlpha : offScreenAlpha;
        FadeAlpha(targetAlpha);

        // Apply transform
        rect.anchoredPosition =
            IndicatorMath.ViewportToAnchored(viewportPos, root);

        rect.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FadeAlpha(float targetAlpha) {
        Color c = image.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        image.color = c;
    }    
}


/*
Properties
Priority: Very High
Persistent while locked
Pulses faster as impact nears
Overrides enemy indicators if overlapping
*/