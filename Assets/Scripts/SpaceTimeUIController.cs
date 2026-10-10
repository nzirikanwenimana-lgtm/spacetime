using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceTimeUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider mass1Slider;
    public Slider mass2Slider;
    public Slider forceSlider;
    public TMP_Text mass1ValueText;
    public TMP_Text mass2ValueText;
    public TMP_Text forceValueText;

    [Header("Distance Display")]
    public TMP_Text distanceText;

    [Header("SpaceTime Objects")]
    public Transform mass1;
    public Transform mass2;

    [Header("Gravity Script")]
    public MassGravityMovement gravityMovement;

    [Header("Grid Deformation")]
    public SpaceTimeGravityGrid gravityGrid;

    void Start()
    {
        // Set initial values
        if (mass1Slider != null)
            mass1Slider.value = 300f;

        if (mass2Slider != null)
            mass2Slider.value = 100f;

        if (forceSlider != null)
            forceSlider.value = 30f;

        // Listen for slider changes
        if (mass1Slider != null)
            mass1Slider.onValueChanged.AddListener(UpdateMass1);

        if (mass2Slider != null)
            mass2Slider.onValueChanged.AddListener(UpdateMass2);

        if (forceSlider != null)
            forceSlider.onValueChanged.AddListener(UpdateForce);

        UpdateMass1(mass1Slider != null ? mass1Slider.value : 300f);
        UpdateMass2(mass2Slider != null ? mass2Slider.value : 100f);
        UpdateForce(forceSlider != null ? forceSlider.value : 30f);
    }

    void Update()
    {
        UpdateDistance();
    }

    public void UpdateMass1(float value)
    {
        if (gravityGrid != null)
            gravityGrid.mass1Value = value;

        if (mass1ValueText != null)
            mass1ValueText.text = value.ToString("F0");
    }

    public void UpdateMass2(float value)
    {
        if (gravityGrid != null)
            gravityGrid.mass2Value = value;

        if (mass2ValueText != null)
            mass2ValueText.text = value.ToString("F0");
    }

    public void UpdateForce(float value)
    {
        if (gravityMovement != null)
            gravityMovement.force = value;

        if (forceValueText != null)
            forceValueText.text = value.ToString("F1");
    }
    void UpdateDistance()
    {
        if (mass1 == null || mass2 == null || distanceText == null)
            return;

        Vector3 position1 = mass1.position;
        Vector3 position2 = mass2.position;

        // Measure horizontal distance across the spacetime plane.
        position1.y = 0f;
        position2.y = 0f;

        float distance = Vector3.Distance(position1, position2);

        distanceText.text =
            "DISTANCE: " + distance.ToString("F2") + " units";
    }

}