/*
This code defines a namespace, which produces a program, which reads data
from a Proton RFID reader, and prints it both to the screen and to a plain
text file.

Change this line beginning "myReader.Connect(..." to change between ETHERNET
and SERIAL configurations.

This is the SERIAL version:

    myReader.Connect(CAENRFIDPort.CAENRFID_RS232, readerPort);

See the SDK manual for the ETHERNET version.
*/

// Imports.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using com.caen.RFIDLibrary;

// The namespace in question.
namespace ConsoleApplication1
{
  // The class in question.
  class Program
  {
    /***********
    ** FIELDS **
    ***********/

    // Constants.
    const String readerIP = "192.168.0.2";
    const String readerPort = "/dev/ttyUSB0";

    private CAENRFIDReader myReader = new CAENRFIDReader();
    private CAENRFIDLogicalSource mySource;
    private CAENRFIDLogicalSource mySource2;
    private CAENRFIDTag[] myTags;
    private CAENRFIDTag[] myTags2;
    private String printoutFilename =
      "output"+DateTime.Now.ToString("d-M-yyyy")+".txt";

    /*****************
    ** CORE METHODS **
    *****************/

    // Set up myReader, mySource and mySource2.
    void Initialise()
    {
      myReader.Connect(CAENRFIDPort.CAENRFID_RS232, readerPort);
      mySource = myReader.GetSource("Source_0");
      mySource2 = myReader.GetSource("Source_1");
    }

    // Update the lists of tags.
    void Update()
    {
      myTags = mySource.InventoryTag();
      myTags2 = mySource2.InventoryTag();
    }

    // Ronseal.
    void AddToPrintout(String s)
    {
      using(System.IO.StreamWriter file =
        new System.IO.StreamWriter(
          AppDomain.CurrentDomain.BaseDirectory+printoutFilename, true))
      {
        file.WriteLine(s);
      }
    }

    // Take an array of RFID tag objects, and print something human-readable
    // to the screen and to a file.
    void HandleTagArray(CAENRFIDTag[] tagArray)
    {
      byte[] data;
      String s;

      if(tagArray == null) return;

      for(int i = 0; i < myTags.Length; i++)
      {
        data = FromHex(BitConverter.ToString(myTags[i].GetId()));
        s = ParseEPC(data)+" "+
            DateTime.Now.ToString("h:mm:ss")+" "+myTags[i].GetReadPoint()+
            " "+myTags[i].GetRSSI().ToString();
        Console.WriteLine(s);
        AddToPrintout(s);
      }
    }

    /********************
    ** RUN AND WRAP UP **
    ********************/

    // Let's get going!
    void Run()
    {
      Initialise();

      Console.WriteLine("Press ESC to stop.");
      do
      {
        while(!Console.KeyAvailable)
        {
          Update();
          HandleTagArray(myTags);
          HandleTagArray(myTags2);
        }
      } while(Console.ReadKey(true).Key != ConsoleKey.Escape);

      myReader.Disconnect();
    }

    // This is where it all begins.
    static void Main(string[] args)
    {
      Program program = new Program();
      program.Run();
    }

    /*******************
    ** STATIC METHODS **
    *******************/

    // Interpret the binary for a tag's EPC into something sensible.
    static String ParseEPC(byte[] data)
    {
      String result = Encoding.ASCII.GetString(data);

      for(int i = 0; i < result.Length; i++)
      {
        if((result[i] < ' ') || (result[i] > '~'))
        {
          if(result[i] == '\0') continue;
          else return "unprintable_epc";
        }
      }

      return result;
    }

    // I'll fill in this comment when I know what this method does!
    static byte[] FromHex(string hex)
    {
      hex = hex.Replace("-", "");
      byte[] raw = new byte[hex.Length/2];

      for(int i = 0; i < raw.Length; i++)
      {
        raw[i] = Convert.ToByte(hex.Substring(i*2, 2), 16);
      }

      return raw;
    }
  }
}
