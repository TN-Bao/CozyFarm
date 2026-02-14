using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CozyFarm.UI
{
    [RequireComponent(typeof(ItemSelectionOutlineUI))]
    public class ItemControllerUI : MonoBehaviour
    {
        [SerializeField] private Image _itemImg;
        [SerializeField] private TextMeshProUGUI _quantityTxt;
        public ItemSelectionOutlineUI Outline { get; private set; }

        private void Awake() {
            Outline = GetComponent<ItemSelectionOutlineUI>();
        }

        public void ResetData()
        {
            _quantityTxt.enabled = false;
            _itemImg.enabled = false;
        }

        public void UpdateData(Sprite image, int quantity)
        {
            _quantityTxt.enabled = true;
            _quantityTxt.text = quantity.ToString();

            _itemImg.enabled = true;
            _itemImg.sprite = image;
        }
    }
}
