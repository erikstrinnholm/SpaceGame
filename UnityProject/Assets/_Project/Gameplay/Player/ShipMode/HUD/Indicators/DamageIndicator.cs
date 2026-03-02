using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;


public class DamageIndicator : BaseThreatIndicator {
    [Header("Visuals")]
    [SerializeField] private Image image;
    [SerializeField] private float lifetime = 2.0f;
    [SerializeField] private float edgePaddingPixels = 40f;

    [Header("Colors")]
    [SerializeField] private GameColorDatabase gameColors;

    private float timer;


    public void Initialize(Camera camera, Damage damage, RectTransform indicatorRoot) {
        base.Initialize(camera);

        // Direction FROM player TO damage source
        Vector3 dir = -damage.Direction; //check?

        // If damage source is already visible → no indicator
        Vector3 viewportPos3 = cam.WorldToViewportPoint(cam.transform.position + dir);
        if (IndicatorMath.IsOnScreen(viewportPos3)) {
            Destroy(gameObject);
            return;
        }

        // Off-screen only → place on edge
        float angle;
        Vector2 viewportPos = IndicatorMath.DirectionToEdgeViewport(
            cam,
            dir,
            out angle,
            indicatorRoot,
            edgePaddingPixels
        );

        // Viewport → UI space
        Vector2 anchoredPos = IndicatorMath.ViewportToAnchored(viewportPos, indicatorRoot);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.rotation = Quaternion.Euler(0f, 0f, angle);

        // Color by damage type
        image.color = gameColors.GetDamageColor(damage.Type);

        timer = lifetime;
        gameObject.SetActive(true);
    }


    private void Update() {
        timer -= Time.deltaTime;

        float t = Mathf.Clamp01(timer / lifetime);
        Color color = image.color;
        color.a *= t;
        image.color = color;

        if (timer <= 0f)
            Destroy(gameObject);
    }
}


/*
Properties
Priority: High
Lifetime: short
Color: shield / hull / critical
No persistence
*/