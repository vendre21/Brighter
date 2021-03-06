#region Licence
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
using System.Collections.Generic;
using NUnit.Framework;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;

namespace Paramore.Brighter.Tests.MessagingGateway.rmq
{
    [Category("RMQ")]
    [TestFixture]
    public class RmqMessageProducerSendMessageTests
    {
        private IAmAMessageProducer _messageProducer;
        private IAmAMessageConsumer _messageConsumer;
        private Message _message;
        private TestRMQListener _client;
        private string _messageBody;
        private IDictionary<string, object> _messageHeaders;

        [SetUp]
        public void Establish ()
        {
            _message = new Message(header: new MessageHeader(Guid.NewGuid(), "test1", MessageType.MT_COMMAND), body: new MessageBody("test content"));

            var rmqConnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672/%2f")),
                Exchange = new Exchange("paramore.brighter.exchange")
            };

            _messageProducer = new RmqMessageProducer(rmqConnection);
            _messageConsumer = new RmqMessageConsumer(rmqConnection, _message.Header.Topic, _message.Header.Topic, false, 1, false);
            _messageConsumer.Purge();

            _client = new TestRMQListener(rmqConnection, _message.Header.Topic);
        }

        [Test]
        public void When_posting_a_message_via_the_messaging_gateway()
        {
            _messageProducer.Send(_message);

            var result = _client.Listen();
            _messageBody = result.GetBody();
            _messageHeaders = result.GetHeaders();

            //_should_send_a_message_via_rmq_with_the_matching_body
            Assert.AreEqual(_message.Body.Value, _messageBody);
            //_should_send_a_message_via_rmq_without_delay_header
            CollectionAssert.DoesNotContain(_messageHeaders.Keys, HeaderNames.DELAY_MILLISECONDS);
            //_should_received_a_message_via_rmq_without_delayed_header
            CollectionAssert.DoesNotContain(_messageHeaders.Keys, HeaderNames.DELAYED_MILLISECONDS);
        }

        [TearDown]
        public void Cleanup()
        {
            _messageConsumer.Purge();
            _messageProducer.Dispose();
        }
    }
}