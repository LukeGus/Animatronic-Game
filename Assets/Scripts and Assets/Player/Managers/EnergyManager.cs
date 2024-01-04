using UnityEngine;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; set; }
    
    public float maxEnergy;
    public float energyIncreaseRate;
    public float currentEnergy;
    
    public TMP_Text energyText;

    void Start()
    {
        currentEnergy = 0f;
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyIncreaseRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }
        
        energyText.text = "Energy: " + Mathf.RoundToInt(currentEnergy);
    }
}