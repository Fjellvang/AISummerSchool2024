using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PointText : MonoBehaviour
{
    
    [SerializeField]
    private PointManager _pointManager;
    [SerializeField]
    private string _prefix = "";
    
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _text.text = _prefix + _pointManager.Points;
    }
}