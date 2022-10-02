using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BeatsDataScriptableObject", order = 1)]
public class BeatsDataScriptableObject : ScriptableObject {
    [TextArea(10,100)] public string lyrics;

    public AudioClip _songSound;
}
