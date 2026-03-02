using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class IndicatorMath {
    public static bool IsOnScreen(Vector3 viewportPos) {
        return viewportPos.z > 0f &&
               viewportPos.x > 0f && viewportPos.x < 1f &&
               viewportPos.y > 0f && viewportPos.y < 1f;
    }

    public static Vector2 DirectionToEdgeViewport(Camera cam, Vector3 worldDirection,
        out float angleDeg, RectTransform root, float edgePaddingPixels
    ){
        Vector3 localDir = cam.transform.InverseTransformDirection(worldDirection.normalized);

        // Flip if behind camera
        /*
        if (localDir.z < 0f) {
            localDir.x = -localDir.x;
            localDir.y = -localDir.y;
        }
        */

        Vector2 dir2D = new Vector2(localDir.x, localDir.y);

        if (dir2D.sqrMagnitude < 0.0001f)
            dir2D = Vector2.up;

        dir2D.Normalize();

        angleDeg = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg - 90f;

        Vector2 center = new Vector2(0.5f, 0.5f);
        Vector2 pos = center + dir2D * 0.5f;

        // Apply padding in viewport space
        Vector2 padding = new Vector2(
            edgePaddingPixels / root.rect.size.x,
            edgePaddingPixels / root.rect.size.y
        );

        pos.x = Mathf.Clamp(pos.x, padding.x, 1f - padding.x);
        pos.y = Mathf.Clamp(pos.y, padding.y, 1f - padding.y);
        return pos;
    }

    public static Vector2 ViewportToAnchored(Vector2 viewportPos, RectTransform root){
        return (viewportPos - new Vector2(0.5f, 0.5f)) * root.rect.size;
    }
}

/*
Single source of truth for projection logic.
(This avoids copy-pasted math bugs.)
*/
