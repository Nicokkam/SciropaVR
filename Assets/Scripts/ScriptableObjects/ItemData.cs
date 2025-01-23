using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private AssetReferenceGameObject objectReference;
    [SerializeField] private AssetReferenceAudioClip audioClipReference;

    public string ItemName { get => itemName; }
    public AssetReferenceGameObject ObjectReference { get => objectReference; }
    public AssetReferenceAudioClip AudioClipReference { get => audioClipReference; }

    public GameObject InstantiatedObject { get; set; }

    public AudioClip LoadedAudioClip { get; set; }
}
