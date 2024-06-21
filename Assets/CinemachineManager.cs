using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Brawl
{
    public class CinemachineManager : MonoBehaviour
    {
        public static CinemachineManager Instance;
        private CinemachineFollow cinemachineFollow;
        private CinemachineCamera cinemachineCamera;
        private CinemachineRotationComposer rotationComposer;
        [SerializeField] private float rotationSpeed = 180f;
        private int currentIndex;
        private readonly List<Controller> controllers = new();
        private const int targetFaction = 0;
        private Vector2 originalDamping;

        public Controller CurrentController => controllers?[currentIndex];
        public event Action<Controller> OnSwitchTarget;

        private void Awake()
        {
            Instance = this;
            cinemachineFollow = GetComponent<CinemachineFollow>();
            cinemachineCamera = GetComponent<CinemachineCamera>();
            rotationComposer = GetComponent<CinemachineRotationComposer>();
            originalDamping = rotationComposer.Damping;
        }

        private void Start()
        {
            foreach(var controller in CharacterManager.Instance.Controllers)
            {
                if(controller.FactionId == targetFaction)
                {
                    controllers.Add(controller);
                }
            }

            cinemachineCamera.Follow = CurrentController.transform;
        }

        private void Update()
        {
            // 玩家操作旋转镜头时，Damping应该为0
            if (Input.GetKeyDown(KeyCode.Q))
            {
                rotationComposer.Damping = Vector2.zero;
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                rotationComposer.Damping = originalDamping;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                rotationComposer.Damping = Vector2.zero;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                rotationComposer.Damping = originalDamping;
            }

            // 旋转镜头
            if (Input.GetKey(KeyCode.Q))
            {
                RotateCamera(rotationSpeed);
            }
            if (Input.GetKey(KeyCode.E))
            {
                RotateCamera(-rotationSpeed);
            }

            // 切换聚焦对象
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentIndex++;
                if (currentIndex >= controllers.Count) currentIndex = 0;
                cinemachineCamera.Follow = CurrentController.transform;
                OnSwitchTarget?.Invoke(CurrentController);
            }
        }

        private void RotateCamera(float angle)
        {
            if (cinemachineFollow != null)
            {
                var currentOffset = cinemachineFollow.FollowOffset;
                Quaternion rotation = Quaternion.AngleAxis(angle * Time.deltaTime, Vector3.up);
                cinemachineFollow.FollowOffset = rotation * currentOffset;
            }
        }
    }
}
