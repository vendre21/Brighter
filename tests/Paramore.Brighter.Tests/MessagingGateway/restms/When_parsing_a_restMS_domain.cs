﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

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

using NUnit.Framework;
using Paramore.Brighter.MessagingGateway.RESTMS.Model;
using Paramore.Brighter.MessagingGateway.RESTMS.Parsers;

namespace Paramore.Brighter.Tests.MessagingGateway.restms
{
    
    [Category("RESTMS")]
    [TestFixture]
    public class ParseRestMSResultTests
    {
        private const string BODY = "<domain xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" name=\"default\" title=\"title\" href=\"http://localhost/restms/domain/default\" xmlns=\"http://www.restms.org/schema/restms\"><feed type=\"Default\" name=\"default\" title=\"Default feed\" href=\"http://localhost/restms/feed/default\" /><profile name=\"3/Defaults\" href=\"href://www.restms.org/spec:3/Defaults\" /></domain>";
        private RestMSDomain _domain;
        private bool _couldParse;

        [Test]
        public void When_parsing_a_restMS_domain()
        {
            _couldParse = XmlResultParser.TryParse(BODY, out _domain);

            //_should_be_able_to_parse_the_result
            Assert.True(_couldParse);
            //_should_have_a_domain_object
            Assert.NotNull(_domain);
        }
    }
}
