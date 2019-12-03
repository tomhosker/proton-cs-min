using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using com.caen.RFIDLibrary;

namespace ConsoleApplication1
{
  class Program
  {
    static void Main(string[] args)
    {
      CAENRFIDReader MyReader = new CAENRFIDReader();

      MyReader.Connect(CAENRFIDPort.CAENRFID_TCP, "192.168.0.2");

      CAENRFIDLogicalSource MySource = MyReader.GetSource("Source_0");
      CAENRFIDLogicalSource MySource2 = MyReader.GetSource("Source_1");

      Console.WriteLine("Press ESC to stop");
      do
      {
        while(!Console.KeyAvailable)
        {
          CAENRFIDTag[] MyTags = MySource.InventoryTag();
          CAENRFIDTag[] MyTags2 = MySource2.InventoryTag();

          if(MyTags != null)
          {                   
            for(int i = 0; i < MyTags.Length; i++)
            {
              byte[] data = FromHex(BitConverter.ToString(MyTags[i].GetId()));
              String s = Encoding.ASCII.GetString(data)+" "+
                         DateTime.Now.ToString("h:mm:ss")+" "+
                         MyTags[i].GetReadPoint()+" "+
                         MyTags[i].GetRSSI().ToString();
              Console.WriteLine(s);

              using(System.IO.StreamWriter file =
                      new System.IO.StreamWriter(
                        AppDomain.CurrentDomain.BaseDirectory+
                          "output"+DateTime.Now.ToString("d-M-yyyy")+".txt",
                        true))
              {
                file.WriteLine(s);
              }
            }
          }
          if(MyTags2 != null)
          {
                        for (int i = 0; i < MyTags2.Length; i++)
                        {
                            byte[] data = FromHex(BitConverter.ToString(MyTags2[i].GetId()));
                            String s = Encoding.ASCII.GetString(data) + " " + DateTime.Now.ToString("h:mm:ss") + " " + MyTags2[i].GetReadPoint() + " " + MyTags2[i].GetRSSI();
                            Console.WriteLine(s);

                            using (System.IO.StreamWriter file =
                                new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "output" + DateTime.Now.ToString("d-M-yyyy") + ".txt", true))
                            {
                                file.WriteLine(s);
                            }
                        }
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            MyReader.Disconnect();
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
