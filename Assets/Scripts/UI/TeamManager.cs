using System;
using System.Collections.Generic;
using UnityEngine;

namespace Brawl
{
    internal class TeamManager
    {
        public static TeamManager Instance { get; } = new TeamManager();

        private readonly Dictionary<int, Team> teams = new Dictionary<int, Team>();

        private TeamManager()
        {

        }

        internal Color GetColor(int factionId)
        {
            var team = GetTeam(factionId);
            return team.color;
        }

        private Team GetTeam(int factionId)
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
        public int id;
        public Color color;

        public Team(int id)
        {
            this.id = id;
            color = colors[id];
        }
    }
}