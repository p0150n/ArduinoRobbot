namespace ARobbot.RobbotControl
{
    using Microsoft.Maker.Serial;

    public class RobbotControler
    {
        private const int DefaultRPM = 20;
        private readonly int numberOfMotors;
        private int[] motorsRPM;
        public bool[] motorsStoped;

        public RobbotControler(IStream stream, int numberOfMotors)
        {
            this.bluetooth = stream;
            this.numberOfMotors = numberOfMotors;
            this.motorsRPM = new int[this.numberOfMotors];
            this.motorsStoped = new bool[this.numberOfMotors];

            for (int i = 0; i < this.motorsRPM.Length; i++)
            {
                this.motorsRPM[i] = DefaultRPM;
                this.motorsStoped[i] = true;
            }
        }

        public IStream bluetooth { get; set; }

        public RobbotControler StartMotor(int motorNumber)
        {
            if (ValidateMotorNumber(motorNumber))
            {
                this.motorsStoped[motorNumber] = false;
                this.SetMotorRpm(motorNumber, this.motorsRPM[motorNumber]);
            }
            
            return this;
        }

        public RobbotControler StartAllMotos()
        {
            for (int i = 0; i < this.numberOfMotors; i++)
            {
                this.motorsStoped[i] = false;
            }

            for (int i = 0; i < this.numberOfMotors; i++)
            {
                this.SetMotorRpm(i, this.motorsRPM[i]);
            }

            return this;
        }

        public RobbotControler StopAllMotors()
        {
            this.bluetooth.write((byte)CommandType.StartMotorCommand);
            this.bluetooth.write((byte)CommandType.All);
            this.bluetooth.write((byte)CommandType.Stop);
            this.bluetooth.write((byte)CommandType.EndCommand);
            this.bluetooth.flush();

            for (int i = 0; i < this.numberOfMotors; i++)
            {
                this.motorsStoped[i] = true;
            }

            return this;
        }

        public RobbotControler StopMotor(int motorNumber)
        {
            if (ValidateMotorNumber(motorNumber))
            {
                this.motorsStoped[motorNumber] = true;
                char motorNumberAsChar = motorNumber.ToString()[0];
                this.bluetooth.write((byte)CommandType.StartMotorCommand);
                this.bluetooth.write((byte)motorNumberAsChar);
                this.bluetooth.write((byte)CommandType.Stop);
                this.bluetooth.write((byte)CommandType.EndCommand);
                this.bluetooth.flush();
            }

            return this;
        }

        public RobbotControler ChangeAllMotorsDirection(MotorDirection direction)
        {
            this.bluetooth.write((byte)CommandType.StartMotorCommand);
            this.bluetooth.write((byte)CommandType.All);
            switch (direction)
            {
                case MotorDirection.Forward:
                    this.bluetooth.write((byte)CommandType.ForwardDirection);
                    break;
                case MotorDirection.Backward:
                    this.bluetooth.write((byte)CommandType.BackwardDirection);
                    break;
            }

            this.bluetooth.write((byte)CommandType.EndCommand);
            this.bluetooth.flush();

            return this;
        }

        public RobbotControler ChangeMotorDirection(int motorNumber, MotorDirection direction)
        {
            if (this.ValidateMotorNumber(motorNumber))
            {
                char motorNumberAsChar = motorNumber.ToString()[0];
                this.bluetooth.write((byte)CommandType.StartMotorCommand);
                this.bluetooth.write((byte)motorNumberAsChar);
                switch (direction)
                {
                    case MotorDirection.Forward:
                        this.bluetooth.write((byte)CommandType.ForwardDirection);
                        break;
                    case MotorDirection.Backward:
                        this.bluetooth.write((byte)CommandType.BackwardDirection);
                        break;
                }

                this.bluetooth.write((byte)CommandType.EndCommand);
                this.bluetooth.flush();
            }

            return this;
        }

        public RobbotControler SetAllMotorsRpm(int rpm)
        {
            if (rpm < 1 || rpm > 999)
            {
                return this;
            }

            for (int i = 0; i < this.numberOfMotors; i++)
            {
                this.motorsRPM[i] = rpm;
                this.motorsStoped[i] = false;
            }

            string rpmAsString = rpm.ToString().PadLeft(3, '0');
            this.bluetooth.write((byte)CommandType.StartMotorCommand);
            this.bluetooth.write((byte)CommandType.All);
            this.bluetooth.write((byte)CommandType.RPM);
            this.bluetooth.write((byte)rpmAsString[0]);
            this.bluetooth.write((byte)rpmAsString[1]);
            this.bluetooth.write((byte)rpmAsString[2]);
            this.bluetooth.write((byte)CommandType.EndCommand);
            this.bluetooth.flush();

            return this;
        }

        public RobbotControler SetMotorRpm(int motorNumber, int rpm)
        {
            if (rpm < 1 || rpm > 999)
            {
                return this;
            }

            if (ValidateMotorNumber(motorNumber))
            {
                this.motorsRPM[motorNumber] = rpm;

                if (this.motorsStoped[motorNumber])
                {
                    return this;
                }

                string rpmAsString = this.motorsRPM[motorNumber].ToString().PadLeft(3, '0');
                char motorNumberAsChar = motorNumber.ToString()[0];

                this.bluetooth.write((byte)CommandType.StartMotorCommand);
                this.bluetooth.write((byte)motorNumberAsChar);
                this.bluetooth.write((byte)CommandType.RPM);
                this.bluetooth.write((byte)rpmAsString[0]);
                this.bluetooth.write((byte)rpmAsString[1]);
                this.bluetooth.write((byte)rpmAsString[2]);
                this.bluetooth.write((byte)CommandType.EndCommand);
                this.bluetooth.flush();
            }

            return this;
        }

        private bool ValidateMotorNumber(int motorNumber)
        {
            if (motorNumber >= 0 && motorNumber < this.numberOfMotors)
            {
                return true;
            }

            return false;
        }
    }
}
