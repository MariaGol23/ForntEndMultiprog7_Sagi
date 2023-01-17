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

        }

        private void Driver_OnSubDevChange(SubDeviceV7 dev)
        {
            try
            {
                //VMDevice.SendReadAsk(dev.CanID);


                VMDevice VMDev = new VMDevice()
                {
                    CanID = dev.CanID,
                    Title = dev.ToString(),
                    AppVer = dev.AppVer,
                };


                lock (_lock)
                {
                    FillOc(VMDev);
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
