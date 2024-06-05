﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Brawl
{
    [RequireComponent(typeof(Health), typeof(NavMeshAgent), typeof(MeleeAttack))]
    public class Controller : MonoBehaviour
    {
        public Action<string> OnSpeak;
        public Transform UIPosition;
        [SerializeField] private int factionId;
        private readonly Dictionary<string, float> attributes = new();
        private Health health;
        private MeleeAttack melee;
        private NavMeshAgent agent;

        public event Action<string, float, float?> OnAttributeChange;

        public Health Health => health;
        public MeleeAttack Melee => melee;
        public NavMeshAgent Agent => agent;
        public int FactionId => factionId;

        public float? GetAttribute(string name)
        {
            if (attributes.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

        public void SetAttribute(string name, float value)
        {
            if (attributes.TryGetValue(name, out var origin))
            {
                attributes[name] = value;
                OnAttributeChange?.Invoke(name, value, origin);
            }
            else
            {
                attributes[name] = value;
                OnAttributeChange?.Invoke(name, value, null);
            }
        }

        public string ShowAttributes()
        {
            string description = "";
            foreach (var attr in attributes)
            {
                description += attr.Key + "=" + attr.Value + "\n";
            }
            return description;
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            melee = GetComponent<MeleeAttack>();
            melee.factionId = FactionId;
        }
    }
}