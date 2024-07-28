using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace MyProject.ReactionBurst.UI
{
    public class ScrollView : MonoBehaviour
    {
        [field: SerializeField] public Transform Content { get; private set;}

        [Header("Moving item animation settings")]
        [SerializeField] private float _itemMoveDuration = 0.5f;
        [SerializeField] private Ease _ease = Ease.Linear;

        private List<MonoBehaviour> _items = new List<MonoBehaviour>();
        private List<MotionHandle> _handles = new List<MotionHandle>();

        public void FillScroll<T>(List<T> items) where T : MonoBehaviour
        {
            foreach (var item in items)
            {
                item.transform.SetParent(Content);
            }

            _items = items as List<MonoBehaviour>;
        }

        public void ClearAndReturnItems(out List<MonoBehaviour> items)
        {
            items = _items;
            _items.Clear();
        }
        
        public void ClearAndDestroyItems()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }
            _items.Clear();
        }

        public async UniTask MoveItemAsync(MonoBehaviour item, int targetIndex)
        {
            var itemIndex = _items.IndexOf(item);
            var targetItemTransform = _items[itemIndex].transform;
            var movingItemTransform = item.transform;
            var itemCount = _items.Count;

            await LMotion.Create(movingItemTransform.localScale, Vector3.one * 1.05f, _itemMoveDuration)
                .WithEase(_ease)
                .BindToLocalScale(movingItemTransform);

            var handle = LMotion.Create(movingItemTransform.position, targetItemTransform.position, _itemMoveDuration)
                .WithEase(_ease)
                .BindToLocalPosition(movingItemTransform);
            
            _handles.Add(handle);
            
            var moveOtherDuration = _itemMoveDuration / itemCount;
            
            for (var i = itemIndex; i < targetIndex; i++)
            {
                handle = LMotion.Create(_items[i + 1].transform.position, item.transform.position, moveOtherDuration)
                    .WithEase(_ease)
                    .BindToLocalPosition(item.transform);
                _handles.Add(handle);
            }
            
            await UniTask.WaitUntil(() => _handles.Any(motionHandle => MotionHandleExtensions.IsActive(motionHandle)));
            _handles.Clear();
            await LMotion.Create(movingItemTransform.localScale, Vector3.one, _itemMoveDuration)
                .WithEase(_ease)
                .BindToLocalScale(movingItemTransform);

            
            _items.Remove(item);
            _items.Insert(targetIndex, item);
        }
    }
}