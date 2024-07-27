using System;
using UnityEngine;

namespace MyProject.ReactionBurst.Data
{
    [Serializable]
    public class GeneratorData
    {
        [Tooltip("Начальная длина списка фигур")]
        [field: SerializeField] public int InitialScrollLength { get; private set; } = 5;
        [Tooltip("Начальное диапазон цвета и фигур для генерации")]
        [field: SerializeField] public int DefaultGeneratorRange { get; private set; } = 2;
        [Tooltip("Значение, на которое будет происходить увеличение  диапазона при правильных ответах")]
        [field: SerializeField] public int RangeIncreasingValue { get; private set; }= 1;
        [Tooltip("Варианты цветов")]
        [field: SerializeField] public Color[] ColorVariants { get; private set; }
        [Tooltip("Варианты фигур")]
        [field: SerializeField] public Sprite[] ShapeVariants { get; private set;}

    }
}
