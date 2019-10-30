using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class UnityEventArgString : UnityEvent<string> {}

public class EffectManager : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private Transform effectParent;
    private Dictionary<string, GameObject> effects;

    private void Awake()
    {
        effects = new Dictionary<string, GameObject>();
        foreach (Transform effect in effectParent)
        {
            effects.Add(effect.name, effect.gameObject);
        }

        foreach (var b in buttons)
        {
            b.onClick.AddListener(delegate { ChangeEffect(b.name); });
        }
    }

    private void ChangeEffect(string name)
    {
        foreach(KeyValuePair<string, GameObject> effect in effects) {
            effect.Value.SetActive(effect.Key == name);
        }
    }
}
