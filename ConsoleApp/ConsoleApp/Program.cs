using System;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Xml.Linq;

namespace Program
{
    class Program
    {

        public static void Main()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
                Console.WriteLine(GetComPortInformation(port));
            }

            Console.ReadLine();
        }

        static string ByteToHex(byte B)
        {
            string hStr = "0123456789ABCDEF";
            string byteToHex = "";
            byteToHex += hStr[(B / 16) + 1];
            byteToHex += hStr[(B % 16) + 1];

            return byteToHex;
        }

        static string Long2Hex(int B)
        {
            //Function Long2Hex(B: Longint):String;
            //Begin
            // Long2Hex:= Byte2Hex(B div $100) + Byte2Hex(B and $0FF);
            //End;
            throw new Exception("Не сделано");
        }

            private static void Max(string[] args)
            {
                foreach (var memory in new ManagementObjectSearcher(
                             "select * from Win32_PnPEntity").Get())
                {
                    var name = (string)memory.Properties["Name"].Value;

                    if (name == null || !name.Contains("COM")) continue;

                    Console.WriteLine("Name = " + memory["Name"]);

                    foreach (var address in new ManagementObjectSearcher(
                                 "ASSOCIATORS OF {Win32_PnPEntity.DeviceID='" +
                                 (string)memory.GetPropertyValue("PNPDeviceID") +
                                 "'} WHERE RESULTCLASS  = Win32_PortResource").Get())
                    {
                        var addr = address["Name"].ToString().Split("-")[0];
                        Console.WriteLine("\tAddress = " + addr);
                    }
                }

                Console.ReadLine();
            }


        /*Возвращает информацию о последовательном порте с указанным именем*/
        public static string GetComPortInformation(string name)
        {
            ManagementObjectSearcher mbs;
            ManagementObjectCollection mbsList = null;
            StringBuilder sb = new StringBuilder(2000);
            object val;

            //попытка получить данные из Win32_SerialPort
            mbs = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort WHERE DeviceID = '" + name + "'");

            using (mbs)
            {
                mbsList = mbs.Get();

                foreach (ManagementObject mo in mbsList)
                {
                    val = mo["Name"];
                    if (val != null) sb.AppendLine(val.ToString());

                    //порт найден, возвращаем данные
                    string deviceId = (string)mo.GetPropertyValue("PNPDeviceID");

                    foreach (var address in new ManagementObjectSearcher(
                                 "ASSOCIATORS OF {Win32_PnPEntity.DeviceID='" + deviceId +
                                 "'} WHERE RESULTCLASS  = Win32_PortResource").Get())
                    {
                        var addr = address["Name"].ToString().Split("-")[0];
                        Console.WriteLine("\tAddress = " + addr);
                    }
                    return sb.ToString();
                }//end foreach                       
            }

            //порт не найден, выберем все последовательные порты из Win32_PnPEntity
            mbs = new ManagementObjectSearcher(
            "SELECT * FROM Win32_PnPEntity where ClassGuid = '{4d36e978-e325-11ce-bfc1-08002be10318}' and Service <> 'Parport'"
            );

           //using (mbs)
           //{
           //    mbsList = mbs.Get();
           //
           //    foreach (ManagementObject mo in mbsList)
           //    {
           //        //находим идентификатор
           //        val = mo["PnpDeviceID"];
           //        if (val == null) continue;
           //
           //        string id = val.ToString();
           //        if (id.Length == 0) continue;
           //
           //        //находим имя порта для данного PnpDeviceID в реестре
           //        if (PortNameFromID(id) == name)
           //        {
           //            //порт найден, возвращаем данные
           //            sb.Clear();
           //            val = mo["Name"];
           //            if (val != null) sb.AppendLine(val.ToString());
           //
           //            foreach (var p in mo.Properties)
           //            {
           //                sb.Append("* " + p.Name + ": ");
           //                if (p.Value != null)
           //                {
           //                    sb.Append(p.Value.ToString());
           //                }
           //                else sb.Append("null");
           //                sb.AppendLine();
           //            }
           //            return sb.ToString();
           //        }
           //    }//end foreach          
           //
           //}

            return "Информация не найдена";
        }
    }


}





//{ Возвращает базовый адрес порта с номером PortIndex
//}
//Function GetBaseAdr(PortIndex : Byte) : Word;
//Var LowAdr : Word;
//Begin
// { вычисляем младшую часть адреса в таблице }
// LowAdr:= (PortIndex - 1) * 2;
//{ получаем базовый адрес порта из таблицы }
//GetBaseAdr:= MemW[$0040:LowAdr];
//End;
//
//Var
// PortIndex : Byte;
//BaseAdr: Word;
//
//Begin
// { Опрос базовых адресов 4 портов }
// For PortIndex:= 1 to 4 do begin
//  { Получить базовый адрес}
//  BaseAdr:= GetBaseAdr(PortIndex);
//
//{ Анализируем базовый адрес }
//If BaseAdr = 0 then
//   WriteLn('COM', PortIndex,' не обнаружен')
//  Else
//   WriteLn('Базовый адрес COM', PortIndex, ' равен ', Long2Hex(BaseAdr));
//End;
//End.
//
//}