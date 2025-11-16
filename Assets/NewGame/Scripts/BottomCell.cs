using System;
using DG.Tweening;
using UnityEngine;

namespace NewGame
{
    [Serializable]
    public class BottomCell : MonoBehaviour
    {
        public Item item;

        public void Assign(Item item, bool anim = true)
        {
            Debug.Log("Assign called");

            this.item = item;
            if (item != null)
            {
                item.SetBottomCell(this);
                
                if (anim)
                {
                    // tween animation 
                    item.transform.DOMove(transform.position, 0.25f);
                }
                else
                {
                    item.transform.position = transform.position;
                }
            }
        }
    }
}
