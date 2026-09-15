using System;
using UnityEngine;

namespace CyberConbini.Gameplay
{
    public enum ChallengeValidationType
    {
        PrintLiteral,
        VariableAssignment,
        VariableAssignmentAndPrint
    }

    public enum ChallengeValueKind
    {
        String,
        Number
    }

    [Serializable]
    public sealed class ChallengeValidationRules
    {
        [SerializeField] private string variableName;
        [SerializeField] private string expectedValue;
        [SerializeField] private ChallengeValueKind valueKind = ChallengeValueKind.String;

        public string VariableName => variableName;
        public string ExpectedValue => expectedValue;
        public ChallengeValueKind ValueKind => valueKind;
    }

    [Serializable]
    public sealed class ChallengeDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string title;
        [TextArea(2, 4)]
        [SerializeField] private string prompt;
        [SerializeField] private string targetDisplayText;
        [TextArea(2, 3)]
        [SerializeField] private string hint;
        [SerializeField] private string expectedOutput;
        [TextArea(2, 3)]
        [SerializeField] private string successFeedback;
        [SerializeField] private ChallengeValidationType validationType;
        [SerializeField] private ChallengeValidationRules rules = new ChallengeValidationRules();

        public string Id => id;
        public string Title => title;
        public string Prompt => prompt;
        public string TargetDisplayText => targetDisplayText;
        public string Hint => hint;
        public string ExpectedOutput => expectedOutput;
        public string SuccessFeedback => successFeedback;
        public ChallengeValidationType ValidationType => validationType;
        public ChallengeValidationRules Rules => rules;

        public bool HasRequiredData =>
            !string.IsNullOrWhiteSpace(id) &&
            !string.IsNullOrWhiteSpace(title) &&
            !string.IsNullOrWhiteSpace(prompt) &&
            !string.IsNullOrWhiteSpace(targetDisplayText) &&
            !string.IsNullOrWhiteSpace(hint) &&
            !string.IsNullOrWhiteSpace(expectedOutput) &&
            !string.IsNullOrWhiteSpace(successFeedback) &&
            rules != null &&
            !string.IsNullOrWhiteSpace(rules.ExpectedValue);
    }
}
