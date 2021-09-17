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
            ConfigOneDropDown<UISquareTileCtrl.ColorMode>(tileColorMode, v => UISquareTileCtrl.Instance.colorMode = v);
            ConfigOneDropDown<UISquareTileCtrl.TextMode>(tileTextMode, v => UISquareTileCtrl.Instance.textMode = v);
            ConfigOneDropDown<UISquareTileCtrl.OperationMode>(tileOpMode, v => UISquareTileCtrl.Instance.opMode = v);
            ConfigOneDropDown<UISquareTileCtrl.CostModificationMode>(tileCstMdfMode, v => UISquareTileCtrl.Instance.costMdfMode = v);
        }

        private static void ConfigOneDropDown<T>(Dropdown dropdownComp, Action<T> onSelected) where T : struct
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
            onSelected((T)Enum.Parse(typeof(T), colorModes[0]));
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
