﻿using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Tests.TestDoubles;

namespace Paramore.Brighter.Tests.Logging.TestDoubles
{
    class MyLoggedHandler : RequestHandler<MyCommand>
    {
        [RequestLogging(step:0, timing: HandlerTiming.Before)]
        public override MyCommand Handle(MyCommand command)
        {
            return base.Handle(command);
        }
    }
}
