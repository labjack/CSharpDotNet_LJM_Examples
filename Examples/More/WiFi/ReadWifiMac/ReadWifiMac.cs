﻿//-----------------------------------------------------------------------------
// ReadWifiMac.cs
//
// Demonstrates how to read the WiFi MAC from a LabJack.
//
// support@labjack.com
//
// Relevant Documentation:
//
// LJM Library:
//     LJM Library Installer:
//         https://labjack.com/support/software/installers/ljm
//     LJM Users Guide:
//         https://labjack.com/support/software/api/ljm
//     Opening and Closing:
//         https://labjack.com/support/software/api/ljm/function-reference/opening-and-closing
//     eReadAddressByteArray:
//         https://labjack.com/support/software/api/ljm/function-reference/ljmereadaddressbytearray
//     NumberToMAC:
//         https://labjack.com/support/software/api/ljm/function-reference/utility/ljmnumbertomac
//
// T-Series and I/O:
//     Modbus Map:
//         https://labjack.com/support/software/api/modbus/modbus-map
//     WiFi:
//         https://labjack.com/support/datasheets/t-series/wifi
//-----------------------------------------------------------------------------
using System;
using LabJack;


namespace ReadWifiMac
{
    class ReadWifiMac
    {
        static void Main(string[] args)
        {
            ReadWifiMac rwm = new ReadWifiMac();
            rwm.performActions();
        }

        public void showErrorMessage(LJM.LJMException e)
        {
            Console.Out.WriteLine("LJMException: " + e.ToString());
            Console.Out.WriteLine(e.StackTrace);
        }

        public void performActions()
        {
            int handle = 0;
            int devType = 0;
            int conType = 0;
            int serNum = 0;
            int ipAddr = 0;
            int port = 0;
            int maxBytesPerMB = 0;
            string ipAddrStr = "";

            try
            {
                //Open first found LabJack
                LJM.OpenS("ANY", "ANY", "ANY", ref handle);  // Any device, Any connection, Any identifier
                //LJM.OpenS("T8", "ANY", "ANY", ref handle);  // T8 device, Any connection, Any identifier
                //LJM.OpenS("T7", "ANY", "ANY", ref handle);  // T7 device, Any connection, Any identifier
                //LJM.Open(LJM.CONSTANTS.dtANY, LJM.CONSTANTS.ctANY, "ANY", ref handle);  // Any device, Any connection, Any identifier

                LJM.GetHandleInfo(handle, ref devType, ref conType, ref serNum, ref ipAddr, ref port, ref maxBytesPerMB);
                LJM.NumberToIP(ipAddr, ref ipAddrStr);
                Console.WriteLine("Opened a LabJack with Device type: " + devType + ", Connection type: " + conType + ",");
                Console.WriteLine("Serial number: " + serNum + ", IP address: " + ipAddrStr + ", Port: " + port + ",");
                Console.WriteLine("Max bytes per MB: " + maxBytesPerMB);

                if (devType == LJM.CONSTANTS.dtT4)
                {
                    Console.WriteLine("\nThe LabJack T4 does not support WiFi.");
                    goto Done;
                }

                //Call eReadAddressByteArray to read the WiFi MAC (address
                //60024) from the LabJack. We are reading a byte array which
                //is the big endian binary representation of the 64-bit MAC.
                byte[] aBytes = new byte[8];
                int errAddr = -1;
                LJM.eReadAddressByteArray(handle, 60024, 8, aBytes, ref errAddr);

                //Convert big endian byte array to a 64-bit unsigned integer
                //value
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(aBytes);
                Int64 macNumber = BitConverter.ToInt64(aBytes, 0);

                //Convert the MAC value/number to its string representation
                string macString = "";
                LJM.NumberToMAC(macNumber, ref macString);

                Console.WriteLine("\nWiFi MAC : " + macNumber + " - " + macString);
            }
            catch (LJM.LJMException e)
            {
                showErrorMessage(e);
            }

        Done:
            LJM.CloseAll();  //Close all handles

            Console.WriteLine("\nDone.\nPress the enter key to exit.");
            Console.ReadLine();  //Pause for user
        }
    }
}
