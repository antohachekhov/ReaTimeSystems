using System;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Program
{
    class Program
    {

        public static void Main()
        {
            // Получение списка имен последовательных портов.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("Были найдены следующие последовательные порты:");

            // Вывод каждого имени порта на консоль.
            foreach (string port in ports)
            {
                Console.WriteLine(GetComPortInformation(port));
            }

            Console.ReadLine();
        }

        // Возвращает информацию о последовательном порте с указанным именем
        public static string GetComPortInformation(string name)
        {
            ManagementObjectSearcher mbs;
            StringBuilder sb = new StringBuilder(2000);
            sb.Append("\tName = " + name);

            // Попытка получить данные из Win32_PortResource,
            // Которые соответствуют строке из Win32_SerialPort с именем девайса = имени порта (#COM1)
            mbs = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_SerialPort.DeviceID='" + name +
                                 "'} WHERE RESULTCLASS  = Win32_PortResource");

            ManagementObjectCollection mbsResult = mbs.Get();

            if (mbsResult.Count == 0)
                return "Информация не найдена";

            // Структура Win32_PortResource:
            // Name - строка с диапазоном адресов, использующихся COM портом (# "0x000002F8-0x000002FF") 
            // Также начальный и конечный адрес можно получить по свойствам StartingAddress и EndingAddress в десятичном виде
            foreach (ManagementObject mo in mbsResult)
            {
                var addr = mo["Name"].ToString().Split('-')[0];
                sb.Append("\tAddress = " + addr);
            }

            return sb.ToString();
        }
    }


}