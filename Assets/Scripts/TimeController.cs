using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI; // Add this for accessing the slider

public class TimeController : MonoBehaviour
{
    [SerializeField] private float timeMultiplier; // Speed at which the time progresses
    [SerializeField] private float startHour; // Starting hour for the game
    [SerializeField] private TextMeshProUGUI timeText; // Reference to the TextMeshPro for displaying the time
    [SerializeField] private Light SunLight;

    [Header("Custom Time Transition Settings")]
    [SerializeField] private float sunriseTime = 6f; // Time when sunrise starts (6 AM)
    [SerializeField] private float dayStartTime = 8f; // Time when daytime starts (8 AM)
    [SerializeField] private float eveningStartTime = 17f; // Time when evening starts (5 PM)
    [SerializeField] private float nightStartTime = 19f; // Time when night starts (7 PM)

    [Header("Lightmaps for Different Times of Day")]
    [SerializeField] private Texture2D[] dayLightmaps;
    [SerializeField] private Texture2D[] dayLightmapDirs;
    [SerializeField] private Texture2D[] eveningLightmaps;
    [SerializeField] private Texture2D[] eveningLightmapDirs;
    [SerializeField] private Texture2D[] nightLightmaps;
    [SerializeField] private Texture2D[] nightLightmapDirs;
    [SerializeField] private Texture2D[] sunriseLightmaps;
    [SerializeField] private Texture2D[] sunriseLightmapDirs;

    [Header("Skybox Materials")]
    [SerializeField] private Material skyboxMaterial;

    [Header("Skybox Colors")]
    [SerializeField] private Color dayColor = new Color(0.514f, 0.812f, 1f); // Hex: #83CFFF
    [SerializeField] private Color eveningColor = new Color(1f, 0.608f, 0.514f); // Hex: #FF9B83
    [SerializeField] private Color nightColor = new Color(0.345f, 0.486f, 0.588f); // Hex: #587C96
    [SerializeField] private Color dawnColor = new Color(0.345f, 0.486f, 0.588f); // Hex: #587C96

    [Header("Time Sliders")]
    [SerializeField] private Slider timeSlider; // Existing slider (time multiplier)
    [SerializeField] private TextMeshProUGUI sliderValueText; // To display the slider value (optional)

    [Header("New Time Slider")]
    [SerializeField] private Slider gameTimeSlider; // New slider for adjusting the game time
    [SerializeField] private TextMeshProUGUI gameTimeText; // To display the new slider value

    private DateTime currentTime;
    private Color currentSkyboxColor;
    private float currentSkyboxBlend;

    private void Start()
    {
        // Set initial time and default skybox
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        // Initialize the skybox color and blend
        currentSkyboxColor = dayColor;
        currentSkyboxBlend = 0f;

        // Apply default lightmaps
        ApplyLightmaps(dayLightmaps, dayLightmapDirs);

        // Set the initial slider values and ranges
        timeSlider.value = timeMultiplier;  // Set slider to current time multiplier value
        timeSlider.onValueChanged.AddListener(UpdateTimeMultiplier);

        // Optional: Display initial value of the time slider
        if (sliderValueText != null)
        {
            sliderValueText.text = timeSlider.value.ToString("F2");
        }

        // Initialize the new game time slider
        gameTimeSlider.onValueChanged.AddListener(UpdateGameTimeFromSlider);

        // Optional: Display initial value of the game time slider
        if (gameTimeText != null)
        {
            gameTimeText.text = gameTimeSlider.value.ToString("F2");
        }
    }

    private void Update()
    {
        UpdateTimeOfDay();
        UpdateLightmapsAndSkybox();
        UpdateSkyboxColor();
        DisplayTime();
    }

    private void UpdateTimeMultiplier(float value)
    {
        // Update the time multiplier based on the slider's value
        timeMultiplier = value;

        // Optionally display the slider value
        if (sliderValueText != null)
        {
            sliderValueText.text = value.ToString("F2");
        }
    }

    private void UpdateGameTimeFromSlider(float value)
    {
        // Update the game's time based on the slider value (0 - 24 hours)
        currentTime = currentTime.Date + TimeSpan.FromHours(value);

        // Optionally display the new game time value
        if (gameTimeText != null)
        {
            gameTimeText.text = value.ToString("F2");
        }
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
    }

    private void UpdateLightmapsAndSkybox()
    {
        float hour = (float)currentTime.TimeOfDay.TotalHours;

        Texture2D[] selectedLightmaps = dayLightmaps;
        Texture2D[] selectedLightmapDirs = dayLightmapDirs;

        float targetIntensity = 2f; // Default to day intensity
        float skyboxBlend = 0f; // Default to day blend

        // Determine which phase we're in based on custom times
        if (hour >= sunriseTime && hour < dayStartTime) // Sunrise
        {
            selectedLightmaps = sunriseLightmaps;
            selectedLightmapDirs = sunriseLightmapDirs;

            float t = (hour - sunriseTime) / (dayStartTime - sunriseTime);
            targetIntensity = Mathf.Lerp(0.8f, 2f, t); // Transition from sunrise to day intensity
            skyboxBlend = Mathf.Lerp(0f, 0.25f, t); // Blend towards sunrise
        }
        else if (hour >= dayStartTime && hour < eveningStartTime) // Daytime
        {
            selectedLightmaps = dayLightmaps;
            selectedLightmapDirs = dayLightmapDirs;
            targetIntensity = 2f; // Constant day intensity
            skyboxBlend = 0f; // Full day
        }
        else if (hour >= eveningStartTime && hour < nightStartTime) // Evening
        {
            selectedLightmaps = eveningLightmaps;
            selectedLightmapDirs = eveningLightmapDirs;

            float t = (hour - eveningStartTime) / (nightStartTime - eveningStartTime);
            targetIntensity = Mathf.Lerp(1f, 0.5f, t); // Transition from evening to night intensity
            skyboxBlend = Mathf.Lerp(0.25f, 0.75f, t); // Blend towards evening
        }
        else if (hour >= nightStartTime || hour < sunriseTime) // Night
        {
            selectedLightmaps = nightLightmaps;
            selectedLightmapDirs = nightLightmapDirs;

            if (hour >= nightStartTime)
            {
                float t = (hour - nightStartTime) / (24f - nightStartTime + sunriseTime);
                targetIntensity = Mathf.Lerp(0.5f, 0.8f, t); // Transition from night to sunrise intensity
                skyboxBlend = Mathf.Lerp(0.75f, 1f, t); // Blend towards night
            }
            else // Before sunrise
            {
                float t = hour / sunriseTime;
                targetIntensity = Mathf.Lerp(0.5f, 0.8f, t);
                skyboxBlend = Mathf.Lerp(1f, 0f, t); // Blend towards sunrise
            }
        }

        // Smoothly transition the sun intensity
        SunLight.intensity = Mathf.Lerp(SunLight.intensity, targetIntensity, Time.deltaTime * 2f);

        // Update the skybox blend value
        skyboxMaterial.SetFloat("_Day_Night_Cycle", Mathf.Lerp(skyboxMaterial.GetFloat("_Day_Night_Cycle"), skyboxBlend, Time.deltaTime * 2f));

        // Apply the correct lightmaps
        ApplyLightmaps(selectedLightmaps, selectedLightmapDirs);
    }

    private void UpdateSkyboxColor()
    {
        float hour = (float)currentTime.TimeOfDay.TotalHours;
        Color targetColor = dayColor;

        if (hour >= sunriseTime && hour < dayStartTime) // Sunrise
        {
            float t = (hour - sunriseTime) / (dayStartTime - sunriseTime);
            targetColor = Color.Lerp(dawnColor, dayColor, t);
        }
        else if (hour >= dayStartTime && hour < eveningStartTime) // Daytime
        {
            targetColor = dayColor;
        }
        else if (hour >= eveningStartTime && hour < nightStartTime) // Evening
        {
            float t = (hour - eveningStartTime) / (nightStartTime - eveningStartTime);
            targetColor = Color.Lerp(eveningColor, nightColor, t);
        }
        else if (hour >= nightStartTime || hour < sunriseTime) // Night
        {
            if (hour >= nightStartTime)
            {
                float t = (hour - nightStartTime) / (24f - nightStartTime + sunriseTime);
                targetColor = Color.Lerp(nightColor, dawnColor, t);
            }
            else
            {
                float t = hour / sunriseTime;
                targetColor = Color.Lerp(dawnColor, dayColor, t);
            }
        }

        // Smoothly transition the skybox color to the target color
        currentSkyboxColor = Color.Lerp(currentSkyboxColor, targetColor, Time.deltaTime * 0.5f);

        // Adjust the blend to fully transition to the correct value (1 for night)
        if (hour >= nightStartTime || hour < sunriseTime)
        {
            currentSkyboxBlend = Mathf.Lerp(currentSkyboxBlend, 1f, Time.deltaTime * 0.5f); // Fully blend towards night
        }
        else
        {
            currentSkyboxBlend = Mathf.Lerp(currentSkyboxBlend, 0f, Time.deltaTime * 0.5f); // Return to day blend
        }

        // Set the color for the skybox material
        skyboxMaterial.SetColor("_Horizontal_color", currentSkyboxColor);
    }

    private void ApplyLightmaps(Texture2D[] lightmapColors, Texture2D[] lightmapDirs = null)
    {
        int lightmapCount = lightmapColors.Length;
        LightmapData[] lightmapData = new LightmapData[lightmapCount];

        for (int i = 0; i < lightmapCount; i++)
        {
            lightmapData[i] = new LightmapData
            {
                lightmapColor = lightmapColors[i],
                lightmapDir = lightmapDirs != null && i < lightmapDirs.Length ? lightmapDirs[i] : null
            };
        }

        if (lightmapData.Length > 0)
        {
            LightmapSettings.lightmaps = lightmapData;
        }
        else
        {
            Debug.LogWarning("No lightmaps found for the selected time of day.");
        }
    }

    private void DisplayTime()
    {
        string formattedTime = currentTime.ToString("HH:mm");

        if (timeText != null)
        {
            timeText.text = formattedTime;
        }
        else
        {
            Debug.LogWarning("Time Text is not assigned in the inspector.");
        }
    }
}
