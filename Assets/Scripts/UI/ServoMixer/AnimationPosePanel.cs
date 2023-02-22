using Demonixis.InMoov.Data;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Systems.Animations;
using System;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public sealed class AnimationPosePanel : MonoBehaviour
    {
        private ServoMixerService _servoMixerService;
        private GesturePlayer _gesturePlayer;

        [SerializeField] private Transform _leftHandGesturesContainer;
        [SerializeField] private Transform _armsGesturesContainer;
        [SerializeField] private GameObject _gesturePrefab;

        private void Start()
        {
            Robot.Instance.WhenStarted(Initialize);
        }

        private void Initialize()
        {
            _servoMixerService = FindObjectOfType<ServoMixerService>();
            _gesturePlayer = new GesturePlayer(_servoMixerService);

            PopulateContainer(_leftHandGesturesContainer, _gesturePrefab, true);
            PopulateContainer(_armsGesturesContainer, _gesturePrefab, false);
        }

        private void PopulateContainer(Transform container, GameObject prefab, bool hand)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);

            string[] names;

            if (hand)
                names = Enum.GetNames(typeof(HandGestures));
            else
                names = Enum.GetNames(typeof(ArmGestures));

            for (var i = 0; i < names.Length; i++)
            {
                var item = Instantiate(prefab, container);
                ServoMixerPanel.ResetTransform(item.transform);

                var handGestureItem = item.AddComponent<UIGestureItem>();

                if (hand)
                {
                    handGestureItem.SetupHandGesture(i);
                    handGestureItem.Clicked += OnHandGestureClicked;
                }
                else
                {
                    handGestureItem.SetupArmGesture(i);
                    handGestureItem.Clicked += OnArmGestureClicked;
                }
            }
        }

        private void OnHandGestureClicked(int obj)
        {
            var gesture = (HandGestures)obj;
            _gesturePlayer.ApplyHandGesture(true, gesture);
            _gesturePlayer.ApplyHandGesture(false, gesture);
        }

        private void OnArmGestureClicked(int obj)
        {
            var gesture = (ArmGestures)obj;
            _gesturePlayer.ApplyArmGesture(true, gesture);
            _gesturePlayer.ApplyArmGesture(false, gesture);
        }
    }
}