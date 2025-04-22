using UnityEngine;

namespace Bitgem.VFX.StylisedWater
{
    public class WateverVolumeFloater : MonoBehaviour
    {
        [Tooltip("Toggle on to see console logs for floater height calculations")]
        public bool EnableDebugLogs = false;
        public WaterVolumeHelper WaterVolumeHelper = null;
        public float scale = 1;

        private float? previousWaterHeight;

        void Start()
        {
            var helper = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
            if (helper == null)
            {
                Debug.LogWarning("[Floater Start] No WaterVolumeHelper assigned.");
                return;
            }

            // Initialize previousWaterHeight from the start position
            previousWaterHeight = helper.GetHeight(transform.position);

            if (EnableDebugLogs)
                Debug.Log($"[Floater Start] previousWaterHeight = {(previousWaterHeight.HasValue ? previousWaterHeight.Value.ToString("F3") : "null")}");
        }

        void Update()
        {
            var helper = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
            if (helper == null) return;

            // Get current water height under us
            var currentHeight = helper.GetHeight(transform.position);

            if (EnableDebugLogs)
                Debug.Log($"[Floater Update] currentHeight = {(currentHeight.HasValue ? currentHeight.Value.ToString("F3") : "null")}"
                          + $", previousWaterHeight = {(previousWaterHeight.HasValue ? previousWaterHeight.Value.ToString("F3") : "null")}");

            // If both are valid, move by their difference
            if (currentHeight.HasValue && previousWaterHeight.HasValue)
            {
                float delta = currentHeight.Value - previousWaterHeight.Value;

                if (EnableDebugLogs)
                    Debug.Log($"[Floater Calc] delta = {delta:F3}");

                // Apply the vertical shift only
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y - (delta * scale),
                    transform.position.z
                );
            }
            else if (EnableDebugLogs)
            {
                Debug.Log("[Floater Update] Skipping movement because height was null.");
            }

            // Store for next frame
            previousWaterHeight = currentHeight;
        }
    }
}