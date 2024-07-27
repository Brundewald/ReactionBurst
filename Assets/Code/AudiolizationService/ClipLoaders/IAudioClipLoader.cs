using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public interface IAudioClipLoader
    {
        /// <summary>
        /// Загружаем необходимый AudioClip и по завершению загрузки вызываем событие для того,
        /// чтобы предупредить высокоуровневую логику о завершении загруски и возвращаем обратно AudioClip
        /// </summary>
        /// <param name="key">Название клипа для загрузки</param>
        /// <returns>AudioClip
        /// </returns>
        UniTask<AudioClip> LoadClipAsync(string key);

        /// <summary>
        /// По окончании проигрывании трека выгружаем его из памяти по ключу.
        /// </summary>
        /// <param name="key"> Название ключа</param>
        void Release(string key);
    }
}