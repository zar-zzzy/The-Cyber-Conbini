using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CyberConbini.Gameplay
{
    /// <summary>
    /// Resultado de la validación de un reto didáctico en la terminal.
    /// </summary>
    public struct ValidationResult
    {
        public bool IsSuccess;
        public string ConsoleOutput;
        public string FeedbackMessage;
    }

    /// <summary>
    /// Valida el subconjunto didáctico permitido sin ejecutar código Python.
    /// </summary>
    public class ChallengeValidator : MonoBehaviour
    {
        public const int MaxInputLength = 1000;

        private static readonly Regex PrintLiteralRegex = new Regex(
            @"\A[ \t]*print[ \t]*\([ \t]*(?<quote>[""'])(?<value>[^\r\n]*?)\k<quote>[ \t]*\)[ \t]*\z",
            RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(50)
        );

        /// <summary>
        /// Valida el texto del jugador contra la regla controlada del reto.
        /// </summary>
        public ValidationResult Validate(ChallengeDefinition challenge, string playerInput)
        {
            if (challenge == null || !challenge.HasRequiredData)
            {
                return ConfigurationError();
            }

            if (string.IsNullOrWhiteSpace(playerInput))
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error: instrucción vacía.",
                    FeedbackMessage = "Escribe tu instrucción en la terminal antes de ejecutar."
                };
            }

            if (playerInput.Length > MaxInputLength)
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error: instrucción demasiado larga.",
                    FeedbackMessage = "La instrucción es demasiado larga. Intenta una solución más breve."
                };
            }

            if (challenge.ValidationType != ChallengeValidationType.PrintLiteral)
            {
                return ConfigurationError();
            }

            return ValidatePrintLiteral(challenge, playerInput.Trim());
        }

        private static ValidationResult ValidatePrintLiteral(
            ChallengeDefinition challenge,
            string trimmedInput
        )
        {
            Match match;

            try
            {
                match = PrintLiteralRegex.Match(trimmedInput);
            }
            catch (RegexMatchTimeoutException)
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error: instrucción no reconocida.",
                    FeedbackMessage = "Aún no funciona. Revisa la instrucción e inténtalo otra vez."
                };
            }

            string expectedValue = challenge.Rules.ExpectedValue;

            if (match.Success)
            {
                string extractedText = match.Groups["value"].Value;

                if (string.Equals(extractedText, expectedValue, StringComparison.Ordinal))
                {
                    return new ValidationResult
                    {
                        IsSuccess = true,
                        ConsoleOutput = "> " + challenge.ExpectedOutput,
                        FeedbackMessage = challenge.SuccessFeedback
                    };
                }

                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> " + EscapeRichText(extractedText),
                    FeedbackMessage = $"Aún no coincide. Se esperaba: \"{expectedValue}\"."
                };
            }

            if (string.Equals(trimmedInput, expectedValue, StringComparison.Ordinal))
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error de sintaxis: falta la función print().",
                    FeedbackMessage = "Recuerda envolver el texto dentro de la función print(\"... \")."
                };
            }

            return new ValidationResult
            {
                IsSuccess = false,
                ConsoleOutput = "> Error: instrucción no reconocida.",
                FeedbackMessage = "Aún no funciona. Revisa la instrucción e inténtalo otra vez."
            };
        }

        private static string EscapeRichText(string value)
        {
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private static ValidationResult ConfigurationError()
        {
            return new ValidationResult
            {
                IsSuccess = false,
                ConsoleOutput = "> Error: reto no disponible.",
                FeedbackMessage = "No se pudo cargar la configuración del reto."
            };
        }
    }
}
