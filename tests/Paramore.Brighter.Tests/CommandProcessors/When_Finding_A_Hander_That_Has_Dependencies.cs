using System.Linq;
using NUnit.Framework;
using Paramore.Brighter.Tests.TestDoubles;

namespace Paramore.Brighter.Tests
{
    [TestFixture]
    public class PipelineWithHandlerDependenciesTests
    {
        private PipelineBuilder<MyCommand> _pipelineBuilder;
        private IHandleRequests<MyCommand> _pipeline;

        [SetUp]
        public void Establish()
        {
            var registry = new SubscriberRegistry();
            registry.Register<MyCommand, MyDependentCommandHandler>();
            var handlerFactory = new TestHandlerFactory<MyCommand, MyDependentCommandHandler>(() => new MyDependentCommandHandler(new FakeRepository<MyAggregate>(new FakeSession())));

            _pipelineBuilder = new PipelineBuilder<MyCommand>(registry, handlerFactory);
        }

        public void When_Finding_A_Hander_That_Has_Dependencies()
        {
            _pipeline = _pipelineBuilder.Build(new RequestContext()).First();

           // _should_return_the_command_handler_as_the_implicit_handler
            Assert.IsAssignableFrom(typeof(MyDependentCommandHandler), _pipeline);
            //  _should_be_the_only_element_in_the_chain
            Assert.AreEqual("MyDependentCommandHandler|", TracePipeline().ToString());
        }

        private PipelineTracer TracePipeline()
        {
            var pipelineTracer = new PipelineTracer();
            _pipeline.DescribePath(pipelineTracer);
            return pipelineTracer;
        }
    }
}