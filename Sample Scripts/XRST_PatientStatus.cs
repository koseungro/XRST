/// 작성자: 고승로
/// 작성일: 2021-12-08
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.ENUM;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using FNI.IS;
#if UNITY_EDITOR
using UnityEditor;
using FNI.XRST.EDITOR;
using FNI.XRST.EDITOR.Property;
using FNI.IS.EDITOR.Property;

#endif

namespace FNI.Patient
{
    /// <summary>
    /// 아이템 세부 상태
    /// </summary>
    [Serializable]
    public class State
    {
        public bool IsArea { get => areaCode.IsNone == false && areaCode.IsArea; }
        public bool IsCloth { get => itemCode.IsCloth; }
        public bool CanOpen { get => IsCloth && (itemCode.Cloth == ClothCode.Top || itemCode.Cloth == ClothCode.Pants); }
        public bool IsOnProp
        {
            get
            {
                return itemCode.Check(PropCode.ElasticBandage) ||
                       itemCode.Check(PropCode.Splint_Arm) ||
                       itemCode.Check(PropCode.Splint_Leg) ||
                       itemCode.Check(PropCode.BPCuff) ||
                       itemCode.Check(PropCode.Catheter) ||
                       itemCode.Check(PropCode.CAT);
            }
        }

        public ClothStateCode state = ClothStateCode.Off;
        public XRST_Item areaCode = new XRST_Item();
        public XRST_Item itemCode = new XRST_Item();

        /// <summary>
        /// 상태 변경
        /// </summary>
        public void StateUpdate()
        {
            switch (state)
            {
                default: state = ClothStateCode.On; break;
                case ClothStateCode.On: state = ClothStateCode.Off; break;
            }
        }
        /// <summary>
        /// 옷의 상태 변경
        /// </summary>
        /// <param name="value">변경 값</param>
        public void ClothState(int value = -1)
        {
            if (CanOpen)
            {
                switch (value)
                {
                    case 0: state = ClothStateCode.Open; break;
                    case 1: state = ClothStateCode.Off; break;
                    case 2: state = ClothStateCode.On; break;

                    default: break;
                }
            }
            else
            {
                switch (state)
                {
                    default: state = ClothStateCode.On; break;
                    case ClothStateCode.On: state = ClothStateCode.Off; break;
                    case ClothStateCode.Off: break;
                }

            }
        }
        public bool Equals(State check)
        {
            return areaCode.Equals(check.areaCode) && itemCode.Equals(check.itemCode);
        }
        public override string ToString()
        {
            return $"<color=green>[{state}]{areaCode.ToFullName()}//{itemCode.ToFullName()}</color>";
        }
    }
    /// <summary>
    /// 몸 기준 상태
    /// </summary>
    [Serializable]
    public class Status
    {
        public bool IsHaveCloth { get => Cloth != null; }

#if UNITY_EDITOR
        [ToggleLeft]
#endif
        public bool IsBpCuff = false;
#if UNITY_EDITOR
        [ToggleLeft]
#endif
        public bool IsDiseased = false;

#if UNITY_EDITOR
        [Fixed_ItemType(ItemType.Body)]
#endif
        public XRST_Item bodyCode;
        public List<State> stateList = new List<State>();

        public State Cloth = null;
        public List<State> lastUsedItem = new List<State>();

        /// <summary>
        /// 이곳에 옷을 추가합니다.
        /// </summary>
        /// <param name="code">추가할 옷 부위</param>
        /// <param name="value">추가할 옷 위치</param>
        /// <param name="state">옷의 상태</param>
        public void AddCloth(ClothCode code, XRST_PatientStatus.Pos value = 0, ClothStateCode state = ClothStateCode.On)
        {
            Cloth = new State() { itemCode = new XRST_Item() { Cloth = code, value = (int)value }, state = state };
            stateList.Add(Cloth);
        }
        ///// <summary>
        ///// Area 상태를 추가합니다.
        ///// </summary>
        ///// <param name="code">추가할 AreaCode</param>
        ///// <param name="value">AreaCode의 상세값</param>
        ///// <param name="state">Area 상태</param>
        //public void AddAreaState(AreaCode code, int value = 0, ClothStateCode state = ClothStateCode.Off)
        //{
        //    State area = new State()
        //    {
        //        state = state,
        //        item = new XRST_Item() { Area = code, value = value }
        //    };
        //    Area = area;
        //    stateList.Add(area);
        //}
        /// <summary>
        /// Area 상태를 추가합니다.
        /// </summary>
        /// <param name="code">추가할 AreaCode</param>
        /// <param name="value">AreaCode의 상세값</param>
        /// <param name="state">Area 상태</param>
        public void AddPropState((AreaCode, int) area, params (PropCode, int)[] prop)
        {
            switch (area.Item1)
            {
                case AreaCode.TheDiseasedPart: IsDiseased = true; break;
                case AreaCode.BPCuff: IsBpCuff = true; break;
            }

            for (int cnt = 0; cnt < prop.Length; cnt++)
            {
                State state = new State()
                {
                    state = ClothStateCode.None,
                    areaCode = new XRST_Item() { Area = area.Item1, value = area.Item2 },
                    itemCode = new XRST_Item() { Prop = prop[cnt].Item1, value = prop[cnt].Item2 }
                };
                stateList.Add(state);
            }
        }
        /// <summary>
        /// Area 상태를 제거합니다.
        /// </summary>
        /// <param name="code">추가할 AreaCode</param>
        /// <param name="value">AreaCode의 상세값</param>
        public void RemoveAreaState(AreaCode code, int value = -1)
        {
            if (stateList != null)
            {
                stateList.RemoveAll(x => x.areaCode.Equals(code, value));

                switch (code)
                {
                    case AreaCode.BPCuff: IsBpCuff = false; break;
                }
            }
        }
        /// <summary>
        /// Area 상태를 제거합니다.
        /// </summary>
        /// <param name="code">추가할 AreaCode</param>
        /// <param name="value">AreaCode의 상세값</param>
        public void RemoveAreaAndLastItem(AreaCode code)
        {
            if (stateList != null)
            {
                if (stateList.TryFind(out State findState, x => x.areaCode.Equals(code)))
                {
                    lastUsedItem.RemoveAll(x => x.itemCode.Equals(findState.itemCode));
                    stateList.Remove(findState);
                }
            }
        }
        /// <summary>
        /// State를 찾습니다.
        /// </summary>
        /// <param name="item">찾을 아이템</param>
        /// <param name="found">찾은 State</param>
        /// <returns></returns>
        public bool Find_State(XRST_Item item, out State found)
        {
            found = stateList.Find(x => x.itemCode.Equals(item));

            return found != null;
        }
        /// <summary>
        /// State를 찾습니다.
        /// </summary>
        /// <param name="code">찾을 Area</param>
        /// <param name="value">Area의 value</param>
        /// <param name="found">찾은 State</param>
        /// <returns></returns>
        public bool Find_State(XRST_Item item, out List<State> found)
        {
            found = stateList.FindAll(x => x.areaCode.Equals(item));

            if (found == null)
                return false;
            else
                return found.Count != 0;
        }
        /// <summary>
        /// State를 찾습니다.
        /// </summary>
        /// <param name="code">찾을 Area</param>
        /// <param name="value">Area의 value</param>
        /// <param name="found">찾은 State</param>
        /// <returns></returns>
        public bool Find_State(AreaCode code, int value, out List<State> found)
        {
            found = stateList.FindAll(x => x.areaCode.Equals(code, value));

            if (found == null)
                return false;
            else
                return found.Count != 0;
        }
        /// <summary>
        /// State를 찾습니다.
        /// </summary>
        /// <param name="code">찾을 Area</param>
        /// <param name="value">Area의 value</param>
        /// <param name="found">찾은 State</param>
        /// <returns></returns>
        public bool Find_State(AreaCode code, int value, out State found)
        {
            found = stateList.Find(x => x.areaCode.Equals(code, value));

            return found != null;
        }
        /// <summary>
        /// 장비의 상태를 업데이트 합니다.
        /// </summary>
        /// <param name="oldUsedItem">사용한 Prop</param>
        /// <param name="areaCode">Prop의 Area</param>
        /// <param name="state">상태 변경된 State</param>
        /// <returns>활성화 상태이면 true</returns>
        public void ItemUsed(XRST_Item oldUsedItem, out State state)
        {
            state = stateList.Find(x => x.itemCode.EqualsShort(oldUsedItem));

            if (state != null)
            {
                state.StateUpdate();

                if (lastUsedItem.Find(x => x.itemCode.Equals(oldUsedItem)) == null)
                    lastUsedItem.Add(state);
            }
        }

        /// <summary>
        /// 장비의 상태를 업데이트 합니다.
        /// </summary>
        /// <param name="oldUsedItem">사용한 Prop</param>
        /// <param name="areaCode">Prop의 Area</param>
        /// <param name="state">상태 변경된 State</param>
        /// <returns>활성화 상태이면 true</returns>
        public void ItemUsed(XRST_Item areaCode, XRST_Item propCode, out State state)
        {
            Find_State(areaCode, out List<State> found);

            state = found.Find(x => x.itemCode.Equals(propCode));
            if (state != null)
            {
                state.StateUpdate();

                if (lastUsedItem.Find(x => x.itemCode.Equals(propCode)) == null)
                    lastUsedItem.Add(state);
            }
        }
        public void RemoveLastItem()
        {
            lastUsedItem.RemoveAt(lastUsedItem.Count - 1);
        }
        public void RemoveItem(State state)
        {
            lastUsedItem.RemoveAll(X => X.areaCode.Equals(state.areaCode) && X.itemCode.Equals(state.itemCode));
        }
        /// <summary>
        /// 상태 확인, 옷의 팔다리 부분은 이 아이템에 의해 상태가 결정된다.
        /// </summary>
        /// <returns></returns>
        public ClothStateCode SetClothState()
        {
            if (Cloth.state != ClothStateCode.Off)
            {
                bool check = false;
                for (int cnt = 0; cnt < stateList.Count; cnt++)
                {
                    //이이템에 해당 코드가 있는지 확인한다.
                    if (stateList[cnt].IsOnProp)
                    {
                        //확인된 코드가 On이면 Check를 true로 만들고 멈춘다.
                        check = stateList[cnt].state == ClothStateCode.On;
                        break;
                    }
                }
                // 옷의 상태 변경
                Cloth.state = check ? ClothStateCode.Off : ClothStateCode.On;
            }

            return Cloth.state;
        }
        /// <summary>
        /// 외부입력에 의한 상태값 변환
        /// </summary>
        /// <param name="check">변경할 상태 값</param>
        /// <returns></returns>
        public ClothStateCode SetClothState(ClothStateCode check)
        {
            switch (check)
            {
                case ClothStateCode.On:
                    for (int cnt = 0; cnt < stateList.Count; cnt++)
                    {
                        if (stateList[cnt].IsOnProp)
                        {
                            if (stateList[cnt].state == ClothStateCode.On)
                            {
                                check = ClothStateCode.Off;
                                break;
                            }
                        }
                    }
                    break;
                case ClothStateCode.Open:
                    if (Cloth.CanOpen == false)
                    {
                        check = ClothStateCode.On;
                        goto case ClothStateCode.On;
                    }
                    break;
            }

            Cloth.state = check;

            return Cloth.state;
        }/// <summary>
         /// 외부입력에 의한 상태값 변환
         /// </summary>
         /// <param name="check">변경할 상태 값</param>
         /// <returns></returns>
        public ClothStateCode SetClothState(ClothStateCode check, bool isAmputation)
        {
            if (isAmputation)
                Cloth.state = ClothStateCode.Off;
            else
            {
                switch (check)
                {
                    case ClothStateCode.On:
                        for (int cnt = 0; cnt < stateList.Count; cnt++)
                        {
                            if (stateList[cnt].IsOnProp)
                            {
                                if (stateList[cnt].state == ClothStateCode.On)
                                {
                                    check = ClothStateCode.Off;
                                    break;
                                }
                            }
                        }
                        break;
                    case ClothStateCode.Open:
                        if (Cloth.CanOpen == false)
                        {
                            check = ClothStateCode.On;
                            goto case ClothStateCode.On;
                        }
                        break;
                }
            }

            Cloth.state = check;

            return Cloth.state;
        }

        public bool FindLastUsedItem(AreaCode findArea, out State find)
        {
            List<State> finded = lastUsedItem.FindAll(x => x.areaCode.Equals(findArea));
            find = finded.Count == 0 ? null : finded[finded.Count - 1];

            return finded.Count != 0;
        }

        public State LastProp()
        {
            List<State> state = stateList.FindAll(x => x.itemCode.IsProp);

            int cnt = state.Count - 1;
            while (0 < cnt)
            {
                if (state[cnt].state == ClothStateCode.On)
                    break;
                cnt--;
            }

            return state[cnt];
        }

        public State GetArea(AreaCode area)
        {
            return stateList.Find(x => x.areaCode.Equals(area));
        }
        public bool GetArea(AreaCode area, out State state)
        {
            state = stateList.Find(x => x.areaCode.Equals(area));

            if (state == null)
                Debug.Log($"[XRST_PatientStatus/GetBody] {area} is not Found.");
            else
                Debug.Log($"[XRST_PatientStatus/GetBody] {area} is Found: {state.areaCode}");

            return state != null;
        }
        public XRST_PatientStatus.Pos ToPosition()
        {
            return (XRST_PatientStatus.Pos)bodyCode.value;
        }

        public override string ToString()
        {
            return $"{bodyCode}=>{string.Join(",", stateList)}";
        }
    }

    /// <summary>
    /// 환자의 상태 값만 기록한다.
    /// </summary>
    public class XRST_PatientStatus : MonoBehaviour
    {
        public bool IsDoubleBPcuff => division.BPcuffPos() == Pos.Other;
        public bool IsSettingBpcuff => setBPCuff;
        public enum Pos
        {
            None = -2,
            Other = -1,

            Default = 0,

            LU = L,
            RU = R,
            LD = U,
            RD = D,

            L = Default,
            R = 1,
            U = 2,
            D = 3,

            _L = 4,
            _R = 5,
            _U = 6,
            _D = 7,
        }

        public List<Status> statusList = new List<Status>();

        public XRST_Item oldUsedItem = new XRST_Item();
        public Pos bpcuffPosition;

        private List<Status> diseasedStatus = new List<Status>();
        public List<Status> bpcuffStatus = new List<Status>();
        [SerializeField]
        private List<Status> clothState = new List<Status>();
        private DataDivision division;

        private bool setBPCuff = false;

        List<AreaCode> liquidityItemList = new List<AreaCode>() { AreaCode.BPCuff, AreaCode.InjectionSite, AreaCode.Torniquet, AreaCode.PulseOXimetry };

        /// <summary>
        /// 초기화
        /// </summary>
        public void Init()
        {
            division = new DataDivision();
            setBPCuff = false;

            statusList.Clear();

            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Body } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Pelvis } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Mouse } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Neck } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Chest } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Abdominal } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Perineum } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Hand, value = (int)Pos.L } });
            statusList.Add(new Status() { bodyCode = new XRST_Item() { Body = BodyCode.Hand, value = (int)Pos.R } });

            CreateBody(BodyCode.UpperBody, ClothCode.Top);
            CreateBody(BodyCode.Arm, ClothCode.Arm, Pos.LU);
            CreateBody(BodyCode.Arm, ClothCode.Arm, Pos.RU);
            CreateBody(BodyCode.Arm, ClothCode.Arm, Pos.LD);
            CreateBody(BodyCode.Arm, ClothCode.Arm, Pos.RD);
            CreateBody(BodyCode.LowerBody, ClothCode.Pants);
            CreateBody(BodyCode.Leg, ClothCode.Leg, Pos.LU);
            CreateBody(BodyCode.Leg, ClothCode.Leg, Pos.RU);
            CreateBody(BodyCode.Leg, ClothCode.Leg, Pos.LD);
            CreateBody(BodyCode.Leg, ClothCode.Leg, Pos.RD);
            CreateBody(BodyCode.Foot, ClothCode.Shoes, Pos.L);
            CreateBody(BodyCode.Foot, ClothCode.Shoes, Pos.R);
        }
        /// <summary>
        /// 새로운 상태를 생성하고 옷을 추가하여 목록에 추가한다.
        /// </summary>
        /// <param name="body">생성부위</param>
        /// <param name="cloth">생성할 옷 부위</param>
        /// <param name="value">생성할 위치</param>
        /// <param name="state">생성하는 옷의 상태</param>
        private void CreateBody(BodyCode body, ClothCode cloth, Pos value = Pos.Default, ClothStateCode state = ClothStateCode.On)
        {
            Status status = new Status() { bodyCode = new XRST_Item() { Body = body, value = (int)value } };
            status.AddCloth(cloth, value, state);

            clothState.Add(status);
            statusList.Add(status);
        }
        /// <summary>
        /// 바디코드 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="code">가져올 신체부위</param>
        /// <param name="value">가져올 위치</param>
        /// <returns></returns>
        public Status GetBody(BodyCode code, Pos value)
        {
            return statusList.Find(x => x.bodyCode.Equals(code, (int)value));
        }
        /// <summary>
        /// 바디코드 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="code">가져올 신체부위</param>
        /// <param name="value">가져올 위치</param>
        /// <returns></returns>
        public Status GetBody(BodyCode code, int value = 0)
        {
            Status find = statusList.Find(x => x.bodyCode.Equals(code, value));

            if (find == null)
                Debug.Log($"[XRST_PatientStatus/GetBody] BodyCode-{code}[{value}] is not Found.");
            else
                Debug.Log($"[XRST_PatientStatus/GetBody] BodyCode-{code}[{value}] is Found: {find.bodyCode}");

            return find;
        }
        /// <summary>
        /// 옷 코드 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="code">가져올 옷 부위</param>
        /// <param name="value">가져올 위치</param>
        /// <returns></returns>
        public Status GetCloth(ClothCode code, Pos value = Pos.Default)
        {
            Status find = null;
            if (value == Pos.None)
                find = clothState.Find(x => x.Cloth.itemCode.EqualsShort(ItemType.Cloth, (int)code));
            else
                find = clothState.Find(x => x.Cloth.itemCode.Equals(code, (int)value));

            if (find != null)
                return find;
            else
                return null;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public Status GetMatchingAreaItem(BodyCode body, int bodyValue, AreaCode area, int value = 0)
        {
            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                if (statusList[cnt].bodyCode.Equals(body, bodyValue))
                {
                    for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                    {
                        if (statusList[cnt].stateList[cnt_B] == null)
                            continue;
                        if (statusList[cnt].stateList[cnt_B].areaCode.Equals(area, value))
                            return statusList[cnt];
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public Status GetMatchingAreaItem(AreaCode area, int value = 0)
        {
            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                {
                    if (statusList[cnt].stateList[cnt_B] == null)
                        continue;
                    if (statusList[cnt].stateList[cnt_B].areaCode.Equals(area, value))
                        return statusList[cnt];
                }
            }

            return null;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public List<Status> GetMatchingAllItem(AreaCode area)
        {
            List<Status> matching = new List<Status>();

            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                {
                    if (statusList[cnt].stateList[cnt_B] == null)
                        continue;
                    if (statusList[cnt].stateList[cnt_B].areaCode.EqualsShort(ItemType.Area, (int)area))
                        matching.Add(statusList[cnt]);
                }
            }

            return matching;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public List<Status> GetMatchingAllItem(XRST_Item area)
        {
            List<Status> matching = new List<Status>();

            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                {
                    if (statusList[cnt].stateList[cnt_B] == null)
                        continue;
                    if (statusList[cnt].stateList[cnt_B].areaCode.EqualsShort(area))
                        matching.Add(statusList[cnt]);
                }
            }

            return matching;
        }
        /// <summary>
        /// 옷 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public Status GetMatchingItem(ClothCode area, int value = 0)
        {
            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                if (statusList[cnt].Cloth != null &&
                    statusList[cnt].Cloth.itemCode.IsNone == false &&
                    statusList[cnt].Cloth.itemCode.Equals(area, value))
                    return statusList[cnt];
            }

            return null;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public Status GetMatchingItem(XRST_Item area)
        {
            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                {
                    if (statusList[cnt].stateList[cnt_B].areaCode.Equals(area))
                        return statusList[cnt];
                }
            }

            return null;
        }
        /// <summary>
        /// 아이템 기준으로 몸을 가져온다.
        /// </summary>
        /// <param name="item">포함되는 아이템</param>
        /// <returns></returns>
        public Status GetMatchingItem(XRST_Item area, XRST_Item item)
        {
            for (int cnt = 0; cnt < statusList.Count; cnt++)
            {
                for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                {
                    if (statusList[cnt].stateList[cnt_B].areaCode.Equals(area) &&
                        statusList[cnt].stateList[cnt_B].itemCode.Equals(item))
                        return statusList[cnt];
                }
            }

            return null;
        }
        /// <summary>
        /// 환자 설정 값으로 환자에 적용할 장비를 설정한다.
        /// </summary>
        /// <param name="division">환자 설정 값</param>
        public void SetDivision(DataDivision division)
        {
            diseasedStatus.Clear();
            this.division = division;

            Status firstBPCuff = null;

            switch (division.BPcuffPos())
            {
                case Pos.Other:
                    firstBPCuff = GetBody(BodyCode.Arm, Pos.LU);
                    Status secondBPCuff = GetBody(BodyCode.Arm, Pos.RU);

                    firstBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 0));
                    secondBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 1));

                    bpcuffStatus.Add(firstBPCuff);
                    bpcuffStatus.Add(secondBPCuff);
                    break;
                case Pos._L:
                    firstBPCuff = GetBody(BodyCode.Arm, Pos.LU);

                    firstBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 0));

                    bpcuffStatus.Add(firstBPCuff);
                    bpcuffPosition = Pos.L;
                    SetOtherProp(Pos.L);
                    break;
                case Pos._R:
                    firstBPCuff = GetBody(BodyCode.Arm, Pos.RU);

                    firstBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 1));

                    bpcuffStatus.Add(firstBPCuff);
                    bpcuffPosition = Pos.R;
                    SetOtherProp(Pos.R);
                    break;
            }

            //환자의 번호에 맞춰 환부와 공통도구를 설정한다.
            switch (division.ToID())
            {
                // 1차년도 환부 1개
                case PatientID.k_M_S1P1A1_32: SetDiseased(BodyCode.Arm, Pos.RU, 0, true, true); goto case "Base";
                case PatientID.k_M_S1P1A2_12: SetDiseased(BodyCode.Arm, Pos.LD, 0, true, true); goto case "Base";

                case PatientID.k_M_S1P3A1_46: SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true); goto case "Base";
                case PatientID.k_M_S1P3A2_14: SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true); goto case "Base";

                case PatientID.k_F_S1P1A1_26: SetDiseased(BodyCode.Arm, Pos.RD, 0, true, true); goto case "Base";
                case PatientID.k_F_S1P1A2_23: SetDiseased(BodyCode.Arm, Pos.LU, 0, true, true); goto case "Base";

                case PatientID.k_F_S1P2A1_43: SetDiseased(BodyCode.Leg, Pos.RU, 0, true, true); goto case "Base";
                case PatientID.k_F_S1P2A2_33: SetDiseased(BodyCode.Leg, Pos.LU, 0, true, true); goto case "Base";

                case PatientID.k_F_S1P3A1_40: SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true); goto case "Base";
                case PatientID.k_F_S1P3A2_19: SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true); goto case "Base";

                //1차년도 환부 2개
                case PatientID.k_M_S1P2A1_16:
                    SetDiseased(BodyCode.Leg, Pos.RU, 0, true, true);
                    SetDiseased(BodyCode.Leg, Pos.LU, 1, true, true); goto case "Base";
                case PatientID.k_M_S1P2A2_37:
                    SetDiseased(BodyCode.Leg, Pos.LU, 0, true, true);
                    SetDiseased(BodyCode.Leg, Pos.RU, 1, true, true); goto case "Base";

                // 2차년도
                case PatientID.k_F_S2P4A3_1222: goto case PatientID.k_M_S2P4A3_1470;
                case PatientID.k_F_S2P5A3_834: goto case PatientID.k_M_S2P5A3_1216;
                case PatientID.k_F_S2P5A4_1585: goto case PatientID.k_M_S2P5A4_1531;
                case PatientID.k_F_S2P5A5_1382: goto case PatientID.k_M_S2P5A5_1144;

                case PatientID.k_M_S2P4A3_1470: SetDiseased(BodyCode.Chest, Pos.Default, 0); goto case "Base";
                case PatientID.k_M_S2P5A3_1216: SetDiseased(BodyCode.Abdominal, Pos.Default, 0); goto case "Base";
                case PatientID.k_M_S2P5A4_1531: SetDiseased(BodyCode.Abdominal, Pos.Default, 0, true, false, false, true); goto case "Base"; // 복부관통_칼
                case PatientID.k_M_S2P5A5_1144:
                    SetDiseased(BodyCode.Abdominal, Pos.Default, 0);
                    SetDiseased(BodyCode.Arm, Pos.LD, 1, true, true);
                    AddState((BodyCode.Arm, Pos.LU), (AreaCode.CATArm, 0), PropCode.CAT);
                    goto case "Base";

                // 3차년도
                case PatientID.k_M_S3P6A3_1084:
                    SetDiseased(BodyCode.Arm, Pos.LU, 0, true, false, true); goto case "Base";
                case PatientID.k_M_S3P6A1_1626:
                    SetDiseased(BodyCode.Arm, Pos.RU, 0, true, false, true); goto case "Base";
                case PatientID.k_M_S3P6A6_1677:
                    SetDiseased(BodyCode.Arm, Pos.LU, 0, true, true, true);
                    AddState((BodyCode.Arm, Pos.LU), (AreaCode.CATArm, 0), PropCode.CAT);
                    goto case "Base";

                case PatientID.k_M_S3P7A6_321:
                    SetDiseased(BodyCode.Leg, Pos.LU, 0, true, true, true);
                    AddState((BodyCode.Leg, Pos.LU), (AreaCode.CATLeg, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_M_S3P7A1_326:
                    SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true, true);
                    AddState((BodyCode.Leg, Pos.RD), (AreaCode.CATLeg, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_M_S3P8A6_1847: // 상지구획
                    SetDiseased(BodyCode.Arm, Pos.RD, 0, false, true);
                    AddState((BodyCode.Arm, Pos.RU), (AreaCode.CompartmentPressure, 0), PropCode.SpineNeedle);
                    goto case "Base";

                case PatientID.k_M_S3P9A3_1831:
                    SetDiseased(BodyCode.Abdominal, Pos.Default, 0, true, false, true);
                    SetDiseased(BodyCode.Leg, Pos.LU, 1, false, true);
                    goto case "Base";
                case PatientID.k_M_S3P9A1_1825:
                    SetDiseased(BodyCode.Abdominal, Pos.Default, 0, true, false, true);
                    SetDiseased(BodyCode.Leg, Pos.RU, 1, false, true);
                    goto case "Base";


                case PatientID.k_F_S3P6A3_1703:
                    SetDiseased(BodyCode.Arm, Pos.RD, 0, true, true, true);
                    AddState((BodyCode.Arm, Pos.RD), (AreaCode.CATArm, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_F_S3P6A6_1788:
                    SetDiseased(BodyCode.Arm, Pos.LU, 0, true, true, true);
                    AddState((BodyCode.Arm, Pos.LU), (AreaCode.CATArm, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_F_S3P6A1_1679:
                    SetDiseased(BodyCode.Arm, Pos.RU, 0, true, true, true);
                    AddState((BodyCode.Arm, Pos.RU), (AreaCode.CATArm, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_F_S3P7A6_1621:
                    SetDiseased(BodyCode.Leg, Pos.LU, 0, true, true, true);
                    AddState((BodyCode.Leg, Pos.LU), (AreaCode.CATLeg, 0), PropCode.CAT);
                    goto case "Base";
                case PatientID.k_F_S3P7A1_1092: // 여_하지절단_환부 2개
                    SetDiseased(BodyCode.Leg, Pos.RD, 0, true, true, true);
                    SetDiseased(BodyCode.Leg, Pos.LD, 1, true, true, true);
                    AddState((BodyCode.Leg, Pos.RD), (AreaCode.CATLeg, 0), PropCode.CAT);
                    AddState((BodyCode.Leg, Pos.LD), (AreaCode.CATLeg, 1), PropCode.CAT);
                    goto case "Base";

                case PatientID.k_F_S3P8A6_1858: //상지구획
                    SetDiseased(BodyCode.Arm, Pos.LD, 0, false, true);
                    AddState((BodyCode.Arm, Pos.LU), (AreaCode.CompartmentPressure, 0), PropCode.SpineNeedle);
                    goto case "Base";
                case PatientID.k_F_S3P9A1_1828: goto case PatientID.k_M_S3P9A3_1831;
                case PatientID.k_F_S3P9A3_1854: goto case PatientID.k_M_S3P9A1_1825;

                case "Base":
                    AddState((BodyCode.Body, Pos.Default), (AreaCode.Blanket, 0), PropCode.Blanket);

                    AddState((BodyCode.Chest, Pos.RU), (AreaCode.EKG_W, 0), (PropCode.EKGPad, 0));
                    AddState((BodyCode.Chest, Pos.LU), (AreaCode.EKG_B, 0), (PropCode.EKGPad, 1));
                    AddState((BodyCode.Abdominal, Pos.LU), (AreaCode.EKG_R, 0), (PropCode.EKGPad, 2));
                    //if (division.sequence == SequenceCode.SQ1)
                    //{
                    //    AddState(BodyCode.Chest, AreaCode.EKG_W, Pos.RU, (PropCode.EKGPad, 0));
                    //    AddState(BodyCode.Chest, AreaCode.EKG_R, Pos.LU, (PropCode.EKGPad, 1));
                    //    AddState(BodyCode.Abdominal, AreaCode.EKG_B, Pos.LU, (PropCode.EKGPad, 2));
                    //}
                    //else
                    //{
                    //    AddState(BodyCode.Chest, AreaCode.EKG_W, Pos.RU, (PropCode.EKGPad, 2));
                    //    AddState(BodyCode.Chest, AreaCode.EKG_R, Pos.LU, (PropCode.EKGPad, 0));
                    //    AddState(BodyCode.Abdominal, AreaCode.EKG_B, Pos.LU, (PropCode.EKGPad, 1));
                    //}
                    AddState((BodyCode.Neck, Pos.Default), (AreaCode.Neck, 0), PropCode.NeckCollar);
                    AddState((BodyCode.Mouse, Pos.Default), (AreaCode.Mouse, 0), PropCode.ReserveMask, PropCode.OPA, PropCode.ValveMask, PropCode.Laryngoscope, PropCode.ET_Tube);
                    AddState((BodyCode.Chest, Pos.Default), (AreaCode.NeedleDecompression, 0), PropCode.DetachableNeedle, PropCode.Gauze_4x4, PropCode.Tape);
                    AddState((BodyCode.Chest, Pos.Default), (AreaCode.ChestTubeInsertion, 0), PropCode.ChestTube, PropCode.Gauze_4x4, PropCode.Tape, PropCode.Mass);

                    AddState((BodyCode.Perineum, Pos.Default), (AreaCode.Foley, 0), PropCode.FoleyCatheter, PropCode.Syringe_DistilledWater, PropCode.UrineBag, PropCode.KidneyDish, PropCode.SterilizationWraps, PropCode.Gauze_Ball);
                    AddState((BodyCode.Pelvis, Pos.Default), (AreaCode.Pelvis, 0), PropCode.PelvicBinder);
                    AddState((BodyCode.Chest, Pos.Default), (AreaCode.Tag, 0), PropCode.Tag_LogRolling);
                    AddState((BodyCode.Pelvis, Pos.Default), (AreaCode.Tag, 1), PropCode.Tag_Foley);
                    AddState((BodyCode.Neck, Pos.Default), (AreaCode.Cricothyoidotomy, 0), PropCode.Tracheostomy_Tube, PropCode.EtCO2Monitor, PropCode.NeckBand, PropCode.SterilizationWraps, PropCode.Gauze_Ball, PropCode.Mass, PropCode.Kelly, PropCode.Gauze_4x4);
                    break;
            }
        }
        private Status AddState((BodyCode, Pos) body, (AreaCode, int) area, params (PropCode, int)[] prop)
        {
            Status status = GetBody(body.Item1, body.Item2);
            if (status == null)
            {
                status = new Status() { bodyCode = new XRST_Item() { Body = body.Item1, value = (int)body.Item2 } };
                statusList.Add(status);
            }
            for (int cnt = 0; cnt < prop.Length; cnt++)
            {
                status.AddPropState(area, prop[cnt]);
            }

            return status;
        }
        private Status AddState((BodyCode, Pos) body, (AreaCode, int) area, params PropCode[] prop)
        {
            Status status = GetBody(body.Item1, body.Item2);
            for (int cnt = 0; cnt < prop.Length; cnt++)
            {
                status.AddPropState(area, (prop[cnt], 0));
            }

            return status;
        }

        /// <summary>
        /// 환부를 옵션에 맞춰 상태를 추가한다.
        /// </summary>
        /// <param name="code">상태를 추가할 몸 부위</param>
        /// <param name="pos">몸 부위(<see cref="BodyCode"/>)의 위치</param>
        /// <param name="num">환부 번호</param>
        /// <param name="addDressing">드레싱 할 것인지</param>
        /// <param name="add2ndDressing">2번째 드레싱 할 것인지</param>
        /// <param name="addSplint">스플린트를 착용 할 것인지</param>
        private void SetDiseased(BodyCode code, Pos pos, int num = 0, bool addDressing = true, bool addSplint = false, bool add2ndDressing = false, bool addSubstances = false)
        {
            Status find = GetBody(code, pos);
            find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Bleeding, 0));
            if (addDressing)
                find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.ElasticBandage, 0));
            if (addSplint)
            {
                if (code == BodyCode.Arm)
                    find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Splint_Arm, 0));
                else if (code == BodyCode.Leg)
                    find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Splint_Leg, 0));
            }

            if (add2ndDressing)
            {
                find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Gauze_4x4, 0));
                find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Gauze_2nd, 0));
                find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.ElasticBandage_2nd, 0));
            }
            if (addSubstances)
                find.AddPropState((AreaCode.TheDiseasedPart, num), (PropCode.Substances, 0));

            diseasedStatus.Add(find);
        }

        /// <summary>
        /// <see cref="XRST_PatientSetting"/>에서 도구에 대한 값을 받아 상태를 설정해준다.
        /// </summary>
        /// <param name="setting1">상태 설정 값, 설정 값은 Prop이지만 Value에는 Area 값을 가진다.</param>
        /// <param name="setting2">설정 후 피드백</param>
        public void Setting_Area(XRST_PatientSetting setting1, Action<List<State>> setting2)
        {
            List<State> findedState = new List<State>();

            for (int cnt_A = 0; cnt_A < setting1.parts_Area.Count; cnt_A++)
            {
                XRST_PatientSetting.States part = setting1.parts_Area[cnt_A];

                for (int cnt_B = 0; cnt_B < statusList.Count; cnt_B++)
                {
                    int value = part.id.Prop == PropCode.EKGPad ?
                                0 : //EKG Pad일 때 0으로 만들어준다.
                                part.id.value;// EKG외에는 value값이 Area값임

                    if (part.id.Prop == PropCode.BPCuff)
                        value = 0;

                    if (statusList[cnt_B].Find_State(XRST_Item.PropToArea(part.id, division),
                                                     value,
                                                     out List<State> finds))
                    {
                        if (finds != null)
                        {
                            for (int cnt_C = 0; cnt_C < finds.Count; cnt_C++)
                            {
                                if (finds[cnt_C].itemCode.EqualsShort(part.id))
                                {
                                    finds[cnt_C].state = part.ToClothState();
                                    // 2022.03.10 불필요 동작으로 삭제
                                    // if (finds[cnt_C].itemCode.IsProp)
                                    // {
                                    //     switch (finds[cnt_C].itemCode.Prop)
                                    //     {
                                    //         case PropCode.Pulse_OX_imetry: goto case PropCode.Catheter;
                                    //         case PropCode.Tourniquet:      goto case PropCode.Catheter;
                                    //         case PropCode.Catheter:
                                    //             finds[cnt_C].itemCode.value = IsDoubleBPcuff ? (int)bpcuffPosition : 0;
                                    //             break;
                                    //     }
                                    // }

                                    State newState = new State()
                                    {
                                        state = finds[cnt_C].state
                                    };
                                    newState.areaCode.Copy(finds[cnt_C].areaCode);
                                    newState.itemCode.Copy(finds[cnt_C].itemCode);

                                    if (newState.areaCode.Area == AreaCode.ChestTubeInsertion ||
                                        newState.areaCode.Area == AreaCode.NeedleDecompression)
                                    {
                                        List<State> cTube = statusList[cnt_B].stateList.FindAll(x => x.areaCode.Area == newState.areaCode.Area &&
                                                                                                     x.itemCode.Prop != newState.itemCode.Prop);

                                        if (cTube != null)
                                        {
                                            for (int cnt = 0; cnt < cTube.Count; cnt++)
                                            {
                                                cTube[cnt].state = part.ToClothState();
                                                findedState.Add(cTube[cnt]);
                                            }
                                        }
                                    }

                                    if (newState.state == ClothStateCode.On)
                                    {
                                        if (statusList[cnt_B].lastUsedItem.Find(x => x.Equals(finds[cnt_C])) == null)
                                            statusList[cnt_B].lastUsedItem.Add(finds[cnt_C]);

                                        if (newState.itemCode.Prop == PropCode.BPCuff)
                                        {
                                            if (setBPCuff)
                                            {
                                                newState.itemCode.value = (int)bpcuffPosition;

                                                Debug.Log($"[<color=orange>bpcuffPosition Value Setting!!</color> // {bpcuffPosition}");
                                            }
                                            else
                                            {
                                                bpcuffPosition = division.BPcuffPos();

                                                SetOtherProp(bpcuffPosition);

                                                Debug.Log($"[<color=orange>bpcuffPosition Setting!!</color> // {bpcuffPosition}");
                                            }
                                        }
                                    }
                                    findedState.Add(newState);
                                }
                            }
                        }
                        else
                            Debug.Log($"[<color=red>{part.id} is Null!!</color> // {part.id.ToFullName()}");

                        //if (setBPCuff == false && setting1.parts_Area[cnt].id.Prop == PropCode.BPCuff)
                        //{
                        //    BodyPosition bp = setting1.parts_Area[cnt].id.value == 0 ? BodyPosition.L : BodyPosition.R;
                        //    SetOtherProp(bp);
                        //}
                    }
                }
            }

            setting2?.Invoke(findedState);
        }
        /// <summary>
        /// <see cref="XRST_PatientSetting"/>에서 옷에 대한 값을 받아 상태를 설정해준다.
        /// </summary>
        /// <param name="setting1">상태 설정 값</param>
        /// <param name="setting2">설정 후 피드백</param>
        public void Setting_Cloth(XRST_PatientSetting setting1, Action<List<State>> setting2)
        {
            List<State> finded = new List<State>();

            for (int cnt = 0; cnt < setting1.parts_Cloth.Count; cnt++)
            {
                XRST_Item target = new XRST_Item();
                switch (setting1.parts_Cloth[cnt].id.Cloth)
                {
                    case ClothCode.TheDiseasedPart:
                        target = new XRST_Item() { Area = AreaCode.TheDiseasedPart, value = setting1.parts_Cloth[cnt].id.value };
                        break;
                    case ClothCode.BPCuff:
                        target = new XRST_Item() { Area = AreaCode.BPCuff, value = setting1.parts_Cloth[cnt].id.value };
                        break;
                    case ClothCode.InjectionAndTourniquet:
                        target = new XRST_Item() { Area = AreaCode.InjectionSite, value = setting1.parts_Cloth[cnt].id.value };
                        break;
                    default:
                        target.Copy(setting1.parts_Cloth[cnt].id);
                        break;
                }

                if (setBPCuff == false &&
                    setting1.parts_Cloth[cnt].id.Cloth == ClothCode.BPCuff &&
                    setting1.parts_Cloth[cnt].state == XRST_PatientSetting.State.Off)
                {
                    setBPCuff = true;
                    bpcuffPosition = division.BPcuffPos();
                    SetOtherProp(bpcuffPosition);
                }

                for (int cnt_B = 0; cnt_B < statusList.Count; cnt_B++)
                {
                    if (statusList[cnt_B].Cloth == null) continue;

                    bool check = false;

                    if (target.IsArea)
                    {
                        if (statusList[cnt_B].Find_State(target.Area, target.value, out State find))
                            check = true;
                    }
                    else
                    {
                        if (statusList[cnt_B].Find_State(target, out State find))
                            check = true;
                    }

                    if (check)
                    {
                        statusList[cnt_B].Cloth.state = setting1.parts_Cloth[cnt].ToClothState();

                        finded.Add(statusList[cnt_B].Cloth);

                        if (statusList[cnt_B].Cloth.itemCode.Cloth == ClothCode.Top ||
                            statusList[cnt_B].Cloth.itemCode.Cloth == ClothCode.Pants)
                            finded.AddRange(SetOtherCloth(statusList[cnt_B].Cloth));
                        else
                        {
                            finded.AddRange(SetClothLink(statusList[cnt_B].Cloth));
                        }
                    }
                }
            }

            setting2?.Invoke(finded);
        }
        /// <summary>
        /// <see cref="XRST_PatientSetting"/>에서 환부에 대한 값을 받아 상태를 설정해준다.
        /// </summary>
        /// <param name="setting1">상태 설정 값</param>
        /// <param name="_state">설정 후 피드백</param>
        public void Setting_Diseased(XRST_PatientSetting setting1, Action<List<State>> _state, Action<List<State>> _bleeding)
        {
            List<State> state = new List<State>();
            List<State> bleedingState = new List<State>();

            for (int cnt = 0; cnt < setting1.diseased.Count; cnt++)
            {
                // (인스펙터상에 체크된 것 & Bleeding) == Bleeding : Bleeding 값이 존재하는지 확인
                bool bleeding = (setting1.diseased[cnt] & DiseasedCode.Bleeding) == DiseasedCode.Bleeding;
                bool dressing = (setting1.diseased[cnt] & DiseasedCode.Dressing) == DiseasedCode.Dressing;
                bool splint = (setting1.diseased[cnt] & DiseasedCode.Splint) == DiseasedCode.Splint;
                bool dressing_2nd = (setting1.diseased[cnt] & DiseasedCode.Dressing_2nd) == DiseasedCode.Dressing_2nd;
                bool guaze = (setting1.diseased[cnt] & DiseasedCode.Guaze) == DiseasedCode.Guaze;

                State find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.Bleeding));
                if (find != null)
                {
                    // 옷 State가 아니라 해당하는 아이템에 그냥 통용되는 State
                    find.state = bleeding ? ClothStateCode.On : ClothStateCode.Off;
                    bleedingState.Add(find);
                }
                find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.Gauze_4x4));
                if (find != null)
                {
                    find.state = guaze ? ClothStateCode.On : ClothStateCode.Off;
                    state.Add(find);

                    if (find.state == ClothStateCode.On && diseasedStatus[cnt].lastUsedItem.Find(x => x.Equals(find)) == null)
                        diseasedStatus[cnt].lastUsedItem.Add(find);
                }

                find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.ElasticBandage));
                if (find != null)
                {
                    find.state = dressing ? ClothStateCode.On : ClothStateCode.Off;
                    state.Add(find);

                    if (find.state == ClothStateCode.On && diseasedStatus[cnt].lastUsedItem.Find(x => x.Equals(find)) == null)
                        diseasedStatus[cnt].lastUsedItem.Add(find);
                }

                find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.ElasticBandage_2nd));
                if (find != null)
                {
                    find.state = dressing_2nd ? ClothStateCode.On : ClothStateCode.Off;
                    state.Add(find);

                    if (find.state == ClothStateCode.On && diseasedStatus[cnt].lastUsedItem.Find(x => x.Equals(find)) == null)
                        diseasedStatus[cnt].lastUsedItem.Add(find);
                }

                find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.Splint_Arm));
                if (find != null)
                {
                    find.state = splint ? ClothStateCode.On : ClothStateCode.Off;
                    state.Add(find);

                    if (find.state == ClothStateCode.On && diseasedStatus[cnt].lastUsedItem.Find(x => x.Equals(find)) == null)
                        diseasedStatus[cnt].lastUsedItem.Add(find);
                }
                find = diseasedStatus[cnt].stateList.Find(x => x.itemCode.Equals(PropCode.Splint_Leg));
                if (find != null)
                {
                    find.state = splint ? ClothStateCode.On : ClothStateCode.Off;
                    state.Add(find);

                    if (find.state == ClothStateCode.On && diseasedStatus[cnt].lastUsedItem.Find(x => x.Equals(find)) == null)
                        diseasedStatus[cnt].lastUsedItem.Add(find);
                }
            }

            _bleeding?.Invoke(bleedingState);
            _state?.Invoke(state);
        }
        /// <summary>
        /// 콜라이더 활성화용, 아이템 목록을 받아 콜라이더를 활성화 시킬때 사용, 미션시작시 호출됨
        /// </summary>
        /// <param name="items">현재 미션에서 사용되는 모든 아이템</param>
        /// <param name="activeColliders">활성화되는 아이템 목록 피드백</param>
        public void ActiveCollider(XRST_ID missionID, (ActionType, XRST_Item)[] items, Action<XRST_ID, List<XRST_Item>> activeColliders)
        {
            oldUsedItem = null;

            if (items == null)
            {
                activeColliders?.Invoke(missionID, null);
                return;
            }

            List<XRST_Item> list = new List<XRST_Item>();
            for (int cnt = 0; cnt < items.Length; cnt++)
            {
                if (items[cnt].Item2.IsCloth)
                    list.AddRange(ChangeClothCode(items[cnt].Item2));
                else if (items[cnt].Item2.IsArea)
                    list.AddRange(ChangeAreaCode(items[cnt].Item2));
                else if (items[cnt].Item2.IsBody)
                    list.Add(ChagneBodyCode(items[cnt].Item2));
            }

            activeColliders?.Invoke(missionID, list);
        }
        /// <summary>
        /// 콜라이더 용 변환
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<XRST_Item> ChangeAreaCode(XRST_Item item)
        {
            List<XRST_Item> change = new List<XRST_Item>();
            switch (item.Area)
            {
                case AreaCode.BPCuff:

                    Status cloth = clothState.Find(x => x.Cloth.itemCode.Cloth == ClothCode.Top);
                    if (cloth.Cloth.state == ClothStateCode.Off)
                    {
                        if (setBPCuff == false && division.BPcuffPos() == Pos.Other)// 환자의 고정된 위치 값이 들어옴
                        {
                            change.Add(new XRST_Item() { Area = item.Area, value = 0 });
                            change.Add(new XRST_Item() { Area = item.Area, value = 1 });
                        }
                        else
                            change.Add(new XRST_Item() { Area = item.Area, value = (int)bpcuffPosition });
                    }
                    else
                        change.Add(new XRST_Item() { Area = item.Area, value = (int)bpcuffPosition });
                    break;
                case AreaCode.PulseOXimetry: goto case AreaCode.InjectionSite;
                case AreaCode.Torniquet: goto case AreaCode.InjectionSite;
                case AreaCode.CATArm: goto case AreaCode.InjectionSite;
                case AreaCode.CATLeg: goto case AreaCode.InjectionSite;
                case AreaCode.CompartmentPressure: goto case AreaCode.InjectionSite;
                case AreaCode.InjectionSite:
                    Status find = GetMatchingAreaItem(item.Area, item.value);
                    if (find != null)
                    {
                        change.Add(new XRST_Item() { Area = item.Area, value = find.bodyCode.value });
                    }
                    else
                    {
                        change.Add(new XRST_Item() { Area = item.Area, value = 0 });

                    }
                    break;
                default:
                    change.Add(new XRST_Item() { Area = item.Area, value = item.value });
                    break;
            }

            return change;
        }
        /// <summary>
        /// 변경 확인 및 변경
        /// </summary>
        /// <param name="item">변결 확인할 아이템</param>
        /// <param name="find">찾은 결과</param>
        /// <returns></returns>
        public List<XRST_Item> ChangeClothCode(XRST_Item item)
        {
            List<Status> find = new List<Status>();

            List<XRST_Item> change = new List<XRST_Item>();

            if (item.IsCloth)
            {
                switch (item.Cloth)
                {
                    case ClothCode.TheDiseasedPart:
                        find.Add(GetMatchingAreaItem(AreaCode.TheDiseasedPart, item.value));
                        Debug.Log($"[Find: {item} => [{find.Count}]/{string.Join(",", find)}");
                        break;
                    case ClothCode.BPCuff: find.AddRange(GetMatchingAllItem(AreaCode.BPCuff)); break;
                    case ClothCode.InjectionAndTourniquet: find.Add(GetMatchingAreaItem(AreaCode.Torniquet, item.value)); break;
                    default: find.Add(GetMatchingItem(item.Cloth, item.value)); break;

                    case ClothCode.Pants: goto case ClothCode.Top;
                    case ClothCode.Top: find.Add(GetCloth(item.Cloth, Pos.None)); break;
                }
                for (int cnt = 0; cnt < find.Count; cnt++)
                {
                    switch (item.Cloth)
                    {
                        case ClothCode.Top: goto case ClothCode.Pants;
                        case ClothCode.Pants:
                            change.Add(new XRST_Item()
                            {
                                Cloth = find[cnt].Cloth.itemCode.Cloth,
                                value = item.value,
                            });
                            break;

                        default:
                            change.Add(new XRST_Item()
                            {
                                Cloth = find[cnt].Cloth.itemCode.Cloth,
                                value = find[cnt].Cloth.itemCode.value,
                            });
                            break;
                    }
                }
            }

            return change;
        }
        /// <summary>
        /// 변경 확인 및 변경
        /// </summary>
        /// <param name="item">변결 확인할 아이템</param>
        /// <param name="find">찾은 결과</param>
        /// <returns></returns>
        public XRST_Item ChangeClothCode(XRST_Item item, out Status find)
        {
            find = new Status();

            XRST_Item change = new XRST_Item();

            if (item.IsCloth)
            {
                Debug.Log($"[XRST_PatientStatus/ChangeClothCode] item:{item.ToFullName()}");

                switch (item.Cloth)
                {
                    case ClothCode.TheDiseasedPart: find = GetMatchingAreaItem(AreaCode.TheDiseasedPart, item.value); break;
                    case ClothCode.BPCuff: find = GetMatchingAreaItem(BodyCode.Arm, item.value, AreaCode.BPCuff, 0); break;
                    case ClothCode.InjectionAndTourniquet: find = GetMatchingAreaItem(AreaCode.Torniquet, item.value); break;
                    default: find = GetMatchingItem(item.Cloth, item.value); break;

                    case ClothCode.Pants: goto case ClothCode.Top;
                    case ClothCode.Top: find = GetCloth(item.Cloth, Pos.None); break;
                }

                if (find == null)
                {
                    Debug.Log($"[XRST_PatientStatus/ChangeClothCode] {item.ToFullName()} is not Found.");
                    return null;
                }
                else
                    Debug.Log($"[XRST_PatientStatus/ChangeClothCode] item:{item.ToFullName()}, find:{find.bodyCode}//(stateList:{find.stateList.Count})");

                switch (item.Cloth)
                {
                    case ClothCode.Top: goto case ClothCode.Pants;
                    case ClothCode.Pants:
                        change = new XRST_Item()
                        {
                            Cloth = find.Cloth.itemCode.Cloth,
                            value = item.value,
                        };
                        break;

                    default:

                        change = new XRST_Item()
                        {
                            Cloth = find.Cloth.itemCode.Cloth,
                            value = find.Cloth.itemCode.value,
                        };
                        break;
                }
            }

            return change;
        }
        public XRST_Item ChagneBodyCode(XRST_Item item)
        {
            switch (item.Body)
            {
                default:
                    return item;
            }
        }
        /// <summary>
        /// 아이템 사용 정보 기록용
        /// </summary>
        /// <param name="item">사용한 아이템</param>
        /// <param name="activeItem">사용한 아이템의 상태를 이벤트로 전달</param>
        public void SetState((XRST_ID, ActionType, XRST_Item) item, Status _status, Action<State[]> activeItem)
        {
            // XRST_Item.value는 위치값을 담고 있음
            Debug.Log($"[<color=orange>[XRST_PatientStatus/SetState] Input</color>: [{item.Item2}]{item.Item3.ToFullName()}.");
            if (item.Item2.IsTouch() && item.Item2 != ActionType.No_Interaction)//터치가 아니면 사용되지 않는다.
            {
                // 환자 적용된 도뇨관 적용
                if (item.Item3.IsProp && item.Item3.Prop == PropCode.FoleyCatheter_Applied)
                    item.Item3 = new XRST_Item { type = ItemType.Area, Area = AreaCode.Foley, value = 0 };

                if (item.Item3.IsArea)// Area일때 상태를 기록한다.
                {
                    //사용 후의 아이템은 value에 위치값을 가진다.
                    if (oldUsedItem == null || oldUsedItem.IsNone)//제거
                    {
                        int value = item.Item3.ISAreaBPCuff ? 0 : item.Item3.value;

                        //Status status = item.Item3.ISAreaBPCuff ?
                        //                GetMatchingAreaItem(BodyCode.Arm, item.Item3.value, AreaCode.BPCuff, 0):
                        //                GetMatchingAreaItem(item.Item3.Area, item.Item3.value);
                        Status status = GetMatchingAreaItem(item.Item3.Area, value);
                        if (status != null)
                        {
                            if (status.FindLastUsedItem(item.Item3.Area, out State lastUsed))
                            {
                                if (lastUsed.areaCode.Equals(item.Item3.Area, value))
                                {
                                    lastUsed.state = ClothStateCode.Off;

                                    lastUsed = ChangeState(lastUsed, item.Item3, (int)bpcuffPosition);

                                    //status.RemoveLastItem();
                                    status.RemoveItem(lastUsed);

                                    activeItem?.Invoke(new State[] { lastUsed });
                                    Debug.Log($"[<color=yellow>[XRST_PatientStatus/SetState] Remove Item</color>: {item.Item3.ToFullName()}. =>{lastUsed}");
                                }
                                else
                                    Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] Remove Fail</color> State Not Found in Status. ({status}=>{item.Item3.ToFullName()}).");
                            }
                            else
                                Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] Remove Fail</color> Not Found Last Used Item. ({item.Item3.ToFullName()}).");
                        }
                        else
                            Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] Remove Fail</color> Status Not Found ({item.Item3.ToFullName()}-({value})).");
                    }
                    else//설치
                    {
                        Status find;
                        State state = null;
                        if (item.Item3.IsAreaFluidItem)
                        {
                            XRST_Item area = new XRST_Item();
                            area.Copy(item.Item3);
                            area.value = item.Item3.ISAreaBPCuff ? 0 : item.Item3.value;
                            XRST_Item prop = new XRST_Item();
                            prop.Copy(oldUsedItem);
                            prop.value = item.Item3.ISAreaBPCuff ? item.Item3.value : (int)bpcuffPosition;

                            find = GetMatchingItem(area, prop);

                            if (find != null)
                            {
                                find.ItemUsed(area, prop, out state);
                            }
                            else
                                Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] Use Fail</color>: {area.ToFullName()}/{prop.ToFullName()} is Not Found.");
                        }
                        else
                        {
                            if (item.Item3.IsProp && item.Item3.Prop == PropCode.FoleyCatheter_Applied)
                                Debug.Log($"[<color=magenta>{item.Item3}/ {oldUsedItem} 2</color>");

                            find = GetMatchingItem(item.Item3/*AreaCode임*/);
                            Debug.Log($"[<color=yellow>{item.Item3}/ {oldUsedItem}</color>");
                            if (find != null)
                            {

                                find.ItemUsed(item.Item3, oldUsedItem, out state);
                                if (item.Item3 != null || oldUsedItem != null)
                                {
                                    Debug.Log($"[<color=cyan>[XRST_PatientStatus/SetState] Find Debug</color>: {item.Item3.ToFullName()}/{oldUsedItem.ToFullName()}");
                                }
                            }
                            else
                                Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] Use Fail</color>: {item.Item3.ToFullName()}/{oldUsedItem.ToFullName()} is Not Found.");
                        }

                        if (state == null)
                            Debug.Log($"[<color=red>[XRST_PatientStatus/SetState] state is Null.</color>: {item.Item3} => {oldUsedItem}.");
                        else
                        {
                            State changeState = ChangeState(state, item.Item3, (int)bpcuffPosition);

                            activeItem?.Invoke(new State[] { changeState });

                            Debug.Log($"[<color=yellow>[XRST_PatientStatus/SetState] Apply Item</color>: {item.Item3}={oldUsedItem} => {state}=>{changeState}.");
                        }
                    }

                    oldUsedItem = null;
                }
                else if (item.Item3.IsCloth)
                {
                    if (item.Item3.Check(ClothCode.Top) ||
                        item.Item3.Check(ClothCode.Pants))
                    {
                        Status find = GetCloth(item.Item3.Cloth, Pos.None);

                        find.Cloth.ClothState(item.Item3.value);
                        List<State> clothStates = UpdateClothState();
                        clothStates.Add(find.Cloth);

                        activeItem?.Invoke(clothStates.ToArray());
                    }
                    else
                    {
                        if (_status == null)
                            ChangeClothCode(item.Item3, out Status find);

                        if (_status != null)
                        {
                            List<State> states = new List<State>();
                            _status.Cloth.ClothState();
                            states.AddRange(SetClothLink(_status.Cloth));

                            if (_status.IsBpCuff && setBPCuff == false && IsDoubleBPcuff)// 환자의 고정된 위치 값이 들어옴
                            {
                                Debug.Log($"[XRST_PatientStatus/SetState]: {_status.bodyCode.ToFullName()}.");
                                bpcuffPosition = _status.bodyCode.value == 0 ? Pos.L : Pos.R;
                                SetOtherProp(bpcuffPosition);
                            }

                            Debug.Log($"[XRST_PatientStatus/SetState] {item.Item3.ToFullName()}=>[{states.Count}]{string.Join<State>(", ", states)}.");

                            activeItem?.Invoke(states.ToArray());
                        }
                    }
                }
                else if (item.Item3.IsProp)// Prop인 경우에만 이전 사용 기록을 남긴다.
                {
                    for (int cnt = 0; cnt < statusList.Count; cnt++)
                    {
                        for (int cnt_B = 0; cnt_B < statusList[cnt].stateList.Count; cnt_B++)
                        {
                            if (statusList[cnt].stateList[cnt_B].itemCode.EqualsShort(item.Item3))
                            {
                                if (item.Item3.IsFluidItem_NoBP)
                                {
                                    oldUsedItem = new XRST_Item();
                                    oldUsedItem.Copy(item.Item3);
                                    oldUsedItem.value = (int)bpcuffPosition;
                                }
                                else
                                {
                                    oldUsedItem = item.Item3;
                                }

                                Debug.Log($"[XRST_PatientStatus/SetState] oldUsedItem: {item.Item3.ToFullName()}.");

                                break;
                            }
                        }
                    }
                }
            }
        }

        private State ChangeState(State copy, XRST_Item areaCode, int newValue)
        {
            State changeState = new State//값 복사
            {
                state = copy.state,
                areaCode = new XRST_Item()
                {
                    type = copy.areaCode.type,
                    code = copy.areaCode.code,
                },
                itemCode = new XRST_Item()
                {
                    type = copy.itemCode.type,
                    code = copy.itemCode.code,
                }
            };

            switch (areaCode.Area)
            {
                case AreaCode.BPCuff:
                    if (setBPCuff == false && IsDoubleBPcuff)
                    {
                        bpcuffPosition = areaCode.value == 0 ? Pos.L : Pos.R;
                        newValue = areaCode.value;
                        changeState.areaCode.value = 0;
                        SetOtherProp(bpcuffPosition);
                    }
                    changeState.itemCode.value = newValue;
                    break;

                case AreaCode.PulseOXimetry:
                    goto case AreaCode.InjectionSite;
                case AreaCode.Torniquet:
                    goto case AreaCode.InjectionSite;
                case AreaCode.InjectionSite:
                    changeState.areaCode.value = copy.areaCode.value;
                    changeState.itemCode.value = newValue;
                    break;

                default:
                    changeState.areaCode.value = copy.areaCode.value;
                    changeState.itemCode.value = copy.itemCode.value;
                    break;
            }

            return changeState;
        }
        public List<State> RemoveBPCuff(Pos pos)
        {
            for (int cnt = 0; cnt < bpcuffStatus.Count; cnt++)
            {
                bpcuffStatus[cnt].RemoveLastItem();
            }
            for (int cnt = 0; cnt < liquidityItemList.Count; cnt++)
            {
                if (statusList.TryFindAll(out List<Status> findesStatus, x => x.GetArea(liquidityItemList[cnt]) != null))
                {
                    for (int cnt_A = 0; cnt_A < findesStatus.Count; cnt_A++)
                    {
                        findesStatus[cnt_A].RemoveAreaAndLastItem(liquidityItemList[cnt]);
                    }
                }
            }
            List<State> remove = new List<State>();
            for (int cnt = 0; cnt < liquidityItemList.Count; cnt++)
            {
                remove.Add(new State() { areaCode = XRST_Item.ToItem(ItemType.Area, (int)liquidityItemList[cnt]),
                                         itemCode = XRST_Item.ToItem(ItemType.Prop, (int)XRST_Item.AreaToProp(liquidityItemList[cnt], division.scenario)),
                                         state = ClothStateCode.Off });
            }

            Debug.Log(string.Join(",", remove));

            bpcuffStatus.Clear();

            Status firstBPCuff = GetBody(BodyCode.Arm, Pos.LU);
            Status secondBPCuff = GetBody(BodyCode.Arm, Pos.RU);

            firstBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 0));
            secondBPCuff.AddPropState((AreaCode.BPCuff, 0), (PropCode.BPCuff, 1));

            bpcuffStatus.Add(firstBPCuff);
            bpcuffStatus.Add(secondBPCuff);

            bpcuffPosition = pos;
            setBPCuff = false;

            return remove;
        }
        /// <summary>
        /// BPCuff를 사용하기 위해 옷을 제거 하면 나머지 Prop의 위치를 특정한다.
        /// 경우에 따라 BPCuff가 양팔에 설치 가능 한 경우가 있어 BPCuff가 착용된 후에 값을 설정한다.
        /// </summary>
        /// <param name="value">사용한 BPCuff의 위치</param>
        public void SetOtherProp(Pos bp)
        {
            if (bp == Pos.None)
            {
                setBPCuff = false;

                return;
            }

            setBPCuff = true;

            int newBP = (int)bp;

            Debug.Log($"[XRST_PatientStatus/SetOderProp] set BP (Pos: {bp} => Num: {newBP})");

            if (1 < bpcuffStatus.Count)
            {
                //bpcuffStatus[newBP].GetArea(AreaCode.BPCuff).areaCode.value = 0;

                int other = newBP == 0 ? 1 : 0;

                bpcuffStatus[other].RemoveAreaState(AreaCode.BPCuff);
                bpcuffStatus.RemoveAt(other);
            }

            List<(AreaCode, int, BodyCode, Pos)> divisionToProp = division.ToProp(newBP);

            for (int cnt = 0; cnt < divisionToProp.Count; cnt++)
            {
                PropCode propCode = PropCode.None;
                switch (divisionToProp[cnt].Item1)
                {
                    case AreaCode.Torniquet:
                        propCode = PropCode.Tourniquet;
                        break;
                    case AreaCode.InjectionSite:
                        propCode = PropCode.Catheter;
                        break;
                    case AreaCode.PulseOXimetry:
                        propCode = PropCode.Pulse_OX_imetry;
                        break;
                }

                Pos itemPos;
                if (3 < (int)divisionToProp[cnt].Item4)
                    itemPos = divisionToProp[cnt].Item4 - 4;
                else
                    itemPos = divisionToProp[cnt].Item4;

                GetBody(divisionToProp[cnt].Item3, itemPos)
                    .AddPropState((divisionToProp[cnt].Item1, divisionToProp[cnt].Item2), (propCode, newBP));

                if (division.scenario == ScenarioCode.ER && divisionToProp[cnt].Item1 == AreaCode.PulseOXimetry)
                {
                    GetBody(divisionToProp[cnt].Item3, itemPos)
                        .AddPropState((divisionToProp[cnt].Item1, divisionToProp[cnt].Item2), (PropCode.Pulse_OX_imetry_H, newBP));
                }
            }
        }
        /// <summary>
        /// 옷 설정, 상의와 하의의 상태에 맞게 옷의 상태가 결정되며 도구 상태에 따라서도 결정된다.
        /// </summary>
        /// <returns></returns>
        private List<State> UpdateClothState()
        {
            List<State> states = new List<State>();

            BodyPosition pos = division.ToAmputationPosition();

            //상의
            ClothStateCode topState = GetCloth(ClothCode.Top).Cloth.state;
            Status armLU = GetCloth(ClothCode.Arm, Pos.LU);
            Status armLD = GetCloth(ClothCode.Arm, Pos.LD);
            Status armRU = GetCloth(ClothCode.Arm, Pos.RU);
            Status armRD = GetCloth(ClothCode.Arm, Pos.RD);

            states.Add(armLU.Cloth);
            states.Add(armLD.Cloth);
            states.Add(armRU.Cloth);
            states.Add(armRD.Cloth);

            armLD.SetClothState(armLU.SetClothState(topState), pos == BodyPosition.ArmLU);
            armRD.SetClothState(armRU.SetClothState(topState), pos == BodyPosition.ArmRU);

            //하의
            ClothStateCode pantsState = GetCloth(ClothCode.Pants).Cloth.state;
            Status legLU = GetCloth(ClothCode.Leg, Pos.LU);
            Status legLD = GetCloth(ClothCode.Leg, Pos.LD);
            Status legRU = GetCloth(ClothCode.Leg, Pos.RU);
            Status legRD = GetCloth(ClothCode.Leg, Pos.RD);
            Status shoesL = GetCloth(ClothCode.Shoes, Pos.L);
            Status shoesR = GetCloth(ClothCode.Shoes, Pos.R);

            states.Add(legLU.Cloth);
            states.Add(legLD.Cloth);
            states.Add(legRU.Cloth);
            states.Add(legRD.Cloth);
            states.Add(shoesL.Cloth);
            states.Add(shoesR.Cloth);

            if (pantsState == ClothStateCode.Open)
            {
                Debug.Log($"[XRST_PatientStatus/UpdateClothState] {pos}");

                legLU.SetClothState(ClothStateCode.Off);
                if ((pos & BodyPosition.LegLU) == BodyPosition.LegLU)
                    shoesL.SetClothState(legLD.SetClothState(ClothStateCode.Off));
                else
                    shoesL.SetClothState(legLD.SetClothState(ClothStateCode.On));

                legRU.SetClothState(ClothStateCode.Off);
                if ((pos & BodyPosition.LegRU) == BodyPosition.LegRU)
                    shoesR.SetClothState(legRD.SetClothState(ClothStateCode.Off));
                else
                    shoesR.SetClothState(legRD.SetClothState(ClothStateCode.On));
            }
            else
            {
                shoesL.SetClothState(legLD.SetClothState(legLU.SetClothState(pantsState), pos == BodyPosition.LegLU));
                shoesR.SetClothState(legRD.SetClothState(legRU.SetClothState(pantsState), pos == BodyPosition.LegRU));
            }

            return states;
        }
        private State[] SetOtherCloth(State state)
        {
            if (state.itemCode.Cloth == ClothCode.Top)
            {
                ClothStateCode stateCode = state.state == ClothStateCode.Off ? ClothStateCode.Off : ClothStateCode.On;

                Status lu = GetCloth(ClothCode.Arm, Pos.LU);
                Status ld = GetCloth(ClothCode.Arm, Pos.LD);
                Status ru = GetCloth(ClothCode.Arm, Pos.RU);
                Status rd = GetCloth(ClothCode.Arm, Pos.RD);

                lu.SetClothState(stateCode);
                ld.SetClothState(stateCode);
                ru.SetClothState(stateCode);
                rd.SetClothState(stateCode);

                return new State[] { lu.Cloth, ld.Cloth, ru.Cloth, rd.Cloth };
            }
            else
            {
                Status lu = GetCloth(ClothCode.Leg, Pos.LU);
                Status ld = GetCloth(ClothCode.Leg, Pos.LD);
                Status ls = GetCloth(ClothCode.Shoes, Pos.L);
                Status ru = GetCloth(ClothCode.Leg, Pos.RU);
                Status rd = GetCloth(ClothCode.Leg, Pos.RD);
                Status rs = GetCloth(ClothCode.Shoes, Pos.R);

                if (state.state == ClothStateCode.Open)
                {
                    lu.SetClothState(ClothStateCode.Off);
                    ld.SetClothState(ClothStateCode.On);
                    ls.SetClothState(ClothStateCode.On);

                    ru.SetClothState(ClothStateCode.Off);
                    rd.SetClothState(ClothStateCode.On);
                    rs.SetClothState(ClothStateCode.On);
                }
                else
                {
                    lu.SetClothState(state.state);
                    ld.SetClothState(state.state);
                    ls.SetClothState(state.state);

                    ru.SetClothState(state.state);
                    rd.SetClothState(state.state);
                    rs.SetClothState(state.state);
                }

                return new State[] { lu.Cloth, ld.Cloth, ls.Cloth, ru.Cloth, rd.Cloth, rs.Cloth };
            }
        }
        private State[] SetClothLink(State code)
        {
            Pos pos = code.itemCode.IsLeft ? Pos.L : Pos.R;
            Pos U = code.itemCode.IsLeft ? Pos.LU : Pos.RU;
            Pos D = code.itemCode.IsLeft ? Pos.LD : Pos.RD;

            Status u = GetCloth(code.itemCode.Cloth, U);
            Status d = GetCloth(code.itemCode.Cloth, D);
            Status s = null;
            if (code.itemCode.Cloth == ClothCode.Arm)
            {
                if (code.itemCode.IsUpper)
                {
                    d.SetClothState(code.state);

                    return new State[] { u.Cloth, d.Cloth };
                }
                else
                {
                    return new State[] { d.Cloth };
                }
            }
            if (code.itemCode.Equals(ClothCode.Shoes))
            {
                s = GetCloth(ClothCode.Shoes, pos);
                return new State[] { s.Cloth };
            }
            else
            {
                s = GetCloth(ClothCode.Shoes, pos);

                if (code.itemCode.IsUpper)
                {
                    d.SetClothState(code.state);
                    s.SetClothState(code.state);

                    return new State[] { u.Cloth, d.Cloth, s.Cloth };
                }
                else
                {
                    s.SetClothState(code.state);

                    return new State[] { d.Cloth, s.Cloth };
                }
            }
        }

        /// <summary>
        /// 사용가능한 아이템 코드로 변경
        /// </summary>
        /// <param name="original">Hololens로 부터 들어온 신호</param>
        /// <param name="actionType">original의 액션 종류</param>
        /// <returns></returns>
        public XRST_Item ChangeItemCode(XRST_Item original, out Status find)
        {
            State findedState;

            switch (original.type)
            {
                case ItemType.Cloth:
                    switch (original.Cloth)
                    {
                        case ClothCode.Arm:
                            find = GetBody(BodyCode.Arm, original.value);
                            if (find.GetArea(AreaCode.TheDiseasedPart, out findedState))
                                return new XRST_Item() { Cloth = ClothCode.TheDiseasedPart, value = findedState.areaCode.value };
                            else if (find.GetArea(AreaCode.BPCuff, out _))
                                return new XRST_Item() { Cloth = ClothCode.BPCuff, value = 0 };
                            //{
                            //    if (division.BPcuffPos() == Pos.Other)
                            //        return new XRST_Item() { Cloth = ClothCode.BPCuff, value = original.value };
                            //    else
                            //        return new XRST_Item() { Cloth = ClothCode.BPCuff, value = 0 };
                            //}
                            else if (find.GetArea(AreaCode.InjectionSite, out findedState))
                                return new XRST_Item() { Cloth = ClothCode.InjectionAndTourniquet, value = findedState.areaCode.value };
                            else
                                goto default;
                        case ClothCode.Leg:
                            find = GetBody(BodyCode.Leg, original.value);
                            if (find.GetArea(AreaCode.TheDiseasedPart, out findedState))
                                return new XRST_Item() { Cloth = ClothCode.TheDiseasedPart, value = findedState.areaCode.value };
                            else if (find.GetArea(AreaCode.InjectionSite, out findedState))
                                return new XRST_Item() { Cloth = ClothCode.InjectionAndTourniquet, value = findedState.areaCode.value };
                            else
                                goto default;
                        case ClothCode.Top:
                            find = null;
                            return new XRST_Item() { Cloth = ClothCode.Top, value = original.value };
                        default:
                            find = null;
                            return original;
                    }
                case ItemType.Area:
                    List<Status> statuses = new List<Status>();
                    switch (original.Area)
                    {
                        //case AreaCode.BPCuff:
                        //    return new XRST_Item() { Area = AreaCode.BPCuff, value = 0 };

                        case AreaCode.InjectionSite:
                            statuses = GetMatchingAllItem(AreaCode.InjectionSite);
                            int value = 0;
                            for (int cnt = 0; cnt < statuses.Count; cnt++)
                            {
                                if (original.value == 0 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Arm && statuses[cnt].bodyCode.IsLeft)
                                {
                                    // HL injection-0이고 검사중인 statuses[cnt]가 왼팔인 경우
                                    value = cnt;
                                }
                                if (original.value == 1 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Arm && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    // HL injection-1이고 검사중인 statuses[cnt]가 오른팔인 경우
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 2 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                {
                                    // HL injection-2이고 검사중인 statuses[cnt]가 왼다리인 경우
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 3 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    // HL injection-3이고 검사중인 statuses[cnt]가 오른다리인 경우
                                    value = cnt;
                                    break;
                                }
                            }
                            find = statuses[value];
                            return find.GetArea(AreaCode.InjectionSite).areaCode;
                        case AreaCode.Torniquet:
                            statuses = GetMatchingAllItem(AreaCode.Torniquet);
                            value = 0;
                            for (int cnt = 0; cnt < statuses.Count; cnt++)
                            {
                                if (original.value == 0 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Arm && statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 1 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Arm && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 2 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 3 &&
                                    statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                            }
                            find = statuses[value];
                            return find.GetArea(AreaCode.Torniquet).areaCode;
                        case AreaCode.PulseOXimetry:
                            find = GetMatchingAreaItem(AreaCode.PulseOXimetry);
                            return find.GetArea(AreaCode.PulseOXimetry).areaCode;
                        case AreaCode.CATArm:
                            statuses = GetMatchingAllItem(AreaCode.CATArm);
                            value = 0;
                            for (int cnt = 0; cnt < statuses.Count; cnt++)
                            {
                                if (original.value == 0 && statuses[cnt].bodyCode.Body == BodyCode.Arm && statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 1 && statuses[cnt].bodyCode.Body == BodyCode.Arm && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 2 && statuses[cnt].bodyCode.Body == BodyCode.Arm && statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                                if (original.value == 3 && statuses[cnt].bodyCode.Body == BodyCode.Arm && !statuses[cnt].bodyCode.IsLeft)
                                {
                                    value = cnt;
                                    break;
                                }
                            }
                            find = statuses[value];
                            return find.GetArea(AreaCode.CATArm).areaCode;

                        case AreaCode.CATLeg:
                            statuses = GetMatchingAllItem(AreaCode.CATLeg);
                            value = 0;
                            if (statuses.Count == 1) // 환부 1개
                            {
                                for (int cnt = 0; cnt < statuses.Count; cnt++)
                                {
                                    if (original.value == 0 && statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 1 && statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 2 && statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 3 && statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                }
                                find = statuses[value];
                                return find.GetArea(AreaCode.CATLeg).areaCode;
                            }
                            else // 환부 2개
                            {
                                for (int cnt = statuses.Count - 1; cnt >= 0; cnt--) // 역 For문(큰 환부가 제일 끝에 있어서)
                                {
                                    if (original.value == 0 && statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 1 && statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 2 && statuses[cnt].bodyCode.Body == BodyCode.Leg && statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                    if (original.value == 3 && statuses[cnt].bodyCode.Body == BodyCode.Leg && !statuses[cnt].bodyCode.IsLeft)
                                    {
                                        value = cnt;
                                        break;
                                    }
                                }
                                find = statuses[value];
                                return find.GetArea(AreaCode.CATLeg).areaCode;
                            }
                        default:
                            find = null;
                            return original;
                    }
                case ItemType.Body:
                    switch (original.Body)
                    {
                        case BodyCode.Ear:
                            find = null;
                            return new XRST_Item() { Body = BodyCode.Ear, value = 0 };
                        default:
                            find = null;
                            return original;
                    }
                default:
                    find = null;
                    return original;
            }
        }
    }


#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(XRST_PatientStatus))]
    public class XRST_PatientStatusEditor : XRST_DataEditor
    {
        private XRST_PatientStatus Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as XRST_PatientStatus;

                return m_target;
            }
        }
        private XRST_PatientStatus m_target;
        private GUIContent noLabel = new GUIContent();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            noLabel.text = "";

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SerializedProperty oldUsedItem = serializedObject.FindProperty("oldUsedItem");
            GUI.enabled = false;
            EditorGUILayout.PropertyField(oldUsedItem);
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SerializedProperty statusList = serializedObject.FindProperty("statusList");
            statusList.isExpanded = GUILayout.Toggle(statusList.isExpanded, "Status List", EditorStyles.foldout, DefWidthLayout);
            if (statusList.isExpanded)
            {
                for (int cnt = 0; cnt < statusList.arraySize; cnt++)
                {
                    DrawStatus(statusList.GetArrayElementAtIndex(cnt), cnt);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SerializedProperty clothState = serializedObject.FindProperty("clothState");
            clothState.isExpanded = GUILayout.Toggle(clothState.isExpanded, "Cloth State", EditorStyles.foldout, DefWidthLayout);
            if (clothState.isExpanded)
            {
                for (int cnt = 0; cnt < clothState.arraySize; cnt++)
                {
                    DrawStatus(clothState.GetArrayElementAtIndex(cnt), cnt, true);
                }
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

        public void DrawStatus(SerializedProperty _status, int _cnt, bool onlyCloth = false)
        {
            SerializedProperty isBpCuff = _status.FindPropertyRelative("IsBpCuff");
            SerializedProperty isDiseased = _status.FindPropertyRelative("IsDiseased");

            SerializedProperty bodyCode = _status.FindPropertyRelative("bodyCode");
            SerializedProperty stateList = _status.FindPropertyRelative("stateList");
            SerializedProperty Cloth = _status.FindPropertyRelative("Cloth");
            SerializedProperty lastUsedItem = _status.FindPropertyRelative("lastUsedItem");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    float width = EditorGUIUtility.currentViewWidth - DefForwardMargin - (8 * DefSpacingBetweenField) - DefaultLabelWidth - 36;

                    if (!_status.isExpanded)
                        width /= 2;

                    EditorGUILayout.BeginHorizontal();
                    if (onlyCloth)
                    {
                        EditorGUILayout.LabelField($"[{_cnt:00}] Cloth State", DefWidthLayout);
                        _status.isExpanded = false;
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        _status.isExpanded = GUILayout.Toggle(_status.isExpanded, $"[{_cnt:00}] Body Code", EditorStyles.foldout, DefWidthLayout);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUIUtility.labelWidth = 1;
                    GUI.enabled = false;
                    if (_status.isExpanded)
                        EditorGUILayout.PropertyField(bodyCode);
                    else
                        EditorGUILayout.PropertyField(bodyCode, GUILayout.Width(width));
                    GUI.enabled = true;
                    EditorGUIUtility.labelWidth = originLabelWidth;
                    EditorGUILayout.EndHorizontal();

                    if (!_status.isExpanded)
                    {
                        EditorGUILayout.BeginHorizontal();
                        DrawState(Cloth, -1, false, width);
                        EditorGUIUtility.labelWidth = 1;
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(isBpCuff, GUILayout.Width(18));
                        EditorGUILayout.PropertyField(isDiseased, GUILayout.Width(18));
                        GUI.enabled = true;
                        EditorGUIUtility.labelWidth = originLabelWidth;
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (_status.isExpanded && !onlyCloth)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Cloth Code", DefWidthLayout);
                    DrawState(Cloth, -1, false);
                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.cyan;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(isBpCuff);
                        EditorGUILayout.PropertyField(isDiseased);
                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();
                        GUI.color = Color.white;

                        EditorGUI.indentLevel++;
                        stateList.isExpanded = EditorGUILayout.Foldout(stateList.isExpanded, "State List", true);
                        EditorGUI.indentLevel--;
                        if (stateList.isExpanded)
                        {
                            for (int cnt = 0; cnt < stateList.arraySize; cnt++)
                            {
                                DrawState(stateList.GetArrayElementAtIndex(cnt), cnt);
                            }
                        }

                    }
                    EditorGUILayout.EndVertical();

                    GUI.color = Color.cyan;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        GUI.color = Color.white;

                        EditorGUI.indentLevel++;
                        lastUsedItem.isExpanded = EditorGUILayout.Foldout(lastUsedItem.isExpanded, "Last Used Items", true);
                        EditorGUI.indentLevel--;
                        if (lastUsedItem.isExpanded)
                        {
                            for (int cnt = 0; cnt < lastUsedItem.arraySize; cnt++)
                            {
                                DrawState(lastUsedItem.GetArrayElementAtIndex(cnt), cnt, false);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndVertical();
        }
        public void DrawState(SerializedProperty _state, int num = -1, bool useAll = true, float singleWidth = -1)
        {
            SerializedProperty state = _state.FindPropertyRelative("state");
            SerializedProperty areaCode = _state.FindPropertyRelative("areaCode");
            SerializedProperty itemCode = _state.FindPropertyRelative("itemCode");

            if (useAll)
            {
                if (num != -1)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"[{num:00}]", GUILayout.Width(40));
                }
                else
                    EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 1;
                GUI.enabled = false;
                EditorGUILayout.PropertyField(areaCode);
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                EditorGUILayout.PropertyField(state, GUILayout.Width(40));
                switch (state.enumValueIndex)
                {
                    case 1: GUI.color = Color.green; break;
                    case 2: GUI.color = Color.gray; break;
                    case 3: GUI.color = Color.yellow; break;
                    default:
                        break;
                }
                EditorGUILayout.PropertyField(itemCode);
                GUI.color = originColor;
                GUI.enabled = true;
                EditorGUIUtility.labelWidth = originLabelWidth;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUIUtility.labelWidth = 1;
                GUI.enabled = false;
                switch (state.enumValueIndex)
                {
                    case 1: GUI.color = Color.green; break;
                    case 2: GUI.color = Color.gray; break;
                    case 3: GUI.color = Color.yellow; break;
                    default:
                        break;
                }
                if (singleWidth == -1)
                    EditorGUILayout.PropertyField(itemCode);
                else
                    EditorGUILayout.PropertyField(itemCode, GUILayout.Width(singleWidth));
                GUI.color = originColor;
                GUI.enabled = true;
                EditorGUIUtility.labelWidth = originLabelWidth;
            }
        }
    }
#endif
}