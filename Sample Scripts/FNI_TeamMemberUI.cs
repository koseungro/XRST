/// 작성자: 고승로
/// 작성일: 2021-08-18
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    /// <summary>
    /// 팀원에게 하달된 오더 표시용 UI 관리 클래스
    /// </summary>
    public class FNI_TeamMemberUI : FNI_PopupBase
    {
        public string Name => gameObject.name;

        protected override Transform Parent
        {
            get
            {
                if (parent == null)
                    parent = transform.Find("Parent");

                return parent;
            }
        }

        public override TextMeshProUGUI Contents
        {
            get
            {
                if (contents == null)
                    contents = Parent.Find("Text Order").GetComponent<TextMeshProUGUI>();

                return contents;
            }
        }

        public Button OrderRefuse_Button
        {
            get
            {
                if (orderRefuse_Button == null)
                    orderRefuse_Button = Parent.Find("Buttons/Order Refuse Button").GetComponent<Button>();

                return orderRefuse_Button;
            }
        }

        public Button OrderConfirm_Button
        {
            get
            {
                if (orderConfirm_Button == null)
                    orderConfirm_Button = Parent.Find("Buttons/Order Confirm Button").GetComponent<Button>();

                return orderConfirm_Button;
            }
        }
        public GameObject AutoConfirm_Text
        {
            get
            {
                if (autoConfirm_Text == null)
                    autoConfirm_Text = Parent.Find("Buttons/Auto Confirm Text").gameObject;

                return autoConfirm_Text;
            }
        }


        private Button orderRefuse_Button;
        private Button orderConfirm_Button;
        private GameObject autoConfirm_Text;

        public MissionOrder order;
        public XRST_Mission mission;

        public Slider gage;
        public float autoHideTime = 3;

        private void Start()
        {
            OrderConfirm_Button.onClick.AddListener(Order_Confirm);
            OrderRefuse_Button.onClick.AddListener(Order_Refuse);
        }

        public void Receive_Order(PlayerBaseInfo player, MissionOrder order)
        {
            this.order = order;
            Contents.text = order.orderText;

            Show();

            OrderConfirm_Button.gameObject.SetActive(order.mainCategory != MissionMainCategory.준비);
            OrderRefuse_Button.gameObject.SetActive(order.mainCategory != MissionMainCategory.준비);
            AutoConfirm_Text.SetActive(order.mainCategory == MissionMainCategory.준비);

            if (order.mainCategory == MissionMainCategory.준비)
            {
                StartCoroutine(AutoOrderConfirm());
            }
        }

        private IEnumerator AutoOrderConfirm()
        {
            Debug.Log($"[FNI_TeamMemberUI/AutoOrderConfirm] Auto Confirm => {order.mainCategory}, {order.id}");

            mission.Send_OrderSelect(MissionOrderFeedbackType.OK, order);

            float cTime = 0;
            gage.value = 0;

            while (cTime < autoHideTime)
            {
                cTime += Time.deltaTime;

                gage.value = cTime / autoHideTime;

                yield return null;
            }

            gage.value = 1;

            Hide();
        }

        /// <summary>
        /// 오더 거부
        /// </summary>
        private void Order_Refuse()
        {
            mission.Send_OrderSelect(MissionOrderFeedbackType.Cancel, order);

            Debug.Log($"[FNI_TeamMemberUI/Order_Refuse] {order.id} Order rejected.");
            // Order 초기화
            order = null;
            Hide();

            //if (XRST_Status.IsLeader)
            //    FNI_TeamUIManager.Instance.Show();
        }
        /// <summary>
        /// 오더 수락
        /// </summary>
        private void Order_Confirm()
        {
            mission.Send_OrderSelect(MissionOrderFeedbackType.OK, order);

            Debug.Log($"[FNI_TeamMemberUI/Order_Confirm] {order.id} Order Accept.");

            Hide();
        }
        /// <summary>
        /// 오더 수락전 회수
        /// </summary>
        public void Order_Recall()
        {
            mission.Send_OrderSelect(MissionOrderFeedbackType.Recall, order);

            Debug.Log($"[FNI_TeamMemberUI/Order_Recall] {order.id} Order Recall.");
            order = null;
            Hide();

            //if (XRST_Status.IsLeader)
            //    FNI_TeamUIManager.Instance.Show();
        }
        public override void Show()
        {
            base.Show();
            Group.alpha = 1;
            IS_GazeGuidedUI.Instance.SetActive(transform, "수신된 명령 확인");
        }
        public override void Hide()
        {
            base.Hide();

            IS_GazeGuidedUI.Instance.SetActive();
        }
    }
}