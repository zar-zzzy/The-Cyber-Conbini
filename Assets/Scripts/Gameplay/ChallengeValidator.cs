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
    /// Validador modular para el primer reto de Python didáctico: print("Bienvenido al Cyber-Conbini").
    /// Utiliza análisis determinístico por patrones de texto/expresiones regulares sin ejecutar código arbitrario.
    /// </summary>
    public class ChallengeValidator : MonoBehaviour
    {
        public const string TARGET_MESSAGE = "Bienvenido al Cyber-Conbini";

        // Expresión regular que acepta comillas simples o dobles y espacios opcionales antes/después del texto dentro de print()
        // Ejemplos válidos:
        // print("Bienvenido al Cyber-Conbini")
        // print( "Bienvenido al Cyber-Conbini" )
        // print('Bienvenido al Cyber-Conbini')
        // print(   'Bienvenido al Cyber-Conbini'   )
        private static readonly Regex PrintRegex = new Regex(
            @"^\s*print\s*\(\s*([""'])(.*?)\1\s*\)\s*$",
            RegexOptions.Compiled | RegexOptions.Singleline
        );

        /// <summary>
        /// Valida el código Python ingresado por el jugador para el reto 1.
        /// </summary>
        /// <param name="rawInput">Código sin procesar escrito en el InputField</param>
        /// <returns>Estructura con el resultado y mensajes asociados</returns>
        public ValidationResult ValidateFirstChallenge(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error: instrucción vacía.",
                    FeedbackMessage = "Escribe tu instrucción en la terminal antes de ejecutar."
                };
            }

            string trimmed = rawInput.Trim();

            // Evaluar coincidencia con la estructura print(...)
            Match match = PrintRegex.Match(trimmed);

            if (match.Success)
            {
                string extractedText = match.Groups[2].Value;

                // Verificar que el contenido del mensaje sea exactamente el solicitado
                if (string.Equals(extractedText, TARGET_MESSAGE, StringComparison.Ordinal))
                {
                    return new ValidationResult
                    {
                        IsSuccess = true,
                        ConsoleOutput = "> " + TARGET_MESSAGE,
                        FeedbackMessage = "¡Correcto! El cliente ha sido recibido."
                    };
                }
                else
                {
                    return new ValidationResult
                    {
                        IsSuccess = false,
                        ConsoleOutput = "> " + extractedText,
                        FeedbackMessage = $"Aún no coincide. Se esperaba: \"{TARGET_MESSAGE}\"."
                    };
                }
            }

            // Si el jugador escribió el texto sin print()
            if (trimmed.Contains(TARGET_MESSAGE))
            {
                return new ValidationResult
                {
                    IsSuccess = false,
                    ConsoleOutput = "> Error de sintaxis: falta la función print().",
                    FeedbackMessage = "Recuerda envolver el texto dentro de la función print(\"... \")."
                };
            }

            // Fallo general de sintaxis
            return new ValidationResult
            {
                IsSuccess = false,
                ConsoleOutput = "> Error: instrucción no reconocida.",
                FeedbackMessage = "Aún no funciona. Revisa la instrucción e inténtalo otra vez."
            };
        }
    }
}
