using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace MRKTExtensions.Ux
{
    public class ButtonStatusController : MonoBehaviour
    {
        private TextMeshPro textMeshPro;
        private Color textOriginalColor;
        private List<Color> iconOriginalColors;
        private List<Renderer> iconRenderers;
        private List<MonoBehaviour> buttonBehaviours;
        private Transform buttonHighLightComponent;
        private bool isInitialized = false;

        private void Awake()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                // Components
                var iconParent = transform.Find("IconAndText");
                textMeshPro = iconParent.GetComponentInChildren<TextMeshPro>();
                iconRenderers = new List<Renderer>();
                iconRenderers.Add(iconParent.Find("UIButtonSquareIcon").gameObject.GetComponent<Renderer>());
                if (iconParent.Find("UIButtonToggleIconOff") != null)
                {
                    iconRenderers.Add(iconParent.Find("UIButtonToggleIconOff").gameObject.GetComponent<Renderer>());
                }

                buttonHighLightComponent = transform.Find("CompressableButtonVisuals");
                buttonBehaviours = GetComponents<MonoBehaviour>().ToList();
                // Material Changes
                textOriginalColor = textMeshPro.color;
                iconOriginalColors = new List<Color>();
                iconOriginalColors.AddRange(iconRenderers.Select(e => e.material.color));
            }
        }
        
        public void SetStatus(bool active)
        {
            foreach (var b in buttonBehaviours.Where( p => p!= this))
            {
                b.enabled = active;
            }
            buttonHighLightComponent.gameObject.SetActive(active);
            textMeshPro.color = active ? textOriginalColor : Color.gray;
            for (var i = 0; i < iconRenderers.Count; i++)
            {
                iconRenderers[i].material.color = active ? iconOriginalColors[i] : Color.gray;
            }
        }

        public void SetStatusAll(bool active)
        {
            foreach (var buttonStatusController in FindObjectsOfType<ButtonStatusController>())
            {
                if (buttonStatusController == this)
                {
                    continue;
                }
                buttonStatusController.SetStatus(active);
            }
        }
        public void SetStatusAllExceptFlip(bool active)
        {
            foreach (var buttonStatusController in FindObjectsOfType<ButtonStatusController>())
            {
                if (buttonStatusController == this || buttonStatusController.gameObject.name == "Drej tegnet")
                {
                    continue;
                }
                buttonStatusController.SetStatus(active);
            }
        }
    }
}
