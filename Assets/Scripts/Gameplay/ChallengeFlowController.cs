using UnityEngine;

namespace CyberConbini.Gameplay
{
    public sealed class ChallengeFlowController : MonoBehaviour
    {
        [SerializeField] private ChallengeCatalog catalog;
        [SerializeField] private ChallengeValidator validator;

        public ChallengeDefinition CurrentChallenge { get; private set; }
        public int CurrentIndex { get; private set; } = -1;
        public int PlannedChallengeCount => catalog != null ? catalog.PlannedChallengeCount : 0;
        public string ModuleTitle => catalog != null ? catalog.ModuleTitle : string.Empty;
        public bool IsInitialized => CurrentChallenge != null;

        private void Awake()
        {
            if (catalog != null && validator != null)
            {
                LoadFirstChallenge();
            }
        }

        public bool Initialize()
        {
            return LoadFirstChallenge();
        }

        public bool Initialize(
            ChallengeCatalog challengeCatalog,
            ChallengeValidator challengeValidator
        )
        {
            catalog = challengeCatalog;
            validator = challengeValidator;
            return LoadFirstChallenge();
        }

        public ValidationResult ValidateCurrent(string playerInput)
        {
            if (!IsInitialized || validator == null)
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error: reto no disponible.",
                    FeedbackMessage = "No se pudo cargar el reto actual."
                };
            }

            return validator.Validate(CurrentChallenge, playerInput);
        }

        private bool LoadFirstChallenge()
        {
            ChallengeDefinition firstChallenge = catalog != null
                ? catalog.GetChallenge(0)
                : null;

            if (validator == null || firstChallenge == null || !firstChallenge.HasRequiredData)
            {
                CurrentIndex = -1;
                CurrentChallenge = null;
                return false;
            }

            CurrentIndex = 0;
            CurrentChallenge = firstChallenge;
            return true;
        }
    }
}
