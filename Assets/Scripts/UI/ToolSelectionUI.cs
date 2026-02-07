using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CozyFarm.UI
{
    public class ToolSelectionUI : MonoBehaviour
    {
        [SerializeField] private Image _toolImage;
        [SerializeField] private Image _toolTipImage;
        [SerializeField] private List<Image> _toolImages;
        [SerializeField] private TextMeshProUGUI _countTxt;
        [SerializeField] private float _alphaOfEmptyImage = 0.04f, _alphaOfFilledImage = 0.5f;

        public void UpdateUI(int selectedImgIndex, List<Sprite> images, int? count)
        {
            ClearToolsList();
            Color fillImageColor = Color.white;
            fillImageColor.a = _alphaOfFilledImage;

            ToggleSwapTip(images.Count > 1);

            for (int i = 0; i < images.Count; i++)
            {
                if (i >= _toolImages.Count) break;
                if (i == selectedImgIndex)
                {
                    _toolImage.sprite = images[i];
                    if (count.HasValue)
                    {
                        _countTxt.gameObject.SetActive(true);
                        _countTxt.text = count.Value.ToString();
                    }
                    else
                    {
                        _countTxt.gameObject.SetActive(false);
                    }
                }

                if (_toolImages.Count > i && images[i] != null)
                {
                    _toolImages[i].sprite = images[i];
                    _toolImages[i].color = fillImageColor;
                }
            }
        }

        private void ToggleSwapTip(bool value)
        {
            _toolTipImage.gameObject.SetActive(value);
        }

        private void ClearToolsList()
        {
            Color c = Color.white;
            c.a = _alphaOfEmptyImage;

            foreach (var image in _toolImages)
            {
                image.sprite = null;
                image.color = c;
            }
        }
    }
}
