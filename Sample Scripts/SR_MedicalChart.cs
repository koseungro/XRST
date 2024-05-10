/// 작성자: 고승로
/// 작성일: 2021-07-07
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.Shutdown;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    /// <summary>
    /// 사용자가 보는 Hololens 상의 Medical Chart UI 데이터 세팅 클래스 
    /// </summary>
    public class SR_MedicalChart : MonoBehaviour
    {
        [Serializable]
        public class ChartDetail
        {
            public GameObject title;
            public GameObject contents;

            public void SetActive(bool isActive)
            {
                title.SetActive(isActive);
                contents.SetActive(isActive);
            }
        }
        public string Name => gameObject.name;

        public ViewChartType viewState = ViewChartType.None;

        [Header("[Data]")]
        public XRST_MedicalChart data = new XRST_MedicalChart();

        [Header("[Panels]")]
        public ChartDetail medicalChart;
        public ChartDetail history;
        public TextMeshProUGUI callingText;
        public ChartDetail patient;

        public List<GameObject> space = new List<GameObject>();

        [Header("[Mission]")]
        public XRST_Mission mission;

        [Header("[List]")]
        public List<SR_Text> textList = new List<SR_Text>();
        public List<SR_Toggle> toggleList = new List<SR_Toggle>();
        public List<SR_VitalSign> vitalSignList = new List<SR_VitalSign>();

        public Toggle medicalChartToggle;
        public GameObject[] toggleSubObjects;

        /// <summary>
        /// 서버에서 클라이언트로 차트의 Data를 불러옵니다.
        /// </summary>
        public void RoadChartData(byte[] _data)
        {
            data.Set(_data, 3);

            medicalChartToggle.gameObject.SetActive(true);
            medicalChartToggle.SetIsOnWithoutNotify(true);

            //mission.Send_MedicalChartHide();

            /// 불러온 차트 Data를 UI에 표시
            Set_CallingHistoryText();
            if (data.viewChartType == ViewChartType.Multiple)
            {
                Set_TextData();
                Set_ToggleData();
                Set_VitalSignData();
            }

            Show();

            IS_GazeGuidedUI.Instance.SetActive(transform, "메디컬 차트 확인");
        }

        public void ToggleActive(bool isActive)
        {
            if (isActive)
                Show();
            else
                Hide();
        }
        public void Show()
        {
            Set_ChartType(data.viewChartType);
        }

        public void Hide()
        {
            Set_ChartType(ViewChartType.None);

            IS_GazeGuidedUI.Instance.SetActive();
        }

        private void SetSubObjectActive(bool isActive)
        {
            for (int cnt = 0; cnt < toggleSubObjects.Length; cnt++)
            {
                toggleSubObjects[cnt].SetActive(isActive);
            }
        }

        /// <summary>
        /// 차트의 표현 타입을 설정합니다.
        /// </summary>
        public void Set_ChartType(ViewChartType chartType)
        {
            viewState = chartType;

            SetSubObjectActive(medicalChartToggle.isOn);

            medicalChart.SetActive(chartType == ViewChartType.Multiple);
            history.SetActive(chartType != ViewChartType.None);
            patient.SetActive(chartType == ViewChartType.Multiple);

            for (int cnt = 0; cnt < space.Count; cnt++)
            {
                space[cnt].SetActive(chartType != ViewChartType.None);
            }
        }

        #region MedicalChart Data

        /// <summary>
        /// 차트의 TextData를 입력합니다.
        /// </summary>
        public void Set_TextData()
        {
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i].SetText(data.patientDatas[i]);
            }
        }

        /// <summary>
        /// 차트의 ToggleData를 입력합니다.
        /// </summary>
        public void Set_ToggleData()
        {
            for (int cnt = 0; cnt < toggleList.Count; cnt++)
            {
                // Group Toggle일 경우
                if (toggleList[cnt] is SR_ToggleGroup)
                {
                    SR_ToggleGroup toggleGroup = (SR_ToggleGroup)toggleList[cnt];
                    switch (toggleGroup.id.type)
                    {
                        case ChartData_Type.환자증상_통증:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.환자증상_통증, (int)data.patientSymptoms_Pain);
                            break;
                        case ChartData_Type.환자증상_외상:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.환자증상_외상, (int)data.patientSymptoms_Trauma);
                            break;
                        case ChartData_Type.기도확보:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.기도확보, (int)data.airway);
                            break;
                        case ChartData_Type.산소투여:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.산소투여, (int)data.oxygenAdministration);
                            break;
                        case ChartData_Type.CPR:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.CPR, (int)data.cpr);
                            break;
                        case ChartData_Type.AED:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.AED, (int)data.aed);
                            break;
                        case ChartData_Type.순환보조:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.순환보조, (int)data.circulatoryAssist);
                            break;
                        case ChartData_Type.상처처치:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.상처처치, (int)data.woundTreatment);
                            break;
                        case ChartData_Type.고정:
                            toggleGroup.SwitchToggle_Generic(ChartData_Type.고정, (int)data.fixing);
                            break;
                        default:
                            break;
                    }
                }

                // Toggle에 추가 값 설정이 필요한 경우
                else if (toggleList[cnt] is SR_ToggleValue)
                {
                    bool toggleCheck;
                    SR_ToggleValue toggleValue = (SR_ToggleValue)toggleList[cnt];

                    switch (toggleValue.id.type)
                    {
                        case ChartData_Type.병력:
                            if ((int)data.medicalHistory == toggleValue.id.code)
                            {
                                toggleValue.SwitchToggle(true);
                                toggleValue.SetToggleValue(ChartData_Type.병력, value: data.medicalHistory_Record);
                            }                            
                            break;
                        case ChartData_Type.환자증상:
                            toggleCheck = (data.patientSymptoms & (PatientSymptomsType)toggleValue.id.code) != 0;
                            // Enum이 체크되어 있는 경우
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                toggleValue.SetToggleValue(ChartData_Type.환자증상, value: data.patientSymptomsETC);
                            }
                            break;
                        case ChartData_Type.교통사고:
                            toggleCheck = (data.trafficAccident & (TrafficAccidentType)toggleValue.id.code) != 0;
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                toggleValue.SetToggleValue(ChartData_Type.교통사고, value: data.otherVehicle);
                            }
                            break;
                        case ChartData_Type.환자분류: // 그냥 Enum
                            if((int)data.triage == toggleValue.id.code)
                            {
                                toggleValue.SwitchToggle(true);
                                toggleValue.SetToggleValue(ChartData_Type.환자분류, value: data.triageAdulation);
                            }
                            break;
                        case ChartData_Type.산소투여:
                            toggleCheck = (data.oxygenAdministration & (FirstAid_OxygenAdministrationType)toggleValue.id.code) != 0;
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                toggleValue.SetToggleValue(ChartData_Type.산소투여, value: data.oxygenValue.ToString());
                            }
                            break;
                        case ChartData_Type.AED:
                            toggleCheck = (data.aed & (FirstAid_AEDType)toggleValue.id.code) != 0;
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                toggleValue.SetToggleValue(ChartData_Type.AED, value: data.shockCount.ToString());
                            }
                            break;
                        case ChartData_Type.순환보조:
                            toggleCheck = (data.circulatoryAssist & (FirstAid_CirculatoryAssistType)toggleValue.id.code) != 0;
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                switch (toggleValue.id.code)
                                {
                                    case 2:
                                        toggleValue.SetToggleValue(ChartData_Type.순환보조, value: data.salineValue);
                                        break;
                                    case 4:
                                        toggleValue.SetToggleValue(ChartData_Type.순환보조, value: data.drugInjection);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case ChartData_Type.상처처치: // 온 or 냉
                            toggleCheck = (data.woundTreatment & (FirstAid_WoundTreatmentType)toggleValue.id.code) != 0;
                            if (toggleCheck)
                            {
                                toggleValue.SwitchToggle(toggleCheck);
                                toggleValue.SetToggleValue(ChartData_Type.상처처치, (int)data.keepWarmType);
                            }
                            break;
                        //case ChartData_Type.고정:
                        //    toggleCheck = (data.fixing & (FirstAid_FixingType)toggleValue.id.code) != 0;
                        //    if (toggleCheck)
                        //    {
                        //        toggleValue.SwitchToggle(toggleCheck);
                        //        toggleValue.SetToggleValue(ChartData_Type.고정, value: data.splint);
                        //    }
                            //break;
                        default:
                            break;
                    }
                }

                // 범주가 없는 Toggle일 경우
                else
                {
                    switch (toggleList[cnt].id.type)
                    {
                        case ChartData_Type.병력:
                            toggleList[cnt].SwitchToggle((int)data.medicalHistory == toggleList[cnt].id.code);
                            break;
                        case ChartData_Type.환자증상:
                            toggleList[cnt].SwitchToggle((data.patientSymptoms & (PatientSymptomsType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.교통사고:
                            toggleList[cnt].SwitchToggle((data.trafficAccident & (TrafficAccidentType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.그외외상:
                            toggleList[cnt].SwitchToggle((data.otherTrauma & (OtherTraumaType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.의식상태:
                            toggleList[cnt].SwitchToggle((data.consciousnessType & (StatesOfConsciousnessType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.동공반응_L:
                            toggleList[cnt].SwitchToggle((data.pupillaryReaction_L & (PupillaryReactionType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.동공반응_R:
                            toggleList[cnt].SwitchToggle((data.pupillaryReaction_R & (PupillaryReactionType)toggleList[cnt].id.code) != 0);
                            break;
                        case ChartData_Type.환자분류:
                            toggleList[cnt].SwitchToggle((int)data.triage == toggleList[cnt].id.code);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 차트의 VitalSignData를 입력합니다.
        /// </summary>
        public void Set_VitalSignData()
        {
            for (int i = 0; i < vitalSignList.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        if (data.vitalSign_1.isUse)
                            vitalSignList[i].Set_VitalSign(data.vitalSign_1); 
                        break;
                    case 1:
                        if (data.vitalSign_2.isUse)
                            vitalSignList[i].Set_VitalSign(data.vitalSign_2);                       
                        break;
                }
            }
        }

        /// <summary>
        /// 출동정보 Data를 입력합니다.
        /// </summary>
        public void Set_CallingHistoryText()
        {
            callingText.text = data.HistoryOfPresentIllness;
        }
        #endregion
    }
}