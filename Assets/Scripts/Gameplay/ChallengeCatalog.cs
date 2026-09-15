using System.Collections.Generic;
using UnityEngine;

namespace CyberConbini.Gameplay
{
    [CreateAssetMenu(
        fileName = "ChallengeCatalog",
        menuName = "Cyber-Conbini/Challenge Catalog"
    )]
    public sealed class ChallengeCatalog : ScriptableObject
    {
        [SerializeField] private string moduleId;
        [SerializeField] private string moduleTitle;
        [Min(1)]
        [SerializeField] private int plannedChallengeCount = 1;
        [SerializeField] private List<ChallengeDefinition> challenges = new List<ChallengeDefinition>();

        public string ModuleId => moduleId;
        public string ModuleTitle => moduleTitle;
        public int PlannedChallengeCount => plannedChallengeCount;
        public int ChallengeCount => challenges?.Count ?? 0;
        public IReadOnlyList<ChallengeDefinition> Challenges => challenges;

        public ChallengeDefinition GetChallenge(int index)
        {
            if (challenges == null || index < 0 || index >= challenges.Count)
            {
                return null;
            }

            return challenges[index];
        }
    }
}
