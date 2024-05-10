/// 작성자: 고승로
/// 작성일: 2021-06-23
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
using FNI.ENUM;

#if UNITY_EDITOR
using UnityEditor;
using FNI.XRST.EDITOR;
#endif

namespace FNI
{
    /// <summary>
    /// 환자 메디컬 정보 커스텀 에디터 편집(입력) 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "new Medical Chart Data", menuName = "FNI/Medical Chart Data")]
    public class XRST_MedicalChartData : ScriptableObject
    {
        public DataDivision division = new DataDivision();

        public XRST_MedicalChart data;

        public PainLevel painLevel;

        public bool isFold_EMCInfo;
        public bool isFold_MedicalHistory;
        public bool isFold_PainInfo;
        public bool isFold_ETCPainInfo;
        public bool isFold_History;
        public bool isFold_Evaluation;
        public bool isFold_VitalSign;
        public bool isFold_FirstAid;
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(XRST_MedicalChartData))]
    public class XRST_MedicalChartDataEditor : XRST_DataEditor
    {
        private XRST_MedicalChartData Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as XRST_MedicalChartData;

                return m_target;
            }
        }
        private XRST_MedicalChartData m_target;


        private readonly string[] names = new string[]
        {
            "신고일시",
            "출동시간",
            "현장 도착",
            "거리",
            "현장에서 출발한 시간",
            "병원도착 시간",
            "귀소 시간",
            "사고위치",
            "환자 이름",
            "환자 성별",
            "환자 나이"
        };
        private void OnEnable()
        {
            if (Target.data.viewChartType == ViewChartType.Single)
            {
                Target.isFold_EMCInfo = false;
                Target.isFold_PainInfo = false;
                Target.isFold_ETCPainInfo = false;
                Target.isFold_Evaluation = false;
                Target.isFold_FirstAid = false;
            }
        }

        
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("메디컬 차트 종류 구분");

                EditorGUILayout.Space();
                Target.painLevel = SetEnumFeild("Pain Level", Target.painLevel);
                EditorGUILayout.Space();

                if (GUILayout.Button("Get Division"))
                {
                    string[] names = Target.name.Split('_');
                    Target.division.sequence = (SequenceCode)Enum.Parse(typeof(SequenceCode), names[0]);
                    Target.division.scenario = (ScenarioCode)Enum.Parse(typeof(ScenarioCode), names[1]);
                    Target.division.type = names[2] == "M" ? CharactorType.S_Male : CharactorType.S_Female;
                    Target.division.injuredSpot = (InjuredSpotCode)Enum.Parse(typeof(InjuredSpotCode), names[3]);
                    Target.division.accident = (AccidentCode)Enum.Parse(typeof(AccidentCode), names[4]);

                    if (Target.division.scenario == ScenarioCode.AC)
                        Target.data.viewChartType = ViewChartType.Single;
                    else
                        Target.data.viewChartType = ViewChartType.Multiple;
                }

                float splitWidth = (EditorGUIUtility.currentViewWidth - 50) * 0.2f;
                EditorGUILayout.BeginHorizontal();
                {
                    Target.division.sequence = (SequenceCode)EditorGUILayout.EnumPopup(Target.division.sequence, GUILayout.Width(splitWidth));
                    Target.division.scenario = (ScenarioCode)EditorGUILayout.EnumPopup(Target.division.scenario, GUILayout.Width(splitWidth));
                    Target.division.type = (CharactorType)EditorGUILayout.EnumPopup(Target.division.type, GUILayout.Width(splitWidth));
                    Target.division.injuredSpot = (InjuredSpotCode)EditorGUILayout.EnumPopup(Target.division.injuredSpot, GUILayout.Width(splitWidth));
                    Target.division.accident = (AccidentCode)EditorGUILayout.EnumPopup(Target.division.accident, GUILayout.Width(splitWidth));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.TextField(EducationalCode.ToString(Target.division.sequence), GUILayout.Width(splitWidth));
                    EditorGUILayout.TextField(EducationalCode.ToString(Target.division.scenario), GUILayout.Width(splitWidth));
                    EditorGUILayout.TextField(EducationalCode.ToString(Target.division.type), GUILayout.Width(splitWidth));
                    EditorGUILayout.TextField(EducationalCode.ToString(Target.division.injuredSpot), GUILayout.Width(splitWidth));
                    EditorGUILayout.TextField(EducationalCode.ToString(Target.division.accident), GUILayout.Width(splitWidth));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("차트 종류", DefWidthLayout);
            Target.data.viewChartType = (ViewChartType)EditorGUILayout.EnumPopup(Target.data.viewChartType);
            EditorGUILayout.EndHorizontal();

            if (Target.data.patientDatas.Count != 11)
            {
                Target.data.patientDatas.Clear();
                Target.data.patientDatas.AddRange(new string[11]);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            Target.isFold_EMCInfo = EditorGUILayout.Foldout(Target.isFold_EMCInfo, "구급 출동 정보", true);
            EditorGUI.indentLevel--;

            if (Target.isFold_EMCInfo)
            {
                for (int cnt = 0; cnt < Target.data.patientDatas.Count; cnt++)
                {
                    if (cnt == 0)
                    {
                        //EditorGUILayout.LabelField("구급 출동", Label_BoldMode);
                    }
                    else if (cnt == 8)
                    {
                        Line(Color.black, 2);
                    }

                    Target.data.patientDatas[cnt] = SetTextFeild(names[cnt], Target.data.patientDatas[cnt], 130);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            Target.isFold_MedicalHistory = EditorGUILayout.Foldout(Target.isFold_MedicalHistory, "환자 병력", true);
            EditorGUI.indentLevel--;

            if (Target.isFold_MedicalHistory)
            {
                Target.data.medicalHistory = SetFlagFeild("병력", Target.data.medicalHistory);

                if((Target.data.medicalHistory & MedicalHistory.있음) == MedicalHistory.있음)
                {
                    EditorGUI.indentLevel++;
                    Target.data.medicalHistory_Record = SetTextFeild(" ", Target.data.medicalHistory_Record);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                Target.isFold_PainInfo = EditorGUILayout.Foldout(Target.isFold_PainInfo, "환자 증상", true);
                EditorGUI.indentLevel--;
                if (Target.isFold_PainInfo)
                {
                    Target.data.patientSymptoms_Pain = SetFlagFeild("통증", Target.data.patientSymptoms_Pain);
                    Target.data.patientSymptoms_Trauma = SetFlagFeild("외상", Target.data.patientSymptoms_Trauma);
                    Target.data.patientSymptoms = SetFlagFeild("기타", Target.data.patientSymptoms);
                    if ((Target.data.patientSymptoms & PatientSymptomsType.기타) == PatientSymptomsType.기타)
                    {
                        EditorGUI.indentLevel++;
                        Target.data.patientSymptomsETC = SetTextFeild(" ", Target.data.patientSymptomsETC);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            Target.isFold_ETCPainInfo = EditorGUILayout.Foldout(Target.isFold_ETCPainInfo, "질병 외", true);
            EditorGUI.indentLevel--;
            if (Target.isFold_ETCPainInfo)
            {
                Target.data.trafficAccident = SetFlagFeild("교통사고/사상자", Target.data.trafficAccident);
                Target.data.otherTrauma = SetFlagFeild("그 외 외상", Target.data.otherTrauma);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            Target.isFold_History = EditorGUILayout.Foldout(Target.isFold_History, "질병 기록", true);
            EditorGUI.indentLevel--;
            if (Target.isFold_History)
                Target.data.HistoryOfPresentIllness = SetTextArea("신고전화내용", Target.data.HistoryOfPresentIllness);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                Target.isFold_Evaluation = EditorGUILayout.Foldout(Target.isFold_Evaluation, "환자 평가", true);
                EditorGUI.indentLevel--;

                if (Target.isFold_Evaluation)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("의식상태 분류", Label_BoldMode);
                    Target.data.consciousnessType = SetFlagFeild("1차", Target.data.consciousnessType);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("동공반응", Label_BoldMode);
                    Target.data.pupillaryReaction_L = SetFlagFeild("왼쪽", Target.data.pupillaryReaction_L);
                    Target.data.pupillaryReaction_R = SetFlagFeild("오른쪽", Target.data.pupillaryReaction_R);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    Target.isFold_VitalSign = EditorGUILayout.Foldout(Target.isFold_VitalSign, "활력징후", true);
                    EditorGUI.indentLevel--;
                    if (Target.isFold_VitalSign)
                    {
                        Target.data.vitalSign_1 = VitalSign("Vital Sign 1차", Target.data.vitalSign_1);
                        Target.data.vitalSign_2 = VitalSign("Vital Sign 2차", Target.data.vitalSign_2);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("환자 분류", Label_BoldMode);
            Target.data.triage = SetEnumFeild("환자 분류", Target.data.triage);
            if (Target.data.triage == TriageType.사망)
                Target.data.triageAdulation = SetTextFeild("사망원인추정", Target.data.triageAdulation);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            Target.isFold_FirstAid = EditorGUILayout.Foldout(Target.isFold_FirstAid, "응급처치", true);
            EditorGUI.indentLevel--;
            if (Target.isFold_FirstAid)
            {
                Target.data.airway = SetFlagFeild("기도확보", Target.data.airway);
                Target.data.oxygenAdministration = SetFlagFeild("산소투여", Target.data.oxygenAdministration);
                if ((Target.data.oxygenAdministration & FirstAid_OxygenAdministrationType.value) == FirstAid_OxygenAdministrationType.value)
                {
                    EditorGUI.indentLevel++;
                    Target.data.oxygenValue = SetFloatFeild("산소투여량", Target.data.oxygenValue);
                    EditorGUI.indentLevel--;
                }
                Target.data.cpr = SetFlagFeild("CPR", Target.data.cpr);
                Target.data.aed = SetFlagFeild("AED", Target.data.aed);
                if ((Target.data.aed & FirstAid_AEDType.Shock) == FirstAid_AEDType.Shock)
                {
                    EditorGUI.indentLevel++;
                    Target.data.shockCount = SetIntFeild("Shock 횟수", Target.data.shockCount);
                    EditorGUI.indentLevel--;
                }
                Target.data.circulatoryAssist = SetFlagFeild("순환보조", Target.data.circulatoryAssist);
                if ((Target.data.circulatoryAssist & FirstAid_CirculatoryAssistType.수액공급) == FirstAid_CirculatoryAssistType.수액공급)
                {
                    EditorGUI.indentLevel++;
                    Target.data.salineValue = SetTextFeild("수액공급량", Target.data.salineValue);
                    EditorGUI.indentLevel--;
                }
                if ((Target.data.circulatoryAssist & FirstAid_CirculatoryAssistType.약물투여) == FirstAid_CirculatoryAssistType.약물투여)
                {
                    EditorGUI.indentLevel++;
                    Target.data.drugInjection = SetTextFeild("약물투여 내용", Target.data.drugInjection);
                    EditorGUI.indentLevel--;
                }
                Target.data.woundTreatment = SetFlagFeild("상처처치", Target.data.woundTreatment);
                if ((Target.data.woundTreatment & FirstAid_WoundTreatmentType.보온) == FirstAid_WoundTreatmentType.보온)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(20));
                    Target.data.keepWarmType = SetEnumFeild("보온 종류", Target.data.keepWarmType);
                    EditorGUILayout.EndHorizontal();
                }
                Target.data.fixing = SetFlagFeild("고정", Target.data.fixing);
            }
            EditorGUILayout.EndVertical();

            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                EditorUtility.SetDirty(Target);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private VitalSign VitalSign(string title, VitalSign _vitalSign)
        {
            VitalSign vitalSign = _vitalSign;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel, GUILayout.Width(110));
            EditorGUILayout.Space();
            if (vitalSign.isUse == false)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                    vitalSign.isUse = true;
            }
            else
            {
                if (GUILayout.Button("-", GUILayout.Width(20)))
                    vitalSign.isUse = false;
            }

            EditorGUILayout.EndHorizontal();
            if (vitalSign.isUse)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUIStyle gUI = new GUIStyle(EditorStyles.textField)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                    EditorGUILayout.LabelField("혈압(H/L BP)", GUILayout.Width(110));

                    vitalSign.HighBP = EditorGUILayout.IntField(vitalSign.HighBP, gUI);
                    EditorGUILayout.LabelField("/", GUILayout.Width(10));
                    vitalSign.LowBP = EditorGUILayout.IntField(vitalSign.LowBP, gUI);
                }
                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.Knob(Vector2.one * 64, Target.PR, 0, 200, "PR", Color.gray, Color.red, true);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("맥박(PR)", GUILayout.Width(110));
                    vitalSign.PR = EditorGUILayout.IntSlider(vitalSign.PR, 0, 200);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("호흡수(RR)", GUILayout.Width(110));
                    vitalSign.RR = EditorGUILayout.IntSlider(vitalSign.RR, 0, 60);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("체온(BT)", GUILayout.Width(110));
                    vitalSign.BT = EditorGUILayout.Slider(vitalSign.BT, 34, 42);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("산소포화도(SPO2)", GUILayout.Width(110));
                    vitalSign.SPO2 = EditorGUILayout.IntSlider(vitalSign.SPO2, 80, 100);
                }
                EditorGUILayout.EndHorizontal();
            }

            return vitalSign;
        }
    }
#endif
}