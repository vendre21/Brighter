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
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Paramore.Brighter.ServiceActivator;
using Paramore.Brighter.ServiceActivator.TestHelpers;
using Paramore.Brighter.Tests.MessageDispatch.TestDoubles;
using Paramore.Brighter.Tests.TestDoubles;

namespace Paramore.Brighter.Tests.MessageDispatch
{
    [TestFixture]
    public class MessagePumpCommandRequeueCountThresholdTests
    {
        private IAmAMessagePump _messagePump;
        private FakeChannel _channel;
        private SpyRequeueCommandProcessor _commandProcessor;
        private MyCommand _command;

        [SetUp]
        public void Establish ()
        {
            _commandProcessor = new SpyRequeueCommandProcessor();
            _channel = new FakeChannel();
            var mapper = new MyCommandMessageMapper();
            _messagePump = new MessagePump<MyCommand>(_commandProcessor, mapper) { Channel = _channel, TimeoutInMilliseconds = 5000, RequeueCount = 3 };

            _command = new MyCommand();

            var message1 = new Message(new MessageHeader(Guid.NewGuid(), "MyTopic", MessageType.MT_COMMAND), new MessageBody(JsonConvert.SerializeObject(_command)));
            var message2 = new Message(new MessageHeader(Guid.NewGuid(), "MyTopic", MessageType.MT_COMMAND), new MessageBody(JsonConvert.SerializeObject(_command)));
            _channel.Add(message1);
            _channel.Add(message2);
        }

        [Test]
        public void When_A_Requeue_Count_Threshold_For_Commands_Has_Been_Reached()
        {
            var task = Task.Factory.StartNew(() => _messagePump.Run(), TaskCreationOptions.LongRunning);
            Task.Delay(1000).Wait();

            var quitMessage = new Message(new MessageHeader(Guid.Empty, "", MessageType.MT_QUIT), new MessageBody(""));
            _channel.Add(quitMessage);

            Task.WaitAll(new[] { task });

            //_should_send_the_message_via_the_command_processor
            Assert.AreEqual(CommandType.Send, _commandProcessor.Commands[0]);
            //_should_have_been_handled_6_times_via_send
            Assert.AreEqual(6, _commandProcessor.SendCount);
            //_should_requeue_the_messages
            Assert.AreEqual(0, _channel.Length);
            //_should_dispose_the_input_channel
            Assert.True(_channel.DisposeHappened);
        }
    }
}