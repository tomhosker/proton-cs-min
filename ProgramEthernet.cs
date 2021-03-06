/*
This code defines a namespace, which produces a program, which reads data
from a Proton RFID reader, and prints it both to the screen and to a plain
text file.
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
  class ProgramEthernet
  {
    /***********
    ** FIELDS **
    ***********/

    // Constants.
    const String readerIP = "192.168.1.2";
    const String readerPort = "/dev/ttyUSB0";

    private CAENRFIDReader myReader = new CAENRFIDReader();
    private CAENRFIDLogicalSource mySource;
    private CAENRFIDTag[] myTags;
    private String printoutFilename =
      "output"+DateTime.Now.ToString("d-M-yyyy")+".txt";

    // Antennae. Mark true A/R.
    private bool ant0 = true;
    private bool ant1 = false;
    private bool ant2 = true;
    private bool ant3 = false;

    /*
    #########################
    # ANT0 ######## ANT2 ####
    #### ANT1 ######## ANT3 #
    #########################
    */

    /*****************
    ** CORE METHODS **
    *****************/

    // Set up myReader, mySource and mySource2.
    void Initialise()
    {
      myReader.Connect(CAENRFIDPort.CAENRFID_TCP, readerIP);
      mySource = myReader.GetSource("Source_0");
      if(ant0) mySource.AddReadPoint("Ant0");
      if(ant1) mySource.AddReadPoint("Ant1");
      if(ant2) mySource.AddReadPoint("Ant2");
      if(ant3) mySource.AddReadPoint("Ant3");
    }

    // Update the lists of tags.
    void Update()
    {
      myTags = mySource.InventoryTag();
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
        }
      } while(Console.ReadKey(true).Key != ConsoleKey.Escape);

      myReader.Disconnect();
    }

    // This is where it all begins.
    static void Main(string[] args)
    {
      ProgramEthernet program = new ProgramEthernet();
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
