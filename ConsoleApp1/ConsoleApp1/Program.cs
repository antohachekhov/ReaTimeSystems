using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using static PortChat;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;

    // typeProtocol: 0 - bin, 1 - string
    static bool typeProtocol = false;
    static CheckSumAlg checkSumAlg = CheckSumAlg.Simple;

    public enum CheckSumAlg
    {
        Simple = 0,
        LRC = 1,
        CRC16 = 2,
        CRC32 = 3
    }

    public static void Main()
    {

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

        // Устанавливаем таймаут для чтения и записи
        _serialPort.ReadTimeout = 50000;
        _serialPort.WriteTimeout = 50000;

        SetTypeProtocol();
        SetCheckSumAlg();

        // Открывает соединение последовательного порта
        _serialPort.Open();
        _continue = true;


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

            if (stringComparer.Equals("QUIT", message))
            {
                _continue = false;
            }
            else
            {

            }
        }

        _serialPort.Close();
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

    public static void SetTypeProtocol()
    {
        Console.WriteLine("Возможные типы протоколо передачи данных:");
        Console.WriteLine("   BIN");
        Console.WriteLine("   STR");
        Console.Write("Протокол передачи данных (По умолчанию: BIN): ");
        string inputType = Console.ReadLine();
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

        if (inputType != "")
        {
            typeProtocol = !stringComparer.Equals("BIN", inputType);
        }
    }

    public static void SetCheckSumAlg()
    {
        Console.WriteLine("Существующие функции контрольной суммы:");
        foreach (string s in Enum.GetNames(typeof(CheckSumAlg)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Функция контрольной суммы (По умолчанию: Simple): ");
        string inputCheckSumAlg = Console.ReadLine();

        if (inputCheckSumAlg != "")
        {
            checkSumAlg = (CheckSumAlg)Enum.Parse(typeof(CheckSumAlg), inputCheckSumAlg, true);
        }
    }


    public static byte CalcSimpleCheckSum(byte[] messageBytes)
    {
        byte checkSum = 0;
        foreach (byte b in messageBytes)
        {
            checkSum ^= b;
        }
        return checkSum;
    }

    public static byte CalcLongRedCheck(byte[] messageBytes)
    {
        int checkSum = 0;

        for (int i = 0; i < messageBytes.Length; i++)
        {
            checkSum += messageBytes[i] % 255;
        }

        checkSum = (255 - checkSum + 1) % 255;

        return (byte)checkSum;

    }


    public static ushort CalcCRC16Sasha(byte[] messageBytes)
    {
        // задаем регистр CRC
        ushort CRC = 0xFFFF;

        for (int i = 0; i < messageBytes.Length; i++)
        {
            // Первый байт сообщения складывается по исключающему ИЛИ с содер жимым регистра CRC
            CRC = (ushort)((CRC & 0xFF00) + (messageBytes[i] ^ CRC));

            for (int j = 0; j < 8; i++)
            {

                CRC <<= 1;
                if ((CRC & 0x0001) != 0)
                {
                    CRC ^= 0xA001;
                }
                else
                    CRC <<= 1;
            }
        }

        return CRC;
    }
    public static ushort CalcCRC16Ira(byte[] messageBytes)
    {
        // Задаем контрольную сумму из 2 байт 
        ushort checkSum = 0;

        // Начинаем обрабатывать сообщение побайтно
        for (int i = 1; i < messageBytes.Length; i++)
        {
            checkSum ^= (ushort)(messageBytes[i] << 8); // сдвиг вправо

            for (int j = 0; j < 8; j++)
            {
                if ((checkSum & 0x8000u) != 0) // если младший бит не равен 0, 32768 = 1000 0000 0000 0000
                    checkSum = (ushort)((checkSum << 1) ^ 0x1021u); // исключающее или регистра, 4129 = 0001 0000 0010 0001
                else // если нет
                    checkSum <<= 1; // то сдвиг
            }
        }

        return checkSum;
    }

    // Получение сообщений
    private void GetMessage()
    {
        // Чтение полученных байтов
        var receivedMessage = new byte[_serialPort.BytesToRead];
        _serialPort.Read(receivedMessage, 0, _serialPort.BytesToRead);

        // Проверка контрольной суммы
        if (CheckControlSum(receivedMessage))
            WriteMessageToConsole(receivedMessage);// Вывод сообщения на экран
    }


    private void WriteMessageToConsole(byte[] messageBytes)
    {
        switch (checkSumAlg)
        {
            case CheckSumAlg.Simple:
                {

                    // Вычиселние сообщения без контрольной суммы
                    Array.Resize(ref messageBytes, messageBytes.Length - 1);
                    Console.WriteLine("Количество байт в полученном сообщении: " + messageBytes.Length.ToString());
                    break;
                }
            case CheckSumAlg.LRC:
                {
                    // Вычиселние сообщения без контрольной суммы
                    Array.Resize(ref messageBytes, messageBytes.Length - 1);
                    Console.WriteLine("Количество байт в полученном сообщении: " + messageBytes.Length.ToString());
                    break;
                }
            case CheckSumAlg.CRC16:
                {
                    break;
                }
            case CheckSumAlg.CRC32:
                {
                    break;
                }
        }


        // Вывод сообщения на экран

        if (typeProtocol)
        {
            Console.WriteLine("Полученное сообщение: " + BitConverter.ToDouble(messageBytes, 0).ToString());
        }
        else
        {
            Console.WriteLine("Полученное сообщение: " + _serialPort.Encoding.GetString(messageBytes));
        }

        return;
    }


    private bool CheckControlSum(byte[] messageBytes)
    {
        bool flagCheckSumEqual = false;
        switch (checkSumAlg)
        {
            case CheckSumAlg.Simple:
                {

                    // Последний байт в сообщении - контрольная сумма
                    byte checkSumInMessage = messageBytes[messageBytes.Length - 1];

                    // Вычиселние сообщения без контрольной суммы
                    Array.Resize(ref messageBytes, messageBytes.Length - 1);
                    byte checkSum = CalcSimpleCheckSum(messageBytes);
                    Console.WriteLine("Контрольная сумма посылки: " + $"0x{ checkSum: X}");
                    // Сравнение двух сумм
                    if (checkSum == checkSumInMessage)
                    {
                        flagCheckSumEqual = true;
                    }
                    break;
                }
            case CheckSumAlg.LRC:
                {
                    // Последний байт в сообщении - контрольная сумма
                    byte checkSumInMessage = messageBytes[messageBytes.Length - 1];

                    // Вычиселние сообщения без контрольной суммы
                    Array.Resize(ref messageBytes, messageBytes.Length - 1);
                    byte checkSum = CalcLongRedCheck(messageBytes);
                    Console.WriteLine("Контрольная сумма посылки: " + $"0x{ checkSum: X}");
                    // Сравнение двух сумм
                    if (checkSum == checkSumInMessage)
                    {
                        flagCheckSumEqual = true;
                    }
                    break;
                }
            case CheckSumAlg.CRC16:
                {

                    break;
                }
            case CheckSumAlg.CRC32:
                {
                    break;
                }
        }

        return flagCheckSumEqual;
    }


    // Вычисление контрольной суммы
    private byte[] CalcCheckSum(byte[] messageBytes)
    {
        byte[] checkSum;

        switch (checkSumAlg)
        {
            case CheckSumAlg.Simple:
                {
                    //контрольная сумма состоит из 1 байта
                    checkSum = new byte[1];
                    checkSum[0] = CalcSimpleCheckSum(messageBytes);
                    Console.WriteLine("Контрольная сумма посылки: " + $"0x{ checkSum: X}");
                    return checkSum;
                }
            case CheckSumAlg.LRC:
                {
                    // контрольная сумма состоит из 1 байта
                    checkSum = new byte[1];
                    checkSum[0] = CalcLongRedCheck(messageBytes);
                    Console.WriteLine("Контрольная сумма посылки: " + $"0x{ checkSum: X}");
                    return checkSum;
                }
            case CheckSumAlg.CRC16:
                {

                    checkSum = BitConverter.GetBytes(CalcCRC16Sasha(messageBytes));
                    Console.WriteLine("Контрольная сумма посылки-Саша: " + $"0x{ checkSum: X}");
                    byte[] checkSum2 = BitConverter.GetBytes(CalcCRC16Ira(messageBytes));
                    Console.WriteLine("Контрольная сумма посылки-Ира: " + $"0x{ checkSum2: X}");
                    return checkSum;
                }
            case CheckSumAlg.CRC32:
                {
                    checkSum = new byte[3];
                    Console.WriteLine("Контрольная сумма посылки: " + $"0x{ checkSum: X}");
                    return checkSum;
                }
            default:
                throw new Exception("Выбран нереализованный алгоритм подсчета контрольной суммы");
        }
    }


    // Отправка сообщения
    private void SendMessage(string message)
    {

        byte[] messageBytes;
        // Если формат данных бинарный
        if (typeProtocol)
            try
            {
                messageBytes = BitConverter.GetBytes(Convert.ToDouble(message));
            }
            catch (FormatException)
            {
                throw new Exception("В биннарном формате передаются только числа");
            }
        else
            messageBytes = _serialPort.Encoding.GetBytes(message);


        // Добавление контрольной суммы
        // Переведем массив байт в список, чтобы было проще добавить байты контрольной суммы
        List<byte> messageListByte = messageBytes.ToList();

        byte[] checkSum = CalcCheckSum(messageBytes);

        for (int i = 0; i < checkSum.Length; i++)
        {
            messageListByte.Add(checkSum[i]);
        }

        messageBytes = messageListByte.ToArray();

        // Отправка сообщения
        _serialPort.Write(messageBytes, 0, message.Length);
    }



}