using System;
using System.Threading.Tasks;
using static DensoBoxTester.General;
using static DensoBoxTester.UiControl;
using static System.Threading.Thread;

namespace DensoBoxTester.TestItems
{
    static class SequenceCheck
    {
        public enum Title { LD1点灯, LD2点灯, CN4導通, LD2消灯_CN4非導通, LD3点灯, LD4点灯_BZオン, LD4消灯_BZオフ, CN2_AC24V出力1, CN2_AC24V出力2, CN2_AC24V_OFF }

        const int WaitTime = 15000;


        /// <summary>
        /// CN1の1,2ピンにAC24Vを印加しLD1が点灯すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckLd1On()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    PowSupply(true);
                    State.VmTestStatus.PicPath = "/Resources/PicLed/LD1ON.JPG";
                    State.VmTestStatus.Message = "LD1(緑)が点灯していますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                    State.VmTestStatus.PicPath = null;
                }
            });
        }

        /// <summary>
        /// CN5にDC12V印加し、LD2が点灯すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckLd2On()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetK2(true);
                    State.VmTestStatus.PicPath = "/Resources/PicLed/LD2ON.jpg";
                    State.VmTestStatus.Message = "LD2(緑)が点灯していますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                    State.VmTestStatus.PicPath = null;
                }
            });
        }

        /// <summary>
        /// CN7の1,2ピンをショートすると、CN4の3,4pinが導通すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn4_3_4_Short()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL1(true);
                    Sleep(1000);
                    var buff = ReadInput();
                    return buff[2] == '1';

                }
                finally
                {

                }
            });
        }

        /// <summary>
        /// CN5を無電圧にすると、LD2が消灯すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckLd2Off()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetK2(false);
                    State.VmTestStatus.PicPath = "/Resources/PicLed/LD1ON.jpg";
                    State.VmTestStatus.Message = "LD2(緑)が消灯していますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                    State.VmTestStatus.PicPath = null;
                }
            });
        }
        /// <summary>
        /// CN5を無電圧にすると、CN4の3,4pinが非導通すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn4_3_4_Open()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    var buff = ReadInput();
                    return buff[2] == '0';

                }
                finally
                {

                }
            });
        }


        /// <summary>
        /// CN4の5,6pinをショートすると、LD3が点灯すること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckLd3On()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL4(true);
                    State.VmTestStatus.PicPath = "/Resources/PicLed/LD3ON.JPG";
                    State.VmTestStatus.Message = "LD3(緑)が点灯していますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                    State.VmTestStatus.PicPath = null;
                }
            });
        }

        /// <summary>
        /// CN4の7,8pinをショートすると、LD4点灯,ブザーONすること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckLd4OnBzOn()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL5(true);
                    State.VmTestStatus.PicPath = "/Resources/PicLed/LD4ON.JPG";
                    State.VmTestStatus.Message = "LD4(赤)が点灯 ＆ ブザーが鳴っていますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                }
            });
        }

        /// <summary>
        /// ブザーが鳴っている状態でS1を押すとブザーが鳴り止むこと
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckBzOff()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    State.VmTestStatus.Message = "S1を押すとブザーがOFFしますか？";
                    PlaySoundAsync(soundNotice);
                    return CheckOkCancel();
                }
                finally
                {
                }
            });
        }

        /// <summary>
        /// CN4の1,2pinをショートすると、CN2の1,3間にAC24Vが出力されること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn2_1_3_Ac24vOn()
        {
            var Max = State.spec.Ac24vMax;
            var Min = State.spec.Ac24vMin;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL3(true);
                    SetK3(false);
                    Sleep(1000);
                    State.VolAc24v_1 = MeasAc24vR100();
                    return (Min < State.VolAc24v_1 && State.VolAc24v_1 < Max);

                }
                finally
                {
                    State.VmTestStatus.Spec = $"規格値： AC{Min.ToString("F1")}V ～ AC{Max.ToString("F1")}V";
                    State.VmTestStatus.MeasValue = $"計測値： AC{State.VolAc24v_1.ToString("F1")}V";
                }
            });
        }


        /// <summary>
        /// CN7の3,4pinをショートすると、CN2の2,4間にAC24Vが出力されること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn2_2_4_Ac24vOn()
        {
            var Max = State.spec.Ac24vMax;
            var Min = State.spec.Ac24vMin;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL2(true);
                    SetK3(true);
                    Sleep(1000);
                    State.VolAc24v_2 = MeasAc24vR100();
                    return (Min < State.VolAc24v_2 && State.VolAc24v_2 < Max);

                }
                finally
                {
                    State.VmTestStatus.Spec = $"規格値： AC{Min.ToString("F1")}V ～ AC{Max.ToString("F1")}V";
                    State.VmTestStatus.MeasValue = $"計測値： AC{State.VolAc24v_2.ToString("F2")}V";
                }
            });
        }

        /// <summary>
        /// CN4の1,2pinをオープンに戻すと、CN2の1,3間が無電圧になること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn2_1_3_Ac24vOff()
        {
            var Max = State.spec.Ac24vOffMax;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL3(false);
                    SetK3(false);
                    Sleep(1000);
                    State.VolAc24vOff_1 = MeasAc24vR10();
                    return (State.VolAc24vOff_1 < Max);

                }
                finally
                {
                    State.VmTestStatus.Spec = $"規格値： AC{Max.ToString("F1")}V以下";
                    State.VmTestStatus.MeasValue = $"計測値： AC{State.VolAc24vOff_1.ToString("F1")}V";

                }
            });
        }

        /// <summary>
        /// CN7の3,4pinをオープンに戻すと、CN2の2,4間が無電圧になること
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCn2_2_4_Ac24vOff()
        {
            var Max = State.spec.Ac24vOffMax;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    SetRL2(false);
                    SetK3(true);
                    Sleep(1000);
                    State.VolAc24vOff_2 = MeasAc24vR10();
                    return (State.VolAc24vOff_2 < Max);

                }
                finally
                {
                    State.VmTestStatus.Spec = $"規格値： AC{Max.ToString("F1")}V以下";
                    State.VmTestStatus.MeasValue = $"計測値： AC{State.VolAc24vOff_2.ToString("F1")}V";
                }
            });
        }


    }

}
