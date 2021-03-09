///////////////////////////////////////////////////////////////////////////////
//    RADAR          RANJITH GOPALANKUTTY      01-06-2018                    //
// This solution works as Socket server application to read event messages   //
// from all the stores CE controllers, there will be three classes and       //
// and does below duties respectively                                        //
// StoresBind Class    = Listen to client sockets on port 1887 and bind it   //
// MessageListenClass  = Start listening to the messagtes                    //
// WriteSocketClass    = Write the messages to a file                        //   
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace RADAR
{
    class StoresBindClass
    {
        static void Main(string[] args)
        {            
            Int32 port = 1887;            
            TcpListener server = new TcpListener(port);
            server.Start();

            Byte[] bytes = new Byte[256];
            String data = null;

            while ((true))
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;
                NetworkStream stream = client.GetStream();
                int i;
                
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine (data);                     
                }

                Console.WriteLine("breaking");        
                 
            }

        }
    }
}
 