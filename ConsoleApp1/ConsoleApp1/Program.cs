using System;
using System.Collections;
using System.IO.Ports;
using System.Text;
using System.Threading;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;

    public static void Main()
    {
        // Создает и контролирует поток, задает приоритет и возвращает статус.
        Thread readThread = new Thread(Read);

        // Создание объекта последовательного порта с настройками по умолчанию 
        _serialPort = new SerialPort();

        // PortName - наименование последовательного порта
        SetPortName(ref _serialPort);

        // BaudRate - Возвращает или задает скорость передачи для последовательного порта (бит в секунду).
        SetPortBaudRate(ref _serialPort);

        // Parity - протокол контроля четности
        // Отвечается за контроль чертности и паритет четности
        // Контроль четности: Even(четное) и Odd(нечетное)
        // Паритет четности: Mark(бит четности = 1) и Space(бит четности = 0)
        SetPortParity(ref _serialPort);


        // ----------------------- скорее всего не надо
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);

        // Handshake - протокол установления связи для передачи данных через последовательный порт
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
        // ----------------------- скорее всего не надо


        // Устанавливаем таймаут для чтения и записи
        _serialPort.ReadTimeout = 50000;
        _serialPort.WriteTimeout = 50000;


        // Открывает соединение последовательного порта
        _serialPort.Open();
        _continue = true;
        readThread.Start();


        // Задаем имя пользователя
        string name;
        Console.Write("Name: ");
        name = Console.ReadLine();



        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Console.WriteLine("Type QUIT to exit");
        while (_continue)
        {
            message = Console.ReadLine();

            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {
                _serialPort.WriteLine(
                    String.Format("<{0}>: {1}", name, message));
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
                Console.WriteLine(message);
            }
            catch (TimeoutException) { }
        }
    }


    // Отображдает существуюшие порты и задает выбранный порт
    public static void SetPortName(ref SerialPort _serialPort)
    {
        string newPortName;

        Console.WriteLine("Существующие порты:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Введите название COM порта (По умолчанию: {0}): ", _serialPort.PortName);
        newPortName = Console.ReadLine();

        if (!(newPortName == "") && newPortName.ToLower().StartsWith("com"))
        {
            _serialPort.PortName = newPortName;
        }
    }



    // Задает скорость передачи для последовательного порта.
    public static void SetPortBaudRate(ref SerialPort _serialPort)
    {
        string newBaudRate;

        Console.Write("Скорость передачи (По умолчанию: {0}): ", _serialPort.BaudRate);
        newBaudRate = Console.ReadLine();

        if (newBaudRate != "")
        {
            _serialPort.BaudRate = int.Parse(newBaudRate);
        }
    }



    // Отображает существующие протоколы контроля четности и задает выбранный протокол
    public static void SetPortParity(ref SerialPort _serialPort)
    {
        string newParity;

        Console.WriteLine("Существующие протоколы контроля четности:");
        foreach (string s in Enum.GetNames(typeof(Parity)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Протокол контроля четности (По умолчанию: {0}):", _serialPort.Parity.ToString());
        newParity = Console.ReadLine();

        if (newParity != "")
        {
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), newParity, true);
        }
    }


    public string GenerateDataPackage(string message, bool flag,  string checksum)
    {
        string startPackage = "$";


        // три десятичные цифры с выравниванием нулями слева
        if (checksum.Length != 3)
            Console.WriteLine("Неверно введена контрольная сумма");

        string endPackage = "\n";


        return startPackage + flag + message + checksum + endPackage;
    }

    public static string calcSimpleCheckSum(string message)
    {
        // Кодировка ASCII
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);


        string checksum = "000";
        if(messageBytes.Length != 0)
        {
            BitArray bitArray = new BitArray(8);
            for (int i = 1; i < messageBytes.Length; i++)
            {
                bitArray
            }
        }

        return checksum;
    }

    public static string calcLongRedCheck(char P, string message)
    {
        // 1 байт
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        byte result = 0;

        for (int i = 0; i < messageBytes.Length; i++)
        {
            result += Byte()
        }

        result -= FF;
        
    }



    // Display DataBits values and prompt user to enter a value.
    // Отобразить значения DataBits и предложить пользователю ввести значение
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits;

        Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
        dataBits = Console.ReadLine();

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    // Отобразить значения StopBits и предложить пользователю ввести значение
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits;

        Console.WriteLine("Available StopBits options:");
        foreach (string s in Enum.GetNames(typeof(StopBits)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter StopBits value (None is not supported and \n" +
         "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
        stopBits = Console.ReadLine();

        if (stopBits == "")
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }
    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake;

        Console.WriteLine("Available Handshake options:");
        foreach (string s in Enum.GetNames(typeof(Handshake)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
        handshake = Console.ReadLine();

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }
}