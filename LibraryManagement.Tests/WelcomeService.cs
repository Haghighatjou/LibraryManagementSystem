using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Tests
{
    internal class WelcomeService
    {
        private readonly IMessageSender _sender;
        public WelcomeService(IMessageSender sender)
        {
            _sender = sender;
        }

        public void Welcome(string name)
        {
            _sender.Send($"Welcome {name}");
            
        }
    }
}
