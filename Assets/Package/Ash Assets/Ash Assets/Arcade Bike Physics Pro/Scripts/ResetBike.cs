using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace ArcadeBP_Pro
{
    public class ResetBike : MonoBehaviour
    {
        public BikeSwitcher bikeSwitcher;
        public Button UnRagdollBikeButton;
        public Button resetBikeButton;
        
        public TextMeshProUGUI fpsText; 
        private float deltaTime;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        void Update()
        {
            Fps();
        }
        void Fps()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
            float fps = 1.0f / deltaTime;
        
            if (fpsText != null)
            {
                fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
            }
        }
        private void Start()
        {
            if (UnRagdollBikeButton != null)
            {
                UnRagdollBikeButton.onClick.AddListener(UnRagdollBike);
            }

            if (resetBikeButton != null)
            {
                resetBikeButton.onClick.AddListener(() => ResetCurrentBike());
            }
        }

        private void UnRagdollBike()
        {
            ArcadeBikeControllerPro currentBike = bikeSwitcher.GetCurrentBike();
            if (currentBike != null)
            {
                RagdollActivator ragdollActivator = currentBike.bikeReferences.ragdollActivator;
                if (ragdollActivator != null)
                {
                    ragdollActivator.ReEnableBike();
                }
            }
        }
        
        
        private void ResetCurrentBike()
        {
            ArcadeBikeControllerPro currentBike = bikeSwitcher.GetCurrentBike();
            if (currentBike != null)
            {
                currentBike.bikeReferences.BikeRb.linearVelocity = Vector3.zero;
                currentBike.bikeReferences.BikeRb.angularVelocity = Vector3.zero;
                currentBike.bikeReferences.Rotator.transform.localRotation = Quaternion.identity;
                currentBike.transform.localPosition = Vector3.zero;

                RagdollActivator ragdollActivator = currentBike.bikeReferences.ragdollActivator;
                if (ragdollActivator != null)
                {
                    ragdollActivator.ReEnableBike();
                }
            }
        }
    }

}
