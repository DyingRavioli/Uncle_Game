using UnityEngine;

public class FrequencyScript : MonoBehaviour
{
    public GameObject lowPanel;
    public GameObject midPanel;
    public GameObject highPanel;

    private RectTransform lowRectTransform;
    private RectTransform midRectTransform;
    private RectTransform highRectTransform;

    public AudioSource musicSource;
    public CameraMoveScript camMoveScript;
    private const int sampleSize = 1024; 
    private float[] spectrumData = new float[sampleSize];

    public float thumpThreshold;
    private float thumpCooldown;
    private float prevLowSum = 0;
    void Start()
    {
        lowRectTransform = lowPanel.GetComponent<RectTransform>();
        midRectTransform = midPanel.GetComponent<RectTransform>();
        highRectTransform = highPanel.GetComponent<RectTransform>();

        musicSource = GetComponent<AudioSource>();
        musicSource.Play();
    }

    void Update()
    {
        if (musicSource.isPlaying)
        {
            musicSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            float lowFreqSum = GetFrequency(20f,250f);
            float midFreqSum = GetFrequency(250f, 4000f);
            float highFreqSum = GetFrequency(4000f, 16000f);

            //low freqs are naturally stronger than high
            float averageLow = lowFreqSum * 25000;
            float averageMid = midFreqSum * 80000;
            float averageHigh = highFreqSum * 600000;

            lowRectTransform.sizeDelta = new Vector2(lowRectTransform.sizeDelta.x, averageLow);
            midRectTransform.sizeDelta = new Vector2(midRectTransform.sizeDelta.x, averageMid);
            highRectTransform.sizeDelta = new Vector2(highRectTransform.sizeDelta.x, averageHigh);


            if ((lowFreqSum - prevLowSum) > thumpThreshold && thumpCooldown <= 0)
            {
                camMoveScript.StartCoroutine(camMoveScript.ThumpFOV(0.15f));
                thumpCooldown = 0.15f;
            }

            if (thumpCooldown > 0)
            {
                thumpCooldown -= Time.deltaTime;
            }

            prevLowSum = lowFreqSum;
        }
    }

    float GetFrequency(float min, float max)
    {
        float sampleRate = AudioSettings.outputSampleRate;

        float freqPerBin = sampleRate/ 2f / sampleSize;

        int minBin = Mathf.FloorToInt(min / freqPerBin);
        int maxBin = Mathf.CeilToInt(max / freqPerBin);

        minBin = Mathf.Clamp(minBin, 0, sampleSize - 1);
        maxBin = Mathf.Clamp(maxBin, 0, sampleSize - 1);

        float freqSum = 0;
        for (int i = minBin; i < maxBin; i++)
        {
            freqSum += spectrumData[i];
        }

        freqSum = freqSum / (maxBin - minBin);

        return freqSum;

    }


}
