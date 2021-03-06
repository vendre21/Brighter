﻿#region Licence
/* The MIT License (MIT)
Copyright © 2015 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Paramore.Brighter.Monitoring.Configuration;
using Paramore.Brighter.Monitoring.Events;
using Paramore.Brighter.Monitoring.Handlers;
using Paramore.Brighter.Tests.Monitoring.TestDoubles;
using Paramore.Brighter.Tests.TestDoubles;
using Paramore.Brighter.Time;
using TinyIoC;

namespace Paramore.Brighter.Tests.Monitoring
{
    [TestFixture]
    public class MonitorHandlerTests
    {
        private MyCommand _command;
        private Exception _thrownException;
        private SpyControlBusSender _controlBusSender;
        private CommandProcessor _commandProcessor;
        private MonitorEvent _afterEvent;
        private string _originalRequestAsJson;
        private DateTime _at;

        [SetUp]
        public void Establish()
        {
            _controlBusSender = new SpyControlBusSender();
            var registry = new SubscriberRegistry();
            registry.Register<MyCommand, MyMonitoredHandlerThatThrows>();

            var container = new TinyIoCContainer();
            var handlerFactory = new TinyIocHandlerFactory(container);
            container.Register<IHandleRequests<MyCommand>, MyMonitoredHandlerThatThrows>();
            container.Register<IHandleRequests<MyCommand>, MonitorHandler<MyCommand>>();
            container.Register<IAmAControlBusSender>(_controlBusSender);
            container.Register<MonitorConfiguration>(new MonitorConfiguration { IsMonitoringEnabled = true, InstanceName = "UnitTests" });

            _commandProcessor = new CommandProcessor(registry, handlerFactory, new InMemoryRequestContextFactory(), new PolicyRegistry());

            _command = new MyCommand();

            _originalRequestAsJson = JsonConvert.SerializeObject(_command);

            _at = DateTime.UtcNow;
            Clock.OverrideTime = _at;
        }

        public void When_Monitoring_We_Should_Record_But_Rethrow_Exceptions()
        {
            _thrownException = Catch.Exception(() => _commandProcessor.Send(_command));
            _controlBusSender.Observe<MonitorEvent>(); //pop but don't inspect before.
            _afterEvent = _controlBusSender.Observe<MonitorEvent>();

            //_should_pass_through_the_exception_not_swallow
            Assert.NotNull(_thrownException);
            //_should_monitor_the_exception
            Assert.IsInstanceOf(typeof(Exception), _afterEvent.Exception);
            //_should_surface_the_error_message
            StringAssert.Contains("monitored", _afterEvent.Exception.Message);
            //_should_have_an_instance_name_after
            Assert.AreEqual("UnitTests", _afterEvent.InstanceName);
            //_should_post_the_handler_fullname_to_the_control_bus_after
            Assert.AreEqual(typeof(MyMonitoredHandler).FullName, _afterEvent.HandlerName);
            //_should_post_the_handler_name_to_the_control_bus_after
            Assert.AreEqual(typeof(MyMonitoredHandler).AssemblyQualifiedName, _afterEvent.HandlerFullAssemblyName);
            //should_post_the_time_of_the_request_after
            Assert.Greater(_afterEvent.EventTime, _at);
            //should_post_the_elapsedtime_of_the_request_after
            Assert.Greater(_afterEvent.EventTime, _at);
            //should_post_the_elapsedtime_of_the_request_after
            Assert.AreEqual((_afterEvent.EventTime - _at).Milliseconds, _afterEvent.TimeElapsedMs);
        }
   }
}
