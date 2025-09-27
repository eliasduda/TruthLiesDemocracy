using UnityEngine;
using UnityEngine.UI;

public class GenerateDaysVisualizer : MonoBehaviour
{

    public int maxDays = 30;
    public GameObject dayVisualizerPrefab;
    public GameObject divider;
    public int dividerAfterDays = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < maxDays; i++)
        {
            Instantiate(dayVisualizerPrefab, transform);
            if((i + 1) % dividerAfterDays == 0 && i != maxDays - 1)
            {
                Instantiate(divider, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
