using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PointManager _pointManager;
    [SerializeField]
    private SkyboxChanger _skyboxChanger;
    [SerializeField] private float _gameTime = 60f;
    [SerializeField]
    FloatVariable _timeLeft;
    
    private string path = Application.dataPath + "/Maria/Ending.txt";
    
    [SerializeField]
    Image GameOverPanel;

    private bool _gameEnded;
    
    private void Start()
    {
        _pointManager.ResetPoints();
        _skyboxChanger.SetRandomSkybox();
        _gameEnded = false;
        _timeLeft.Value = _gameTime;
    }

    private void Update()
    {
        _timeLeft.Value -= Time.deltaTime;
        if (_timeLeft.Value <= 0 && !_gameEnded 
            #if UNITY_EDITOR
            || Input.GetKeyDown(KeyCode.Space)
            #endif
            ) 
        {
            _gameEnded = true;
            StartCoroutine(FadeInEnumerator());
        }
    }

    IEnumerator FadeInEnumerator()
    {
        CreateOrUpdateTextFile();
        
        while (GameOverPanel.color.a < 1)
        {
            GameOverPanel.color = new Color(GameOverPanel.color.r, GameOverPanel.color.g, GameOverPanel.color.b, GameOverPanel.color.a + Time.deltaTime);
            yield return null;
        }

        SceneManager.LoadScene("End");
    }
    
    void CreateOrUpdateTextFile()
    {
        try
        {
            // Append the new result to the text file
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                if (writer.BaseStream.Length > 0)
                {
                    writer.Write($",{_pointManager.Points}");
                }
                else
                {
                    writer.Write(_pointManager.Points);
                }
            }
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to write to file: {ex.Message}");
        }
    }
}
