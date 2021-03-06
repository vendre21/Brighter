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
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.Tests.MessagingGateway.TestDoubles;

namespace Paramore.Brighter.Tests.MessagingGateway.rmq
{
    [Category("RMQ")]
    [TestFixture]
    public class RmqMessageConsumerChannelFailureTests
    {
        private IAmAMessageProducer _sender;
        private IAmAMessageConsumer _receiver;
        private IAmAMessageConsumer _badReceiver;
        private Message _sentMessage;
        private Exception _firstException;

        [SetUp]
        public void Establish()
        {
            var messageHeader = new MessageHeader(Guid.NewGuid(), "test2", MessageType.MT_COMMAND);

            messageHeader.UpdateHandledCount();
            _sentMessage = new Message(header: messageHeader, body: new MessageBody("test content"));

            var rmqConnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672/%2f")),
                Exchange = new Exchange("paramore.brighter.exchange")
            };

            _sender = new RmqMessageProducer(rmqConnection);
            _receiver = new RmqMessageConsumer(rmqConnection, _sentMessage.Header.Topic, _sentMessage.Header.Topic, false, 1, false);
            _badReceiver = new NotSupportedRmqMessageConsumer(rmqConnection, _sentMessage.Header.Topic, _sentMessage.Header.Topic, false, 1, false);

            _receiver.Purge();
            _sender.Send(_sentMessage);
        }

        [Test]
        public void When_a_message_consumer_throws_an_not_supported_exception_when_connecting()
        {
            _firstException = Catch.Exception(() => _badReceiver.Receive(2000));

            //_should_return_a_channel_failure_exception
            Assert.IsInstanceOf<ChannelFailureException>(_firstException);
            //_should_return_an_explainging_inner_exception
            Assert.IsInstanceOf<NotSupportedException>(_firstException.InnerException);
        }

        public void Cleanup()
        {
            _receiver.Purge();
            _sender.Dispose();
            _receiver.Dispose();
        }
    }
}