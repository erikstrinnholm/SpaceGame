using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
public class TractorBeam : BeamWeaponBase {
    public enum TractorMode { Pull, Push }

    [Header("Cone Settings")]
    [SerializeField] private GameObject beamPrefab;         // prefab with cone mesh
    [SerializeField] private float coneMaxRange = 1000f;    // Z-axis in world units
    [SerializeField] private float coneMaxWidth = 500f;     // Full diameter at far end
    [SerializeField] private Transform firePoint;           // where the beam originates

    [Header("Pull Settings")]
    [SerializeField] private TractorMode mode = TractorMode.Pull;
    [SerializeField] private float forceStrength = 500f;
    [SerializeField] private LayerMask obstacleMask;        // For raycasting blocking objects
    [SerializeField] private LayerMask targetMask;          // For detecting TractorObjects

    [Header("Visuals")]
    [SerializeField] private bool reverseDirection = true;
    [SerializeField] private float visualScrollSpeed = 0.5f;

    private Transform[] coneMeshes;
    private Renderer[] beamRenderers;
    private Vector2 textureOffset;
    private Collider[] coneHits = new Collider[20]; // adjust based on expected targets
    private Vector3[] originalMeshScales;


    //WORKING
    protected override void Start() {
        base.Start();

        if (!beamPrefab) {
            Debug.LogError("TractorBeam beamPrefab not assigned!");
            return;
        }
        if (!firePoint) {
            Debug.LogError("TractorBeam firePoint not assigned!");
            return;
        }
        // Instantiate prefab
        beamInstance = Instantiate(beamPrefab, firePoint.position, firePoint.rotation, ProjectileSpawnParent);
        beamInstance.transform.localScale = Vector3.one; // keep parent scale at 1,1,1
        beamInstance.SetActive(false);

        // Cache
        coneMeshes = beamInstance.GetComponentsInChildren<Transform>();
        beamRenderers = beamInstance.GetComponentsInChildren<Renderer>();

        // Store original local scales
        originalMeshScales = new Vector3[coneMeshes.Length];
        for (int i = 0; i < coneMeshes.Length; i++)
            originalMeshScales[i] = coneMeshes[i].localScale;

        // Duplicate materials to avoid shared texture offsets
        foreach (var rend in beamRenderers)
            rend.material = new Material(rend.material);

        // Initial scaling
        foreach (var mesh in coneMeshes) {
            if (mesh == beamInstance.transform) continue;
            SetConeSize(mesh, coneMaxRange, coneMaxWidth, mesh.localScale);
        }
    }

    

    private void SetConeSize(Transform mesh, float worldLength, float worldWidth, Vector3 baseScale) {
        if (mesh == null) return;
        mesh.localScale = new Vector3(
            (worldWidth / coneMaxWidth) * baseScale.x,
            (worldWidth / coneMaxWidth) * baseScale.y,
            (worldLength / coneMaxRange) * baseScale.z
        );
    }


    protected override IEnumerator FireBeamRoutine(Transform shipTransform, Transform crosshairTransform) {
        if (beamInstance != null) beamInstance.SetActive(true);

        while (firing && CanFire()) {
            AddHeat(Time.deltaTime);

            Vector3 fireDir = GetFireDirection(firePoint, crosshairTransform, coneMaxRange);
            float effectiveRange = coneMaxRange;

            // --- 1. Initial Obstacle Block Check ---
            Ray ray = new Ray(firePoint.position, fireDir);
            if (Physics.Raycast(ray, out RaycastHit hit, coneMaxRange, obstacleMask)){
                effectiveRange = hit.distance;
            }

            // --- 2. Get Targets ---
            int count = Physics.OverlapSphereNonAlloc(firePoint.position, effectiveRange, coneHits, targetMask);
            //float radius = coneMaxWidth * 0.5f;
            float currentRadius = coneMaxWidth * 0.5f * (effectiveRange / coneMaxRange);
            float cosHalfAngle = Mathf.Cos(Mathf.Atan(currentRadius / effectiveRange));
            //float cosHalfAngle = Mathf.Cos(Mathf.Atan(radius / effectiveRange));

            // Go through hits (overlapping sphere)
            for (int i = 0; i < count; i++) {
                Collider col = coneHits[i];
                Vector3 toObj = col.transform.position - firePoint.position;
                float dist = toObj.magnitude;
                Vector3 dirToObj = toObj.normalized;

                // Check if inside cone
                //if (Vector3.Dot(fireDir, dirToObj) < cosHalfAngle) continue;
                float alongBeam = Vector3.Dot(toObj, fireDir); // how far along the cone's axis the object is
                if (alongBeam < 0f || alongBeam > effectiveRange) continue;

                // Compute cone radius at that depth
                float tightness = 0.80f; // smaller = tighter cone (less reach to sides)
                float radiusAtDepth = (coneMaxWidth * 0.5f) * (alongBeam / coneMaxRange) * tightness;

                // Get lateral distance from beam centerline
                Vector3 radialOffset = toObj - fireDir * alongBeam;
                float lateralDist = radialOffset.magnitude;

                // Must be inside radius
                if (lateralDist > radiusAtDepth) continue;


                // Check line of sight
                if (Physics.Raycast(firePoint.position, dirToObj, out RaycastHit obstacleHit, dist, obstacleMask)) {
                    if (obstacleHit.collider != col) continue; // blocked
                }

                // Apply pull or push (depending on mode)
                if (col.TryGetComponent<TractorObject>(out var tractorObj)){
                    Vector3 forceDir = (mode == TractorMode.Pull) ? -dirToObj : dirToObj;
                    tractorObj.ApplyTractorForce(forceDir, dist, forceStrength, Time.deltaTime);
                }
            }

            // 5) Update Visuals
            UpdateBeamVisual(firePoint.position, fireDir, effectiveRange);
            yield return null;
        }
        if (beamInstance != null) beamInstance.SetActive(false);
    }



    private void UpdateBeamVisual(Vector3 origin, Vector3 direction, float length) {
        if (beamInstance == null || coneMeshes == null || beamRenderers == null) return;

        // 1) Position and rotate beam
        beamInstance.transform.position = origin;
        beamInstance.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(180f, 0f, 0f);

        // 2) Scale meshes proportionally
        float scaledWidth = coneMaxWidth * (length / coneMaxRange);

        // Scale meshes
        for (int i = 0; i < coneMeshes.Length; i++) {
            var mesh = coneMeshes[i];
            if (mesh == beamInstance.transform) continue;
            SetConeSize(mesh, length, scaledWidth, originalMeshScales[i]);

            // Offset the mesh along local Z so tip stays at origin
            //float meshLength = originalMeshScales[i].z * (length / coneMaxRange);
            //mesh.localPosition = new Vector3(0, 0, -meshLength * 0.5f);
            //SetConeSize(coneMeshes[i], length, coneMaxWidth, originalMeshScales[i]);
        }

        // Scroll texture UVs
        float dir = reverseDirection ? -1f : 1f;
        textureOffset.y += visualScrollSpeed * dir * Time.deltaTime;
        foreach (var rend in beamRenderers) {
            if (rend != null) rend.material.mainTextureOffset = textureOffset;
        }
    }
}


*/