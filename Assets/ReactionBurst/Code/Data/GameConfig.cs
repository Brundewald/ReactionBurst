using UnityEngine;
using UnityEngine.Localization;

namespace MyProject.ReactionBurst.Data
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Configs/" + nameof(GameConfig))]
    public sealed class GameConfig : ScriptableObject
    {
        [Tooltip("Задержка перед началом игры в секундах")]
        [field: SerializeField] public int SecondsBeforeStart { get; private set; } = 3;
        [Tooltip("Длительность уровня")]
        [field: SerializeField] public int LevelDurationInSeconds { get; private set; } = 60;
        [Tooltip("Количество очков за отгданную фигуру по-умолчанию")]
        [field: SerializeField] public int DefaultScore { get; private set; } = 50;
        [Tooltip("Начальное значение множителя очков")]
        [field: SerializeField] public int DefaultScoreModifier { get; private set; } = 1;
        [Tooltip("Количество ответов необходимых для увеличения множителя")]
        [field: SerializeField] public int ScoreIncreaseSequenceLength { get; private set; } = 5;
        [Tooltip("Максимальное количество очков для полоски прогресса финального экрана")]
        [field: SerializeField] public int MaxScore { get; private set; } = 5000;
        [Tooltip("Максимальное количество очков для положения звезды на финальном экране")]
        [field: SerializeField] public int StartMarkScore { get; private set; }= 5000;
        [Tooltip("Нотация на игровом экране")]
        [field: SerializeField] public LocalizedString Notation { get; private set; }
        [Tooltip("Данные для генератора")]
        [field: SerializeField] public GeneratorData GeneratorData { get; private set; }
    }
}
