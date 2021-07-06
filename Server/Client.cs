using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
   public class Client 
    {
        public Client()
        {
            id = Guid.NewGuid();
        }
        Guid id { get; set; }
        public string ipAddress { get; set; }
        public string status { get; set; }
        public string port { get; set; }
        public int sumOfNums { get; set; }
        public Guid getId()
        {
            return id;
        }
    }
}
