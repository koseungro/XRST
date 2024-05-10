/// 작성자: 고승로
/// 작성일: 2021-06-23
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.IS;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FNI.XRST;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    /// <summary>
    /// 환자 메디컬 정보 데이터 클래스
    /// </summary>
    [Serializable]
    public class XRST_MedicalChart
    {
        public ViewChartType viewChartType;

        [Header("구급 출동")]
        /// <summary>
        /// 구급 출동
        /// = 신고일시,
        /// = 출동시간,
        /// = 현장 도착,
        /// = 거리,
        /// = 현장에서 출발한 시간,
        /// = 병원도착 시간,
        /// = 귀소 시간,
        /// = 사고위치
        /// 환자 인적사항
        /// = 환자 이름
        /// = 환자 성별
        /// = 환자 나이
        /// </summary>
        public List<string> patientDatas = new List<string>();

        //public string reportDate;
        //public string departureTime;
        //public string arrivalTime;
        //public float distance;
        //public string departureToHospitalTime;
        //public string arrivalAtHospitalTime;
        //public string homecomingTime;
        //public string accidentLocation;
        //public string patientName;
        //public string patientGender;
        //public string patientAge;

        [Header("환자 증상")]

        /// <summary>
        /// 환자병력
        /// </summary>
        public MedicalHistory medicalHistory;
        /// <summary>
        /// 환자병력 기록
        /// </summary>
        public string medicalHistory_Record;
        /// <summary>
        /// 환자증상-통증
        /// </summary>
        public PatientSymptoms_PainType patientSymptoms_Pain;
        /// <summary>
        /// 환자증상-외상
        /// </summary>
        public PatientSymptoms_TraumaType patientSymptoms_Trauma;
        /// <summary>
        /// 환자증상
        /// </summary>
        public PatientSymptomsType patientSymptoms;
        /// <summary>
        /// 환자증상-기타
        /// </summary>
        public string patientSymptomsETC;

        [Header("질병 외")]
        /// <summary>
        /// 질병 외-교통사고/사상자
        /// </summary>
        public TrafficAccidentType trafficAccident;
        /// <summary>
        /// 교통사고-그 외 탈것
        /// </summary>
        public string otherVehicle;
        /// <summary>
        /// 질병 외-그 외 외상
        /// </summary>
        public OtherTraumaType otherTrauma;

        [Header("신고 전화 내용")]
        /// <summary>
        /// 질병기록, 신고전화내용
        /// </summary>
        public string HistoryOfPresentIllness;

        [Header("환자 평가")]
        [Header("의식상태")]
        /// <summary>
        /// 의식상태 분류
        /// </summary>
        public StatesOfConsciousnessType consciousnessType;

        [Header("동공반응")]
        /// <summary>
        /// 동공반응 왼쪽눈
        /// </summary>
        public PupillaryReactionType pupillaryReaction_L;
        /// <summary>
        /// 동공반응 오른쪽눈
        /// </summary>
        public PupillaryReactionType pupillaryReaction_R;
        [Header("활력징후")]
        /// <summary>
        /// 활력징후
        /// </summary>
        public VitalSign vitalSign_1;
        public VitalSign vitalSign_2;
        [Header("환자 분류")]
        /// <summary>
        /// 환자 분류
        /// </summary>
        public TriageType triage;
        /// <summary>
        /// 환자 분류상 사망시 추정 내용
        /// </summary>
        public string triageAdulation;
        [Header("응급처치")]
        [Header("기도확보")]
        /// <summary>
        /// 응급처치 - 기도확보
        /// </summary>
        public FirstAid_AirwayType airway;
        [Header("산소투여")]
        /// <summary>
        /// 응급처치 - 산소투여
        /// </summary>
        public FirstAid_OxygenAdministrationType oxygenAdministration;
        /// <summary>
        /// 산소투여량
        /// </summary>
        public float oxygenValue;
        [Header("CPR")]
        /// <summary>
        /// 응급처치 - CPR
        /// </summary>
        public FirstAid_CPRType cpr;
        [Header("AED")]
        /// <summary>
        /// 응급처치 - AED
        /// </summary>
        public FirstAid_AEDType aed;
        /// <summary>
        /// Shock 횟수
        /// </summary>
        public int shockCount;
        [Header("순환보조")]
        /// <summary>
        /// 응급처치 - 순환보조
        /// </summary>
        public FirstAid_CirculatoryAssistType circulatoryAssist;
        /// <summary>
        /// 수액공급량
        /// </summary>
        public string salineValue;
        /// <summary>
        /// 약물투여 내용
        /// </summary>
        public string drugInjection;
        [Header("상처처치")]
        /// <summary>
        /// 응급처치 - 상처처치
        /// </summary>
        public FirstAid_WoundTreatmentType woundTreatment;
        /// <summary>
        /// 보온 종류
        /// </summary>
        public KeepWarmType keepWarmType;
        [Header("고정")]
        /// <summary>
        /// 응급처치 - 고정
        /// </summary>
        public FirstAid_FixingType fixing;

        /// <summary>
        /// 메디컬 정보 데이터를 byte 값으로 저장
        /// </summary>
        /// <returns></returns>
        public byte[] Get()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)viewChartType);

            //신고 전화 내용
            data.AddRange(HistoryOfPresentIllness.ToByte());

            if (viewChartType == ViewChartType.Multiple)
            {
                data.AddRange(patientDatas.Count.ToByte());
                for (int cnt = 0; cnt < patientDatas.Count; cnt++)
                {
                    data.AddRange(patientDatas[cnt].ToByte());
                }

                //환자 증상
                data.Add((byte)patientSymptoms_Pain);
                data.Add((byte)patientSymptoms_Trauma);
                data.Add((byte)patientSymptoms);
                data.AddRange(patientSymptomsETC.ToByte());
                //질병 외
                data.Add((byte)trafficAccident);
                data.AddRange(otherVehicle.ToByte());
                data.Add((byte)otherTrauma);
                //환자 병력
                data.Add((byte)medicalHistory);
                data.AddRange(medicalHistory_Record.ToByte());

                //환자 평가 - 의식상태
                data.Add((byte)consciousnessType);

                //환자 평가 - 동공반응
                data.Add((byte)pupillaryReaction_L);
                data.Add((byte)pupillaryReaction_R);

                //환자 평가 - 활력징후
                data.AddRange(vitalSign_1.Get());
                data.AddRange(vitalSign_2.Get());

                //환자 평가 - 환자 분류
                data.Add((byte)triage);
                data.AddRange(triageAdulation.ToByte());
                //응급처치
                data.Add((byte)airway);
                data.Add((byte)oxygenAdministration);
                data.AddRange(oxygenValue.ToByte());
                data.Add((byte)cpr);
                data.Add((byte)aed);
                data.AddRange(shockCount.ToByte());
                data.Add((byte)circulatoryAssist);
                data.AddRange(salineValue.ToByte());
                data.AddRange(drugInjection.ToByte());
                data.Add((byte)woundTreatment);
                data.Add((byte)keepWarmType);
                data.Add((byte)fixing);
            }
            data.InsertRange(0, data.Count.ToByte());

            return data.ToArray();
        }

        /// <summary>
        /// 저장된 메디컬 차트 byte 데이터를 불러오기(UI 표시용)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_start"></param>
        public void Set(byte[] data, int _start)
        {
            int start = _start;

            _ = data.ByteToInt(out int count, start);
            start += count;

            viewChartType = (ViewChartType)data[start];
            start++;

            HistoryOfPresentIllness = data.ByteToString(out count, start);

            if (viewChartType == ViewChartType.Multiple)
            {
                start += count;
                int patientDatasCount = data.ByteToInt(out count, start);
                start += count;

                patientDatas.Clear();
                for (int cnt = 0; cnt < patientDatasCount; cnt++)
                {
                    patientDatas.Add(data.ByteToString(out count, start));
                    start += count;
                }

                patientSymptoms_Pain = (PatientSymptoms_PainType)data[start];
                start++;
                patientSymptoms_Trauma = (PatientSymptoms_TraumaType)data[start];
                start++;
                patientSymptoms = (PatientSymptomsType)data[start];
                start++;
                patientSymptomsETC = data.ByteToString(out count, start);
                start += count;
                trafficAccident = (TrafficAccidentType)data[start];
                start++;
                otherVehicle = data.ByteToString(out count, start);
                start += count;
                otherTrauma = (OtherTraumaType)data[start];
                start++;
                medicalHistory = (MedicalHistory)data[start];
                start++;
                medicalHistory_Record = data.ByteToString(out count, start);
                start += count;

                consciousnessType = (StatesOfConsciousnessType)data[start];
                start++;
                pupillaryReaction_L = (PupillaryReactionType)data[start];
                start++;
                pupillaryReaction_R = (PupillaryReactionType)data[start];
                start++;

                if (vitalSign_1 == null) vitalSign_1 = new VitalSign();
                if (vitalSign_2 == null) vitalSign_2 = new VitalSign();

                int length = vitalSign_1.Set(data, start);
                start += length;
                length = vitalSign_2.Set(data, start);
                start += length;


                triage = (TriageType)data[start];
                start++;
                triageAdulation = data.ByteToString(out count, start);
                start += count;
                airway = (FirstAid_AirwayType)data[start];
                start++;
                oxygenAdministration = (FirstAid_OxygenAdministrationType)data[start];
                start++;
                oxygenValue = data.ByteToFloat(out count, start);
                start += count;
                cpr = (FirstAid_CPRType)data[start];
                start++;
                aed = (FirstAid_AEDType)data[start];
                start++;
                shockCount = data.ByteToInt(out count, start);
                start += count;
                circulatoryAssist = (FirstAid_CirculatoryAssistType)data[start];
                start++;
                salineValue = data.ByteToString(out count, start);
                start += count;
                drugInjection = data.ByteToString(out count, start);
                start += count;
                woundTreatment = (FirstAid_WoundTreatmentType)data[start];
                start++;
                keepWarmType = (KeepWarmType)data[start];
                start++;
                fixing = (FirstAid_FixingType)data[start];
            }
        }
    }
    /// <summary>
    /// 차트 표현 타입
    /// </summary>
    public enum ViewChartType
    {
        None,
        Single,
        Multiple
    }

    /// <summary>
    /// 차트 데이터 Enum 타입의 Enum 타입
    /// </summary>
    public enum ChartData_Type
    {
        병력,
        환자증상_통증,
        환자증상_외상,
        환자증상,
        교통사고,
        그외외상,
        의식상태,
        동공반응_L,
        동공반응_R,
        환자분류,
        기도확보,
        산소투여,
        CPR,
        AED,
        순환보조,
        상처처치,
        고정
    }

    public enum MedicalHistory
    {
        없음,
        있음,
        미상
    }

    [Flags]
    public enum PatientSymptoms_PainType
    {
        없음 = 0x00,
        두통 = 0x01,
        흉통 = 0x02,
        복통 = 0x04,
        요통 = 0x08,
        분만진통 = 0x10,
        그밖의통증 = 0x20,
    }
    [Flags]
    public enum PatientSymptoms_TraumaType
    {
        없음 = 0x0000,
        골절 = 0x0001,
        탈구 = 0x0002,
        염좌 = 0x0004,
        열상 = 0x0008,
        철과상 = 0x0010,
        타박상 = 0x0020,
        절단 = 0x0040,
        압궤 = 0x0080,
        화상 = 0x0100
    }
    [Flags]
    public enum PatientSymptomsType
    {
        없음 = 0x00000000,
        의식장애 = 0x00000001,
        어지러움 = 0x00000002,
        경련_발작 = 0x00000004,
        실신 = 0x00000008,
        마비 = 0x00000010,
        전신쇠약 = 0x00000020,
        기도이물 = 0x00000040,
        호흡곤란 = 0x00000080,
        호흡정지 = 0x00000100,
        그_외_이물 = 0x00000200,
        기침 = 0x00000400,
        심계항진 = 0x00000800,
        가슴불편 = 0x00001000,
        저체온증 = 0x00002000,
        고열 = 0x00004000,
        토혈 = 0x00008000,
        혈변 = 0x00010000,
        비출혈 = 0x00020000,
        질출혈 = 0x00040000,
        그_외_출혈 = 0x00080000,
        오심 = 0x00100000,
        구토 = 0x00200000,
        설사 = 0x00400000,
        배뇨장애 = 0x00800000,
        변비 = 0x01000000,
        기타 = 0x02000000
    }

    [Flags]
    public enum TrafficAccidentType
    {
        없음 = 0x00,
        운전자 = 0x01,
        동승자 = 0x02,
        보행자 = 0x04,
        자전거 = 0x08,
        오토바이 = 0x10,
        그_외_탈것 = 0x20,
        미상 = 0x40
    }
    [Flags]
    public enum OtherTraumaType
    {
        없음 = 0x00,
        낙상 = 0x01,
        추락 = 0x02,
        열상 = 0x04,
        자상 = 0x08,
        그밖의_둔상 = 0x10,
        총상 = 0x20,
        관통 = 0x40,
        기계 = 0x80,
    }

    [Flags]
    public enum StatesOfConsciousnessType
    {
        None = 0x00,
        A = 0x01,
        V = 0x02,
        P = 0x04,
        U = 0x08
    }
    [Flags]
    public enum PupillaryReactionType
    {
        None = 0x00,
        정상 = 0x01,
        축동 = 0x02,
        산동 = 0x04,
        측정불가 = 0x08
    }
    public enum TriageType
    {
        None,
        응급,
        준응급,
        잠재응급,
        대상외,
        사망
    }
    [Flags]
    public enum FirstAid_AirwayType
    {
        None = 0x00,
        도수조작 = 0x01,
        기도유지 = 0x02,
        l_gel = 0x04,
        기관삽관 = 0x08,
        흡인기 = 0x10,
        기도폐쇄처치 = 0x20
    }
    [Flags]
    public enum FirstAid_OxygenAdministrationType
    {
        None = 0x00,
        value = 0x01,
        비관 = 0x02,
        안면 = 0x04,
        비재호흡 = 0x08,
        BVM = 0x10,
        네블라이저 = 0x20,
        기타 = 0x40
    }
    [Flags]
    public enum FirstAid_CPRType
    {
        None = 0x00,
        실시 = 0x01,
        거부 = 0x02,
        DNR = 0x04
    }
    [Flags]
    public enum FirstAid_AEDType
    {
        None = 0x00,
        Shock = 0x01,
        Monitoring = 0x02,
        기타 = 0x04
    }
    [Flags]
    public enum FirstAid_CirculatoryAssistType
    {
        None = 0x00,
        정맥로확보 = 0x01,
        수액공급 = 0x02,
        약물투여 = 0x04
    }
    [Flags]
    public enum FirstAid_WoundTreatmentType
    {
        None = 0x00,
        지혈 = 0x01,
        상처드레싱 = 0x02,
        보온 = 0x04
    }
    [Flags]
    public enum FirstAid_FixingType
    {
        None = 0x00,
        경추 = 0x01,
        척추 = 0x02,
        머리 = 0x04,
        상지 = 0X08,
        하지 = 0x10
    }
    public enum KeepWarmType
    {
        None,
        Warm,
        Cold
    }
}