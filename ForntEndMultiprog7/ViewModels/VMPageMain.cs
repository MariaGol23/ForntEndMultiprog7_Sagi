using ForntEndMultiprog7.Model;
using LKDSFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
namespace ForntEndMultiprog7.ViewModels
{
    public class VMPageMain : BaseViewModel
    {
        #region Vars

        const string CanDevTxt = "Устройства CAN-шины.";
        const string ModeFat = "Продвинутый режим";
        const string ModeSimple = "Упрощенный режим";
        const string SendCmdOnSubdev = "Отправка команд на Device #";
        const int FatX = 1172;
        const int FatY = 804;
        const int SimpleX = 690;
        const int SimpleY = 365;
        const int PartSize = 928;

        public static DriverV7 Driver = new DriverV7();
        List<SubDeviceV7> SubDevices = new List<SubDeviceV7>();
        LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns FirmwareLoadPackAns;
        SubDeviceV7 FocusedDev;
        string ActivePageTxt;
        string FileExt;

        //Process OpenedProcLKDS;

        bool SendLastFragFlag = false;
        bool FatMode = false;
        bool IsLiftBlock = false;
        bool IsConnected = false;

        int CounterDevSubdev = 0;
        int x, y;
        int PageNum;

        byte SelectedDevOrSubDevCANID;
        List<byte[]> FirmwareFragments = new List<byte[]>();
        List<string> FwOnPages = new List<string>();

        #endregion


        private ObservableCollection<VMDevice> ocVMDevice;
        private object _lock = new object();
        bool FWGet = false;
        bool LBCheck = false;

        public ObservableCollection<VMDevice> OcVMDevice { get { return ocVMDevice; } }

        public VMPageMain()
        {
            ocVMDevice = new ObservableCollection<VMDevice>();
            BindingOperations.EnableCollectionSynchronization(ocVMDevice,_lock);


            Driver.OnSubDevChange += Driver_OnSubDevChange; ;
            Driver.OnReceiveData += Driver_OnReceiveData; ;
            if (!Driver.Init())
            {
                return;
            }

            if (App.Args.Length != 0)
            {
                try
                {
                    var Devices = DeviceV7.FromArgs(App.Args);
                    Driver.AddDevice(ref Devices[0]);


                    /*LVCanDevList.ItemsSource = OcVMDevice;
                    lbConnectionForm.Device = Devices[0];
                    IsConnected = true;
                    lbConnectionForm.Enabled = false;*/
                }
                catch { }
            }

        }

        private void Driver_OnReceiveData(LKDSFramework.Packs.PackV7 pack)
        {
            if (FocusedDev != null)
            {
                if (pack.CanID != FocusedDev.CanID)
                {
                    return;
                }
            }

            if (pack is LKDSFramework.Packs.DataDirect.PackV7IAPService)
            {
                LKDSFramework.Packs.DataDirect.PackV7IAPService IAPAns = (LKDSFramework.Packs.DataDirect.PackV7IAPService)pack;
                if (IAPAns.Error != LKDSFramework.Packs.DataDirect.PackV7IAPService.IAPErrorType.NoError)
                {
                    //WriteError(IAPAns);
                }
                else
                {
                    if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns)
                    {
                        LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns PackStateAns = IAPAns as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns;
                        FileExt = "*.b" + PackStateAns.IAPState.AppVer.ToString("X2");
                        /*Invoke(new Action(() =>
                        {
                            SelectFirmwareBTN.Enabled = true;
                            LBAndSubDeviceLV.Enabled = true;
                        }));
                        FwOnPages.Clear();
                        FWView(PackStateAns);
                        WriteRecieve(IAPAns);*/
                        //GenerateBTN(IAPAns);
                    }
                    else if (!SendLastFragFlag && IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns)
                    {
                        FirmwareLoadPackAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns;

                        Console.WriteLine(FirmwareLoadPackAns.Offset);
                        //WriteRecieve(FirmwareLoadPackAns);

                        int Pos = FirmwareLoadPackAns.Offset * 32 / PartSize;
                        int Offset = FirmwareLoadPackAns.Offset;
                        SendLastFragFlag = Pos == FirmwareFragments.Count - 1;

                        if (Pos > FirmwareFragments.Count)
                        {
                            throw new Exception("нет такого");
                        }

                        Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk()
                        {
                            UnitID = FocusedDev.UnitID,
                            CanID = FocusedDev.CanID,
                            Num = (byte)PageNum,
                            Fragment = new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk.FragmentPageSturct()
                            {
                                Buff = FirmwareFragments[Pos],
                                Offset = (short)Offset,
                                isLastFrag = SendLastFragFlag
                            }
                        });

                        //WriteRecieve(IAPAns);
                    }
                    else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAns)
                    {
                        //WriteRecieve(IAPAns);
                    }
                    else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPClearAns)
                    {
                        //WriteRecieve(IAPAns);
                    }
                    else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns)
                    {
                        LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns packV7IAPReadAns = IAPAns as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;
                        FwOnPages.Add(packV7IAPReadAns.PageState.Name);
                        Console.WriteLine(packV7IAPReadAns.PageState.Name);
                        Console.WriteLine(packV7IAPReadAns.PageState.App);
                        Console.WriteLine(packV7IAPReadAns.PageState.Description);
                        Console.WriteLine(packV7IAPReadAns.PageState.Lenght);
                        Console.WriteLine(packV7IAPReadAns.PageState.Soft);
                        Console.WriteLine(packV7IAPReadAns.PageState.UnitType);
                        VMDevice VMDev = null;

                            /*FWGet = false;
                            if (FWGet)
                            {
                                
                            }*/
                        lock (_lock)
                        {
                            string FWVer = "";
                            char[] FWName = packV7IAPReadAns.PageState.Name.ToCharArray();
                            for (int i = FWName.Length-1; i>0 ; i--)
                            {
                                if (Char.IsDigit(FWName[i]))
                                {
                                    FWVer += FWName[i];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            FWName = FWVer.Reverse().ToArray();
                            FWVer = "";

                            foreach (char ch in FWName)
                            {
                                FWVer += ch + ".";
                            }
                            FWVer = FWVer.Trim();
                            SubDeviceV7 dev;
                            if (pack.CanID.Equals(0) && LBCheck)
                            {
                                return;
                            }
                            else
                            {
                                dev = Driver.Devices[0];

                                LBCheck = true;
                            }
                            if (!pack.CanID.Equals(0))
                            {
                                dev = Driver.Devices[0].SubDevices[pack.CanID];
                            }
                            VMDev = new VMDevice()
                            {
                                CanID = dev.CanID,
                                Title = dev.ToString(),
                                AppVer = dev.AppVer,
                                FWTitle = packV7IAPReadAns.PageState.Name,
                                FWVersion = FWVer,
                                FWDate = "Не определено"
                            };
                            FillOc(VMDev);
                        }
                        //WriteRecieve(IAPAns);
                    }
                }
                   
            }
            else
            {
                //WriteRecieve(IAPAns);
            }
        }

        private void Driver_OnSubDevChange(SubDeviceV7 dev)
        {
            try
            {
                lock (_lock)
                {
                    FWGet = true;
                    VMDevice.SendReadAsk(dev.CanID);
                }
                

                if (SubDevices.Count > 0)
                {
                    foreach (SubDeviceV7 addedDev in SubDevices)
                    {
                        if (addedDev.CanID.Equals(dev.CanID))
                        {
                            SubDevices.Remove(addedDev);
                            SubDevices.Add(dev);
                            return;
                        }
                    }
                }

                /*DevCount = $"{CanDevTxt} Количество подключенных устройств: {CounterDevSubdev + 1}";
                LVDevCount.Content = DevCount;
                LVCanDevList.Items.Refresh()*/;

                SubDevices.Add(dev);
            }
            catch { }
        }

        public void FillOc(VMDevice vMDevice)
        {
            OcVMDevice.Add(vMDevice);
            OnPropertyChanged(nameof(OcVMDevice));
        }
    }


}
