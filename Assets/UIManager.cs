using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public RectTransform RootRect { get; private set; }

    private void Awake()
    {
        Instance = this;
        RootRect = transform as RectTransform;
    }

    internal async void CreateHPBar(Health health)
    {
        var HPBar = await Addressables.InstantiateAsync("HPBar", transform);
        var bar = HPBar.GetComponent<HPBar>();
        bar.Init(health);
    }

    internal async UniTask CreateStateBar(EnemyController enemyController)
    {
        var StateBar = await Addressables.InstantiateAsync("StateBar", transform);
        var bar = StateBar.GetComponent<StateBar>();
        bar.Init(enemyController);
    }
}
