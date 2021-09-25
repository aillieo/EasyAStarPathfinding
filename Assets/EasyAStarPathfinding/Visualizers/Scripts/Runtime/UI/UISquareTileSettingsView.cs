using AillieoUtils.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileSettingsView : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action onTileDisplayModeChanged;

        [SerializeField]
        private Dropdown tileColorMode;
        [SerializeField]
        private Dropdown tileTextMode;
        [SerializeField]
        private Dropdown tileOpMode;
        [SerializeField]
        private Dropdown tileCstMdfMode;

        private void Awake()
        {
            ConfigDropdowns();
        }

        private void OnEnable()
        {
            ResetDropDown<UISquareTileCtrl.ColorMode>(tileColorMode, UISquareTileCtrl.Instance.colorMode);
            ResetDropDown<UISquareTileCtrl.TextMode>(tileTextMode, UISquareTileCtrl.Instance.textMode);
            ResetDropDown<UISquareTileCtrl.OperationMode>(tileOpMode, UISquareTileCtrl.Instance.opMode);
            ResetDropDown<UISquareTileCtrl.CostModificationMode>(tileCstMdfMode, UISquareTileCtrl.Instance.costMdfMode);
        }

        private Vector2 startPos;
        private Vector2 startPosLocal;
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = (this.transform as RectTransform).anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform.parent as RectTransform, eventData.position, this.GetComponentInParent<Canvas>().worldCamera, out startPosLocal);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform.parent as RectTransform, eventData.position, this.GetComponentInParent<Canvas>().worldCamera, out Vector2 newPos);
            (this.transform as RectTransform).anchoredPosition = startPos + (newPos - startPosLocal);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        private void ConfigDropdowns()
        {
            ConfigOneDropDown<UISquareTileCtrl.ColorMode>(tileColorMode,
                v =>
                {
                    UISquareTileCtrl.Instance.colorMode = v;
                    onTileDisplayModeChanged?.Invoke();
                });
            ConfigOneDropDown<UISquareTileCtrl.TextMode>(tileTextMode,
                v =>
                {
                    UISquareTileCtrl.Instance.textMode = v;
                    onTileDisplayModeChanged?.Invoke();
                });
            ConfigOneDropDown<UISquareTileCtrl.OperationMode>(tileOpMode, v => UISquareTileCtrl.Instance.opMode = v);
            ConfigOneDropDown<UISquareTileCtrl.CostModificationMode>(tileCstMdfMode, v => UISquareTileCtrl.Instance.costMdfMode = v);
        }

        private static void ConfigOneDropDown<T>(Dropdown dropdownComp, Action<T> onSelected) where T : struct
        {
            string[] enumNames = typeof(T).GetEnumNames();
            var colorModeOptions = enumNames.Select(s => new Dropdown.OptionData { text = s }).ToList();
            dropdownComp.options = colorModeOptions;
            dropdownComp.onValueChanged.AddListener(i =>
            {
                string enumName = enumNames[i];
                T enumValue = default;
                if (Enum.TryParse<T>(enumName, out enumValue))
                {
                    onSelected(enumValue);
                }
            });
        }

        private static void ResetDropDown<T>(Dropdown dropdownComp, T initialValue) where T : struct
        {
            string[] enumNames = typeof(T).GetEnumNames();
            dropdownComp.value = Array.FindIndex(enumNames, s => s == typeof(T).GetEnumName(initialValue));
        }

        public void ApplyRecordedTileCostModifications()
        {
            UISquareTileCtrl.Instance.ApplyRecordedTileCostModifications();
        }

        public void DiscardRecordedTileCostModifications()
        {
            UISquareTileCtrl.Instance.DiscardRecordedTileCostModifications();
        }
    }
}
