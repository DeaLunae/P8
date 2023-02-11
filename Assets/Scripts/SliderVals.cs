// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/ShowSliderValue")]
    
    public class SliderVals : ShowSliderValue
    {
        private TextMeshPro _tmpHolder = null;
        [SerializeField] private Vector2 minMaxValues;
        
        
        public new void OnSliderUpdated(SliderEventData eventData)
        {
            eventData.Slider.SliderStepDivisions = Mathf.RoundToInt(math.ceil(minMaxValues.y-minMaxValues.x));
            if (_tmpHolder == null)
            {
                _tmpHolder = GetComponent<TextMeshPro>();
            }

            if (_tmpHolder != null)
            {
                var val = math.remap(0f, 1f, minMaxValues.x, minMaxValues.y, eventData.NewValue);
                _tmpHolder.text = $"{Mathf.RoundToInt(val)}";
            }
        }
    }
}
