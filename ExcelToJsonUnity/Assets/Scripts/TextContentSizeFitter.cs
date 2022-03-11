using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class TextContentSizeFitter : MonoBehaviour
    {
        public Text m_targetText;

        public bool fixWidth = true;
        public bool fixHeight = true;

        private string m_old;

        void Update()
        {
            if (null == m_targetText) return;

            if (m_old != m_targetText.text)
            {
                m_old = m_targetText.text;
                UpdateSize();
            }
        }

        public void UpdateSize()
        {
            if (null == m_targetText) return;

            var rtf = transform as RectTransform;
            var old = rtf.sizeDelta;
            if(fixWidth)
            {
                old.x = m_targetText.preferredWidth;
            }

            if(fixHeight)
            {
                old.y = m_targetText.preferredHeight;
            }
            rtf.sizeDelta = old;
        }
    }
}