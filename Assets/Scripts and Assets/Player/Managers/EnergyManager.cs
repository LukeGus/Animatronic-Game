using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public float maxEnergy;
    public float energyIncreaseRate;
    private float currentEnergy;

    void Start()
    {
        currentEnergy = 0f;
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyIncreaseRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }
    }
}