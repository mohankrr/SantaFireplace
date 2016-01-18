using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.IoT.Core.HWInterfaces.MPR121;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SantaFirePlace
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        Image[] santas = new Image[6];
        int currentSanta = 0;
        public MainPage()
        {
            this.InitializeComponent();

            santas[0] = image0;
            santas[1] = image1;
            santas[2] = image2;
            santas[3] = image3;
            santas[4] = image4;
            santas[5] = image5;

            for(int i=0;i<6;i++)
            {
                santas[i].Visibility = Visibility.Collapsed;
            }

            __initMPR121();
        }

        private MPR121 __mpr121 = null;
        private async void __initMPR121()
        {
            __mpr121 = new MPR121();
            //Get the I2C device list on the Raspberry Pi.
            string aqs = I2cDevice.GetDeviceSelector(); //get the device selector AQS  (adavanced query string)
            var i2cDeviceList = await DeviceInformation.FindAllAsync(aqs); //get the I2C devices that match the device selector aqs

            //if the device list is not null, try to establish I2C connection between the master and the MPR121
            if (i2cDeviceList != null && i2cDeviceList.Count > 0)
            {
                bool connected = await __mpr121.OpenConnection(i2cDeviceList[0].Id);
                if (connected)
                {
                    //MPR121 will raise Touched and Released events if the IRQ pin is connected and configured corectly.. 
                    //Adding event handlers for those events
                    __mpr121.PinTouched += __mpr121_PinTouched; ;
                    __mpr121.PinReleased += __mpr121_PinReleased; ; ;
                }
            }

        }

        private void __mpr121_PinReleased(object sender, PinReleasedEventArgs e)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                santas[currentSanta].Visibility = Visibility.Collapsed;
            });
        }

        private void __mpr121_PinTouched(object sender, PinTouchedEventArgs e)
        {

            switch (e.Touched[0]) //using only the first pin in the list of touched pins
            {
                case PinId.PIN_0:
                    currentSanta = 0;
                    break;
                case PinId.PIN_2:
                    currentSanta = 1;
                    break;
                case PinId.PIN_4:
                    currentSanta = 2;
                    break;
                case PinId.PIN_6:
                    currentSanta = 3;
                    break;
                case PinId.PIN_8:
                    currentSanta = 4;
                    break;
                case PinId.PIN_10:
                    currentSanta = 5;
                    break;
                case PinId.PIN_11:
                    currentSanta = 0;   //just using six images
                    break;
            }

            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                mediaElement.Play();

                santas[currentSanta].Visibility = Visibility.Visible;
            });
        }
    }
}
