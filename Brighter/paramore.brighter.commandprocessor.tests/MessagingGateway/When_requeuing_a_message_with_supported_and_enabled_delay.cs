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

using System;
using FakeItEasy;
using Machine.Specifications;
using paramore.brighter.commandprocessor;
using System.Diagnostics;

namespace paramore.commandprocessor.tests.MessagingGateway
{
    [Subject(typeof(InputChannel))]
    public class When_Requeuing_A_Message_With_Supported_And_Enabled_Delay
    {
        private static IAmAnInputChannel s_channel;
        private static IAmAMessageConsumerSupportingDelay s_gateway;
        private static Message s_requeueMessage;
        private static Stopwatch s_stopWatch;

        private Establish _context = () =>
        {
            s_gateway = A.Fake<IAmAMessageConsumerSupportingDelay>();
            A.CallTo(() => s_gateway.DelaySupported).Returns(true);

            s_channel = new InputChannel("test", s_gateway);

            s_requeueMessage = new Message(
                new MessageHeader(Guid.NewGuid(), "key", MessageType.MT_EVENT),
                new MessageBody("a test body"));

            s_stopWatch = new Stopwatch();
        };

        private Because _of = () =>
        {
            s_stopWatch.Start();
            s_channel.Requeue(s_requeueMessage, 1000);
            s_stopWatch.Stop();
        };

        private It _should_call_the_messaging_gateway = () => A.CallTo(() => s_gateway.Requeue(s_requeueMessage, 1000)).MustHaveHappened();
        private It _should_have_used_gateway_delay_support = () => (s_stopWatch.ElapsedMilliseconds < 500).ShouldBeTrue();
    }
}
