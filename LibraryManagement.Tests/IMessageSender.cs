using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Tests
{
    public interface IMessageSender
    {
        void Send(string message);
    }
}
