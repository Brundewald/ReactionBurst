using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Data;
using UnityEngine;
using Random = System.Random;

namespace MyProject.ReactionBurst.Core.SubSystems
{
    public sealed class GeneratorService
    {
        private readonly GeneratorData _generatorData;
        private readonly Random _random;
        private ComparableFigure _previousGeneratedFigure;
        private int _spriteGenerationRange;
        private int _colorGenerationRange;

        public GeneratorService(GameConfig  config)
        {
            _random = new Random(DateTime.Now.Ticks.GetHashCode());
            _generatorData = config.GeneratorData;
            _spriteGenerationRange = _colorGenerationRange = _generatorData.DefaultGeneratorRange;
        }

        /// <summary>
        /// Генерирует фигуры для начала игры в количестве равном пулу
        /// </summary>
        /// <returns></returns>
        public async UniTask<ComparableFigure[]> GenerateFiguresPoolAsync()
        {
            var figuresPool = new ComparableFigure[_generatorData.InitialScrollLength];

            for (var i = 0; i < figuresPool.Length; i++)
            {
                figuresPool[i] = await GenerateRandomFigureAsync();
            }

            return figuresPool;
        }
        
        /// <summary>
        /// Генерирует рандомную фигуру прикаждом обращении
        /// </summary>
        /// <returns></returns>
        public async UniTask<ComparableFigure> GetComparableFigureAsync()
        {
            var newComparable = await GenerateRandomFigureAsync();
            return newComparable;
        }

        /// <summary>
        /// Увеличиваем диапазон выборки спрайтов и цветов
        /// </summary>
        public void IncreaseGeneratorRange()
        {
            _spriteGenerationRange = Mathf.Clamp(
                _spriteGenerationRange + _generatorData.RangeIncreasingValue, 
                _generatorData.DefaultGeneratorRange, _generatorData.ShapeVariants.Length);
            _colorGenerationRange = Mathf.Clamp(
                _spriteGenerationRange + _generatorData.RangeIncreasingValue, 
                _generatorData.DefaultGeneratorRange, _generatorData.ColorVariants.Length);
        }

        /// <summary>
        /// Уменьшаем диапазон выборки спрайтов и цветов
        /// </summary>
        public void DecreaseGeneratorRange()
        {
            _spriteGenerationRange = Mathf.Clamp(
                _spriteGenerationRange - _generatorData.RangeIncreasingValue, 
                _generatorData.DefaultGeneratorRange, _generatorData.ShapeVariants.Length);
            _colorGenerationRange = Mathf.Clamp(
                _spriteGenerationRange + _generatorData.RangeIncreasingValue, 
                _generatorData.DefaultGeneratorRange, _generatorData.ColorVariants.Length);
        }

        /// <summary>
        /// Генерирует случайную фигуру в процессе игры с возможностью повторения предыдущей
        /// </summary>
        /// <returns></returns>
        private async UniTask<ComparableFigure> GenerateRandomFigureAsync()
        {
            if (_previousGeneratedFigure is null)
                return _previousGeneratedFigure = await GenerateFullRandomFigureAsync();
            else
            {
                var random = new Random(DateTime.Now.Ticks.GetHashCode());
                
                var newFigure = new ComparableFigure();

                //Сначала определяем с шансом 50/50 меняем фигуру или нет
                var changeFigure = random.Next(0, 2) == 0;

                if (changeFigure)
                {
                    //определяем меняем форму или нет 
                    var changeSprite = random.Next(0, 2) == 0;
                    newFigure.Sprite = changeSprite ? GetRandomSprite(true) : _previousGeneratedFigure.Sprite;
                    //определяем меняем цвет или нет 
                    var changeColor = random.Next(0, 2) == 0;
                    newFigure.Color = changeColor ? GetRandomColor(true) : _previousGeneratedFigure.Color;
                }
                else 
                    newFigure = _previousGeneratedFigure;
                
                return _previousGeneratedFigure = newFigure;
            }
        }

        /// <summary>
        /// Генерирует первую случайную фигуру
        /// </summary>
        /// <param name="limitRange"></param>
        /// <returns></returns>
        private async UniTask<ComparableFigure> GenerateFullRandomFigureAsync(bool limitRange = false)
        {
            var sprite = GetRandomSprite(limitRange);
            var color = GetRandomColor(limitRange);
            var newComparable = new ComparableFigure {Sprite = sprite, Color = color};
            return newComparable;
        }

        /// <summary>
        /// Возвращает случайный спрайт
        /// </summary>
        /// <param name="limitRange"></param>
        /// <returns></returns>
        private Sprite GetRandomSprite(bool limitRange)
        {
            if (limitRange)
                return _generatorData.ShapeVariants[_random.Next(0, _spriteGenerationRange)];
            else
                return _generatorData.ShapeVariants[_random.Next(0, _generatorData.ShapeVariants.Length)];
        }

        /// <summary>
        /// Возвращает случайный цвет
        /// </summary>
        /// <param name="limitRange"></param>
        /// <returns></returns>
        private Color GetRandomColor(bool limitRange)
        {
            if(limitRange)
                return _generatorData.ColorVariants[_random.Next(0, _colorGenerationRange)];
            else
                return _generatorData.ColorVariants[_random.Next(0, _generatorData.ColorVariants.Length)];
        }
    }
}