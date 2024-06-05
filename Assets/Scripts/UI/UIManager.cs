using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public RectTransform RootRect { get; private set; }

        private void Awake()
        {
            Instance = this;
            RootRect = transform as RectTransform;
        }

        internal async UniTask CreateOverheadUI(AgentController agent)
        {
            var overheadUIGO = await Addressables.InstantiateAsync("OverheadUI", transform);
            var overheadUI = overheadUIGO.GetComponent<OverheadUI>();
            overheadUI.Set(agent);
        }
    }
}