﻿#region Licence
/* The MIT License (MIT)
Copyright � 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the �Software�), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED �AS IS�, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using NUnit.Framework;
using Paramore.Brighter.ServiceActivator.Ports;
using Paramore.Brighter.ServiceActivator.Ports.Commands;

namespace Paramore.Brighter.Tests.ControlBus
{
    [TestFixture]
    public class ConfigurationCommandMessageMapperTests
    {
        private IAmAMessageMapper<ConfigurationCommand> _mapper;
        private Message _message;
        private ConfigurationCommand _command;

        [SetUp]
        public void Establish()
        {
            _mapper = new ConfigurationCommandMessageMapper();

            _message = new Message(
                new MessageHeader(Guid.NewGuid(), "myTopic", MessageType.MT_COMMAND), 
                new MessageBody(string.Format("{{\"Type\":1,\"ConnectionName\":\"getallthethings\",\"Id\":\"{0}\"}}", Guid.NewGuid()))
                );
        }

        [Test]
        public void When_mapping_from_a_configuration_command_from_a_message()
        {
            _command = _mapper.MapToRequest(_message);

            //_should_rehydrate_the_command_type
            Assert.AreEqual(ConfigurationCommandType.CM_STARTALL, _command.Type);
            // _should_rehydrate_the_connection_name
            Assert.AreEqual("getallthethings", _command.ConnectionName);
        }
    }
}