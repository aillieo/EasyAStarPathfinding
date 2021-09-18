using AillieoUtils.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileSettingsView : MonoBehaviour
    {
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

        private void ConfigDropdowns()
        {
            ConfigOneDropDown<UISquareTileCtrl.ColorMode>(tileColorMode, v => UISquareTileCtrl.Instance.colorMode = v, UISquareTileCtrl.Instance.colorMode);
            ConfigOneDropDown<UISquareTileCtrl.TextMode>(tileTextMode, v => UISquareTileCtrl.Instance.textMode = v, UISquareTileCtrl.Instance.textMode);
            ConfigOneDropDown<UISquareTileCtrl.OperationMode>(tileOpMode, v => UISquareTileCtrl.Instance.opMode = v, UISquareTileCtrl.Instance.opMode);
            ConfigOneDropDown<UISquareTileCtrl.CostModificationMode>(tileCstMdfMode, v => UISquareTileCtrl.Instance.costMdfMode = v, UISquareTileCtrl.Instance.costMdfMode);
        }

        private static void ConfigOneDropDown<T>(Dropdown dropdownComp, Action<T> onSelected, T initialValue) where T : struct
        {
            string[] colorModes = typeof(T).GetEnumNames();
            var colorModeOptions = colorModes.Select(s => new Dropdown.OptionData { text = s }).ToList();
            dropdownComp.options = colorModeOptions;
            dropdownComp.onValueChanged.AddListener(i =>
            {
                string enumName = colorModes[i];
                T enumValue = default;
                if (Enum.TryParse<T>(enumName, out enumValue))
                {
                    onSelected(enumValue);
                }
            });
            dropdownComp.value = Array.FindIndex(colorModes, s => s == typeof(T).GetEnumName(initialValue));
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
