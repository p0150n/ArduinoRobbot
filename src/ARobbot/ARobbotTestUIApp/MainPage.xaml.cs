using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Maker.Serial;
using Microsoft.Maker.Firmata;
using Microsoft.Maker.RemoteWiring;
using ARobbot.RobbotControl;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409


namespace ARobbotTestUIApp
{
    public sealed partial class MainPage : Page
    {
        BluetoothSerial bluetooth;
        private const int NumberOfMotors = 2;
        private const int Motor1 = 0;
        private const int Motor2 = 1;
        private const int MotorsMinRPM = 1;
        private const int MotorsMaxRPM = 400;
        private const int MotorsRPMStep = 1;
        private const int MotorsStartRPM = 1;

        public MainPage()
        {
            this.InitializeComponent();
            bluetooth = new BluetoothSerial("HC-05");
            RobbotControler robbot = new RobbotControler(bluetooth, NumberOfMotors);
            //bluetooth.ConnectionEstablished += OnConnectionEstablished;
            //bluetooth.ConnectionLost += OnConnectionLost;
            bluetooth.begin(0, 0);
            
            this.sliderBarSpeedMotor1.Minimum = MotorsMinRPM;
            this.sliderBarSpeedMotor1.Maximum = MotorsMaxRPM;
            this.sliderBarSpeedMotor1.Value = MotorsStartRPM;
            this.sliderBarSpeedMotor1.StepFrequency = MotorsRPMStep;

            this.sliderBarSpeedMotor2.Minimum = MotorsMinRPM;
            this.sliderBarSpeedMotor2.Maximum = MotorsMaxRPM;
            this.sliderBarSpeedMotor2.Value = MotorsStartRPM;
            this.sliderBarSpeedMotor2.StepFrequency = MotorsRPMStep;
            
            this.sliderBarSpeedAllMotors.Minimum = MotorsMinRPM;
            this.sliderBarSpeedAllMotors.Maximum = MotorsMaxRPM;
            this.sliderBarSpeedAllMotors.Value = MotorsStartRPM;
            this.sliderBarSpeedAllMotors.StepFrequency = MotorsRPMStep;

            this.buttonMotor1Start.Click += (o, e) => robbot.StartMotor(Motor1);
            this.buttonMotor2Start.Click += (o, e) => robbot.StartMotor(Motor2);
            this.buttonAllMotorsStart.Click += (o, e) => robbot.StartAllMotos();

            this.buttonMotor1Stop.Click += (o, e) => robbot.StopMotor(Motor1);
            this.buttonMotor2Stop.Click += (o, e) => robbot.StopMotor(Motor2);
            this.buttonAllMotorsStop.Click += (o, e) => robbot.StopAllMotors();

            this.buttonMotor1Forward.Click += (o, e) => robbot.ChangeMotorDirection(Motor1, MotorDirection.Forward);
            this.buttonMotor2Forward.Click += (o, e) => robbot.ChangeMotorDirection(Motor2, MotorDirection.Forward);
            this.buttonAllMotorsForward.Click += (o, e) => robbot.ChangeAllMotorsDirection(MotorDirection.Forward);

            this.buttonMotor1Backward.Click += (o, e) => robbot.ChangeMotorDirection(Motor1, MotorDirection.Backward);
            this.buttonMotor2Backward.Click += (o, e) => robbot.ChangeMotorDirection(Motor2, MotorDirection.Backward);
            this.buttonAllMotorsBackward.Click += (o, e) => robbot.ChangeAllMotorsDirection(MotorDirection.Backward);

            this.sliderBarSpeedMotor1.ValueChanged += (o, e) =>
            {
                Slider slider = o as Slider;
                int rpm = (int)slider.Value;
                string rpmAsString = rpm.ToString().PadLeft(3, '0');
                this.textBlockSpeed1.Text = rpmAsString;

                robbot.SetMotorRpm(Motor1, rpm);
            };

            this.sliderBarSpeedMotor2.ValueChanged += (o, e) =>
            {
                Slider slider = o as Slider;
                int rpm = (int)slider.Value;
                string rpmAsString = rpm.ToString().PadLeft(3, '0');
                this.textBlockSpeed2.Text = rpmAsString;

                robbot.SetMotorRpm(Motor2, rpm);
            };

            this.sliderBarSpeedAllMotors.ValueChanged += (o, e) =>
            {
                Slider slider = o as Slider;
                int rpm = (int)slider.Value;

                this.sliderBarSpeedMotor1.Value = rpm;
                this.sliderBarSpeedMotor2.Value = rpm;

                string rpmAsString = rpm.ToString().PadLeft(3, '0');

                this.textBlockSpeed1.Text = rpmAsString;
                this.textBlockSpeed2.Text = rpmAsString;

                robbot.SetAllMotorsRpm(rpm);
            };
        }
    }
}
