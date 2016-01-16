namespace Arobbot.ControlUIApp
{
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
    using ARobbot.RobbotControl;

    public sealed partial class MainPage : Page
    {
        private const int NumberOfMotors = 2;
        private const int LeftMotor = 0;
        private const int RightMotor = 1;
        private const int MotorsMaxSpeed = 80;
        private const int MotorsRPMStep = 1;
        private const int MotorsStartSpeed = 0;
        private const int WheelStep = 3;
        private MotorDirection[] motorDirection;
        private BluetoothSerial bluetooth;
        private RobbotControler robbot;

        public MainPage()
        {
            this.InitializeComponent();
            this.PivotItemControl.IsEnabled = false;

            bluetooth = new BluetoothSerial("HC-05");
            robbot = new RobbotControler(bluetooth, NumberOfMotors);
            bluetooth.ConnectionEstablished += OnConnectionEstablished;
            bluetooth.ConnectionLost += OnConnectionLost;
            bluetooth.ConnectionFailed += OnConnectionLost;
            bluetooth.begin(0, 0);

            motorDirection = new MotorDirection[NumberOfMotors];
            motorDirection[LeftMotor] = MotorDirection.Forward;
            motorDirection[RightMotor] = MotorDirection.Forward;

            this.ButtonConnect.Click += Connect;
            this.ButtonDisconect.Click += Disconect;

            this.ButtonStartLeft.Click += (o, e) => robbot.StartMotor(LeftMotor);
            this.ButtonStartRight.Click += (o, e) => robbot.StartMotor(RightMotor);
            this.ButtonStartAll.Click += (o, e) => robbot.StartAllMotos();

            this.ButtonStopLeft.Click += (o, e) => robbot.StopMotor(LeftMotor);
            this.ButtonStopRight.Click += (o, e) => robbot.StopMotor(RightMotor);
            this.ButtonStopAll.Click += (o, e) => robbot.StopAllMotors();

            this.SliderSpeed.Minimum = -MotorsMaxSpeed;
            this.SliderSpeed.Maximum = MotorsMaxSpeed;
            this.SliderSpeed.StepFrequency = MotorsRPMStep;
            this.SliderSpeed.Value = MotorsStartSpeed;

            this.SliderSpeed.ValueChanged += OnDriverSlidersChanged;

            this.SliderWheel.Minimum = -0.5 * MotorsMaxSpeed;
            this.SliderWheel.Maximum = 0.5 * MotorsMaxSpeed;
            this.SliderSpeed.StepFrequency = WheelStep;
            this.SliderWheel.Value = 0;

            this.SliderWheel.ValueChanged += OnDriverSlidersChanged;
        }

        private void Disconect(object sender, RoutedEventArgs e)
        {
            this.PivotItemControl.IsEnabled = false;
            bluetooth.end();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            bluetooth.begin(0, 0);
        }

        private void OnConnectionLost(string message)
        {
            this.PivotItemControl.IsEnabled = false;
        }

        private void OnConnectionEstablished()
        {
            this.PivotItemControl.IsEnabled = true;
        }

        private void OnDriverSlidersChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var speed = (int)SliderSpeed.Value;
            var wheel = (int)SliderWheel.Value;

            var leftSpeed = speed + wheel;
            var rightSpeed = speed - wheel;

            if (leftSpeed > MotorsMaxSpeed)
            {
                this.TextBlockRPMLeft.Text = MotorsMaxSpeed.ToString();
            }
            else if (leftSpeed < -MotorsMaxSpeed)
            {
                this.TextBlockRPMLeft.Text = MotorsMaxSpeed.ToString();
            }
            else
            {
                this.TextBlockRPMLeft.Text = leftSpeed.ToString();
            }

            if (rightSpeed > MotorsMaxSpeed)
            {
                this.TextBlockRPMRight.Text = MotorsMaxSpeed.ToString();
            }
            else if (rightSpeed < -MotorsMaxSpeed)
            {
                this.TextBlockRPMRight.Text = MotorsMaxSpeed.ToString();
            }
            else
            {
                this.TextBlockRPMRight.Text = rightSpeed.ToString();
            }

            ChangeMotorState(LeftMotor, leftSpeed);
            ChangeMotorState(RightMotor, rightSpeed);
        }

        private void ChangeMotorState(int motor, int speed)
        {
            var absoluteSpeed = Math.Abs(speed);

            if (absoluteSpeed > MotorsMaxSpeed)
            {
                absoluteSpeed = MotorsMaxSpeed;
            }

            if (speed < 0 && this.motorDirection[motor] == MotorDirection.Forward)
            {
                this.motorDirection[motor] = MotorDirection.Backward;
                this.robbot.ChangeMotorDirection(motor, MotorDirection.Backward);
            }
            else if (speed > 0 && this.motorDirection[motor] == MotorDirection.Backward)
            {
                this.motorDirection[motor] = MotorDirection.Forward;
                this.robbot.ChangeMotorDirection(motor, MotorDirection.Forward);
            }

            if (!this.robbot.motorsStoped[motor] && absoluteSpeed == 0)
            {
                this.robbot.StopMotor(motor);
            }
            else if (this.robbot.motorsStoped[motor] && absoluteSpeed != 0)
            {
                this.robbot.SetMotorRpm(motor, absoluteSpeed);
                this.robbot.StartMotor(motor);
            }
            else if (absoluteSpeed != 0)
            {
                this.robbot.SetMotorRpm(motor, absoluteSpeed);
            }
        }
    }
}
