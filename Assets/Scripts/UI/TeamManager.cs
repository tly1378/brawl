using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Brawl
{
    internal class TeamManager
    {
        public static TeamManager Instance { get; } = new TeamManager();

        private readonly Dictionary<int, Team> teams = new();

        private TeamManager()
        {

        }

        public Team GetTeam(int factionId)
        {
            if (teams.TryGetValue(factionId, out var team))
            {
                return team;
            }

            var newTeam = new Team(factionId);
            teams[factionId] = newTeam;
            return newTeam;
        }
    }

    internal class Team
    {
        private static readonly Color[] colors = new[] { Color.green, Color.red, Color.blue, Color.gray };
        public int Id { get; private set; }
        public Color Color { get; private set; }
        public Vector3 Base { get; set; }

        // 冲锋事件
        public event Action OnCharge;

        public Team(int id)
        {
            Id = id;
            Color = colors[id];
            TeamClock();
        }

        private async void TeamClock()
        {
            while (true)
            {
                float time = UnityEngine.Random.Range(20, 35);
                Debug.LogFormat("队伍{0}将在{1}秒后开始冲锋", Id, time);
                await UniTask.WaitForSeconds(time);
                Debug.LogFormat("队伍{0}开始冲锋", Id);
                OnCharge?.Invoke();
            }
        }
    }
}