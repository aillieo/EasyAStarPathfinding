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

        private void Awake()
        {
            ConfigDropdowns();
        }

        private void ConfigDropdowns()
        {
            string[] colorModes = typeof(UISquareTileCtrl.ColorMode).GetEnumNames();
            var colorModeOptions = colorModes.Select(s => new Dropdown.OptionData { text = s }).ToList();
            tileColorMode.options = colorModeOptions;
            tileColorMode.onValueChanged.AddListener(i =>
            {
                string enumName = colorModes[i];
                Enum.TryParse<UISquareTileCtrl.ColorMode>(enumName, out UISquareTileCtrl.Instance.colorMode);
            });

            string[] textModes = typeof(UISquareTileCtrl.TextMode).GetEnumNames();
            var textModeOptions = textModes.Select(s => new Dropdown.OptionData { text = s }).ToList();
            tileTextMode.options = textModeOptions;
            tileTextMode.onValueChanged.AddListener(i =>
            {
                string enumName = textModes[i];
                Enum.TryParse<UISquareTileCtrl.TextMode>(enumName, out UISquareTileCtrl.Instance.textMode);
            });
        }
    }
}
