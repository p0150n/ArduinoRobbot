/*
M => Motor
A => All Motors
M1 => Motor 1

M0S => Motor 0 STOP
M0F => Motor 0 Forward directions
M0B => Motor 0 Forward directions
M1R[001-999] => Set Motor 1 RPM;
M1P[000-255] => Motor Power
*/

namespace ARobbot.TestConsoleApp
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    public class Startup
    {
        public static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (var p in ports)
            {
                Console.WriteLine("Port: {0}", p);
            }

            string port = Console.ReadLine();
            SerialPort serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.PortName = port; // Set in Windows
            serialPort.Open();

            while (serialPort.IsOpen)
            {
                serialPort.WriteLine(Console.ReadLine());
                System.Threading.Thread.Sleep(2);
                // WRITE THE INCOMING BUFFER TO CONSOLE
                while (serialPort.BytesToRead > 0)
                {
                    Console.Write(Convert.ToChar(serialPort.ReadChar()));
                }
                // SEND
            }
        }
    }
}
