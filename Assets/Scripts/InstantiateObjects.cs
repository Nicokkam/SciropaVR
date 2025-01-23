using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

[System.Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}

public class InstantiateObjects : MonoBehaviour
{
    [SerializeField] private List<ItemData> items;
    private Dictionary<string, InputAction> _spawnActions = new Dictionary<string, InputAction>();
    private Dictionary<string, InputAction> _despawnActions = new Dictionary<string, InputAction>();

    private void Awake()
    {
        foreach (var item in items)
        {
            // Preload the AudioClip
            item.AudioClipReference.LoadAssetAsync().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    item.LoadedAudioClip = handle.Result;
                }
                else
                {
                    Debug.LogError($"Failed to preload AudioClip for {item.ItemName}");
                }
            };
        }
    }

    private void OnEnable()
    {
        foreach (var item in items)
        {
            // Create input actions dynamically for each item
            var spawnAction = new InputAction(type: InputActionType.Button, binding: $"<Keyboard>/{item.ItemName[0]}");
            spawnAction.performed += context => SpawnItem(item);
            spawnAction.Enable();
            _spawnActions[item.ItemName] = spawnAction;

            var despawnAction = new InputAction(type: InputActionType.Button, binding: $"<Keyboard>/{item.ItemName[1]}");
            despawnAction.performed += context => DespawnItem(item);
            despawnAction.Enable();
            _despawnActions[item.ItemName] = despawnAction;
        }
    }

    private void SpawnItem(ItemData item)
    {
        item.ObjectReference.InstantiateAsync().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                item.InstantiatedObject = handle.Result;

                // Attach the audio clip if available
                var audioSource = item.InstantiatedObject.GetComponent<AudioSource>();
                if (audioSource != null && item.LoadedAudioClip != null)
                {
                    audioSource.clip = item.LoadedAudioClip;
                    audioSource.Play();
                }
            }
            else
            {
                Debug.LogError($"Failed to instantiate {item.ItemName}");
            }
        };
    }

    private void DespawnItem(ItemData item)
    {
        if (item.InstantiatedObject != null)
        {
            item.ObjectReference.ReleaseInstance(item.InstantiatedObject);
            item.InstantiatedObject = null;
        }
        else
        {
            Debug.LogWarning($"No {item.ItemName} instance to despawn.");
        }
    }

    private void OnDisable()
    {
        foreach (var action in _spawnActions.Values)
        {
            action.Disable();
        }

        foreach (var action in _despawnActions.Values)
        {
            action.Disable();
        }

        foreach (var item in items)
        {
            // Release preloaded audio clip
            if (item.AudioClipReference != null && item.LoadedAudioClip != null)
            {
                item.AudioClipReference.ReleaseAsset();
                item.LoadedAudioClip = null;
            }
        }
    }
}