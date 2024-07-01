using System;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextFloatDisplayer : MonoBehaviour
{
    [SerializeField]
    FloatVariable _floatVariable;
    private TMP_Text _text;
    [SerializeField] private string _prefix = "";
    [SerializeField] private bool _runInUpdate = true;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_runInUpdate && Time.frameCount % 10 == 0)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        _text.text = _prefix + _floatVariable.Value.ToString("F2");
    }
}
