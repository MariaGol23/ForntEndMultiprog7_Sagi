using ForntEndMultiprog7.Model;
using LKDSFramework;
using LKDSFramework.Packs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForntEndMultiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
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

        Process OpenedProcLKDS;

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



        string DevCount;
        ObservableCollection<VMDevice> OcVMDev = new ObservableCollection<VMDevice>();
        public PageMain()
        {
            InitializeComponent();


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*Driver.OnSubDevChange += Driver_OnSubDevChange;
            Driver.OnReceiveData += Driver_OnReceiveData;
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
                    LVCanDevList.ItemsSource = OcVMDev;
                    *//*lbConnectionForm.Device = Devices[0];
                    IsConnected = true;
                    lbConnectionForm.Enabled = false;*//*
                }
                catch { }
            }*/
        }

        /*private void Driver_OnReceiveData(PackV7 pack)
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
                        Invoke(new Action(() =>
                        {
                            SelectFirmwareBTN.Enabled = true;
                            LBAndSubDeviceLV.Enabled = true;
                        }));
                        FwOnPages.Clear();
                        FWView(PackStateAns);
                        WriteRecieve(IAPAns);
                        //GenerateBTN(IAPAns);
                    }
                    else if (!SendLastFragFlag && IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns)
                    {
                        FirmwareLoadPackAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns;

                        Console.WriteLine(FirmwareLoadPackAns.Offset);
                        WriteRecieve(FirmwareLoadPackAns);

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

                        WriteRecieve(IAPAns);
                    }
                    else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAns)
                    {
                        WriteRecieve(IAPAns);
                    }
                    else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPClearAns)
                    {
                        WriteRecieve(IAPAns);
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
                        VMDevice.FWTitle = packV7IAPReadAns.PageState.Name;
                        VMDevice.FWVersion = packV7IAPReadAns.PageState.Name;
                        VMDevice.FWTitle = packV7IAPReadAns.PageState.Name;
                        WriteRecieve(IAPAns);
                    }
                    else
                    {
                        WriteRecieve(IAPAns);
                    }
                }
            }
        }

        private void Driver_OnSubDevChange(SubDeviceV7 dev)
        {

            try
            {
                VMDevice.SendReadAsk(dev.CanID);


                VMDevice VMDev = new VMDevice()
                {
                    CanID = dev.CanID,
                    Title = dev.ToString(),
                    AppVer = dev.AppVer,
                };

                try
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        OcVMDev.Add(VMDev);
                    }));
                }
                catch { }
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
                DevCount = $"{CanDevTxt} Количество подключенных устройств: {CounterDevSubdev + 1}";
                LVDevCount.Content = DevCount;
                LVCanDevList.Items.Refresh();

                SubDevices.Add(dev);
            }
            catch { }
        }*/

        #region Btn Click events

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion


        /*void WriteRecieve(LKDSFramework.Packs.DataDirect.PackV7IAPService IAPAns)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    SendRecievePacksRTB.Text += $"Recieve Pack '{IAPAns.IAPCommand}', ";
                    SendRecievePacksRTB.Text += $"on Device {FocusedDev.CanID}, {ActivePageTxt}";
                    SendRecievePacksRTB.Text += $"\n-----------------Result: -----------------" +
                   $"\nPack id: {IAPAns.PackID}" +
                   $"\nPack class: {IAPAns.Class}" +
                   $"\nPack type: {IAPAns.Type}" +
                   $"\nPack state: {IAPAns.State}" +
                   $"\nPack data: ";
                    foreach (byte bt in IAPAns.Data)
                    {
                        SendRecievePacksRTB.Text += $"{bt} ";
                    }
                    SendRecievePacksRTB.Text += $"\n------------------------------------------\n\n";
                    SendRecievePacksRTB.SelectionStart = SendRecievePacksRTB.TextLength;
                    SendRecievePacksRTB.ScrollToCaret();


                }));
            }
            catch { };



        }


        void FWView(LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns pack)
        {

            for (int i = 1; i != pack.IAPState.PageCount; i++)

                Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAsk()
                {
                    UnitID = FocusedDev.UnitID,
                    Num = (byte)i,
                    CanID = FocusedDev.CanID
                });
        }


        void SendStateAsk()
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAsk()
            {
                UnitID = FocusedDev.UnitID,
                CanID = FocusedDev.CanID
            });
        }

        void SendReadAsk()
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAsk()
            {
                UnitID = FocusedDev.UnitID,
                CanID = FocusedDev.CanID,
                Num = (byte)PageNum
            });
        }

        void SendUpdateAsk()
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAsk()
            {
                UnitID = FocusedDev.UnitID,
                CanID = FocusedDev.CanID,
                Num = (byte)PageNum
            });
        }

        void SendActivateAsk()
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAsk()
            {
                UnitID = FocusedDev.UnitID,
                CanID = FocusedDev.CanID,
                Num = (byte)PageNum
            });
        }

        void SendClearAsk()
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPClearAsk()
            {
                UnitID = FocusedDev.UnitID,
                CanID = FocusedDev.CanID,
                Num = (byte)PageNum
            });
        }*/
    }

}
