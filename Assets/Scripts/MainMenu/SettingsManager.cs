using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public Button btnNormalDifficulty;
    public Button btnHardDifficulty;

    public Slider sldSmoothSpeed;
    public Slider sldSensitivity;



    public Button btnSave;

    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private int baseZombiePerWave;
    [SerializeField] private float multiplicatorZombiePerWave;

    public Button btnMap1;
    public Button btnMap2;
    private string selectedMap;

    public Button btnPlay;

    void Start()
    {
        LoadSettings();

        btnNormalDifficulty.onClick.AddListener(SetNormalDifficulty);
        btnHardDifficulty.onClick.AddListener(SetHardDifficulty);

        btnMap1.onClick.AddListener(() => SelectMap("modif"));
        btnMap2.onClick.AddListener(() => SelectMap("Map2Scene"));

        btnPlay.onClick.AddListener(LoadSelectedMap);

        btnSave.onClick.AddListener(SaveSettings);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("TimeBetweenWaves", timeBetweenWaves);
        PlayerPrefs.SetInt("BaseZombiePerWave", baseZombiePerWave);
        PlayerPrefs.SetFloat("MultiplicatorZombiePerWave", multiplicatorZombiePerWave);
        PlayerPrefs.SetString("SelectedMap", selectedMap);

        PlayerPrefs.Save();

        Debug.Log("Settings saved: " + timeBetweenWaves + "s between waves, " + baseZombiePerWave + " zombies per wave, multiplicator: " + multiplicatorZombiePerWave);
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("TimeBetweenWaves"))
        {
            timeBetweenWaves = PlayerPrefs.GetFloat("TimeBetweenWaves");
        }
        if (PlayerPrefs.HasKey("BaseZombiePerWave"))
        {
            baseZombiePerWave = PlayerPrefs.GetInt("BaseZombiePerWave");
        }
        if (PlayerPrefs.HasKey("MultiplicatorZombiePerWave"))
        {
            multiplicatorZombiePerWave = PlayerPrefs.GetFloat("MultiplicatorZombiePerWave");
        }
        if (PlayerPrefs.HasKey("SelectedMap"))
        {
            selectedMap = PlayerPrefs.GetString("SelectedMap");
        }
    }

    private void SetNormalDifficulty()
    {
        timeBetweenWaves = 15f;
        baseZombiePerWave = 5;
        multiplicatorZombiePerWave = 1.5f;
    }

    private void SetHardDifficulty()
    {
        timeBetweenWaves = 10f;
        baseZombiePerWave = 10;
        multiplicatorZombiePerWave = 1.6f;
    }

    private void SelectMap(string mapName)
    {
        selectedMap = mapName;

        PlayerPrefs.SetString("SelectedMap", selectedMap);

        Debug.Log("Selected map: " + selectedMap);
    }

    public void LoadSelectedMap()
    {
        if (!string.IsNullOrEmpty(selectedMap))
        {
            SceneManager.LoadScene(selectedMap);
        }
        else
        {
            Debug.LogWarning("No map selected!");
        }
    }
}
